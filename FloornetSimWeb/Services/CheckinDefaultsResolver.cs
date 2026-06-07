using IGT.FloorNet.Tools.ServiceSimulator.Models;
using Microsoft.Extensions.Configuration;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services
{
    /// <summary>
    /// Resolves effective Check-in defaults for a given SMIB UID by binding the
    /// global <c>Checkin</c> section from <c>servicesettings.json</c> and applying
    /// any per-UID overrides found in <c>Checkin:Machines[]</c>.
    /// </summary>
    public static class CheckinDefaultsResolver
    {
        /// <summary>
        /// Build the effective <see cref="CheckinSettings"/> for the supplied UID.
        /// When <paramref name="uid"/> is null/empty or has no matching machine entry,
        /// the global defaults are returned unchanged.
        /// </summary>
        public static CheckinSettings Resolve(IConfiguration? configuration, string? uid)
        {
            var settings = new CheckinSettings();
            var section = configuration?.GetSection(CheckinSettings.SectionName);
            if (section != null && section.Exists())
            {
                section.Bind(settings);
            }

            if (string.IsNullOrWhiteSpace(uid))
                return settings;

            var match = settings.Machines?
                .FirstOrDefault(m => string.Equals(m.Uid, uid, System.StringComparison.OrdinalIgnoreCase));
            if (match == null)
                return settings;

            ApplyOverride(settings, match);
            return settings;
        }

        private static void ApplyOverride(CheckinSettings target, MachineCheckinSettings o)
        {
            if (o.SiteId != null) target.SiteId = o.SiteId;
            if (o.MachineNum.HasValue) target.MachineNum = o.MachineNum.Value;
            if (o.MachineLoc != null) target.MachineLoc = o.MachineLoc;
            if (o.Enabled.HasValue) target.Enabled = o.Enabled.Value;
            if (o.Registered.HasValue) target.Registered = o.Registered.Value;
            if (o.NotRegisteredReason != null) target.NotRegisteredReason = o.NotRegisteredReason;
            if (o.Vip.HasValue) target.Vip = o.Vip.Value;
            if (o.ReportDenomId.HasValue) target.ReportDenomId = o.ReportDenomId.Value;
            if (o.PointsCount.HasValue) target.PointsCount = o.PointsCount.Value;
            if (o.PointsAward.HasValue) target.PointsAward = o.PointsAward.Value;
            if (o.MachineStatus != null) target.MachineStatus = o.MachineStatus;
            if (o.HaveInitialMeters.HasValue) target.HaveInitialMeters = o.HaveInitialMeters.Value;
            if (o.TitoEnabled.HasValue) target.TitoEnabled = o.TitoEnabled.Value;
            if (o.TruePlayerWinEnabled.HasValue) target.TruePlayerWinEnabled = o.TruePlayerWinEnabled.Value;
            if (o.MdmgEnabled.HasValue) target.MdmgEnabled = o.MdmgEnabled.Value;
            if (o.HotPlayerPeriod.HasValue) target.HotPlayerPeriod = o.HotPlayerPeriod.Value;
            if (o.HotPlayerWagers.HasValue) target.HotPlayerWagers = o.HotPlayerWagers.Value;
            if (o.HotPlayerGames.HasValue) target.HotPlayerGames = o.HotPlayerGames.Value;
            if (o.HotPlayerInactivityTimer.HasValue) target.HotPlayerInactivityTimer = o.HotPlayerInactivityTimer.Value;
            if (o.BonusEnabled.HasValue) target.BonusEnabled = o.BonusEnabled.Value;
        }

        /// <summary>
        /// Converts a machineStatus string into the single <see cref="char"/> expected by
        /// <c>iReg.setRegistration</c>. Guarantees the result is never the NUL char
        /// (<c>'\0'</c>) — sending a NUL machineStatus to the C++ BE2 has been observed to
        /// destabilise it. Falls back to <c>'A'</c> (Active) for null/empty/NUL input.
        /// </summary>
        public static char ToMachineStatusChar(string? machineStatus)
        {
            if (string.IsNullOrEmpty(machineStatus))
                return 'A';
            var c = machineStatus[0];
            return c == '\0' ? 'A' : c;
        }
    }
}
