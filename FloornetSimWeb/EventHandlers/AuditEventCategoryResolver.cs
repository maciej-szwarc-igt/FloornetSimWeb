using System.Collections.Concurrent;
using System.Reflection;
using IGT.FloorNet;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers;

/// <summary>
/// Resolves the FloorNet auditEvent <c>category</c> for a <see cref="t_eventCode"/> by reading the
/// <c>[EventCategory("...")]</c> attribute declared on each enum member (e.g. Cabinet, Tito, Meters).
/// The protocol carries the category in the routing key (<c>auditEvent.category.code</c>) but the
/// deserialized <c>auditEvent</c> object does not expose it as a field, so we reflect it once and cache.
/// </summary>
internal static class AuditEventCategoryResolver
{
    private static readonly ConcurrentDictionary<t_eventCode, string?> Cache = new();

    /// <summary>Returns the category name for the given event code, or <c>null</c> if none is declared.</summary>
    public static string? Resolve(t_eventCode code)
    {
        return Cache.GetOrAdd(code, static c =>
        {
            var member = typeof(t_eventCode).GetMember(c.ToString());
            if (member.Length == 0)
            {
                return null;
            }

            // The attribute type is IGT.FloorNet.EventCategory with a public string member "name".
            foreach (var attr in member[0].GetCustomAttributes())
            {
                var type = attr.GetType();
                if (type.Name == "EventCategory")
                {
                    var nameField = type.GetField("name", BindingFlags.Public | BindingFlags.Instance);
                    if (nameField?.GetValue(attr) is string value)
                    {
                        return value;
                    }

                    var nameProp = type.GetProperty("name", BindingFlags.Public | BindingFlags.Instance);
                    if (nameProp?.GetValue(attr) is string propValue)
                    {
                        return propValue;
                    }
                }
            }

            return null;
        });
    }
}
