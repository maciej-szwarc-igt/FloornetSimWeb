using IGT.FloorNet.EX.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iPin
{
    public class ValidatePinModel : ModelBase
    {
        private t_pinStatus _status;
        private string _message;
        private string _jwt;
        private string _playerId;
        private string _playerName;
        private string _playerLastName;        
        private bool _isValidPin = false;


        public t_pinStatus Status         
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged( nameof(Status) ); }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(nameof(Message) ); }
        }

        public string Jwt
        {
            get { return _jwt; }
            set { _jwt = value; }
        }

        public string PlayerId
        {
            get { return _playerId; }
            set { _playerId = value; OnPropertyChanged(nameof(PlayerId)); }
        }
        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; OnPropertyChanged(nameof(PlayerName)); }
        }
        public string PlayerLastName
        {
            get { return _playerLastName; }
            set { _playerLastName = value; OnPropertyChanged(nameof(PlayerLastName)); }
        }

        public bool IsValidPin
        {
            get { return _isValidPin; }
            set { _isValidPin = value; OnPropertyChanged(nameof(IsValidPin)); }
        }

        public IEnumerable<t_pinStatus> PinStatusResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_pinStatus)).Cast<t_pinStatus>();
            }
        }

        public void Clear()
        {
            Status = t_pinStatus.pin_ok;
            Message = null;
            Jwt = null;
            PlayerId = null;
            PlayerName = null;
            PlayerLastName = null;
            IsValidPin = false;
        }
    }
}
