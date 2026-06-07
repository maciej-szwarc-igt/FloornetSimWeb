using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent
{
    public class LevelResetModel : ModelBase
    {
        private long _levelId = 0;
        private long _hitSeqNum = 0;
        private long _controlStringId = 0;
        private long _newPoolAmount = 0;
        private string _dontPlayUID = string.Empty;
        private long _timeToDisplay = 0;
        private bool _isClcLevel = false;

        public long LevelId { get => _levelId; set { _levelId = value; OnPropertyChanged(nameof(LevelId)); } }
        public long HitSeqNum { get => _hitSeqNum; set { _hitSeqNum = value; OnPropertyChanged(nameof(HitSeqNum)); } }
        public long ControlStringId { get => _controlStringId; set { _controlStringId = value; OnPropertyChanged(nameof(ControlStringId)); } }
        public long NewPoolAmount { get => _newPoolAmount; set { _newPoolAmount = value; OnPropertyChanged(nameof(NewPoolAmount)); } }
        public string DontPlayUID { get => _dontPlayUID; set { _dontPlayUID = value; OnPropertyChanged(nameof(DontPlayUID)); } }
        public long TimeToDisplay { get => _timeToDisplay; set { _timeToDisplay = value; OnPropertyChanged(nameof(TimeToDisplay)); } }
        public bool IsClcLevel { get => _isClcLevel; set { _isClcLevel = value; OnPropertyChanged(nameof(IsClcLevel)); } }

        public void Clear()
        {
            LevelId = 0;
            HitSeqNum = 0;
            ControlStringId = 0;
            NewPoolAmount = 0;
            DontPlayUID = string.Empty;
            TimeToDisplay = 0;
        }
    }
}
