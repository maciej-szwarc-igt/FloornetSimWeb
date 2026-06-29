using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Persistence;
using IGT.FloorNet.Tools.ServiceSimulator.Services;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers;

/// <summary>
/// Shared helper for converting a FloorNet meter snapshot's two-dimensional <c>meters</c> array
/// (<c>long[,]</c> of [meterCode, value]) into one <see cref="MeterRecord"/> per code and writing
/// them to the <see cref="SimDbStore"/>. Centralized so every meter handler persists consistently.
/// </summary>
internal static class MeterPersistence
{
    /// <summary>
    /// Flattens the protocol meter array and persists each (code, value) pair.
    /// </summary>
    /// <param name="db">The persistence store (no-op when disabled).</param>
    /// <param name="meterType">The <c>t_meterTypes</c> value as a char (e.g. 'I', 'F', 'M').</param>
    /// <param name="meterTime">The protocol meter time.</param>
    /// <param name="meters">Two-dimensional [code, value] array from the meter event.</param>
    /// <param name="context">The event call context carrying SMIB identity fields.</param>
    public static void Persist(SimDbStore db, char meterType, long meterTime, long[,]? meters, EventCallContext? context)
    {
        if (!db.Enabled || meters is null)
        {
            return;
        }

        var receivedUtc = DateTime.UtcNow;
        var meterTypeStr = meterType == '\0' ? null : meterType.ToString();
        var rows = new List<MeterRecord>(meters.GetLength(0));

        // First dimension is the meter code; second dimension is [code, value].
        for (var i = 0; i < meters.GetLength(0); i++)
        {
            rows.Add(new MeterRecord
            {
                ReceivedUtc = receivedUtc,
                MeterType = meterTypeStr,
                MeterTime = meterTime,
                MeterCode = meters[i, 0],
                Value = meters[i, 1],
                Uid = context?.Uid,
                MachineNum = context?.MachineNum,
                SiteId = context?.SiteId
            });
        }

        db.InsertMeters(rows);
    }
}
