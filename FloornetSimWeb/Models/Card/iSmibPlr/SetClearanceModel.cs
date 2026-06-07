using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr
{
    public class SetClearanceModel : ModelBase
    {
        private bool _coinClearance;
        private bool _cashClearance;
        private string _rpcSmibUid;


        public bool CoinClearance
        {
            get => _coinClearance;
            set
            {
                _coinClearance = value;
                OnPropertyChanged(nameof(CoinClearance));
            }
        }
        public bool CashClearance
        {
            get => _cashClearance;
            set
            {
                _cashClearance = value;
                OnPropertyChanged(nameof(CashClearance));
            }
        }

        public string RpcSmibUid
        {
            get => _rpcSmibUid;
            set
            {
                _rpcSmibUid = value;
                OnPropertyChanged(nameof(RpcSmibUid));
            }
        }
    }
}
