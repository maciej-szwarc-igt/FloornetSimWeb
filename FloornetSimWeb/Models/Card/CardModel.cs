using IGT.FloorNet.EX.Player;
using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card
{
    public class CardModel : ModelBase
    {
        private string _server_key_b64;
        private string _pin;
        private string client_iv_b64;
        private string client_encpin_b64;
        private string _server_private_key;
        private string _server_public_key;
        private string _card_id;
        private long _card_in_count;
        private DateTime _start_time = DateTime.UtcNow;
        private long _currentKeyNumber;
        private string _shared_secret_key;
        private bool useEmptyMsg;
        private string sleepBefResp;
        private string _rpcSmibUid;
        public CardModel()
        {
            Init();
        }       

        public bool IsKeysValid => !string.IsNullOrEmpty(ServerPrivateKey) && !string.IsNullOrEmpty(ServerPublicKey) && !string.IsNullOrEmpty(ClientIV);

        public bool IsEncryptPinValid =>
              !string.IsNullOrEmpty(CardId) && !string.IsNullOrEmpty(CardInCount.ToString())
              && !string.IsNullOrEmpty(StartTime.ToString()) && !string.IsNullOrEmpty(Pin)
              && !string.IsNullOrEmpty(ServerPublicKey) && !string.IsNullOrEmpty(ClientIV) && !string.IsNullOrEmpty(ServerKey) && !string.IsNullOrEmpty(SharedSecretKey);

        public bool IsSecretKeyValid => !string.IsNullOrEmpty(ServerKey) && IsKeysValid;

        public bool IsValidatePinValid => IsEncryptPinValid
                                            && !string.IsNullOrEmpty(ClientEncryptPin);

        public string ServerKey
        {
            get => _server_key_b64;
            set
            {
                _server_key_b64 = value;
                OnPropertyChanged("ServerKey");
            }
        }

        public string Pin
        {
            get => _pin;
            set
            {
                _pin = value;
                OnPropertyChanged("Pin");
            }
        }

        public string ClientIV
        {
            get => client_iv_b64;
            set
            {
                client_iv_b64 = value;
                OnPropertyChanged("ClientIV");
            }
        }

        /*public bool UseEmptyMessage
        {
            get => useEmptyMsg;
            set
            {
                useEmptyMsg = value;
                Startup.cardProvider.UseEmptyMessage(useEmptyMsg);
                OnPropertyChanged("UseEmptyMessage");
            }
        }*/

        /*public string SleepBeforeResponse
        {
            get => sleepBefResp;
            set
            {
                sleepBefResp = value;
                Startup.cardProvider.UseSleepBeforeResponse(sleepBefResp);
                OnPropertyChanged("SleepBeforeResponse");
            }
        }*/

        public string ClientEncryptPin
        {
            get => client_encpin_b64;
            set
            {
                client_encpin_b64 = value;
                OnPropertyChanged("ClientEncryptPin");
            }
        }

        public string ServerPrivateKey
        {
            get => _server_private_key;
            set
            {
                _server_private_key = value;
                OnPropertyChanged("ServerPrivateKey");
            }
        }

        public string ServerPublicKey
        {
            get => _server_public_key;
            set
            {
                _server_public_key = value;
                OnPropertyChanged("ServerPublicKey");
            }
        }

        public string CardId
        {
            get => _card_id;
            set
            {
                _card_id = value;
                OnPropertyChanged("CardId");
            }
        }

        public long CardInCount
        {
            get => _card_in_count;
            set
            {
                _card_in_count = value;
                OnPropertyChanged("CardInCount");
            }
        }

        public DateTime StartTime
        {
            get => _start_time;
            set
            {
                _start_time = value;
                OnPropertyChanged("StartTime");
            }
        }

        public long CurrentKeyNumber
        {
            get => _currentKeyNumber;
            set
            {
                _currentKeyNumber = value;
                OnPropertyChanged("CurrentKeyNumber");
            }
        }

        public string SharedSecretKey
        {
            get => _shared_secret_key;
            set
            {
                _shared_secret_key = value;
                OnPropertyChanged("SharedSecretKey");
            }
        }

        public string RpcSmibUid
        {
            get => _rpcSmibUid;
            set
            {
                _rpcSmibUid = value;
                OnPropertyChanged("RpcSmibUid");
            }
        }

        public void Init()
        {
            Pin = "4321";
            ServerPublicKey = null;
            ServerPrivateKey = null;
            SharedSecretKey = null;
            ClientIV = null;
            CardId = "12345665000003692645";
            CardInCount = 1;
            StartTime = DateTime.UtcNow;
            sleepBefResp = "0";
            RpcSmibUid = string.Empty;
        }

        public void ClearPublicAndPrivateKey()
        {
            ServerPublicKey = null;
            ServerPrivateKey = null;
        }

        public void ClearClientIV()
        {
            ClientIV = null;
        }

        public void ClearValidatePinValues()
        {
            CardId = ClientEncryptPin = Pin = null;
            CardInCount = 0;
            StartTime = DateTime.UtcNow;
        }

        public void ClearServerPublicKey()
        {
            ServerKey = null;
        }

        public void ClearSharedKey()
        {
            SharedSecretKey = null;
        }

        public void Clear()
        {
            Pin = "4321";
            ServerPublicKey = null;
            ServerPrivateKey = null;
            SharedSecretKey = null;
            ClientIV = null;
            CardId = "12345665000003692645";
            CardInCount = 1;
            StartTime = DateTime.UtcNow;
            ServerKey = null;
        }
    }
}