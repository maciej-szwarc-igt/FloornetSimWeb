using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Handpay;
using RabbitMQ.Client;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Handpay
{
    public class ResetHandpayModel : ModelBase
    {
        private string _uid = string.Empty;
        private string _requestId = string.Empty;
        private string _idendity = string.Empty;
        private bool _remote = false;
        private bool _keyToCredit = false;
        private bool _sendInvalidSignature = false;

        public string UID
        {
            get { return _uid; }
            set { _uid = value; OnPropertyChanged(nameof(UID)); }
        }

        public string RequestId
        {
            get { return _requestId; }
            set { _requestId = value; OnPropertyChanged(nameof(RequestId)); }
        }
        
        public string Identity
        {
            get { return _idendity;}
            set { _idendity = value; OnPropertyChanged(nameof(Identity));}
        }

        public bool Remote
        {
            get { return _remote; }
            set { _remote = value; OnPropertyChanged(nameof(Remote)); }
        }
        public bool KeyToCredit
        {
            get { return _keyToCredit; }
            set { _keyToCredit = value; OnPropertyChanged(nameof(KeyToCredit)); }
        }

        public bool SendInvalidSignature
        {
            get { return _sendInvalidSignature; }
            set { _sendInvalidSignature = value; OnPropertyChanged(nameof(SendInvalidSignature)); } 
        }

        public string Signature { get; set; }

        public void Clear()
        {
            UID = string.Empty;
            RequestId = string.Empty;
            Identity = string.Empty;
            Remote = false;
            KeyToCredit = false;
        }
    }
}
