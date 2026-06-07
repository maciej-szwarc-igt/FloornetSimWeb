using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr
{
    public class ForceCardOutModel : ModelBase
    {

        private string _cardId;
        private long _cardInCount;
        private string _rpcSmibUid;

        private bool _sendCustomCardId;
        private bool _sendCustomCardOut;

        private string _cardIdFromCardInRPC;
        private long _cardInCountFromCardinRPC;

        public string CardId
        {
            get { return _cardId; }
            set { _cardId = value; OnPropertyChanged(nameof(CardId)); }
        }

        public long CardInCount
        {
            get { return _cardInCount; }
            set { _cardInCount = value; OnPropertyChanged(nameof(CardInCount)); }
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

        public bool SendCustomCardId
        {
            get => _sendCustomCardId;
            set { _sendCustomCardId = value; OnPropertyChanged(nameof(SendCustomCardId)); }
        }

        public bool SendCustomCardInCount
        {
            get => _sendCustomCardOut;
            set { _sendCustomCardOut = value; OnPropertyChanged(nameof(SendCustomCardInCount)); }
        }

        public string CardIdFromCardInRPC
        {
            get => _cardIdFromCardInRPC;
            set { _cardIdFromCardInRPC = value; OnPropertyChanged(nameof(CardIdFromCardInRPC)); }
        }

        public long CardInCountFromCardInRPC
        {
            get => _cardInCountFromCardinRPC;
            set { _cardInCountFromCardinRPC = value; OnPropertyChanged(nameof(CardInCountFromCardInRPC)); }
        }


        public void Clear()
        {
            CardId = null;
            CardInCount = 0;
            RpcSmibUid = string.Empty;
            SendCustomCardId = false;
            SendCustomCardInCount = false;
            CardIdFromCardInRPC = string.Empty;
            CardInCountFromCardInRPC = 0;
            
        }

    }
}
