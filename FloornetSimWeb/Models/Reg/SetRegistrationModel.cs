using IGT.FloorNet.EX.Player;
using IGT.FloorNet.EX.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Reg
{
    public class SetRegistrationModel : ModelBase
    {
        private bool _enabled;
        private bool _registered;
        private string _notRegisteredReason;
        private bool _vip;
        private long _machineNum;
        private string _machineLoc;
        private string _siteId;
        private long _reportDenomId;
        private long _pointsCount;
        private long _pointsAward;
        private char _machineStatus;
        private t_machineStatuses _machineStatusesSelected;
        private bool _haveInitialMeters;
        private bool _titoEnabled;
        private bool _truePlayerWinEnabled;
        private bool _mdmgEnabled;
        private long _hotPlayerPeriod;
        private long _hotPlayerWagers;
        private long _hotPlayerGames;
        private long _hotPlayerInactivityTimer;
        private bool _bonusEnabled;

        private string _uId;


        public bool Enable
        {
            get => _enabled;
            set { _enabled = value; OnPropertyChanged(nameof(Enable)); }
        }

        public bool Registered
        {
            get => _registered;
            set { _registered = value; OnPropertyChanged(nameof(Registered)); }
        }

        public string NotRegisteredReason
        {
            get => _notRegisteredReason;
            set { _notRegisteredReason = value; OnPropertyChanged(nameof(NotRegisteredReason)); }
        }

        public bool Vip
        {
            get => _vip;
            set { _vip = value; OnPropertyChanged(nameof(Vip)); }
        }

        public long MachineNum
        {
            get => _machineNum;
            set { _machineNum = value; OnPropertyChanged(nameof(MachineNum)); }
        }

        public string MachineLoc
        {
            get => _machineLoc;
            set { _machineLoc = value; OnPropertyChanged(nameof(MachineLoc)); }
        }

        public string SiteId
        {
            get => _siteId;
            set { _siteId = value; OnPropertyChanged(nameof(SiteId)); }
        }

        public long ReportDenomId
        {
            get => _reportDenomId;
            set { _reportDenomId = value; OnPropertyChanged(nameof(ReportDenomId)); }
        }

        public long PointsCount
        {
            get => _pointsCount;
            set { _pointsCount = value; OnPropertyChanged(nameof(PointsCount)); }
        }

        public long PointsAward
        {
            get => _pointsAward;
            set { _pointsAward = value; OnPropertyChanged(nameof(PointsAward)); }
        }

        public char MachineStatus
        {
            get => _machineStatus;
            set { _machineStatus = value; OnPropertyChanged(nameof(MachineStatus)); }
        }
        public t_machineStatuses MachineStatusesSelected
        {
            get => _machineStatusesSelected;
            set { _machineStatusesSelected = value; OnPropertyChanged(nameof(MachineStatusesSelected)); }
        }

        public bool HaveInitialMeters
        {
            get => _haveInitialMeters;
            set { _haveInitialMeters = value; OnPropertyChanged(nameof(HaveInitialMeters)); }
        }

        public bool TitoEnabled
        {
            get => _titoEnabled;
            set { _titoEnabled = value; OnPropertyChanged(nameof(TitoEnabled)); }
        }

        public bool TruePlayerWinEnabled
        {
            get => _truePlayerWinEnabled;
            set { _truePlayerWinEnabled = value; OnPropertyChanged(nameof(TruePlayerWinEnabled)); }
        }

        public bool MdmgEnabled
        {
            get => _mdmgEnabled;
            set { _mdmgEnabled = value; OnPropertyChanged(nameof(MdmgEnabled)); }
        }
        public long HotPlayerPeriod
        {
            get => _hotPlayerPeriod;
            set { _hotPlayerPeriod = value; OnPropertyChanged(nameof(HotPlayerPeriod)); }
        }
        public long HotPlayerWagers
        {
            get => _hotPlayerWagers;
            set { _hotPlayerWagers = value; OnPropertyChanged(nameof(HotPlayerWagers)); }
        }

        public long HotPlayerGames
        {
            get => _hotPlayerGames;
            set { _hotPlayerGames = value; OnPropertyChanged(nameof(HotPlayerGames)); }
        }

        public long HotPlayerInactivityTimer
        {
            get => _hotPlayerInactivityTimer;
            set { _hotPlayerInactivityTimer = value; OnPropertyChanged(nameof(HotPlayerInactivityTimer)); }
        }

        public bool BonusEnabled
        {
            get => _bonusEnabled;
            set { _bonusEnabled = value; OnPropertyChanged(nameof(BonusEnabled)); }
        }

        public string uId
        {
            get => _uId;
            set
            {
                _uId = value;
                OnPropertyChanged(nameof(uId));
            }
        }

        public IEnumerable<t_machineStatuses> MachineStatusResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_machineStatuses)).Cast<t_machineStatuses>();
            }
        }

        public void Clear()
        {
            Enable = false;
            Registered = false;
            NotRegisteredReason = string.Empty;
            Vip = false;
            MachineNum = 0;
            MachineLoc = string.Empty;
            SiteId = string.Empty;
            ReportDenomId = 0;
            PointsCount = 0;
            PointsAward = 0;
            MachineStatus = '\0';
            MachineStatusesSelected = t_machineStatuses.inactive;
            HaveInitialMeters = false;
            TitoEnabled = false;
            TruePlayerWinEnabled = false;
            MdmgEnabled = false;
            HotPlayerPeriod = 0;
            HotPlayerWagers = 0;
            HotPlayerGames = 0;
            HotPlayerInactivityTimer = 0;
            BonusEnabled = false;
            uId = string.Empty;
        }

    }
}
