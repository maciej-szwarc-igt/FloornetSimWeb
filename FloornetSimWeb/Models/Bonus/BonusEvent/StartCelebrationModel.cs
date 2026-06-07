using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent
{
    public class StartCelebrationModel : ModelBase
    {
        private long _levelId = 0;
        private long _hitSeqNum = 0;
        private long _controlStringId = 0;
        private bool _testEligible = false;
        private bool _carded = false;
        private bool _lockEgm = false;

        public long LevelId { get => _levelId; set { _levelId = value; OnPropertyChanged(nameof(LevelId)); } }
        public long HitSeqNum { get => _hitSeqNum; set { _hitSeqNum = value; OnPropertyChanged(nameof(HitSeqNum)); } }
        public long ControlStringId { get => _controlStringId; set { _controlStringId = value; OnPropertyChanged(nameof(ControlStringId)); } }
        public bool TestEligible { get => _testEligible; set { _testEligible = value; OnPropertyChanged(nameof(TestEligible)); } }
        public bool Carded { get => _carded; set { _carded = value; OnPropertyChanged(nameof(Carded)); } }
        public bool LockEgm { get => _lockEgm; set { _lockEgm = value; OnPropertyChanged(nameof(LockEgm)); } }

        public void Clear() 
        {
            LevelId = 0;
            HitSeqNum = 0;
            ControlStringId = 0;
            TestEligible = false;
            Carded = false;
            LockEgm = false;
        }
    }
}
