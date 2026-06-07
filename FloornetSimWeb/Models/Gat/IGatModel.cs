using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Gat
{
    public class IGatModel : ModelBase
    {
        private long _requestId = 0;
        private string _name1 = "";
        private bool _pass1 = true;
        private string _name2 = "";
        private bool _pass2 = true;
        private string _name3 = "";
        private bool _pass3 = true;
        private bool _sendCustomRequestId = false;
        private bool _sendPassFailComponentData1 = true;
        private bool _sendPassFailComponentData2 = false;
        private bool _sendPassFailComponentData3 = false;

        public long RequestId { get => _requestId; set { _requestId = value; OnPropertyChanged(nameof(RequestId)); } }
        public string Name1 { get => _name1; set { _name1 = value; OnPropertyChanged(nameof(Name1)); } }
        public bool Pass1 { get => _pass1; set { _pass1 = value; OnPropertyChanged(nameof(Pass1)); } }
        public string Name2 { get => _name2; set { _name2 = value; OnPropertyChanged(nameof(Name2)); } }
        public bool Pass2 { get => _pass2; set { _pass2 = value; OnPropertyChanged(nameof(Pass2)); } }
        public string Name3 { get => _name3; set { _name3 = value; OnPropertyChanged(nameof(Name3)); } }
        public bool Pass3 { get => _pass3; set { _pass3 = value; OnPropertyChanged(nameof(Pass3)); } }
        public bool SendPassFailComponentData1 { get => _sendPassFailComponentData1; set { _sendPassFailComponentData1 = value; OnPropertyChanged(nameof(SendPassFailComponentData1)); } }
        public bool SendPassFailComponentData2 { get => _sendPassFailComponentData2; set { _sendPassFailComponentData2 = value; OnPropertyChanged(nameof(SendPassFailComponentData2)); } }
        public bool SendPassFailComponentData3 { get => _sendPassFailComponentData3; set { _sendPassFailComponentData3 = value; OnPropertyChanged(nameof(SendPassFailComponentData3)); } }
        public bool SendCustomRequestId { get => _sendCustomRequestId; set { _sendCustomRequestId = value; OnPropertyChanged(nameof(SendCustomRequestId)); } }


        public void Clear()
        {
            RequestId = 0;
            Name1 = "";
            Pass1 = true;
            Name2 = "";
            Pass2 = true;
            Name3 = "";
            Pass3 = true;
            SendCustomRequestId = false;
            SendPassFailComponentData1 = true;
            SendPassFailComponentData2 = false;
            SendPassFailComponentData3 = false;
        }
    }
}
