using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models
{
    public class StatusResponseModel :ModelBase
    {
        private t_result _result = t_result.pending;
        private string _message = "pending";
        private long _progress = 0;
        private string _function = "CardOut";
        private long _requestId = 0;
        private string _requestIdStr = "0";
        private long? _sequence = null;
        private bool _sendSequence;

        public t_result Result
        {
            get { return _result; }
            set { _result = value; OnPropertyChanged(nameof(Result)); }
        }
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(nameof(Message)); }
        }

        public long Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(nameof(Progress)); }
        }
        public string Function
        {
            get { return _function; }
            set { _function = value; OnPropertyChanged(nameof(Function)); }
        }
        public long RequestId
        {
            get { return _requestId; }
            set { _requestId = value; OnPropertyChanged(nameof(RequestId)); }
        }
        public string RequestIdStr
        {
            get { return _requestIdStr; }
            set { _requestIdStr = value; OnPropertyChanged(nameof(RequestIdStr)); }
        }
        public long? Sequence
        {
            get { return _sequence; }
            set { _sequence = value; OnPropertyChanged(nameof(Sequence)); }
        }
        public bool SendSequence
        {
            get { return _sendSequence; }
            set { _sendSequence = value; OnPropertyChanged(nameof(SendSequence)); }

        }

        public IEnumerable<t_result> ResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_result)).Cast<t_result>();
            }
        }

        public void Clear()
        {
            Result = t_result.pending;
            Message = "pending";
            Progress = 0;
            Function = "CardOut";
            RequestId = 0;
            RequestIdStr = "0";
            Sequence = null;
            SendSequence = false;
        }
    }
}
