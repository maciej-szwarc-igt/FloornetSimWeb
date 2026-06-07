using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IGT.FloorNet.EX.ISM;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.ISM
{
   public class AdjustAccountModel : ModelBase
    {

        private string _cardId;
        private long _amountCents;
        private t_IsmAccountTypes _account;
        private long _msgTimeSecs;
        private bool _msgTone;
        private string _msg;
        private t_IsmDirection _direction;        
        private string _signature;
        private bool _sendInvalidSignature;
        private string _uId;

        public AdjustAccountModel()
        {
            Clear();
        }

        public string CardId 
        { 
            get => _cardId;
            set { _cardId = value; OnPropertyChanged(nameof(CardId));}
        }

        public long AmountCents
        {
            get => _amountCents;
            set { _amountCents = value; OnPropertyChanged(nameof(AmountCents)); }
        }

        public t_IsmAccountTypes Account
        {
            get => _account;
            set { _account = value; OnPropertyChanged(nameof(Account)); }
        }

        public long MsgTimeSecs
        {
            get => _msgTimeSecs;
            set { _msgTimeSecs = value; OnPropertyChanged(nameof(MsgTimeSecs)); }
        }

        public bool MsgTone
        {
            get => _msgTone;
            set { _msgTone = value; OnPropertyChanged(nameof(MsgTone)); }
        }

        public string Msg
        {
            get => _msg;
            set
            {
                _msg = value;
                OnPropertyChanged(nameof(Msg));
                OnPropertyChanged(nameof(MsgUI)); //Update the value in order UI
            }
         }

        public string MsgUI
        {
            get => _msg != null && _msg.StartsWith("\u0006H") ? _msg.Substring(2) : _msg;
            set
            {
                // Allways use the hiden value "\u0006H"
                Msg = "\u0006H" + value;
            }
        }

        public t_IsmDirection Direction
        {
            get => _direction;
            set { _direction = value; OnPropertyChanged(nameof(Direction)); }
        }

        public string Signature
        {
            get => _signature;
            set { _signature = value; OnPropertyChanged(nameof(Signature)); }
        }

        public string UID
        {
            get => _uId;
            set { _uId = value; OnPropertyChanged(nameof(UID)); }
        }

        public bool SendInvalidSignature
        {
            get => _sendInvalidSignature;
            set { _sendInvalidSignature = value; OnPropertyChanged(nameof(SendInvalidSignature)); }
        }

        public void Clear()
        {
            CardId = null;
            AmountCents = 0;
            Account = t_IsmAccountTypes.UNKNOWN;
            MsgTimeSecs = 0;
            MsgTone = true;
            Msg = null;
            Direction = t_IsmDirection.credit;            
            Signature = null;
            UID = null;
        }
    }
}
