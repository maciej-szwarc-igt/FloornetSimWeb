using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.RG
{
    public class DisableEMGOnCardOutModel : ModelBase
    {

        private bool _state;
        private string _disableKey;        
        private string _uId;

        public DisableEMGOnCardOutModel()
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

        public string DisableKey
        {
            get => _disableKey;
            set
            {
                _disableKey = value;
                OnPropertyChanged("DisableKey");
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
            State = true;
            DisableKey = string.Empty;
            uId = string.Empty;
        }
    }
}
