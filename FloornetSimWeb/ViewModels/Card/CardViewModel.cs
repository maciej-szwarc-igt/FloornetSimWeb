using IGT.FloorNet.EX.Player;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.SecurityUtils;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Card;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr;
using IGT.FloorNet.Tools.ServiceSimulator.Models.RG;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card
{
    public class CardViewModel : ModelBase, IJwtObservable
    {
        /// <summary>
        /// Reference to the list of observers this class has.
        /// </summary>
        private List<IJwtObserver> JwtObservers { get; set; } = new List<IJwtObserver>();
        private RelayCommand _getPublicKeyCommand;
        private RelayCommand _clearServerPublicKeyCommand;
        private RelayCommand _validatePinCommand;
        private RelayCommand _clearValidatePinCommand;
        private RelayCommand _getPublicAndPrivateKeyCommand;
        private RelayCommand _clearKeysCommand;
        private RelayCommand _getClientIVCommand;
        private RelayCommand _clearClientIVCommand;
        private RelayCommand _getSharedSecretKeyCommand;
        private RelayCommand _clearSharedKeyCommand;
        private RelayCommand _encryptPinCommand;
        private RelayCommand _getCardPresentCommand;
        private RelayCommand _forceCardInCommand;
        private RelayCommand _forceCardOutCommand;
        private RelayCommand _setClearanceCommand;
        private RelayCommand _employeeCardDataCommand;
        private RelayCommand _playerCardDataCommand;
        private RelayCommand _clearforceCardInCommand;
        private RelayCommand _clearforceCardOutCommand;
        private RelayCommand _clearEmployeeCardDataCommand;
        private RelayCommand _clearPlayerCardDataCommand;

        private readonly ResponseViewModel _responseViewModel;
        private bool _rpcProcess = true;
        private bool _enableCardService = false;


        public CardModel IPinCrypto { get; set; } = new CardModel();
        public static ForceCardInModel forceCardInModel { get; set; } = new ForceCardInModel();
        public static ForceCardOutModel forceCardOutModel { get; set; } = new ForceCardOutModel();
        public SetClearanceModel setClearanceModel { get; set; } = new SetClearanceModel();
        public static EmployeeCardDataModel employeeCardDataModel { get; set; } = new EmployeeCardDataModel();
        public static PlayerCardDataModel playerCardDataModel { get; set; } = new PlayerCardDataModel();


        private FloornetECDHProvider floornetECDHProvider = new FloornetECDHProvider();

        public CardViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public ResponseModel Model { get; } = new ResponseModel();

        /// <summary>
        /// Allow RPC Process
        /// </summary>
        public bool RpcProcess
        {
            get => _rpcProcess;
            set
            {
                _rpcProcess = value;
                if (_rpcProcess)
                    Model.Response = string.Empty;
            }
        }

        public bool EnableCardService
        {
            get => _enableCardService;
            set
            {
                _enableCardService = value;
                if (_enableCardService)
                    Model.Response = string.Empty;
                OnPropertyChanged("EnableCardService");
            }
        }

        public RelayCommand GetPublicAndPrivateKeyCommand
        {
            get
            {
                _getPublicAndPrivateKeyCommand = new RelayCommand(
                    GetPublicAndPrivateKey,
                    param => true
                );

                return _getPublicAndPrivateKeyCommand;
            }
        }

        public RelayCommand ClearPublicAndPrivateKeyCommand
        {
            get
            {
                _clearKeysCommand = new RelayCommand(
                   ClearPublicAndPrivateKey,
                    param => true);
                return _clearKeysCommand;
            }
        }

        public RelayCommand GetClientIVCommand
        {
            get
            {
                _getClientIVCommand = new RelayCommand(
                    GetClientIV,
                    param => true
                );

                return _getClientIVCommand;
            }
        }

        public RelayCommand ClearClientIVCommand
        {
            get
            {
                _clearClientIVCommand = new RelayCommand(
                    ClearClientIV,
                    param => true
                );

                return _clearClientIVCommand;
            }
        }
        public RelayCommand ClearforceCardInCommand
        {
            get
            {
                _clearforceCardInCommand = new RelayCommand(
                    ClearForceCardIn,
                    param => true
                );

                return _clearforceCardInCommand;
            }
        }
        public RelayCommand ClearEmployeeCardDataCommand
        {
            get
            {
                _clearEmployeeCardDataCommand = new RelayCommand(
                    ClearEmployeeCardData,
                    param => true
                );

                return _clearEmployeeCardDataCommand;
            }
        }
        public RelayCommand ClearPlayerCardDataCommand
        {
            get
            {
                _clearPlayerCardDataCommand = new RelayCommand(
                    ClearPlayerCardData,
                    param => true
                );

                return _clearPlayerCardDataCommand;
            }
        }
        public RelayCommand ClearforceCardOutCommand
        {
            get
            {
                _clearforceCardOutCommand = new RelayCommand(
                    ClearForceCardOut,
                    param => true
                );

                return _clearforceCardOutCommand;
            }
        }

        public RelayCommand EncryptPinCommand
        {
            get
            {
                _encryptPinCommand = new RelayCommand(
                    GetEncryptPin,
                    param => IPinCrypto.IsEncryptPinValid
                );

                return _encryptPinCommand;
            }
        }

        public RelayCommand ClearValidatePinCommand
        {
            get
            {
                _clearValidatePinCommand = new RelayCommand(
                   ClearValidatePin,
                    param => true);
                return _clearValidatePinCommand;
            }
        }

        public RelayCommand GetSharedSecretKeyCommand
        {
            get
            {
                _getSharedSecretKeyCommand = new RelayCommand(
                  GetSharedSecretKey,
                  param => IPinCrypto.IsSecretKeyValid
              );

                return _getSharedSecretKeyCommand;
            }
        }

        public RelayCommand ClearServerPublicKeyCommand
        {
            get
            {
                _clearServerPublicKeyCommand = new RelayCommand(
                  ClearServerPublicKey,
                  param => true
              );

                return _clearServerPublicKeyCommand;
            }
        }

        public RelayCommand ClearSharedKeyCommand
        {
            get
            {
                _clearSharedKeyCommand = new RelayCommand(
                  ClearSharedKey,
                  param => true
              );

                return _clearSharedKeyCommand;
            }
        }

        public RelayCommand GetCardPresentCommand
        {
            get
            {
                _getCardPresentCommand = new RelayCommand(
                  CardPresent,
                  param => !string.IsNullOrEmpty(IPinCrypto.RpcSmibUid));
                return _getCardPresentCommand;
            }
        }

        public RelayCommand ForceCardInCommand
        {
            get
            {
                _forceCardInCommand = new RelayCommand(
                  ForceCardIn,
                  param => true

                );

                return _forceCardInCommand;
            }
        }
        public RelayCommand EmployeeCardDataCommand
        {
            get
            {
                _employeeCardDataCommand = new RelayCommand(
                  EmployeeCardData,
                  param => true

                );

                return _employeeCardDataCommand;
            }
        }
        public RelayCommand PlayerCardDataCommand
        {
            get
            {
                _playerCardDataCommand = new RelayCommand(
                  PLayerCardDatacomand,
                  param => true

                );

                return _playerCardDataCommand;
            }
        }
        public RelayCommand ForceCardOutCommand
        {
            get
            {
                _forceCardOutCommand = new RelayCommand(
                  ForceCardOut,
                  param => true

                );

                return _forceCardOutCommand;
            }
        }

        public RelayCommand SetClearanceCommand
        {
            get
            {
                _setClearanceCommand = new RelayCommand(
                  SetClearance,
                  param => true

                );

                return _setClearanceCommand;
            }
        }

        #region Card private methods

        public ECDiffieHellman CreateECDH(string privateKey = null, string publicKey = null)
        {
            return floornetECDHProvider.CreateECDH(privateKey, publicKey);
        }

        public void GetPublicAndPrivateKey(object obj)
        {
            try
            {
                var serverPrivateKey = IPinCrypto.ServerPrivateKey == string.Empty ? null : IPinCrypto.ServerPrivateKey;
                var serverPublicKey = IPinCrypto.ServerPublicKey == string.Empty ? null : IPinCrypto.ServerPublicKey;

                ECDiffieHellman ecdh = CreateECDH(serverPrivateKey, serverPublicKey);

                IPinCrypto.ServerPrivateKey = floornetECDHProvider.GetPrivateKey(ecdh);
                IPinCrypto.ServerPublicKey = floornetECDHProvider.GetPublicKey(ecdh);
                Model.Response = string.Empty;
            }
            catch (Exception ex)
            {
                // Response
                Model.Response = string.Empty;
                Model.Response = $"{Model.Response}{Environment.NewLine}{Constants.Error.ToUpper()} :{Environment.NewLine}{ex.Message.ToString()}";
            }
        }

        private void ClearPublicAndPrivateKey(object obj)
        {
            IPinCrypto?.ClearPublicAndPrivateKey();
            Model.Response = string.Empty;
        }

        private void GetClientIV(object obj)
        {
            IPinCrypto.ClientIV = floornetECDHProvider.GenerateIV();
        }

        private void ClearClientIV(object obj)
        {
            IPinCrypto?.ClearClientIV();
            Model.Response = string.Empty;
        }
        private void ClearForceCardIn(object obj)
        {
            forceCardInModel.Clear();
            Model.Response = string.Empty;
        }
        private void ClearEmployeeCardData(object obj)
        {
            employeeCardDataModel.Clear();
            Model.Response = string.Empty;
        }
        private void ClearPlayerCardData(object obj)
        {
            playerCardDataModel.Clear();
            Model.Response = string.Empty;
        }
        private void ClearForceCardOut(object obj)
        {
            forceCardOutModel.Clear();
            Model.Response = string.Empty;
        }

          private void GetEncryptPin(object obj)
        {
            try
            {
                IPinCrypto.ClientEncryptPin = floornetECDHProvider.EncryptText(IPinCrypto.SharedSecretKey, IPinCrypto.ClientIV, IPinCrypto.Pin);
                Model.Response = string.Empty;
            }
            catch (Exception ex)
            {
                // Response
                Model.Response = string.Empty;
                Model.Response = $"{Model.Response}{Environment.NewLine}{Constants.Error.ToUpper()} :{Environment.NewLine}{ex.Message.ToString()}";
            }
        }

        private void ClearValidatePin(object obj)
        {
            IPinCrypto?.ClearValidatePinValues();
            Model.Response = string.Empty;
        }

        private void CardPresent(Object obj)
        {
            Model.Response = string.Empty;
            Model.Response = $"{Model.Response}{Environment.NewLine}{Constants.Request.ToUpper()}";

            GetCardPresent();
        }

        private void ForceCardIn(Object obj)
        {
            if (!RpcProcess) return;
            RPCForceCardIn();
        }
        private void EmployeeCardData(Object obj)
        {
            if (!RpcProcess) return;
            RPCEmployeeCardData();
        }
        private void PLayerCardDatacomand(Object obj)
        {
            if (!RpcProcess) return;
            RPCPlayerCardData();
        }
        private void ForceCardOut(Object obj)
        {
            if (!RpcProcess) return;
            RPCForceCardOut();
        }
        private void SetClearance(Object obj)
        {
            if (!RpcProcess) return;
            RPCSetClearance();
        }

        private void GetCardPresent()
        {
            if (!RpcProcess) return;
            RPCGetCardPresent();
        }

        private void GetSharedSecretKey(object obj)
        {
            IPinCrypto.SharedSecretKey = floornetECDHProvider.ComputeSessionKey(CreateECDH(IPinCrypto.ServerPrivateKey, IPinCrypto.ServerPublicKey), IPinCrypto.ServerKey);
            RefreshCommandBound();
        }

        private void ClearServerPublicKey(object obj)
        {
            IPinCrypto?.ClearServerPublicKey();
            Model.Response = string.Empty;
        }

        private void ClearSharedKey(object obj)
        {
            IPinCrypto?.ClearSharedKey();
        }

        private void RefreshCommandBound()
        {
            // CommandManager removed (no WPF)
        }

        private object JsonResponse(object resp)
        {
            var json = JsonConvert.SerializeObject(resp);
            return JToken.Parse(json).ToString(Formatting.Indented);
        }

        private async void RPCGetCardPresent()
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(IPinCrypto.RpcSmibUid);

            var resp = await Startup._iSmibPlr.getCardPresent();

            _responseViewModel.LogRpc(Constants.GetCardPresent, req, resp, RpcCallContext.Current);
        }

        private async void RPCForceCardIn()
        {

            string CardId = "";

            if (forceCardInModel.SendCustomCardId)
                CardId = forceCardInModel.CardId;
            else
                CardId = forceCardInModel.CardIdFromCardInRPC;

            t_idReaderType IdReaderType = forceCardInModel.IdReaderType;
            bool IsCardUnregister = forceCardInModel.IsCardUnregistered;
            bool IsBadCard = forceCardInModel.IsBadCard;

            var req = new Dictionary<string, object> {
                {nameof(CardId), CardId},
                {nameof(IdReaderType), IdReaderType},
                {nameof(IsCardUnregister), IsCardUnregister},
                {nameof(IsBadCard), IsBadCard}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(forceCardInModel.RpcSmibUid);

            var resp = await Startup._iSmibPlr.forceCardIn(CardId, IdReaderType, IsCardUnregister, IsBadCard);

            _responseViewModel.LogRpc(Constants.ForceCardIn, req, resp, RpcCallContext.Current);
        }
        private async void RPCForceCardOut()
        {

            string CardId = "";
            long CardInCount = 0;

            if (forceCardOutModel.SendCustomCardId)
                CardId = forceCardOutModel.CardId;
            else
                CardId = forceCardOutModel.CardIdFromCardInRPC;

            if (forceCardOutModel.SendCustomCardInCount)
                CardInCount = forceCardOutModel.CardInCount;
            else
                CardInCount = forceCardOutModel.CardInCountFromCardInRPC;


            var req = new Dictionary<string, object> {
                {nameof(CardId), CardId},
                {nameof(CardInCount), CardInCount}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(forceCardOutModel.RpcSmibUid);

            var resp = await Startup._iSmibPlr.forceCardOut(CardId, CardInCount);

            _responseViewModel.LogRpc(Constants.ForceCardOut, req, resp, RpcCallContext.Current);
        }
        private async void RPCSetClearance()
        {
            bool CoinClearance = setClearanceModel.CoinClearance;
            bool CashClearance = setClearanceModel.CashClearance;

            var req = new Dictionary<string, object> {
                {nameof(CoinClearance), CoinClearance},
                {nameof(CashClearance), CashClearance}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(setClearanceModel.RpcSmibUid);

            var resp = await Startup._iSmibPlr.setClearance(CoinClearance, CashClearance);

            _responseViewModel.LogRpc(Constants.SetClearance, req, resp, RpcCallContext.Current);
        }

        private async void RPCEmployeeCardData()
        {

            string CardId = "";
            t_idReaderType IdReaderType;
            long CardInCount;

            if (employeeCardDataModel.SendCustomCardId)
                CardId = employeeCardDataModel.CustomCardId;
            else
                CardId = employeeCardDataModel.CardId;

            if (employeeCardDataModel.SendCustomIdReaderType)
                IdReaderType = employeeCardDataModel.CustomIdReaderType;
            else
                IdReaderType = employeeCardDataModel.IdReaderType;
            
            if (employeeCardDataModel.SendCustomCardInCount)
                CardInCount = employeeCardDataModel.CustomCardInCount;
            else
                CardInCount = employeeCardDataModel.CardInCount;

            t_cardType CardType = employeeCardDataModel.CardType;
            long StaffId = employeeCardDataModel.StaffId;
            string FirstName = employeeCardDataModel.FirstName;
            string LastName = employeeCardDataModel.LastName;
            long LicenseNumber = employeeCardDataModel.LicenseNumber;
            bool AuditEnabled = employeeCardDataModel.AuditEnabled;
            bool CmmEnabled = employeeCardDataModel.CmmEnabled;
            long PinCheckLevel = employeeCardDataModel.PinCheckLevel;
            t_userLevel FJP_UserLevel = employeeCardDataModel.FJP_UserLevel;
            long FJP_MaxFill = employeeCardDataModel.FJP_MaxFill;
            long FJP_MaxJackpots = employeeCardDataModel.FJP_MaxJackpots;
            long FJP_FillOverride = employeeCardDataModel.FJP_FillOverride;
            long FJP_JPOverride = employeeCardDataModel.FJP_JPOverride;
            long FJP_PouchPayLimit = employeeCardDataModel.FJP_PouchPayLimit;
            bool FJP_KeyToCreditEnabled = employeeCardDataModel.FJP_KeyToCreditEnabled;
            long FJP_FillAmount = employeeCardDataModel.FJP_FillAmount;
            long FJP_FillBags = employeeCardDataModel.FJP_FillBags;
            long FJP_AuxBagCount = employeeCardDataModel.FJP_AuxBagCount;
            long FJP_MaxAuxBagCount = employeeCardDataModel.FJP_MaxAuxBagCount;


            var req = new Dictionary<string, object> {
                {nameof(CardId), CardId},
                {nameof(IdReaderType), IdReaderType},
                {nameof(CardInCount), CardInCount},
                {nameof(CardType), CardType},
                {nameof(StaffId), StaffId},
                {nameof(FirstName), FirstName},
                {nameof(LastName), LastName},
                {nameof(LicenseNumber), LicenseNumber},
                {nameof(AuditEnabled), AuditEnabled},
                {nameof(CmmEnabled), CmmEnabled},
                {nameof(PinCheckLevel), PinCheckLevel},
                {nameof(FJP_UserLevel), FJP_UserLevel},
                {nameof(FJP_MaxFill), FJP_MaxFill},
                {nameof(FJP_MaxJackpots), FJP_MaxJackpots},
                {nameof(FJP_FillOverride), FJP_FillOverride},
                {nameof(FJP_JPOverride), FJP_JPOverride},
                {nameof(FJP_PouchPayLimit), FJP_PouchPayLimit},
                {nameof(FJP_KeyToCreditEnabled), FJP_KeyToCreditEnabled},
                {nameof(FJP_FillAmount), FJP_FillAmount},
                {nameof(FJP_FillBags), FJP_FillBags},
                {nameof(FJP_AuxBagCount), FJP_AuxBagCount},
                {nameof(FJP_MaxAuxBagCount), FJP_MaxAuxBagCount}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(employeeCardDataModel.RpcSmibUid);

            var resp = await Startup._iSmibPlr.employeeCardData(CardId, IdReaderType, CardInCount,CardType, StaffId ,FirstName, LastName,LicenseNumber,AuditEnabled,CmmEnabled,PinCheckLevel,FJP_UserLevel,FJP_MaxFill,
                                                                FJP_MaxJackpots, FJP_FillOverride,FJP_JPOverride,FJP_PouchPayLimit,FJP_KeyToCreditEnabled,FJP_FillAmount, FJP_FillBags, FJP_AuxBagCount, FJP_MaxAuxBagCount);

            _responseViewModel.LogRpc(Constants.RPCEmployeeCardData, req, resp, RpcCallContext.Current);
        }
        private async void RPCPlayerCardData()
        {

            string CardId = "";
            t_idReaderType IdReaderType;
            long CardInCount;
            DateTime? Birthdate = playerCardDataModel.Birthdate;
            DateTime? AnniversaryDate = playerCardDataModel.AnniversaryDate;
            DateTime? SpXierStartTime = playerCardDataModel.SpXierStartTime;
            DateTime? SpXierStopTime = playerCardDataModel.SpXierStopTime;
            string Ranking = playerCardDataModel.Ranking;
            string LocaleId = playerCardDataModel.LocaleId;

            if (playerCardDataModel.SendCustomCardId)
                CardId = playerCardDataModel.CustomCardId;
            else
                CardId = playerCardDataModel.CardId;

            if (playerCardDataModel.SendCustomIdReaderType)
                IdReaderType = playerCardDataModel.CustomIdReaderType;
            else
                IdReaderType = playerCardDataModel.IdReaderType;
            
            if (playerCardDataModel.SendCustomCardInCount)
                CardInCount = playerCardDataModel.CustomCardInCount;
            else
                CardInCount = playerCardDataModel.CardInCount;

            if (playerCardDataModel.SendBirthdateNull)
                Birthdate = null;
            else
                Birthdate = playerCardDataModel.Birthdate;

            if (playerCardDataModel.SendAnniversaryDateNull)
                AnniversaryDate = null;
            else
                AnniversaryDate = playerCardDataModel.AnniversaryDate;

            if(playerCardDataModel.SendSpXierStartTimeNull)
                SpXierStartTime = null;
            else
                SpXierStartTime = playerCardDataModel.SpXierStartTime;
            
            if (playerCardDataModel.SendSpXierStopTimeNull)
                SpXierStopTime = null;
            else
                SpXierStopTime = playerCardDataModel.SpXierStopTime;
            
            if (playerCardDataModel.SendLocaleIdNull)
                LocaleId = null;
            else
                LocaleId = playerCardDataModel.LocaleId;
            
            if (playerCardDataModel.SendRankingNull)
                Ranking = null;
            else
                Ranking = playerCardDataModel.Ranking;

            t_cardType CardType = playerCardDataModel.CardType;
            long PlayerId = playerCardDataModel.PlayerId;
            string FirstName = playerCardDataModel.FirstName;
            string LastName = playerCardDataModel.LastName;
            long PointBalance = playerCardDataModel.PointBalance;
            long CarryOver = playerCardDataModel.CarryOver;
            bool PinLock = playerCardDataModel.PinLock;
            bool FirstVisit = playerCardDataModel.FirstVisit;
            bool NewMember = playerCardDataModel.NewMember;
            bool Banned = playerCardDataModel.Banned;
            bool SelfBanned = playerCardDataModel.SelfBanned;
            long CompBalance = playerCardDataModel.CompBalance;
            bool PtpEnable = playerCardDataModel.PtpEnable;
            long PtpBalance = playerCardDataModel.PtpBalance;
            bool XcEnable = playerCardDataModel.XcEnable;
            long XCAwardBalance = playerCardDataModel.XCAwardBalance;
            long XpcAward = playerCardDataModel.XpcAward;
            bool PpEnable = playerCardDataModel.PpEnable;
            long PpPoolBalance = playerCardDataModel.PpPoolBalance;
            long PpLuckyNumber = playerCardDataModel.PpLuckyNumber;
            long PpTotalWon = playerCardDataModel.PpTotalWon;
            double SpXier = playerCardDataModel.SpXier;
            double SpRankXier = playerCardDataModel.SpRankXier;
            bool RpEnable = playerCardDataModel.RpEnable;
            long RpPointBalance = playerCardDataModel.RpPointBalance;
            long RpEarnedDay = playerCardDataModel.RpEarnedDay;
            bool SrpEnable = playerCardDataModel.SrpEnable;
            long SrpLevel = playerCardDataModel.SrpLevel;
            long SrpPointBalance = playerCardDataModel.SrpPointBalance;
            long SrpAward = playerCardDataModel.SrpAward;
            bool HidePoints = playerCardDataModel.HidePoints;
            bool Abandoned = playerCardDataModel.Abandoned;
            double PartialPointCarryover = playerCardDataModel.PartialPointCarryover;
            bool DupCard = playerCardDataModel.DupCard;
            bool IsVip = playerCardDataModel.IsVip;
            long SpRate = playerCardDataModel.SpRate;
            long SpAward = playerCardDataModel.SpAward;
            e_pointMeters PointMeter = playerCardDataModel.PointMeter;
            long PtpUsed = playerCardDataModel.PtpUsed;
            double PtpRate = playerCardDataModel.PtpRate;
            long PtpLimit = playerCardDataModel.PtpLimit;
            bool CountUp = playerCardDataModel.CountUp;
            bool ZeroUnused = playerCardDataModel.ZeroUnused;
            bool DisableCardedBonuses = playerCardDataModel.DisableCardedBonuses;
            string DupLocation = playerCardDataModel.DupLocation;
            long MaxCashlessTxIn = playerCardDataModel.MaxCashlessTxIn;
            long MaxCashlessTxOut = playerCardDataModel.MaxCashlessTxOut;
            long CurrentKeyNumber = IKeysViewModel.keysModelCard.CurrentKeyNum;

            List<t_bbpgLevel> Bonusmeters = new List<t_bbpgLevel>();
            List<t_giftPointItem> GiftPointPrograms = new List<t_giftPointItem>();

            if (!playerCardDataModel.SendBonusmetersNull)
            {
                t_bbpgLevel bbpgLevel = new t_bbpgLevel();
                bbpgLevel.levelId = playerCardDataModel.LevelId;
                bbpgLevel.poolName = playerCardDataModel.PoolName;
                bbpgLevel.meterIdx = playerCardDataModel.MeterIdx;

                Bonusmeters.Add(bbpgLevel);
            }
            else
                Bonusmeters = null;

            if (!playerCardDataModel.SendGiftPointProgramsNull)
            {
                t_giftPointItem giftPointItem = new t_giftPointItem();
                giftPointItem.id = playerCardDataModel.GifPointItemId;
                giftPointItem.name = playerCardDataModel.GifPointItemName;
                giftPointItem.balance = playerCardDataModel.GifPointItemBalance;

                GiftPointPrograms.Add(giftPointItem);
            }
            else
                GiftPointPrograms = null;

            string Signature;
            if (playerCardDataModel.SendInvalidSignature == false)
            {
                Signature = IKeysViewModel.keysModelBonus.FloornetECDsaProvider.ComputeSignature(
                        IKeysViewModel.keysModelCard.ECDsa,
                        CardId,
                        IdReaderType,
                        CardInCount,
                        CardType,
                        PlayerId,
                        FirstName,
                        LastName,
                        LocaleId,
                        PointBalance,
                        CarryOver,
                        Birthdate,
                        AnniversaryDate,
                        Ranking,
                        PinLock,
                        FirstVisit,
                        NewMember,
                        Banned,
                        SelfBanned,
                        CompBalance,
                        PtpEnable,
                        PtpBalance,
                        XcEnable,
                        XCAwardBalance,
                        XpcAward,
                        PpEnable,
                        PpPoolBalance,
                        PpLuckyNumber,
                        PpTotalWon,
                        SpXier,
                        SpXierStartTime,
                        SpXierStopTime,
                        SpRankXier,
                        RpEnable,
                        RpPointBalance,
                        RpEarnedDay,
                        SrpEnable,
                        SrpLevel,
                        SrpPointBalance,
                        GiftPointPrograms,
                        Bonusmeters,
                        SrpAward,
                        HidePoints,
                        Abandoned,
                        PartialPointCarryover,
                        DupCard,
                        IsVip,
                        SpRate,
                        SpAward,
                        PointMeter,
                        PtpUsed,
                        PtpLimit,
                        CountUp,
                        ZeroUnused,
                        DisableCardedBonuses,
                        DupLocation,
                        MaxCashlessTxIn,
                        MaxCashlessTxOut,
                        CurrentKeyNumber
                    );
            }
            else
            {
                Signature = IKeysViewModel.keysModelBonus.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelCard.ECDsa,
                    CardId,
                    IdReaderType,
                    CardInCount,
                    CardType,
                    PlayerId,
                    FirstName,
                    LastName,
                    LocaleId,
                    PointBalance,
                    CarryOver,
                    Birthdate,
                    AnniversaryDate,
                    Ranking,
                    PinLock,
                    FirstVisit,
                    NewMember,
                    Banned,
                    SelfBanned,
                    CompBalance,
                    PtpEnable,
                    PtpBalance,
                    XcEnable,
                    XCAwardBalance,
                    XpcAward,
                    PpEnable,
                    PpPoolBalance,
                    PpLuckyNumber,
                    PpTotalWon,
                    SpXier,
                    SpXierStartTime,
                    SpXierStopTime,
                    SpRankXier,
                    RpEnable,
                    RpPointBalance,
                    RpEarnedDay,
                    SrpEnable,
                    SrpLevel,
                    SrpPointBalance,
                    GiftPointPrograms,
                    Bonusmeters,
                    SrpAward,
                    HidePoints,
                    Abandoned,
                    PartialPointCarryover,
                    DupCard,
                    IsVip,
                    SpRate,
                    SpAward,
                    PointMeter,
                    PtpUsed,
                    PtpLimit,
                    CountUp,
                    ZeroUnused,
                    DisableCardedBonuses,
                    DupLocation,
                    MaxCashlessTxIn,
                    MaxCashlessTxOut,
                    CurrentKeyNumber,
                    CurrentKeyNumber //Adding CurrentKeyNumber twice to induce a bad signature.
                );
            }

            var req = new Dictionary<string, object> {
                {nameof(CardId), CardId},
                {nameof(IdReaderType), IdReaderType},
                {nameof(CardInCount), CardInCount},
                {nameof(CardType), CardType},
                {nameof(PlayerId), PlayerId},
                {nameof(FirstName), FirstName},
                {nameof(LastName), LastName},
                {nameof(LocaleId), LocaleId},
                {nameof(PointBalance), PointBalance},
                {nameof(CarryOver), CarryOver},
                {nameof(Birthdate), Birthdate},
                {nameof(AnniversaryDate), AnniversaryDate},
                {nameof(Ranking), Ranking},
                {nameof(PinLock), PinLock},
                {nameof(FirstVisit), FirstVisit},
                {nameof(NewMember), NewMember},
                {nameof(Banned), Banned},
                {nameof(SelfBanned), SelfBanned},
                {nameof(PtpEnable), PtpEnable},
                {nameof(PtpBalance), PtpBalance},
                {nameof(XcEnable), XcEnable},
                {nameof(XCAwardBalance), XCAwardBalance},
                {nameof(XpcAward), XpcAward},
                {nameof(PpEnable), PpEnable},
                {nameof(PpPoolBalance), PpPoolBalance},
                {nameof(PpLuckyNumber), PpLuckyNumber},
                {nameof(PpTotalWon), PpTotalWon},
                {nameof(SpXier), SpXier},
                {nameof(SpXierStartTime), SpXierStartTime},
                {nameof(SpXierStopTime), SpXierStopTime},
                {nameof(SpRankXier), SpRankXier},
                {nameof(RpEnable), RpEnable},
                {nameof(RpPointBalance), RpPointBalance},
                {nameof(RpEarnedDay), RpEarnedDay},
                {nameof(SrpEnable), SrpEnable},
                {nameof(SrpLevel), SrpLevel},
                {nameof(SrpPointBalance), SrpPointBalance},
                {nameof(GiftPointPrograms), GiftPointPrograms},
                {nameof(Bonusmeters), Bonusmeters},
                {nameof(SrpAward), SrpAward},
                {nameof(HidePoints), HidePoints},
                {nameof(Abandoned), Abandoned},
                {nameof(PartialPointCarryover), PartialPointCarryover},
                {nameof(DupCard), DupCard},
                {nameof(IsVip), IsVip},
                {nameof(SpRate), SpRate},
                {nameof(SpAward), SpAward},
                {nameof(PointMeter), PointMeter},
                {nameof(PtpUsed), PtpUsed},
                {nameof(PtpRate), PtpRate},
                {nameof(PtpLimit), PtpLimit},
                {nameof(CountUp), CountUp},
                {nameof(ZeroUnused), ZeroUnused},
                {nameof(DisableCardedBonuses), DisableCardedBonuses},
                {nameof(DupLocation), DupLocation},
                {nameof(MaxCashlessTxIn), MaxCashlessTxIn},
                {nameof(MaxCashlessTxOut), MaxCashlessTxOut},
                {nameof(CurrentKeyNumber), CurrentKeyNumber},
                {nameof(Signature), Signature}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(playerCardDataModel.RpcSmibUid);

            var resp = await Startup._iSmibPlr.playerCardData(CardId,IdReaderType,CardInCount,CardType,PlayerId,FirstName,LastName,LocaleId,PointBalance,CarryOver,Birthdate,AnniversaryDate,Ranking,PinLock,
                                                              FirstVisit,NewMember,Banned,SelfBanned,CompBalance, PtpEnable,PtpBalance,XcEnable,XCAwardBalance,XpcAward,PpEnable,PpPoolBalance,PpLuckyNumber,PpTotalWon,SpXier,
                                                              SpXierStartTime,SpXierStopTime,SpRankXier,RpEnable,RpPointBalance,RpEarnedDay,SrpEnable,SrpLevel,SrpPointBalance,GiftPointPrograms,Bonusmeters,SrpAward,HidePoints,
                                                              Abandoned,PartialPointCarryover,DupCard,IsVip, SpRate, SpAward, PointMeter, PtpUsed, PtpRate, PtpLimit, CountUp, ZeroUnused, DisableCardedBonuses, DupLocation,
                                                              MaxCashlessTxIn, MaxCashlessTxOut, CurrentKeyNumber, Signature);

            _responseViewModel.LogRpcResponse(Constants.RPCPlayerCardData, req, resp, RpcCallContext.Current);
        }

        #endregion Card private methods


        #region IJwtObservable
        /// <summary>
        /// Registers Jwt observers. 
        /// This is done in a secondary thread to avoid any application locks
        /// </summary>
        /// <param name="observer"></param>
        public void Register(IJwtObserver observer)
        {
            Task.Run(() => {
                lock (JwtObservers)
                {
                    JwtObservers.Add(observer);
                }
            });
        }
        /// <summary>
        /// Notifies all observers a new token.
        /// Done in a secondary thread to avoid any application locks.
        /// </summary>
        /// <param name="Jwt"></param>
        private void NotifyNewToken(string Jwt)
        {
            Task.Run(() =>
            {
                lock (JwtObservers)
                {
                    IEnumerator<IJwtObserver> JwtObserverEnum = JwtObservers.GetEnumerator();
                    while (JwtObserverEnum.MoveNext())
                        JwtObserverEnum.Current.UpdateJwt(Jwt);
                }
            });
        }
        #endregion 
    }
}
