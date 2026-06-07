using IGT.FloorNet.EX.Tito;
using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Tito
{
    public class RedeemModel : ModelBase
    {
        private long? _voucherAmount;

        private t_creditType _creditType;

        private long? _poolId;

        private DateTime _expirationDateTime;

        private t_hostException _hostException;

        public RedeemModel()
        {
            Clear();
        }

        public long? VoucherAmount
        {
            get => _voucherAmount;
            set
            {
                _voucherAmount = value;
                OnPropertyChanged("VoucherAmount");
            }
        }

        public t_creditType CreditType
        {
            get => _creditType;
            set
            {
                _creditType = value;
                OnPropertyChanged("CreditType");
            }
        }

        public long? PoolId
        {
            get => _poolId;
            set
            {
                _poolId = value;
                OnPropertyChanged("PoolId");
            }
        }

        public DateTime ExpirationDateTime
        {
            get => _expirationDateTime;
            set
            {
                _expirationDateTime = value;
                OnPropertyChanged("ExpirationDateTime");
            }
        }

        public t_hostException HostException
        {
            get => _hostException;
            set
            {
                _hostException = value;
                OnPropertyChanged("HostException");
            }
        }

        public bool IsValid => VoucherAmount != null && PoolId != null && ExpirationDateTime != null;

        public void Clear()
        {
            VoucherAmount = null;
            PoolId = null;
            ExpirationDateTime = DateTime.UtcNow;
            CreditType = t_creditType.cashable;
            HostException = t_hostException.host_success;
        }
    }
}
