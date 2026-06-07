using IGT.FloorNet.EX.Player;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr
{
    public class EmployeeCardDataModel : ModelBase
    {
        private string _cardId;
        private string _customCardId;
        private bool _sendCustomCardId;
        private t_idReaderType _idReaderType;
        private t_idReaderType _customIdReaderType;
        private bool _sendCustomIdReaderType;
        private long _cardInCount;
        private long _customCardInCount;
        private bool _sendCustomCardInCount;
        private t_cardType _cardType;
        private long _staffId;
        private string _firstName;
        private string _lastName;
        private long _licenseNumber;
        private bool _auditEnabled;
        private bool _cmmEnabled;
        private long _pinCheckLevel;
        private t_userLevel _fJP_UserLevel;
        private long _fJP_MaxFill;
        private long _fJP_MaxJackpots;
        private long _fJP_FillOverride;
        private long _fJP_JPOverride;
        private long _fJP_PouchPayLimit;
        private bool _fJP_KeyToCreditEnabled;
        private long _fJP_FillAmount;
        private long _fJP_FillBags;
        private long _fJP_AuxBagCount;
        private long _fJP_MaxAuxBagCount;

        private string _rpcSmibUid;

        public string CardId 
        {
            get => _cardId;
            set { _cardId = value; OnPropertyChanged(nameof(CardId)); }
        }
        public string CustomCardId
        {
            get => _customCardId;
            set { _customCardId = value; OnPropertyChanged(nameof(CustomCardId)); }
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
        public t_idReaderType CustomIdReaderType
        {
            get => _customIdReaderType;
            set { _customIdReaderType = value; OnPropertyChanged(nameof(CustomIdReaderType)); }
        }
        public bool SendCustomIdReaderType
        {
            get => _sendCustomIdReaderType;
            set { _sendCustomIdReaderType = value; OnPropertyChanged(nameof(SendCustomIdReaderType)); }
        }
        public long CardInCount
        {
            get => _cardInCount;
            set { _cardInCount = value; OnPropertyChanged(nameof(CardInCount)); }
        }
        public long CustomCardInCount
        {
            get => _customCardInCount;
            set { _customCardInCount = value; OnPropertyChanged(nameof(CustomCardInCount)); }
        }
        public bool SendCustomCardInCount
        {
            get => _sendCustomCardInCount;
            set { _sendCustomCardInCount = value; OnPropertyChanged(nameof(SendCustomCardInCount)); }
        }
        public t_cardType CardType
        {
            get => _cardType;
            set { _cardType = value; OnPropertyChanged(nameof(CardType)); }
        }
        public long StaffId
        {
            get => _staffId;
            set { _staffId = value; OnPropertyChanged(nameof(StaffId)); }
        }
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(nameof(LastName)); }
        }
        public long LicenseNumber
        {
            get => _licenseNumber;
            set { _licenseNumber = value; OnPropertyChanged(nameof(LicenseNumber)); }
        }
        public bool AuditEnabled
        {
            get => _auditEnabled;
            set { _auditEnabled = value; OnPropertyChanged(nameof(AuditEnabled)); }
        }
        public bool CmmEnabled
        {
            get => _cmmEnabled;
            set { _cmmEnabled = value; OnPropertyChanged(nameof(CmmEnabled)); }
        }
        public long PinCheckLevel
        {
            get => _pinCheckLevel;
            set { _pinCheckLevel = value; OnPropertyChanged(nameof(PinCheckLevel)); }
        }
        public t_userLevel FJP_UserLevel
        {
            get => _fJP_UserLevel;
            set { _fJP_UserLevel = value; OnPropertyChanged(nameof(FJP_UserLevel)); }
        }
        public long FJP_MaxFill
        {
            get => _fJP_MaxFill;
            set { _fJP_MaxFill = value; OnPropertyChanged(nameof(FJP_MaxFill)); }
        }
        public long FJP_MaxJackpots
        {
            get => _fJP_MaxJackpots;
            set { _fJP_MaxJackpots = value; OnPropertyChanged(nameof(FJP_MaxJackpots)); }
        }
        public long FJP_FillOverride
        {
            get => _fJP_FillOverride;
            set { _fJP_FillOverride = value; OnPropertyChanged(nameof(FJP_FillOverride)); }
        }
        public long FJP_JPOverride
        {
            get => _fJP_JPOverride;
            set { _fJP_JPOverride = value; OnPropertyChanged(nameof(FJP_JPOverride)); }
        }
        public long FJP_PouchPayLimit
        {
            get => _fJP_PouchPayLimit;
            set { _fJP_PouchPayLimit = value; OnPropertyChanged(nameof(FJP_PouchPayLimit)); }
        }
        public bool FJP_KeyToCreditEnabled
        {
            get => _fJP_KeyToCreditEnabled;
            set { _fJP_KeyToCreditEnabled = value; OnPropertyChanged(nameof(FJP_KeyToCreditEnabled)); }
        }
        public long FJP_FillAmount
        {
            get => _fJP_FillAmount;
            set { _fJP_FillAmount = value; OnPropertyChanged(nameof(FJP_FillAmount)); }
        }
        public long FJP_FillBags
        {
            get => _fJP_FillBags;
            set { _fJP_FillBags = value; OnPropertyChanged(nameof(FJP_FillBags)); }
        }
        public long FJP_AuxBagCount
        {
            get => _fJP_AuxBagCount;
            set { _fJP_AuxBagCount = value; OnPropertyChanged(nameof(FJP_AuxBagCount)); }
        }
        public long FJP_MaxAuxBagCount
        {
            get => _fJP_MaxAuxBagCount;
            set { _fJP_MaxAuxBagCount = value; OnPropertyChanged(nameof(FJP_MaxAuxBagCount)); }
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

        public IEnumerable<t_idReaderType> CustomidReaderTypeResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_idReaderType)).Cast<t_idReaderType>();
            }
        }

        public IEnumerable<t_cardType> CardTypeResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_cardType)).Cast<t_cardType>();
            }
        }

        public IEnumerable<t_userLevel> FJP_UserLevelResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_userLevel)).Cast<t_userLevel>();
            }
        }

        public void Clear()
        {
            CardId = string.Empty;
            CustomCardId = string.Empty;
            SendCustomCardId = false;
            IdReaderType = t_idReaderType.none;
            CustomIdReaderType = t_idReaderType.none;
            SendCustomIdReaderType = false;
            CardInCount = 0;
            CustomCardInCount = 0;
            SendCustomCardInCount = false;
            CardType = t_cardType.invalid;
            StaffId = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            LicenseNumber = 0;
            AuditEnabled = false;
            CmmEnabled = false;
            PinCheckLevel = 0;
            FJP_UserLevel = 0;
            FJP_MaxFill = 0;
            FJP_MaxJackpots = 0;
            FJP_FillOverride = 0;
            FJP_JPOverride = 0;
            FJP_PouchPayLimit = 0;
            FJP_KeyToCreditEnabled = false;
            FJP_FillAmount = 0;
            FJP_FillBags = 0;
            FJP_AuxBagCount = 0;
            FJP_MaxAuxBagCount = 0;
    }

    }




}
