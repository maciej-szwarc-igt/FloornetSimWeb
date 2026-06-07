using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent
{
    public class LevelUpdateModel : ModelBase
    {
        private long _levelId = 0;
        private long _hitSeqNum = 0;
        private long _machineLevel = 0;
        private long _amount = 0;
        private long _bbPGIdx = 0;
        private string _bbPGName = string.Empty;
        private bool _isBbpgLevel = false;
        private bool _isProg = false;

        public long LevelId { get => _levelId; set { _levelId = value; OnPropertyChanged(nameof(LevelId)); } }
        public long HitSeqNum { get => _hitSeqNum; set { _hitSeqNum = value; OnPropertyChanged(nameof(HitSeqNum)); } }
        public long MachineLevel { get => _machineLevel; set { _machineLevel = value; OnPropertyChanged(nameof(MachineLevel)); } }
        public long Amount { get => _amount; set { _amount = value; OnPropertyChanged(nameof(Amount)); } }
        public long BbPGIdx { get => _bbPGIdx; set { _bbPGIdx = value; OnPropertyChanged(nameof(BbPGIdx)); } }
        public string BbPGName { get => _bbPGName; set { _bbPGName = value; OnPropertyChanged(nameof(BbPGName)); } }
        public bool IsBbpgLevel { get => _isBbpgLevel; set { _isBbpgLevel = value; OnPropertyChanged(nameof(IsBbpgLevel)); } }

        public bool IsProg { get => _isProg; set { _isProg = value; OnPropertyChanged(nameof(IsProg)); } }

        public void Clear()
        {
            LevelId = 0;
            HitSeqNum = 0;
            MachineLevel = 0;
            Amount = 0;
            BbPGIdx = 0;
            BbPGName = string.Empty;
        }
    }
}
