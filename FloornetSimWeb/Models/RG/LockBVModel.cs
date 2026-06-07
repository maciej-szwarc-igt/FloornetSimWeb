using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.RG
{
    public  class LockBVModel : ModelBase
    {

        private bool _disable;
        private string _lockKey;        
        private string _uId;

        public LockBVModel()
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

        public void Clear()
        {
            Disable = true;
            LockKey = string.Empty;            
            uId = string.Empty;
        }
    }
}
