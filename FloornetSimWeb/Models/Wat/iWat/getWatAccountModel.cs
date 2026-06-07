using IGT.FloorNet.EX.EZPay;
using IGT.FloorNet.EX.Wat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat
{
    public class getWatAccountModel : ModelBase
    {
        private string _cardId;
        private List<t_watAccount> _watAccountList;
        private t_watException _hostException;

        /// <summary>
        /// t_watAccount attributes
        /// </summary>
        private string _accountId;
        private bool _authRequired;
        private t_creditType _creditType;
        private bool _withdrawOk;
        private bool _depositOk;
        private bool _selectAmt;
        private long _defaultAmt;
        private long _withdrawMax;
        private long _withdrawMin;
        private long _depositMax;
        private long _depositMin;
        private long _balance;
        private t_accountState _accountState;
        private DateTime? _expirationDate;

        public string CardId 
        { 
            get => _cardId;
            set { _cardId = value; OnPropertyChanged(nameof(CardId)); }
        }
        public List<t_watAccount> WatAccountsList
        {
            get => _watAccountList;
            set { _watAccountList = value; OnPropertyChanged(nameof(WatAccountsList)); }
        }

        public string accountId
        {
            get => _accountId;
            set { _accountId = value; OnPropertyChanged(nameof(accountId));}
        }
        public bool authRequired
        {
            get => _authRequired;
            set { _authRequired = value; OnPropertyChanged(nameof(authRequired)); }
        }
        public t_creditType creditType
        {
            get => _creditType;
            set { _creditType = value; OnPropertyChanged(nameof(creditType)); }
        }
        public bool withdrawOk
        {
            get => _withdrawOk;
            set { _withdrawOk = value; OnPropertyChanged(nameof(withdrawOk)); }
        }
        public bool depositOk
        {
            get => _depositOk;
            set { _depositOk = value; OnPropertyChanged(nameof(depositOk)); } 
        }
        public bool selectAmt
        {
            get => _selectAmt;
            set { _selectAmt = value; OnPropertyChanged(nameof(selectAmt)); }
        }
        public long defaultAmt
        {
            get => _defaultAmt;
            set { _defaultAmt = value; OnPropertyChanged(nameof(defaultAmt)); }
        }
        public long withdrawMax
        {
            get => _withdrawMax;
            set { _withdrawMax = value; OnPropertyChanged(nameof(withdrawMax)); }
        }
        public long withdrawMin
        {
            get => _withdrawMin;
            set { _withdrawMin = value; OnPropertyChanged(nameof(withdrawMin)); }
        }
        public long depositMax
        {
            get => _depositMax;
            set {_depositMax=value; OnPropertyChanged(nameof(depositMax));}
        }
        public long depositMin
        {
            get => _depositMin;
            set { _depositMin = value; OnPropertyChanged(nameof(depositMin)); }
        }
        public long balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(nameof(balance));}
        }
        public t_accountState accountState
        {
            get => _accountState;
            set { _accountState = value; OnPropertyChanged(nameof(accountState)); }
        }
        public DateTime? expirationDate
        {
            get => _expirationDate;
            set { _expirationDate = value; OnPropertyChanged(nameof(expirationDate)); }
        }

        public t_watException HostException 
        {
            get => _hostException; 
            set { _hostException = value; OnPropertyChanged(nameof(HostException)); } 
        }

        public IEnumerable<t_watException> WatExceptionValues
        {
            get
            {
                return Enum.GetValues(typeof(t_watException)).Cast<t_watException>();
            }
        }
        public IEnumerable<t_accountState> AccountStateValues
        {
            get
            {
                return Enum.GetValues(typeof(t_accountState)).Cast<t_accountState>();
            }
        }
        public IEnumerable<t_creditType> CreditTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(t_creditType)).Cast<t_creditType>();
            }
        }

        public void Clear()
        { 
            CardId = string.Empty;
            WatAccountsList.Clear();
            HostException = t_watException.no_account;

            accountId = string.Empty;
            authRequired = false;
            creditType = t_creditType.promoPool;
            withdrawOk = false;
            depositOk = false;
            selectAmt = false;
            defaultAmt = 0;
            withdrawMax = 0;
            withdrawMin = 0;
            depositMax = 0;
            depositMin = 0;
            balance = 0;
            accountState = t_accountState.ENABLED;
            expirationDate = DateTime.Now;

        }
    }
}
