using IGT.FloorNet.Tools.ServiceSimulator.Models.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services;

/// <summary>
/// Lightweight, cross-platform (Windows + Linux) persistence store for the simulator.
/// Backed by SQLite via <c>Microsoft.Data.Sqlite</c> — chosen as the simplest embedded,
/// zero-install database that runs identically on both platforms (no server process, single file).
///
/// <para>The store persists three FloorNet activity streams: events (auditEvent + discrete
/// event classes), TITO cashouts/vouchers, and meter snapshots. It deliberately avoids an ORM
/// (e.g. EF Core) to keep the dependency footprint minimal and the SQL transparent.</para>
///
/// <para>Writes are serialized through a single shared connection guarded by a lock; SQLite WAL
/// mode is enabled so concurrent reads (from the query API) are not blocked by writes.</para>
/// </summary>
public sealed class SimDbStore : IDisposable
{
    private readonly ILogger<SimDbStore> _logger;
    private readonly SqliteConnection? _connection;
    private readonly object _writeLock = new();

    /// <summary>True when persistence is enabled and the database opened successfully.</summary>
    public bool Enabled { get; }

    /// <summary>Absolute path to the SQLite database file (for diagnostics / query API echo).</summary>
    public string DatabasePath { get; }

    /// <summary>
    /// Creates the store, opens (or creates) the SQLite database file, and ensures the schema exists.
    /// Persistence can be disabled via configuration key <c>Persistence:Enabled</c> (default true).
    /// The file location is configurable via <c>Persistence:DatabasePath</c> (default
    /// <c>floornetsim.db</c> in the content root).
    /// </summary>
    public SimDbStore(IConfiguration configuration, ILogger<SimDbStore> logger)
    {
        _logger = logger;

        Enabled = configuration.GetValue("Persistence:Enabled", true);
        var configuredPath = configuration.GetValue<string>("Persistence:DatabasePath") ?? "floornetsim.db";
        DatabasePath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(AppContext.BaseDirectory, configuredPath);

        if (!Enabled)
        {
            _logger.LogInformation("SimDbStore: persistence is disabled (Persistence:Enabled=false).");
            return;
        }

        try
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = DatabasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            _connection = new SqliteConnection(connectionString);
            _connection.Open();

            // WAL improves read/write concurrency; the query API can read while events stream in.
            ExecuteNonQuery("PRAGMA journal_mode=WAL;");
            ExecuteNonQuery("PRAGMA synchronous=NORMAL;");

            EnsureSchema();
            _logger.LogInformation("SimDbStore: SQLite persistence ready at {Path}", DatabasePath);
        }
        catch (Exception ex)
        {
            // Persistence is best-effort for a developer tool; never let a DB failure crash the sim.
            Enabled = false;
            _connection = null;
            _logger.LogError(ex, "SimDbStore: failed to initialize SQLite at {Path}; persistence disabled.", DatabasePath);
        }
    }

    /// <summary>Creates the tables and indexes if they do not already exist.</summary>
    private void EnsureSchema()
    {
        const string ddl = """
            CREATE TABLE IF NOT EXISTS Events (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                ReceivedUtc TEXT    NOT NULL,
                EventName   TEXT    NOT NULL,
                EventCode   TEXT    NULL,
                Category    TEXT    NULL,
                Uid         TEXT    NULL,
                MachineNum  INTEGER NULL,
                SiteId      TEXT    NULL,
                DeviceType  TEXT    NULL,
                PayloadJson TEXT    NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IX_Events_ReceivedUtc ON Events(ReceivedUtc);
            CREATE INDEX IF NOT EXISTS IX_Events_EventCode   ON Events(EventCode);
            CREATE INDEX IF NOT EXISTS IX_Events_Uid         ON Events(Uid);

            CREATE TABLE IF NOT EXISTS Cashouts (
                Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                ReceivedUtc   TEXT    NOT NULL,
                Operation     TEXT    NOT NULL,
                VoucherId     TEXT    NULL,
                AmountCents   INTEGER NOT NULL,
                TransactionId INTEGER NULL,
                CardId        TEXT    NULL,
                Uid           TEXT    NULL,
                MachineNum    INTEGER NULL,
                SiteId        TEXT    NULL,
                RequestJson   TEXT    NOT NULL,
                ResponseJson  TEXT    NULL
            );
            CREATE INDEX IF NOT EXISTS IX_Cashouts_ReceivedUtc   ON Cashouts(ReceivedUtc);
            CREATE INDEX IF NOT EXISTS IX_Cashouts_TransactionId ON Cashouts(TransactionId);
            CREATE INDEX IF NOT EXISTS IX_Cashouts_Operation     ON Cashouts(Operation);

            CREATE TABLE IF NOT EXISTS Meters (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                ReceivedUtc TEXT    NOT NULL,
                MeterType   TEXT    NULL,
                MeterTime   INTEGER NOT NULL,
                MeterCode   INTEGER NOT NULL,
                Value       INTEGER NOT NULL,
                Uid         TEXT    NULL,
                MachineNum  INTEGER NULL,
                SiteId      TEXT    NULL
            );
            CREATE INDEX IF NOT EXISTS IX_Meters_ReceivedUtc ON Meters(ReceivedUtc);
            CREATE INDEX IF NOT EXISTS IX_Meters_MeterCode   ON Meters(MeterCode);
            CREATE INDEX IF NOT EXISTS IX_Meters_Uid         ON Meters(Uid);
            """;

        ExecuteNonQuery(ddl);
    }

    /// <summary>Inserts an observed event. No-op when persistence is disabled.</summary>
    public void InsertEvent(EventRecord record)
    {
        if (!Enabled || _connection is null)
        {
            return;
        }

        try
        {
            lock (_writeLock)
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = """
                    INSERT INTO Events (ReceivedUtc, EventName, EventCode, Category, Uid, MachineNum, SiteId, DeviceType, PayloadJson)
                    VALUES ($received, $name, $code, $category, $uid, $machine, $site, $device, $payload);
                    """;
                cmd.Parameters.AddWithValue("$received", record.ReceivedUtc.ToString("o"));
                cmd.Parameters.AddWithValue("$name", record.EventName);
                cmd.Parameters.AddWithValue("$code", (object?)record.EventCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$category", (object?)record.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$uid", (object?)record.Uid ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$machine", (object?)record.MachineNum ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$site", (object?)record.SiteId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$device", (object?)record.DeviceType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$payload", record.PayloadJson);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SimDbStore: failed to insert event {EventName}.", record.EventName);
        }
    }

    /// <summary>Inserts a TITO cashout/voucher activity record. No-op when persistence is disabled.</summary>
    public void InsertCashout(CashoutRecord record)
    {
        if (!Enabled || _connection is null)
        {
            return;
        }

        try
        {
            lock (_writeLock)
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = """
                    INSERT INTO Cashouts (ReceivedUtc, Operation, VoucherId, AmountCents, TransactionId, CardId, Uid, MachineNum, SiteId, RequestJson, ResponseJson)
                    VALUES ($received, $op, $voucher, $amount, $txn, $card, $uid, $machine, $site, $req, $resp);
                    """;
                cmd.Parameters.AddWithValue("$received", record.ReceivedUtc.ToString("o"));
                cmd.Parameters.AddWithValue("$op", record.Operation);
                cmd.Parameters.AddWithValue("$voucher", (object?)record.VoucherId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$amount", record.AmountCents);
                cmd.Parameters.AddWithValue("$txn", (object?)record.TransactionId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$card", (object?)record.CardId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$uid", (object?)record.Uid ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$machine", (object?)record.MachineNum ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$site", (object?)record.SiteId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$req", record.RequestJson);
                cmd.Parameters.AddWithValue("$resp", (object?)record.ResponseJson ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SimDbStore: failed to insert cashout {Operation}.", record.Operation);
        }
    }

    /// <summary>
    /// Inserts a batch of meter rows (one per meter code) under a single transaction.
    /// No-op when persistence is disabled or the batch is empty.
    /// </summary>
    public void InsertMeters(IReadOnlyCollection<MeterRecord> records)
    {
        if (!Enabled || _connection is null || records.Count == 0)
        {
            return;
        }

        try
        {
            lock (_writeLock)
            {
                using var transaction = _connection.BeginTransaction();
                using var cmd = _connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = """
                    INSERT INTO Meters (ReceivedUtc, MeterType, MeterTime, MeterCode, Value, Uid, MachineNum, SiteId)
                    VALUES ($received, $type, $time, $code, $value, $uid, $machine, $site);
                    """;

                // Reuse parameter objects across the batch for efficiency.
                var pReceived = cmd.Parameters.Add("$received", SqliteType.Text);
                var pType = cmd.Parameters.Add("$type", SqliteType.Text);
                var pTime = cmd.Parameters.Add("$time", SqliteType.Integer);
                var pCode = cmd.Parameters.Add("$code", SqliteType.Integer);
                var pValue = cmd.Parameters.Add("$value", SqliteType.Integer);
                var pUid = cmd.Parameters.Add("$uid", SqliteType.Text);
                var pMachine = cmd.Parameters.Add("$machine", SqliteType.Integer);
                var pSite = cmd.Parameters.Add("$site", SqliteType.Text);

                foreach (var record in records)
                {
                    pReceived.Value = record.ReceivedUtc.ToString("o");
                    pType.Value = (object?)record.MeterType ?? DBNull.Value;
                    pTime.Value = record.MeterTime;
                    pCode.Value = record.MeterCode;
                    pValue.Value = record.Value;
                    pUid.Value = (object?)record.Uid ?? DBNull.Value;
                    pMachine.Value = (object?)record.MachineNum ?? DBNull.Value;
                    pSite.Value = (object?)record.SiteId ?? DBNull.Value;
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SimDbStore: failed to insert {Count} meter rows.", records.Count);
        }
    }

    /// <summary>Queries persisted events, newest first, with optional filters.</summary>
    public IReadOnlyList<EventRecord> QueryEvents(string? code, string? uid, DateTime? sinceUtc, int limit)
    {
        if (!Enabled || _connection is null)
        {
            return Array.Empty<EventRecord>();
        }

        lock (_writeLock)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                SELECT Id, ReceivedUtc, EventName, EventCode, Category, Uid, MachineNum, SiteId, DeviceType, PayloadJson
                FROM Events
                WHERE ($code IS NULL OR EventCode = $code)
                  AND ($uid IS NULL OR Uid = $uid)
                  AND ($since IS NULL OR ReceivedUtc >= $since)
                ORDER BY Id DESC
                LIMIT $limit;
                """;
            cmd.Parameters.AddWithValue("$code", (object?)code ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$uid", (object?)uid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$since", (object?)sinceUtc?.ToString("o") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$limit", limit);

            var results = new List<EventRecord>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new EventRecord
                {
                    Id = reader.GetInt64(0),
                    ReceivedUtc = DateTime.Parse(reader.GetString(1), null, System.Globalization.DateTimeStyles.RoundtripKind),
                    EventName = reader.GetString(2),
                    EventCode = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Category = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Uid = reader.IsDBNull(5) ? null : reader.GetString(5),
                    MachineNum = reader.IsDBNull(6) ? null : reader.GetInt64(6),
                    SiteId = reader.IsDBNull(7) ? null : reader.GetString(7),
                    DeviceType = reader.IsDBNull(8) ? null : reader.GetString(8),
                    PayloadJson = reader.GetString(9)
                });
            }

            return results;
        }
    }

    /// <summary>Queries persisted cashouts, newest first, with optional filters.</summary>
    public IReadOnlyList<CashoutRecord> QueryCashouts(string? operation, string? uid, DateTime? sinceUtc, int limit)
    {
        if (!Enabled || _connection is null)
        {
            return Array.Empty<CashoutRecord>();
        }

        lock (_writeLock)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                SELECT Id, ReceivedUtc, Operation, VoucherId, AmountCents, TransactionId, CardId, Uid, MachineNum, SiteId, RequestJson, ResponseJson
                FROM Cashouts
                WHERE ($op IS NULL OR Operation = $op)
                  AND ($uid IS NULL OR Uid = $uid)
                  AND ($since IS NULL OR ReceivedUtc >= $since)
                ORDER BY Id DESC
                LIMIT $limit;
                """;
            cmd.Parameters.AddWithValue("$op", (object?)operation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$uid", (object?)uid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$since", (object?)sinceUtc?.ToString("o") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$limit", limit);

            var results = new List<CashoutRecord>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new CashoutRecord
                {
                    Id = reader.GetInt64(0),
                    ReceivedUtc = DateTime.Parse(reader.GetString(1), null, System.Globalization.DateTimeStyles.RoundtripKind),
                    Operation = reader.GetString(2),
                    VoucherId = reader.IsDBNull(3) ? null : reader.GetString(3),
                    AmountCents = reader.GetInt64(4),
                    TransactionId = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                    CardId = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Uid = reader.IsDBNull(7) ? null : reader.GetString(7),
                    MachineNum = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                    SiteId = reader.IsDBNull(9) ? null : reader.GetString(9),
                    RequestJson = reader.GetString(10),
                    ResponseJson = reader.IsDBNull(11) ? null : reader.GetString(11)
                });
            }

            return results;
        }
    }

    /// <summary>Queries persisted meter rows, newest first, with optional filters.</summary>
    public IReadOnlyList<MeterRecord> QueryMeters(long? meterCode, string? uid, DateTime? sinceUtc, int limit)
    {
        if (!Enabled || _connection is null)
        {
            return Array.Empty<MeterRecord>();
        }

        lock (_writeLock)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                SELECT Id, ReceivedUtc, MeterType, MeterTime, MeterCode, Value, Uid, MachineNum, SiteId
                FROM Meters
                WHERE ($code IS NULL OR MeterCode = $code)
                  AND ($uid IS NULL OR Uid = $uid)
                  AND ($since IS NULL OR ReceivedUtc >= $since)
                ORDER BY Id DESC
                LIMIT $limit;
                """;
            cmd.Parameters.AddWithValue("$code", (object?)meterCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$uid", (object?)uid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$since", (object?)sinceUtc?.ToString("o") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$limit", limit);

            var results = new List<MeterRecord>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new MeterRecord
                {
                    Id = reader.GetInt64(0),
                    ReceivedUtc = DateTime.Parse(reader.GetString(1), null, System.Globalization.DateTimeStyles.RoundtripKind),
                    MeterType = reader.IsDBNull(2) ? null : reader.GetString(2),
                    MeterTime = reader.GetInt64(3),
                    MeterCode = reader.GetInt64(4),
                    Value = reader.GetInt64(5),
                    Uid = reader.IsDBNull(6) ? null : reader.GetString(6),
                    MachineNum = reader.IsDBNull(7) ? null : reader.GetInt64(7),
                    SiteId = reader.IsDBNull(8) ? null : reader.GetString(8)
                });
            }

            return results;
        }
    }

    /// <summary>Returns row counts per table for a quick summary endpoint.</summary>
    public (long Events, long Cashouts, long Meters) GetCounts()
    {
        if (!Enabled || _connection is null)
        {
            return (0, 0, 0);
        }

        lock (_writeLock)
        {
            return (CountTable("Events"), CountTable("Cashouts"), CountTable("Meters"));
        }
    }

    /// <summary>Returns the row count for a known (hard-coded) table name.</summary>
    private long CountTable(string table)
    {
        using var cmd = _connection!.CreateCommand();
        // Table name is a fixed internal literal, never user input — safe to interpolate.
        cmd.CommandText = $"SELECT COUNT(*) FROM {table};";
        var result = cmd.ExecuteScalar();
        return result is long l ? l : Convert.ToInt64(result);
    }

    /// <summary>Executes a non-query SQL statement against the shared connection.</summary>
    private void ExecuteNonQuery(string sql)
    {
        using var cmd = _connection!.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    /// <summary>Closes the SQLite connection.</summary>
    public void Dispose()
    {
        _connection?.Dispose();
    }
}
