using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.RG
{
    public class LockBVOnCardOut : ModelBase
    {
        private bool _disable;
        private string _lockkey;
        private string _uId;

        public LockBVOnCardOut()
        {
            Clear();
        }

        public bool Disable
        {
            get => _disable;
            set
            {
                _disable = value;
                OnPropertyChanged("Disable");
            }
        }

        public string LockKey
        {
            get => _lockkey;
            set
            {
                _lockkey = value;
                OnPropertyChanged("Lockkey");
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

        public void Clear()
        {
            Disable = true;
            LockKey = string.Empty;
            uId = string.Empty;
        }
    }
}
