using IGT.FloorNet.EX.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iPlayer
{
    public class CardInModel : ModelBase
    {
        private t_cardType _cardType = t_cardType.invalid;
        private long _playerId;
        private string _firstName;
        private bool _isUnknownPlayer;
        private bool _isEmployee;


        public t_cardType CardType
        {
            get { return _cardType; }
            set { _cardType = value; OnPropertyChanged(nameof(CardType)); }
        }

        public long PlayerId
        {
            get { return _playerId; }
            set { _playerId = value; OnPropertyChanged(nameof(PlayerId)); }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        public bool IsUnknownPlayer
        {
            get => _isUnknownPlayer;
            set { _isUnknownPlayer = value; OnPropertyChanged(nameof(_isUnknownPlayer)); }
        }
        public bool IsEmployee
        {
            get => _isEmployee;
            set { _isEmployee = value; OnPropertyChanged(nameof(IsEmployee)); }
        }


        public IEnumerable<t_cardType> CardTypesResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_cardType)).Cast<t_cardType>();
            }
        }

        public void Clear()
        {
            _cardType = t_cardType.invalid;
            _firstName = "";
            _playerId = 0;
            _isEmployee = false;
            _isUnknownPlayer = false;
        }

    }
}
