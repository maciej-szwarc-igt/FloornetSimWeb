using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.EX.Gat;
using IGT.FloorNet.EX.Meters;
using IGT.FloorNet.EX.Player.evt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc
{
    public class IMetersModel : ModelBase
    {
        private string _getMetersUID;
        private string _getMgaDescriptionsUID;
        private char _meterType;

        public IMetersModel()
        {
            MeterTypeList = new List<char> {'\0', 'M', 'I', 'F', 'L', 'H', 'S', 'K', 'J', 'X', 'Q', 'A', 'T', 'D', 'G', 'E', 'Y', 'Z', 'V' };
            Clear();
        }

        public List<char> MeterTypeList { get; set; }
        public string GetMetersUID { get => _getMetersUID; set { _getMetersUID = value; OnPropertyChanged(nameof(GetMetersUID)); } }
        public string GetMgaDescriptionsUID { get => _getMgaDescriptionsUID; set { _getMgaDescriptionsUID = value; OnPropertyChanged(nameof(GetMgaDescriptionsUID)); } }
        public char MeterType { get => _meterType; set { _meterType = value; OnPropertyChanged(nameof(MeterType)); } }

        public void Clear()
        {
            GetMetersUID = string.Empty;
            GetMgaDescriptionsUID = string.Empty;
            MeterType = '\0';
        }
    }
}
