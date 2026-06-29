namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Persistence;

/// <summary>
/// Persisted record of a FloorNet event observed by the simulator.
/// Covers both <c>auditEvent</c> (Guaranteed-delivery) codes and discrete event classes
/// (keepAlive, gameplay, progress, etc.). See FloorNet protocol chapter 4 (Events &amp; auditEvent).
/// </summary>
public sealed class EventRecord
{
    /// <summary>Auto-increment primary key (assigned by SQLite).</summary>
    public long Id { get; set; }

    /// <summary>UTC timestamp when the simulator received the event.</summary>
    public DateTime ReceivedUtc { get; set; }

    /// <summary>
    /// Event class / command name (e.g. <c>auditEvent</c>, <c>meterEvent</c>, <c>keepAlive</c>).
    /// For audit events this is the wrapper class; <see cref="EventCode"/> carries the specifics.
    /// </summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Symbolic audit-event code name (e.g. <c>cabLogic_Door_Open</c>) when the event is an
    /// <c>auditEvent</c>; otherwise <c>null</c>.
    /// </summary>
    public string? EventCode { get; set; }

    /// <summary>auditEvent category (e.g. Cabinet, Tito, Meters) when applicable; otherwise <c>null</c>.</summary>
    public string? Category { get; set; }

    /// <summary>SMIB unique id (uid) from the event call context.</summary>
    public string? Uid { get; set; }

    /// <summary>EGM machine number from the event call context.</summary>
    public long? MachineNum { get; set; }

    /// <summary>Site id from the event call context.</summary>
    public string? SiteId { get; set; }

    /// <summary>Device type from the event call context.</summary>
    public string? DeviceType { get; set; }

    /// <summary>Full event payload serialized as JSON for ad-hoc inspection.</summary>
    public string PayloadJson { get; set; } = string.Empty;
}

/// <summary>
/// Persisted record of a TITO cashout/voucher activity (issue, redeem, commit) and the
/// host-side response. Grounded in FloorNet TITO service message reference (iTito/iTitoV2).
/// Amounts are stored in cents, matching the protocol's <c>voucherAmt</c>/<c>transferAmount</c>.
/// </summary>
public sealed class CashoutRecord
{
    /// <summary>Auto-increment primary key (assigned by SQLite).</summary>
    public long Id { get; set; }

    /// <summary>UTC timestamp when the simulator processed the voucher RPC.</summary>
    public DateTime ReceivedUtc { get; set; }

    /// <summary>
    /// Voucher operation: <c>issueVoucher</c> (cashout printed), <c>redeemVoucher</c> (ticket-in),
    /// or <c>commitVoucher</c> (final disposition).
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>Validation identifier of the voucher (may be encrypted under iTitoV2).</summary>
    public string? VoucherId { get; set; }

    /// <summary>Voucher / transfer amount in cents.</summary>
    public long AmountCents { get; set; }

    /// <summary>SMIB-assigned transaction identifier correlating redeem and commit.</summary>
    public long? TransactionId { get; set; }

    /// <summary>Player card id, when a card is present.</summary>
    public string? CardId { get; set; }

    /// <summary>SMIB unique id (uid) from the RPC call context.</summary>
    public string? Uid { get; set; }

    /// <summary>EGM machine number from the RPC call context.</summary>
    public long? MachineNum { get; set; }

    /// <summary>Site id from the RPC call context.</summary>
    public string? SiteId { get; set; }

    /// <summary>Request parameters serialized as JSON.</summary>
    public string RequestJson { get; set; } = string.Empty;

    /// <summary>Host response serialized as JSON (null when the simulator declined to respond).</summary>
    public string? ResponseJson { get; set; }
}

/// <summary>
/// Persisted record of a single meter value extracted from a FloorNet <c>meterEvent</c>
/// (or related meter snapshot). Each row is one (meterCode, value) pair so meters can be
/// queried/aggregated relationally. See FloorNet Meters service message reference.
/// </summary>
public sealed class MeterRecord
{
    /// <summary>Auto-increment primary key (assigned by SQLite).</summary>
    public long Id { get; set; }

    /// <summary>UTC timestamp when the simulator received the meter snapshot.</summary>
    public DateTime ReceivedUtc { get; set; }

    /// <summary>
    /// Meter record type (a <c>t_meterTypes</c> value, e.g. 'I' initial, 'F' final,
    /// 'M' periodic, 'L' location).
    /// </summary>
    public string? MeterType { get; set; }

    /// <summary>Protocol meter time (echoes the event's <c>meterTime</c>).</summary>
    public long MeterTime { get; set; }

    /// <summary>Numeric meter code (<c>t_meterCode</c> value).</summary>
    public long MeterCode { get; set; }

    /// <summary>Meter value (life-to-date or delta, per the snapshot semantics).</summary>
    public long Value { get; set; }

    /// <summary>SMIB unique id (uid) from the event call context.</summary>
    public string? Uid { get; set; }

    /// <summary>EGM machine number from the event call context.</summary>
    public long? MachineNum { get; set; }

    /// <summary>Site id from the event call context.</summary>
    public string? SiteId { get; set; }
}
