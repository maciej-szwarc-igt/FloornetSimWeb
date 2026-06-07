using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent
{
    public class StartAnticipationModel : ModelBase
    {
        private long _levelId = 0;
        private long _hitSeqNum = 0;
        private long _controlStringId = 0;
        private long _timeout = 0;

        public long LevelId { get => _levelId; set { _levelId = value; OnPropertyChanged(nameof(LevelId)); } }
        public long HitSeqNum { get => _hitSeqNum; set { _hitSeqNum = value; OnPropertyChanged(nameof(HitSeqNum)); } }
        public long ControlStringId { get => _controlStringId; set { _controlStringId = value; OnPropertyChanged(nameof(ControlStringId)); } }
        public long Timeout { get => _timeout; set { _timeout = value; OnPropertyChanged(nameof(Timeout)); } }


        public void Clear()
        {
            LevelId = 0;
            HitSeqNum = 0;
            ControlStringId = 0;
            Timeout = 0;
        }
    }
}
