using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Reg
{
    public class ResetModel : ModelBase
    {
        private bool _hard { get; set; }
        private string _uId;

        public ResetModel()
        {
            Clear();
        }

        public bool Hard
        {
            get => _hard;
            set
            {
                _hard = value;
                OnPropertyChanged("Hard");
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
            Hard= false;    
            uId = string.Empty;
        }
    }
}
