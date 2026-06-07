using IGT.FloorNet.EX.Player;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.Tools.ServiceSimulator.Models.PCS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels
{
    public class PCSViewModel : INotifyPropertyChanged
    {
        private ResponseViewModel _responseViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<PCSContainer> PCSContainer { get; set; }

        public string smib_key = "iv";

        public string iv = "IGT";

        public string encpin = "1234";

        private t_idReaderType _idReaderType;

        private string _cardid;
        public string Cardid
        {
            get { return _cardid; }

            set
            {
                _cardid = value;
                OnPropertyChanged(nameof(Cardid));
            }
        }

        public t_idReaderType IdReaderType
        {
            get { return _idReaderType; }

            set
            {
                _idReaderType = value;
                OnPropertyChanged(nameof(IdReaderType));
            }
        }

        private t_PCSLimitDetail _limits;

        public t_PCSLimitDetail Limits
        {
            get { return _limits; }

            set
            {
                _limits = value;
                OnPropertyChanged(nameof(Limits));
            }
        }

        private string _alertMsg = "Default Alert Message";
        public string AlertMsg
        {
            get { return _alertMsg; }

            set
            {
                _alertMsg = value;
                OnPropertyChanged(nameof(AlertMsg));
            }
        }

        public ObservableCollection<string> _pinDetails { get; set; }
        public ObservableCollection<string> PinDetails
        {
            get { return _pinDetails; }
            set
            {
                _pinDetails = value;
                OnPropertyChanged(nameof(PinDetails));
            }
        }

        public int PinDetailsSelectedIndex { get; set; }

        private long _cardInCount;
        public long CardInCount
        {
            get { return _cardInCount; }

            set
            {
                _cardInCount = value;
                OnPropertyChanged(nameof(CardInCount));
            }
        }

        private DateTime _noLimitTime;
        public DateTime NoLimitTime
        {
            get { return _noLimitTime; }

            set
            {
                _noLimitTime = value;
                OnPropertyChanged(nameof(NoLimitTime));
            }
        }

        private bool _pinValid = true;
        public bool PinValid
        {
            get { return _pinValid; }

            set
            {
                _pinValid = value;
                OnPropertyChanged(nameof(PinValid));
            }
        }

        private bool _activityStatement = false;
        public bool ActivityStatement
        {
            get { return _activityStatement; }

            set
            {
                _activityStatement = value;
                OnPropertyChanged(nameof(ActivityStatement));
            }
        }

        private bool _accountInUse = false;
        public bool AccountInUse
        {
            get { return _accountInUse; }

            set
            {
                _accountInUse = value;
                OnPropertyChanged(nameof(AccountInUse));
            }
        }

        private bool _pcsDown = false;
        public bool PcsDown
        {
            get { return _pcsDown; }

            set
            {
                _pcsDown = value;
                OnPropertyChanged(nameof(PcsDown));
            }
        }

        private bool _pinLocked = false;
        public bool PinLocked
        {
            get { return _pinLocked; }

            set
            {
                _pinLocked = value;
                OnPropertyChanged(nameof(PinLocked));
            }
        }

        private bool _accountCanceled = false;
        public bool AccountCanceled
        {
            get { return _accountCanceled; }

            set
            {
                _accountCanceled = value;
                OnPropertyChanged(nameof(AccountCanceled));
            }
        }

        private bool _invalidPCSID = false;
        public bool InvalidPCSID
        {
            get { return _invalidPCSID; }

            set
            {
                _invalidPCSID = value;
                OnPropertyChanged(nameof(InvalidPCSID));
            }
        }

        
        public void SendData()
        {
            Startup._iPCS.getPublicKey();
            Startup._iPCS.StartPCSSession(Cardid, IdReaderType, CardInCount, smib_key, iv, encpin);
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PCSViewModel()
        {
            PCSContainer = new ObservableCollection<PCSContainer>
            {
                new PCSContainer(e_PCSLimitType.time, e_PCSPeriodType.weekly, 60*10, 60*8),
                new PCSContainer(e_PCSLimitType.loss, e_PCSPeriodType.daily, 1000, 100)
            };

            //Lists all pin details available in enum.
            var names = Enum.GetNames(typeof(t_pinStatus));
            PinDetails = new ObservableCollection<string>(names);
            PinDetailsSelectedIndex = 0;


         }

        public List<t_PCSLimitDetail> GetLimits()
        {
            List<t_PCSLimitDetail> limits = new List<t_PCSLimitDetail>();

            var enumerator = PCSContainer.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var container = enumerator.Current;
                limits.Add(new t_PCSLimitDetail
                {
                    type = container.type,
                    period = container.period,
                    threshold = container.threshold,
                    currentValue = container.currentValue,
                    limitReached = container.limitReached

                });

            }

            return limits;
        }
      
    }
}


