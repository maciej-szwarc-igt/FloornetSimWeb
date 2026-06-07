using IGT.FloorNet.EX.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr
{
    public class PlayerCardDataModel : ModelBase
    {

        private bool _sendInvalidSignature;

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
        private long _playerId;
        private string _firstName;
        private string _lastName;
        private string  _localeid;
        private bool _sendLocaleidNull;
        private long _pointBalance;

        private long _carryOver;
        private DateTime? _birthdate;
        private bool _sendBirthdateNull;
        private DateTime? _anniversaryDate;
        private bool _sendAnniversaryDateNull;
        private string _ranking;
        private bool _sendRankingNull;
        private bool _pinLock;
        private bool _firstVisit;
        private bool _newMember;
        private bool _banned;
        private bool _selfBanned;
        private long _compBalance;
        private bool _ptpEnable;
        private long _ptpBalance;
        private bool _xcEnable;
        private long _xcAwardBalance;
        private long _xpcAward;
        private bool _ppEnable;
        private long _ppPoolBalance;
        private long _ppLuckyNumber;
        private long _ppTotalWon;
        private double _spXier;
        private DateTime? _spXierStartTime;
        private bool _sendSpXierStartTimeNull;
        private DateTime? _spXierStopTime;
        private bool _sendSpXierStopTimeNull;
        private double _spRankXier;
        private bool _rpEnable;
        private long _rpPointBalance;
        private long _rpEarnedDay;
        private bool _srpEnable;
        private long _srpLevel;
        private long _srpPointBalance;
        private List< t_giftPointItem > _giftPointPrograms;
        private bool _sendGiftPointProgramsNull;
        private List<t_bbpgLevel> _bonusMeters;
        private bool _sendBonusMetersNull;
        private long _srpAward;
        private bool _hidePoints;
        private bool _abandoned;
        private double _partialPointCarryover;
        private bool _dupCard;
        private bool _isVIP;
        private long _spRate;
        private long _spAward;
        private e_pointMeters _pointMeter;
        private long _ptpUsed;
        private double _ptpRate;
        private long _ptpLimit;
        private bool _countUp;
        private bool _zeroUnused;
        private bool _disableCardedBonuses;
        private string _dupLocation;
        private long _maxCashlessTxIn;
        private long _maxCashlessTxOut;
        private long _currentKeyNumber;
        private string _signature;


        private long _levelId;
        private string _poolName;
        private long _meterIdx;

        private long _gifPointItemId;
        private string _gifPointItemName;
        private long _gifPointItemBalance;

        private string _rpcSmibUid;

        public bool SendInvalidSignature
        {
            get { return _sendInvalidSignature; }
            set { _sendInvalidSignature = value; OnPropertyChanged(nameof(SendInvalidSignature)); }
        }

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

        public long PlayerId
        {
            get =>_playerId;
            set { _playerId = value; OnPropertyChanged(nameof(PlayerId)); }
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

        public string LocaleId
        {
            get => _localeid;
            set { _localeid = value; OnPropertyChanged(nameof(LocaleId)); }
        }

        public bool SendLocaleIdNull
        {
            get => _sendLocaleidNull;
            set { _sendLocaleidNull = value; OnPropertyChanged(nameof(SendLocaleIdNull)); }
        }

        public long PointBalance
        {
            get => _pointBalance;
            set { _pointBalance = value; OnPropertyChanged(nameof(PointBalance)); }
        }

        public long CarryOver 
        {
            get => _carryOver;
            set { _carryOver = value; OnPropertyChanged(nameof(CarryOver)); }
        }

        public DateTime? Birthdate
        {
            get => _birthdate;
            set { _birthdate = value; OnPropertyChanged(nameof(Birthdate)); }
        }

        public bool SendBirthdateNull
        {
            get => _sendBirthdateNull;
            set { _sendBirthdateNull = value; OnPropertyChanged(nameof(SendBirthdateNull));}
        }

        public DateTime? AnniversaryDate
        {
            get => _anniversaryDate;
            set { _anniversaryDate = value; OnPropertyChanged(nameof(AnniversaryDate)); }
        }

        public bool SendAnniversaryDateNull
        {
            get => _sendAnniversaryDateNull;
            set { _sendAnniversaryDateNull = value; OnPropertyChanged(nameof(SendAnniversaryDateNull)); }
        }

        public string Ranking
        {
            get => _ranking;
            set { _ranking = value; OnPropertyChanged(nameof(Ranking));}
        }
        public bool SendRankingNull
        {
            get => _sendRankingNull;
            set { _sendRankingNull = value; OnPropertyChanged(nameof(SendRankingNull)); }
        }
        public bool PinLock
        {
            get => _pinLock;
            set { _pinLock = value; OnPropertyChanged(nameof(PinLock)); }
        }
        public bool FirstVisit
        {
            get => _firstVisit;
            set { _firstVisit = value; OnPropertyChanged(nameof(FirstVisit)); }
        }
        public bool NewMember
        {
            get => _newMember;
            set { _newMember = value; OnPropertyChanged(nameof(NewMember)); }
        }
        public bool Banned
        {
            get => _banned;
            set { _banned = value; OnPropertyChanged(nameof(Banned)); }
        }
        public bool SelfBanned
        {
            get => _selfBanned;
            set { _selfBanned = value; OnPropertyChanged(nameof(SelfBanned)); }
        }
        public long CompBalance
        {
            get => _compBalance;
            set { _compBalance = value; OnPropertyChanged(nameof(CompBalance)); }
        }
        public bool PtpEnable
        {
            get => _ptpEnable;
            set { _ptpEnable = value; OnPropertyChanged(nameof(PtpEnable)); }
        }
        public long PtpBalance
        {
            get => _ptpBalance;
            set { _ptpBalance = value; OnPropertyChanged(nameof(PtpBalance)); }
        }

        public bool XcEnable
        {
            get => _xcEnable;
            set { _xcEnable = value; OnPropertyChanged(nameof(XcEnable)); }
        }

        public long XCAwardBalance
        {
            get => _xcAwardBalance;
            set { _xcAwardBalance = value;OnPropertyChanged(nameof(XCAwardBalance));}
        }
        public long XpcAward
        {
            get => _xpcAward;
            set { _xpcAward = value; OnPropertyChanged(nameof(XpcAward)); }
        }

        public bool PpEnable
        {
            get => _ppEnable;
            set { _ppEnable = value; OnPropertyChanged(nameof(PpEnable)); }
        }

        public long PpPoolBalance
        {
            get => _ppPoolBalance;
            set { _ppPoolBalance = value; OnPropertyChanged(nameof(PpPoolBalance)); }
        }
        public long PpLuckyNumber
        {
            get => _ppLuckyNumber;
            set { _ppLuckyNumber = value; OnPropertyChanged(nameof(PpLuckyNumber)); }
        }
        public long PpTotalWon
        {
            get => _ppTotalWon;
            set { _ppTotalWon = value; OnPropertyChanged(nameof(PpTotalWon)); }
        }

        public double SpXier
        {
            get => _spXier;
            set { _spXier = value; OnPropertyChanged(nameof(SpXier)); }
        }
        public DateTime? SpXierStartTime
        {
            get => _spXierStartTime;
            set { _spXierStartTime = value; OnPropertyChanged(nameof(SpXierStartTime)); }
        }

        public bool SendSpXierStartTimeNull
        {
            get => _sendSpXierStartTimeNull;
            set { _sendSpXierStartTimeNull = value; OnPropertyChanged(nameof(SendSpXierStartTimeNull)); }
        }



        public DateTime? SpXierStopTime
        {
            get => _spXierStopTime;
            set { _spXierStopTime = value; OnPropertyChanged(nameof(SpXierStopTime)); }
        }

        public bool SendSpXierStopTimeNull
        {
            get => _sendSpXierStopTimeNull;
            set { _sendSpXierStopTimeNull = value; OnPropertyChanged(nameof(SendSpXierStopTimeNull)); }
        }

        public double SpRankXier
        {
            get => _spRankXier;
            set { _spRankXier = value;OnPropertyChanged(nameof(SpRankXier)); }
        }
        public bool RpEnable
        {
            get => _rpEnable;
            set { _rpEnable = value; OnPropertyChanged(nameof(RpEnable)); }
        }
        public long RpPointBalance
        {
            get => _rpPointBalance;
            set { _rpPointBalance = value; OnPropertyChanged(nameof(RpPointBalance)); }
        }
        public long RpEarnedDay
        {
            get => _rpEarnedDay;
            set { _rpEarnedDay = value; OnPropertyChanged(nameof(RpEarnedDay)); }
        }
        
        public bool SrpEnable
        {
            get => _srpEnable;
            set { _srpEnable = value; OnPropertyChanged(nameof(SrpEnable)); }
        }
        public long SrpLevel
        {
            get => _srpLevel;
            set { _srpLevel = value; OnPropertyChanged(nameof(SrpLevel)); }
        }
        public long SrpPointBalance
        {
            get => _srpPointBalance;
            set { _srpPointBalance = value; OnPropertyChanged(nameof(SrpPointBalance)); }
        }
        public List<t_giftPointItem> GiftPointPrograms 
        {  get => _giftPointPrograms;
            set { _giftPointPrograms = value; OnPropertyChanged(nameof(GiftPointPrograms)); }
        }
        public bool SendGiftPointProgramsNull
        {
            get => _sendGiftPointProgramsNull;
            set { _sendGiftPointProgramsNull = value; OnPropertyChanged(nameof(SendGiftPointProgramsNull)); }
        }
        public List<t_bbpgLevel> Bonusmeters
        {
            get => _bonusMeters;
            set { _bonusMeters = value; OnPropertyChanged(nameof(Bonusmeters)); }
        }
        public bool SendBonusmetersNull
        {
            get => _sendBonusMetersNull;
            set { _sendBonusMetersNull = value; OnPropertyChanged(nameof(SendBonusmetersNull)); }
        }
        public long SrpAward
        {
            get => _srpAward;
            set { _srpAward = value; OnPropertyChanged(nameof(SrpAward)); }
        }
        public bool HidePoints
        {
            get => _hidePoints;
            set { _hidePoints = value; OnPropertyChanged(nameof(HidePoints)); }
        }
        public bool Abandoned
        {
            get => _abandoned;
            set { _abandoned = value; OnPropertyChanged(nameof(Abandoned)); }
        }
        public double PartialPointCarryover
        {
            get => _partialPointCarryover;
            set { _partialPointCarryover = value; OnPropertyChanged(nameof(PartialPointCarryover)); }
        }
        public bool DupCard
        {
            get => _dupCard;
            set { _dupCard = value; OnPropertyChanged(nameof(DupCard)); }
        }
        public bool IsVip
        {
            get => _isVIP;
            set { _isVIP = value; OnPropertyChanged(nameof(IsVip)); }
        }
        public long SpRate
        {
            get => _spRate;
            set { _spRate = value; OnPropertyChanged(nameof(SpRate)); }
        }
        public long SpAward
        {
            get => _spAward;
            set { _spAward = value; OnPropertyChanged(nameof(SpAward)); }
        }
        public e_pointMeters PointMeter
        {
            get => _pointMeter;
            set { _pointMeter = value; OnPropertyChanged(nameof(PointMeter)); }
        }
        public long PtpUsed
        {
            get => _ptpUsed;
            set { _ptpUsed = value; OnPropertyChanged(nameof(PtpUsed)); }
        }
        public double PtpRate
        {
            get => _ptpRate;
            set { _ptpRate = value; OnPropertyChanged(nameof(PtpRate)); }
        }
        public long PtpLimit
        {
            get => _ptpLimit;
            set { _ptpLimit = value; OnPropertyChanged(nameof(PtpLimit)); }
        }
        public bool CountUp
        {
            get => _countUp;
            set { _countUp = value; OnPropertyChanged(nameof(CountUp)); }
        }
        public bool ZeroUnused
        {
            get => _zeroUnused;
            set { _zeroUnused = value; OnPropertyChanged(nameof(ZeroUnused)); }
        }
        public bool DisableCardedBonuses
        {
            get => _disableCardedBonuses;
            set { _disableCardedBonuses = value; OnPropertyChanged(nameof(DisableCardedBonuses)); }
        }
        public string DupLocation
        {
            get => _dupLocation;
            set { _dupLocation = value; OnPropertyChanged(nameof(DupLocation)); }
        }
        public long MaxCashlessTxIn
        {
            get => _maxCashlessTxIn;
            set { _maxCashlessTxIn = value; OnPropertyChanged(nameof(MaxCashlessTxIn)); }
        }
        public long MaxCashlessTxOut
        {
            get => _maxCashlessTxOut;
            set { _maxCashlessTxOut = value; OnPropertyChanged(nameof(MaxCashlessTxOut)); }
        }
        public long CurreentKeyNumber
        {
            get => _currentKeyNumber;
            set { _currentKeyNumber = value; OnPropertyChanged(nameof(CurreentKeyNumber)); }
        }
        public string Signature
        {
            get => _signature;
            set { _signature = value; OnPropertyChanged(nameof(Signature)); }
        }

        public long LevelId
        {
            get => _levelId;
            set { _levelId = value; OnPropertyChanged(nameof(LevelId)); }
        }
           
        public string PoolName
        {
            get => _poolName;
            set { _poolName = value; OnPropertyChanged(nameof(PoolName)); }
        }
        public long MeterIdx
        {
            get => _meterIdx;
            set { _meterIdx = value;OnPropertyChanged(nameof(MeterIdx)); }
        }

        public long GifPointItemId
        {
            get => _gifPointItemId;
            set { _gifPointItemId = value; OnPropertyChanged(nameof(GifPointItemId)); }
        }

        public string GifPointItemName
        {
            get => _gifPointItemName;
            set { _gifPointItemName = value;OnPropertyChanged(nameof(GifPointItemName));}
        }

        public long GifPointItemBalance
        {
            get => _gifPointItemBalance;
            set { _gifPointItemBalance = value; OnPropertyChanged(nameof(GifPointItemBalance)); }
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

        public IEnumerable<e_pointMeters> PointMeterResultValues
        {
            get
            {
                return Enum.GetValues(typeof(e_pointMeters)).Cast<e_pointMeters>();
            }
        }

        public void Clear()
        {
            SendInvalidSignature = false;
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
            PlayerId = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            LocaleId = string.Empty;
            PointBalance = 0;
            CarryOver = 0;
            Birthdate = null;
            AnniversaryDate = null;
            Ranking = string.Empty;
            PinLock = false;
            FirstVisit = false;
            NewMember = false;
            Banned = false;
            SelfBanned = false;
            CompBalance = 0;
            PtpEnable = false;
            PtpBalance = 0;
            XcEnable = false;
            XCAwardBalance = 0;
            XpcAward = 0;
            PpEnable = false;
            PpPoolBalance = 0;
            PpLuckyNumber = 0;
            PpTotalWon = 0;
            SpXier = 0.0;
            SpXierStartTime = null;
            SpXierStopTime = null;
            SpRankXier = 0.0;
            RpEnable = false;
            RpPointBalance = 0;
            RpEarnedDay = 0;
            SrpEnable = false;
            SrpLevel = 0;
            SrpPointBalance = 0;
            GiftPointPrograms = new List<t_giftPointItem>();
            Bonusmeters = new List<t_bbpgLevel>();
            SrpAward = 0;
            HidePoints = false;
            Abandoned = false;
            PartialPointCarryover = 0.0;
            DupCard = false;
            IsVip = false;
            SpRate = 0;
            SpAward = 0;
            PointMeter = e_pointMeters.DISABLED;
            PtpUsed = 0;
            PtpRate = 0.0;
            PtpLimit = 0;
            CountUp = false;
            ZeroUnused = false;
            DisableCardedBonuses = false;
            DupLocation = string.Empty;
            MaxCashlessTxIn = 0;
            MaxCashlessTxOut = 0;
            CurreentKeyNumber = 0;
            Signature = string.Empty;

    }

    }
}

