using IGT.FloorNet.EX.EZPay;
using IGT.FloorNet.EX.Tito;
using IGT.FloorNet.EX.Wat;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat
{
    public class initiateTransferModel : ModelBase
    {
        private string _requestId;
        private string _resourceId;
        private t_idReaderType _idReaderType;
        private string _cardId;
        private t_transferDirection _transferDirection;
        private long _authCashableAmt;
        private long _authPromoAmt;
        private long _authNonCashAmt;
        private t_watException _hostException;
        private long? _currentKeyNumber;
        private string _signature;

        public string requestId
        {
            get => _requestId;
            set { _requestId = value; OnPropertyChanged(nameof(requestId));  }
        }
        public string resourceId
        {
            get => _resourceId;
            set { _resourceId = value; OnPropertyChanged(nameof(resourceId));}
        }
        public t_idReaderType idReaderType
        {
            get => _idReaderType;
            set { _idReaderType = value; OnPropertyChanged(nameof(idReaderType)); }
        }
        public string cardId
        {
            get => _cardId;
            set { _cardId = value; OnPropertyChanged(nameof(cardId)); }
        }
        public t_transferDirection transferDirection
        {
            get => _transferDirection;
            set { _transferDirection = value; OnPropertyChanged(nameof(transferDirection)); }
        }
        public long authCashableAmt
        {
            get => _authCashableAmt;
            set { _authCashableAmt = value; OnPropertyChanged(nameof(authCashableAmt));}
        }
        public long authPromoAmt
        {
            get => _authPromoAmt;
            set { _authPromoAmt = value; OnPropertyChanged(nameof(authPromoAmt)); }
        }
        public long authNonCashAmt
        {
            get => _authNonCashAmt;
            set { _authNonCashAmt = value; OnPropertyChanged(nameof(authNonCashAmt)); }
        }
        public t_watException hostException
        {
            get => _hostException;
            set { _hostException = value; OnPropertyChanged(nameof(hostException)); }
        }
        public long? currentKeyNumber
        {
            get => _currentKeyNumber;
            set { _currentKeyNumber = value; OnPropertyChanged(nameof(currentKeyNumber)); }
        }
        public string signature
        {
            get => _signature;
            set { _signature = value; OnPropertyChanged(nameof(signature));}
        }

        public IEnumerable<t_transferDirection> transferDirectionValues
        {
            get
            {
                return Enum.GetValues(typeof(t_transferDirection)).Cast<t_transferDirection>();
            }
        }

        public IEnumerable<t_watException> hostExceptionValues
        {
            get
            {
                return Enum.GetValues(typeof(t_watException)).Cast<t_watException>();
            }
        }

        public void Clear()
        {
            requestId = string.Empty;
            resourceId = string.Empty;
            idReaderType = t_idReaderType.none;
            cardId = string.Empty;
            transferDirection = t_transferDirection.from_EGM;
            authCashableAmt = 0;
            authPromoAmt = 0;
            authNonCashAmt = 0;
            hostException = t_watException.authorized;
            currentKeyNumber = 0;
            signature = string.Empty;
        }
    }
}
