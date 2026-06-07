using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat.WatEvents
{
    public class WatConfigModel : ModelBase
    {
        private bool _enable;
        private bool _cashoutInterceptEnable;
        private bool _autoTransferOnCardIn;
        private bool _autoTransferOnCardOut;
        private bool _noCardBvDisable;
        private bool _autoPlayDisable;
        private bool _noPinAtEGM;

        public bool Enable
        {
            get => _enable;
            set { _enable = value; OnPropertyChanged(nameof(Enable)); }
        }

        public bool CashoutInterceptEnable
        {
            get => _cashoutInterceptEnable;
            set { _cashoutInterceptEnable = value; OnPropertyChanged(nameof(CashoutInterceptEnable)); }
        }

        public bool AutoTransferOnCardIn
        {
            get => _autoTransferOnCardIn;
            set { _autoTransferOnCardIn = value; OnPropertyChanged(nameof(AutoTransferOnCardIn)); }
        }

        public bool AutoTransferOnCardOut
        {
            get => _autoTransferOnCardOut;
            set { _autoTransferOnCardOut = value; OnPropertyChanged(nameof(AutoTransferOnCardOut)); }
        }

        public bool NoCardBvDisable
        {
            get => _noCardBvDisable;
            set { _noCardBvDisable = value; OnPropertyChanged(nameof(NoCardBvDisable)); }
        }

        public bool AutoPlayDisable
        {
            get => _autoPlayDisable;
            set { _autoPlayDisable = value; OnPropertyChanged(nameof(AutoPlayDisable)); }
        }

        public bool NoPinAtEGM
        {
            get => _noPinAtEGM;
            set { _noPinAtEGM = value; OnPropertyChanged(nameof(NoPinAtEGM)); }
        }

        public void Clear()
        {

        }


    }
}
