using System.Collections.Concurrent;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services;

/// <summary>
/// In-memory store for simulator response-control fields that the browser UI manages but that have
/// no corresponding property on the existing RPC ViewModels/Models.
///
/// Background: the web UI (<c>wwwroot/js/app.js</c>) PUTs richer state payloads than the desktop
/// ViewModels were designed for. Fields that DO map onto an existing model property are written
/// directly to that model (so the RPC providers honor them). Fields that have no model home are
/// retained here per-feature so that <c>GET /api/&lt;feature&gt;/state</c> round-trips correctly and
/// the UI tabs do not 404 or lose data. State is process-lifetime only (no Redis/DB), matching the
/// project's "in-memory only" decision.
/// </summary>
public sealed class SimFeatureState
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object?>> _features = new();

    /// <summary>Merge the supplied values into the named feature's bag.</summary>
    public void Set(string feature, IDictionary<string, object?> values)
    {
        var bag = _features.GetOrAdd(feature, _ => new ConcurrentDictionary<string, object?>());
        foreach (var kvp in values)
        {
            bag[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>Set a single value for the named feature.</summary>
    public void Set(string feature, string key, object? value)
    {
        var bag = _features.GetOrAdd(feature, _ => new ConcurrentDictionary<string, object?>());
        bag[key] = value;
    }

    /// <summary>Get a snapshot of the named feature's bag (empty if never set).</summary>
    public IReadOnlyDictionary<string, object?> Get(string feature)
    {
        return _features.TryGetValue(feature, out var bag)
            ? new Dictionary<string, object?>(bag)
            : new Dictionary<string, object?>();
    }
}
