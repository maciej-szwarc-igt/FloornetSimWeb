using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Reg
{
    public class LockEgmModel : ModelBase
    {
        private long _timer;
        private bool _state;
        private string _lockKey;
        private string _uId;

        public LockEgmModel()
        {
            Clear();
        }

        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public string LockKey
        {
            get => _lockKey;
            set
            {
                _lockKey = value;
                OnPropertyChanged("LockKey");
            }
        }

        public string uId
        {
            get => _uId;
            set
            {
                _uId = value;
                OnPropertyChanged("UID");
            }
        }

        public long Timer
        {
            get => _timer;
            set
            {
                _timer = value;
                OnPropertyChanged("Timer");
            }
        }

        public void Clear()
        {
            State = true;
            LockKey = string.Empty;
            uId = string.Empty;
            Timer = 0;
        }
    }
}
