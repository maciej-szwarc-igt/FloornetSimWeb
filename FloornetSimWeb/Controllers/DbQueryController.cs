using IGT.FloorNet.Tools.ServiceSimulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace FloornetSimWeb.Controllers;

/// <summary>
/// Read-only query API over the SQLite persistence store. Exposes the three persisted FloorNet
/// activity streams — events, cashouts, and meters — plus a summary of row counts.
///
/// <para>All list endpoints support optional filters and a <c>limit</c> (default 100, max 1000),
/// returning newest-first. This surface is anonymous-by-default, matching the rest of the
/// developer-facing simulator UI.</para>
/// </summary>
[ApiController]
[Route("api/db")]
public class DbQueryController : ControllerBase
{
    private const int DefaultLimit = 100;
    private const int MaxLimit = 1000;

    private readonly SimDbStore _db;

    /// <summary>Creates the controller with the shared persistence store.</summary>
    public DbQueryController(SimDbStore db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns persistence status and per-table row counts.
    /// </summary>
    /// <example>GET /api/db/summary</example>
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        var (events, cashouts, meters) = _db.GetCounts();
        return Ok(new
        {
            enabled = _db.Enabled,
            databasePath = _db.DatabasePath,
            counts = new { events, cashouts, meters }
        });
    }

    /// <summary>
    /// Queries persisted events, newest first.
    /// </summary>
    /// <param name="code">Optional audit-event code name filter (e.g. <c>cabLogic_Door_Open</c>).</param>
    /// <param name="uid">Optional SMIB uid filter.</param>
    /// <param name="since">Optional ISO-8601 UTC lower bound on <c>ReceivedUtc</c>.</param>
    /// <param name="limit">Maximum rows to return (default 100, capped at 1000).</param>
    /// <example>GET /api/db/events?code=cabLogic_Door_Open&amp;limit=50</example>
    [HttpGet("events")]
    public IActionResult GetEvents(
        [FromQuery] string? code,
        [FromQuery] string? uid,
        [FromQuery] DateTime? since,
        [FromQuery] int limit = DefaultLimit)
    {
        var results = _db.QueryEvents(code, uid, NormalizeSince(since), ClampLimit(limit));
        return Ok(new { count = results.Count, items = results });
    }

    /// <summary>
    /// Queries persisted TITO cashouts/vouchers, newest first.
    /// </summary>
    /// <param name="operation">Optional operation filter (<c>issueVoucher</c>, <c>redeemVoucher</c>, <c>commitVoucher</c>).</param>
    /// <param name="uid">Optional SMIB uid filter.</param>
    /// <param name="since">Optional ISO-8601 UTC lower bound on <c>ReceivedUtc</c>.</param>
    /// <param name="limit">Maximum rows to return (default 100, capped at 1000).</param>
    /// <example>GET /api/db/cashouts?operation=issueVoucher</example>
    [HttpGet("cashouts")]
    public IActionResult GetCashouts(
        [FromQuery] string? operation,
        [FromQuery] string? uid,
        [FromQuery] DateTime? since,
        [FromQuery] int limit = DefaultLimit)
    {
        var results = _db.QueryCashouts(operation, uid, NormalizeSince(since), ClampLimit(limit));
        return Ok(new { count = results.Count, items = results });
    }

    /// <summary>
    /// Queries persisted meter rows, newest first.
    /// </summary>
    /// <param name="meterCode">Optional numeric <c>t_meterCode</c> filter.</param>
    /// <param name="uid">Optional SMIB uid filter.</param>
    /// <param name="since">Optional ISO-8601 UTC lower bound on <c>ReceivedUtc</c>.</param>
    /// <param name="limit">Maximum rows to return (default 100, capped at 1000).</param>
    /// <example>GET /api/db/meters?meterCode=0&amp;limit=200</example>
    [HttpGet("meters")]
    public IActionResult GetMeters(
        [FromQuery] long? meterCode,
        [FromQuery] string? uid,
        [FromQuery] DateTime? since,
        [FromQuery] int limit = DefaultLimit)
    {
        var results = _db.QueryMeters(meterCode, uid, NormalizeSince(since), ClampLimit(limit));
        return Ok(new { count = results.Count, items = results });
    }

    /// <summary>Clamps a requested limit into the [1, MaxLimit] range.</summary>
    private static int ClampLimit(int limit)
    {
        return limit switch
        {
            < 1 => DefaultLimit,
            > MaxLimit => MaxLimit,
            _ => limit
        };
    }

    /// <summary>Converts an incoming <c>since</c> value to UTC for consistent string comparison.</summary>
    private static DateTime? NormalizeSince(DateTime? since)
    {
        return since is null ? null : since.Value.ToUniversalTime();
    }
}
