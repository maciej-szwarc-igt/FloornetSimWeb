using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr
{
    public class ForceCardInModel : ModelBase
    {
        private string _cardId;
        private t_idReaderType _idReaderType;
        private bool _isCardUnregistered;
        private bool _isBadCard;
        private string _rpcSmibUid;

        private bool _sendCustomCardId;
        private string _cardIdFromCardInRPC;

        public string CardId
        {
            get => _cardId;
            set { _cardId = value; OnPropertyChanged(nameof(CardId)); }
        }

        public bool SendCustomCardId
        {
            get => _sendCustomCardId;
            set { _sendCustomCardId = value; OnPropertyChanged(nameof(SendCustomCardId)); }
        }

        public t_idReaderType IdReaderType
        {
            get => _idReaderType;
            set { _idReaderType = value; OnPropertyChanged(nameof(IdReaderType)); }
        }
        public bool IsCardUnregistered
        {
            get => _isCardUnregistered;
            set { _isCardUnregistered = value; OnPropertyChanged(nameof(IsCardUnregistered)); }
        }

        public bool IsBadCard
        {
            get => _isBadCard;
            set { _isBadCard = value; OnPropertyChanged(nameof(IsBadCard)); }
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

        public string CardIdFromCardInRPC
        {
            get => _cardIdFromCardInRPC;
            set { _cardIdFromCardInRPC = value; OnPropertyChanged(nameof(CardIdFromCardInRPC)); }
        }

        public IEnumerable<t_idReaderType> ReaderTypeResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_idReaderType)).Cast<t_idReaderType>();
            }
        }

        public void Clear()
        {
            CardId = null;
            IdReaderType = 0;
            IsCardUnregistered = false;
            IsBadCard = false;
            RpcSmibUid = null;
        }
    }
}
