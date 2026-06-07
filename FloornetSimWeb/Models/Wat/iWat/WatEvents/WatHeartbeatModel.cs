using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat.WatEvents
{
    public class WatHeartbeatModel : ModelBase
    {
        private bool _hostAvailable;

        public bool hostAvailable
        {
            get => _hostAvailable;
            set { _hostAvailable = value; OnPropertyChanged(nameof(hostAvailable)); }
        }

        public void Clear()
        {
            hostAvailable = false;
        }

    }
}
