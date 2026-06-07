using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.evt;
using Pipelines.Sockets.Unofficial.Arenas;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Registration
{
    public class GetDisableKeysModel : ModelBase
    {

        private string _disableKeys;
        private string _uId;

        public string DisableKeys
        {
            get => _disableKeys;
            set
            {
                _disableKeys = value;
                OnPropertyChanged("DisableKeys");
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

        public GetDisableKeysModel()
        {
            Clear();           
        }

        public void Clear()
        {
            DisableKeys = null;
            uId = string.Empty;
        }
    }
}
