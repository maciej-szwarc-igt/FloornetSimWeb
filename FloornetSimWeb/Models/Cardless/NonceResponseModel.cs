using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Cardless
{
    public class NonceResponseModel : ModelBase
    {
        private int _expireTimeSec;
        private string _smibUIDReq;
        private long _customNonce;
        private bool _sendCustomNonce;      

        public int ExpireTimeSec {
            get { return _expireTimeSec; }
            set { _expireTimeSec = value; OnPropertyChanged(nameof(ExpireTimeSec)); }
        }

        public string SmibUIDReq
        {
            get { return _smibUIDReq; }
            set { _smibUIDReq = value; OnPropertyChanged(nameof(SmibUIDReq)); }
        }

        public long CustomNonce
        {
            get { return _customNonce; }
            set { _customNonce = value; OnPropertyChanged(nameof(CustomNonce)); }
        }
        public bool SendCustomNonce
        {
            get { return _sendCustomNonce; }
            set { _sendCustomNonce = value; OnPropertyChanged(nameof(SendCustomNonce)); }
        }
    }
}
