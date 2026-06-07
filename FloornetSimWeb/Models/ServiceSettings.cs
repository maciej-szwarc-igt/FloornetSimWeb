namespace IGT.FloorNet.Tools.ServiceSimulator.Models
{
    /// <summary>
    /// Strongly-typed configuration bound to servicesettings.json.
    /// Supports real-time reload via IOptionsMonitor when the file is edited on disk.
    /// Each section is independently bindable (e.g., "Discovery", "Features").
    /// </summary>
    public class CheckinSettings
    {
        public const string SectionName = "Checkin";
        public string SiteId { get; set; } = "1";
        public long MachineNum { get; set; } = 100;
        public string MachineLoc { get; set; } = "S01B0101";

        // Registration parameters (previously hardcoded in EventProcessors/KeepAliveEventHandler)
        public bool Enabled { get; set; } = true;
        public bool Registered { get; set; } = true;
        public string? NotRegisteredReason { get; set; }
        public bool Vip { get; set; } = false;
        public long ReportDenomId { get; set; } = 0;
        public long PointsCount { get; set; } = 0;
        public long PointsAward { get; set; } = 0;
        public string MachineStatus { get; set; } = "A";
        public bool HaveInitialMeters { get; set; } = false;
        public bool TitoEnabled { get; set; } = false;
        public bool TruePlayerWinEnabled { get; set; } = false;
        public bool MdmgEnabled { get; set; } = false;
        public long HotPlayerPeriod { get; set; } = 0;
        public long HotPlayerWagers { get; set; } = 0;
        public long HotPlayerGames { get; set; } = 0;
        public long HotPlayerInactivityTimer { get; set; } = 0;
        public bool BonusEnabled { get; set; } = false;

        /// <summary>
        /// Optional per-SMIB-UID overrides. When a UID matches an entry here,
        /// the non-null/overridden values are applied on top of the global defaults above.
        /// </summary>
        public List<MachineCheckinSettings> Machines { get; set; } = new();
    }

    /// <summary>
    /// Per-SMIB-UID Check-in override. Any property left null/unset falls back to the
    /// global <see cref="CheckinSettings"/> values. Value-type props use nullable so an
    /// absent JSON key means "inherit the global default".
    /// </summary>
    public class MachineCheckinSettings
    {
        public string Uid { get; set; } = "";
        public string? SiteId { get; set; }
        public long? MachineNum { get; set; }
        public string? MachineLoc { get; set; }
        public bool? Enabled { get; set; }
        public bool? Registered { get; set; }
        public string? NotRegisteredReason { get; set; }
        public bool? Vip { get; set; }
        public long? ReportDenomId { get; set; }
        public long? PointsCount { get; set; }
        public long? PointsAward { get; set; }
        public string? MachineStatus { get; set; }
        public bool? HaveInitialMeters { get; set; }
        public bool? TitoEnabled { get; set; }
        public bool? TruePlayerWinEnabled { get; set; }
        public bool? MdmgEnabled { get; set; }
        public long? HotPlayerPeriod { get; set; }
        public long? HotPlayerWagers { get; set; }
        public long? HotPlayerGames { get; set; }
        public long? HotPlayerInactivityTimer { get; set; }
        public bool? BonusEnabled { get; set; }
    }

    public class DownloadSettings
    {
        public const string SectionName = "DownloadService";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string IpAddress { get; set; } = "https://localhost:5003";
    }

    public class DiscoverySettings
    {
        public const string SectionName = "Registration:Discovery";
        public bool CardlessInterface { get; set; } = true;
        public bool EftInterface { get; set; } = true;
        public bool AMLInterface { get; set; } = true;
        public bool PlayerInterface { get; set; } = true;
        public bool BonusInterface { get; set; } = true;
        public bool DiagsInterface { get; set; } = true;
        public bool GatInterface { get; set; } = true;
        public bool ConfInterface { get; set; } = true;
        public bool DownloadInterface { get; set; } = true;
        public bool TitoInterface { get; set; } = true;
        public bool PinInterface { get; set; } = true;
        public bool MarkerInterface { get; set; } = true;
        public bool RGInterface { get; set; } = true;
        public bool RegInterface { get; set; } = true;
        public bool HandpayInterface { get; set; } = true;
        public bool PCSInterface { get; set; } = true;
        public bool IsmInterface { get; set; } = true;
        public bool WatInterface { get; set; } = true;
    }

    public class RegistrationSettings
    {
        public const string SectionName = "Registration";
        public bool AutoRegister { get; set; } = true;
        public DiscoverySettings Discovery { get; set; } = new();
    }

    public class FeatureSettings
    {
        public const string SectionName = "Features";
        public bool EnableCardService { get; set; } = true;
        public bool EnableBonusMeters { get; set; } = true;
        public bool EnableAuditEvents { get; set; } = true;
        public bool EnableResponseView { get; set; } = true;
    }
}
