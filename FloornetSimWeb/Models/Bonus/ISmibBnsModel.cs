using IGT.FloorNet.EX.Bonus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus
{
    public class ISmibBnsModel : ModelBase
    {
        private long _levelId;
        private long _hitSeqNum;
        private bool _celebration;
        private long _controlStringId;
        private t_bonusCode _type;
        private long _defaultAmt;
        private long _cardedAmt;
        private t_payTo _payTo;
        private t_payMethod _payMethod;
        private bool _carded;
        private bool _ackRequired;
        private bool _lockUntilAcked;
        private bool _testEligible;
        private bool _payAbandoned;
        private long _mjtDefaultLen;
        private long _mjtCardedlen;
        private long _mjtExpireTime;
        private long _mjtMinWin;
        private long _mjtMaxWin;
        private string _cardId;
        private long _playerId;
        private bool _mjtRollUp;
        private long _ackTimeOut;
        private long _messageTimeout;
        private long _timestamp;
        private string _uId;
        private bool _sendInvalidSignature;
        private long _machineLevel;
        private long _preferredAmt;
        private long _mjtPreferredLen;
        

        public ISmibBnsModel()
        {
            Clear();
        }

        public long LevelId { get => _levelId; set { _levelId = value; OnPropertyChanged(nameof(LevelId)); } }
        public long HitSeqNum { get => _hitSeqNum; set { _hitSeqNum = value; OnPropertyChanged(nameof(HitSeqNum)); } }
        public bool Celebration { get => _celebration; set { _celebration = value; OnPropertyChanged(nameof(Celebration)); } }
        public long ControlStringId { get => _controlStringId; set { _controlStringId = value; OnPropertyChanged(nameof(ControlStringId)); } }
        public t_bonusCode Type { get => _type; set { _type = value; OnPropertyChanged(nameof(Type)); } }
        public long DefaultAmt { get => _defaultAmt; set { _defaultAmt = value; OnPropertyChanged(nameof(DefaultAmt)); } }
        public long CardedAmt { get => _cardedAmt; set { _cardedAmt = value; OnPropertyChanged(nameof(CardedAmt)); } }
        public t_payTo PayTo { get => _payTo; set { _payTo = value; OnPropertyChanged(nameof(PayTo)); } }
        public t_payMethod PayMethod { get => _payMethod; set { _payMethod = value; OnPropertyChanged(nameof(PayMethod)); } }
        public bool Carded { get => _carded; set { _carded = value; OnPropertyChanged(nameof(Carded)); } }
        public bool AckRequired { get => _ackRequired; set { _ackRequired = value; OnPropertyChanged(nameof(AckRequired)); } }
        public bool LockUntilAcked { get => _lockUntilAcked; set { _lockUntilAcked = value; OnPropertyChanged(nameof(LockUntilAcked)); } }
        public bool TestEligible { get => _testEligible; set { _testEligible = value; OnPropertyChanged(nameof(TestEligible)); } }
        public bool PayAbandoned { get => _payAbandoned; set { _payAbandoned = value; OnPropertyChanged(nameof(PayAbandoned)); } }
        public long MjtDefaultLen { get => _mjtDefaultLen; set { _mjtDefaultLen = value; OnPropertyChanged(nameof(MjtDefaultLen)); } }
        public long MjtCardedLen { get => _mjtCardedlen; set { _mjtCardedlen = value; OnPropertyChanged(nameof(MjtCardedLen)); } }
        public long MjtExpireTime { get => _mjtExpireTime; set { _mjtExpireTime = value; OnPropertyChanged(nameof(MjtExpireTime)); } }
        public long MjtMinWin { get => _mjtMinWin; set { _mjtMinWin = value; OnPropertyChanged(nameof(MjtMinWin)); } }
        public long MjtMaxWin { get => _mjtMaxWin; set { _mjtMaxWin = value; OnPropertyChanged(nameof(MjtMaxWin)); } }
        public string CardId { get => _cardId; set { _cardId = value; OnPropertyChanged(nameof(CardId)); } }
        public long PlayerId { get => _playerId; set { _playerId = value; OnPropertyChanged(nameof(PlayerId)); } }
        public bool MjtRollUp { get => _mjtRollUp; set { _mjtRollUp = value; OnPropertyChanged(nameof(MjtRollUp)); } }
        public long AckTimeout { get => _ackTimeOut; set { _ackTimeOut = value; ; OnPropertyChanged(nameof(AckTimeout)); } }
        public long MessageTimeout { get => _messageTimeout; set { _messageTimeout = value; ; OnPropertyChanged(nameof(MessageTimeout)); } }
        public long Timestamp { get => _timestamp; set { _timestamp = value; ; OnPropertyChanged(nameof(Timestamp)); } }
        public string Signature { get; set;  }
        public string UID { get => _uId; set { _uId = value; OnPropertyChanged(nameof(UID)); } }
        public bool SendInvalidSignature { get => _sendInvalidSignature; set { _sendInvalidSignature = value; OnPropertyChanged(nameof(SendInvalidSignature)); } }
        public long MachineLevel { get => _machineLevel; set { _machineLevel = value; OnPropertyChanged(nameof(MachineLevel)); } }
        public long PreferredAmt { get => _preferredAmt; set { _preferredAmt = value; OnPropertyChanged(nameof(PreferredAmt)); } }
        public long MjtPreferredLen { get => _mjtPreferredLen; set { _mjtPreferredLen = value; OnPropertyChanged(nameof(MjtPreferredLen)); } }

        public IEnumerable<t_bonusCode> BonusCodeValues
        {
            get
            {
                return Enum.GetValues(typeof(t_bonusCode)).Cast<t_bonusCode>();
            }
        }
        public IEnumerable<t_payTo> PayToValues
        {
            get
            {
                return Enum.GetValues(typeof(t_payTo)).Cast<t_payTo>();
            }
        }
        public IEnumerable<t_payMethod> PayMethodValues
        {
            get
            {
                return Enum.GetValues(typeof(t_payMethod)).Cast<t_payMethod>();
            }
        }

        public void Clear()
        {
            LevelId = 0;
            HitSeqNum = 0;
            Celebration = false;
            ControlStringId = 0;
            Type = 0;
            DefaultAmt = 0;
            CardedAmt = 0;
            PayTo = 0;
            PayMethod = 0;
            Carded = false;
            AckRequired = false;
            LockUntilAcked = false;
            TestEligible = false;
            PayAbandoned = false;
            MjtDefaultLen = 0;
            MjtCardedLen = 0;
            MjtExpireTime = 0;
            MjtMinWin = 0;
            MjtMaxWin = 0;
            CardId = string.Empty;
            PlayerId = 0;
            MjtRollUp = false;
            AckTimeout = 0;
            MessageTimeout = 0;
            Timestamp = 0;
            Signature = string.Empty;
            UID = string.Empty;
            MachineLevel = 0;
            PreferredAmt = 0;
            MjtPreferredLen = 0;

    }
}
}
