using IGT.FloorNet.EX.Player;
using IGT.FloorNet.EX.Wat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iSmibWat
{
    public class RequestTransferModel : ModelBase
    {

        private string _requestId;
        private string _resourceId;
        private string _cardId;
        private long _playerId;
        private long _cardInCount;
        private t_transferDirection _transferDirection;
        private long _reqCashableAmt;
        private long _reqPromoAmt;
        private long _reqNonCashAmt;
        private bool _printTicket;
        private string _jwt;

        private string _rpcSmibUid;
        private string _machineLocation;
        private long _machineNum;
        private string _siteId;

        public string RpcSmibUid
        {
            get => _rpcSmibUid;
            set
            {
                _rpcSmibUid = value;
                OnPropertyChanged(nameof(RpcSmibUid));
            }
        }

        public string MachineLocation
        {
            get => _machineLocation;
            set
            {
                _machineLocation = value;
                OnPropertyChanged(nameof(MachineLocation));
            }
        }

        public long MachineNum
        {
            get => _machineNum;
            set
            {
                _machineNum = value;
                OnPropertyChanged(nameof(MachineNum));
            }
        }

        public string SiteId
        {
            get => _siteId;
            set
            {
                _siteId = value;
                OnPropertyChanged(nameof(SiteId));
            }
        }

        public string RequestId
        {
            get => _requestId;
            set { _requestId = value; OnPropertyChanged(nameof(RequestId)); }
        }

        public string ResourceId
        {
            get => _resourceId;
            set { _resourceId = value; OnPropertyChanged(nameof(ResourceId)); }
        }

        public string CardId
        {
            get => _cardId;
            set { _cardId = value; OnPropertyChanged(nameof(CardId)); }
        }

        public long PlayerId
        {
            get => _playerId;
            set { _playerId = value; OnPropertyChanged(nameof(PlayerId)); }
        }

        public long CardInCount
        {
            get => _cardInCount;
            set { _cardInCount = value; OnPropertyChanged(nameof(CardInCount)); }
        }

        public t_transferDirection TransferDirection
        {
            get => _transferDirection;
            set { _transferDirection = value; OnPropertyChanged(nameof(TransferDirection)); }
        }

        public long RequestCashableAmt
        {
            get => _reqCashableAmt;
            set { _reqCashableAmt = value; OnPropertyChanged(nameof(RequestCashableAmt)); }
        }

        public long ReqPromoAmt
        {
            get => _reqPromoAmt;
            set { _reqPromoAmt = value;OnPropertyChanged(nameof(ReqPromoAmt)); }
        }

        public long ReqNonCashAmt
        {
            get => _reqNonCashAmt;
            set { _reqNonCashAmt = value; OnPropertyChanged(nameof(ReqNonCashAmt)); }
        }

        public bool PrintTicket
        {
            get => _printTicket;
            set { _printTicket = value; OnPropertyChanged(nameof(PrintTicket)); }
        }

        public string Jwt
        {
            get => _jwt;
            set { _jwt = value; OnPropertyChanged(nameof(Jwt));}
        }

        public IEnumerable<t_transferDirection> TransferDirectionValues
        {
            get
            {
                return Enum.GetValues(typeof(t_transferDirection)).Cast<t_transferDirection>();
            }
        }

        public void Clear()
        {
            RpcSmibUid = string.Empty;
            MachineLocation = string.Empty;
            MachineNum = 0;
            SiteId = string.Empty;
            RequestId = string.Empty;
            ResourceId = string.Empty;
            CardId = string.Empty;
            PlayerId = 0;
            CardInCount = 0;
            TransferDirection = t_transferDirection.from_EGM;
            RequestCashableAmt = 0;
            ReqPromoAmt = 0;
            ReqNonCashAmt = 0;
            PrintTicket = false;
            Jwt = string.Empty;
        }
    }
}
