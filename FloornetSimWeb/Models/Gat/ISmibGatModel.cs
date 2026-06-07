using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.EX.Gat;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Gat
{
    public class ISmibGatModel : ModelBase
    {
        private string _doVerificationUID;
        private string _getPackageListUID;
        private long _requestId = 0;

        private string _name1 = string.Empty;
        private bool _isHardware1 = false;
        private t_algorithm _algorithm1 = t_algorithm.crc;
        private long _seed1 = 0;
        private string _salt1 = string.Empty;
        private long _offset1 = 0;
        private bool _sendVerifyComponentData1 = true;

        private string _name2 = string.Empty;
        private bool _isHardware2 = false;
        private t_algorithm _algorithm2 = t_algorithm.crc;
        private long _seed2 = 0;
        private string _salt2 = string.Empty;
        private long _offset2 = 0;
        private bool _sendVerifyComponentData2 = false;

        private string _name3 = string.Empty;
        private bool _isHardware3 = false;
        private t_algorithm _algorithm3 = t_algorithm.crc;
        private long _seed3 = 0;
        private string _salt3 = string.Empty;
        private long _offset3 = 0;
        private bool _sendVerifyComponentData3 = false;

        public string DoVerificationUID { get => _doVerificationUID; set { _doVerificationUID = value; OnPropertyChanged(nameof(DoVerificationUID)); } }
        public string GetPackageListUID { get => _getPackageListUID; set { _getPackageListUID = value; OnPropertyChanged(nameof(GetPackageListUID)); } }
        public long RequestId { get => _requestId; set { _requestId = value; OnPropertyChanged(nameof(RequestId)); } }
        
        public string Name1 { get => _name1; set { _name1 = value; OnPropertyChanged(nameof(Name1)); } }
        public bool IsHardware1 { get => _isHardware1; set { _isHardware1 = value; OnPropertyChanged(nameof(IsHardware1)); } }
        public t_algorithm Algorithm1 { get => _algorithm1; set { _algorithm1 = value; OnPropertyChanged(nameof(Algorithm1)); } }
        public long Seed1 { get => _seed1; set { _seed1 = value; OnPropertyChanged(nameof(Seed1)); } }
        public string Salt1 { get => _salt1; set { _salt1 = value; OnPropertyChanged(nameof(Salt1)); } }
        public long Offset1 { get => _offset1; set { _offset1 = value; OnPropertyChanged(nameof(Offset1)); } }
        public bool SendVerifyComponentData1 { get => _sendVerifyComponentData1; set { _sendVerifyComponentData1 = value; OnPropertyChanged(nameof(SendVerifyComponentData1)); } }

        public string Name2 { get => _name2; set { _name2 = value; OnPropertyChanged(nameof(Name2)); } }
        public bool IsHardware2 { get => _isHardware2; set { _isHardware2 = value; OnPropertyChanged(nameof(IsHardware2)); } }
        public t_algorithm Algorithm2 { get => _algorithm2; set { _algorithm2 = value; OnPropertyChanged(nameof(Algorithm2)); } }
        public long Seed2 { get => _seed2; set { _seed2 = value; OnPropertyChanged(nameof(Seed2)); } }
        public string Salt2 { get => _salt2; set { _salt2 = value; OnPropertyChanged(nameof(Salt2)); } }
        public long Offset2 { get => _offset2; set { _offset2 = value; OnPropertyChanged(nameof(Offset2)); } }

        public bool SendVerifyComponentData2 { get => _sendVerifyComponentData2; set { _sendVerifyComponentData2 = value; OnPropertyChanged(nameof(SendVerifyComponentData2)); } }

        public string Name3 { get => _name3; set { _name3 = value; OnPropertyChanged(nameof(Name3)); } }
        public bool IsHardware3 { get => _isHardware3; set { _isHardware3 = value; OnPropertyChanged(nameof(IsHardware3)); } }
        public t_algorithm Algorithm3 { get => _algorithm3; set { _algorithm3 = value; OnPropertyChanged(nameof(Algorithm3)); } }
        public long Seed3 { get => _seed3; set { _seed3 = value; OnPropertyChanged(nameof(Seed3)); } }
        public string Salt3 { get => _salt3; set { _salt3 = value; OnPropertyChanged(nameof(Salt3)); } }
        public long Offset3 { get => _offset3; set { _offset3 = value; OnPropertyChanged(nameof(Offset3)); } }

        public bool SendVerifyComponentData3 { get => _sendVerifyComponentData3; set { _sendVerifyComponentData3 = value; OnPropertyChanged(nameof(SendVerifyComponentData3)); } }



        public IEnumerable<t_algorithm> AlgorithmValues
        {
            get
            {
                return Enum.GetValues(typeof(t_algorithm)).Cast<t_algorithm>();
            }
        }

        public void Clear()
        {
            RequestId = 0;

            Name1 = string.Empty;
            IsHardware1 = false;
            Algorithm1 = t_algorithm.crc;
            Seed1 = 0;
            Salt1 = string.Empty;
            Offset1 = 0; 
            SendVerifyComponentData1 = true;

            Name2 = string.Empty;
            IsHardware2 = false;
            Algorithm2 = t_algorithm.crc;
            Seed2 = 0;
            Salt2 = string.Empty;
            Offset2 = 0;
            SendVerifyComponentData2 = false;

            Name3 = string.Empty;
            IsHardware3 = false;
            Algorithm3 = t_algorithm.crc;
            Seed3 = 0;
            Salt3 = string.Empty;
            Offset3 = 0;
            SendVerifyComponentData3 = false;
        }   
    }
}
