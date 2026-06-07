using IGT.FloorNet.EX.Bonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus
{
    public class IBonusModel : ModelBase
    {
        private t_result _result = t_result.success;
        private string _message = "Completed";
        private long _progress = 0;
        private string _function = "bonusCommit";
        private long _requestId = 0;
        private string _requestIdStr = "0";
        private long? _sequence = 0;
        private bool _sendSequence = false;

        public t_result Result { get => _result; set { _result = value; OnPropertyChanged(nameof(Result)); } }
        public string Message { get => _message; set { _message = value; OnPropertyChanged(nameof(Message)); } }
        public long Progress { get => _progress; set { _progress = value; OnPropertyChanged(nameof(Progress)); } }
        public string Function { get => _function; set { _function = value; OnPropertyChanged(nameof(Function)); } }
        public long RequestId { get => _requestId; set { _requestId = value; OnPropertyChanged(nameof(RequestId)); } }
        public string RequestIdStr { get => _requestIdStr; set { _requestIdStr = value; OnPropertyChanged(nameof(RequestIdStr)); } }
        public long? Sequence { get => _sequence; set { _sequence = value; OnPropertyChanged(nameof(Sequence)); } }
        public bool SendSequence { get => _sendSequence; set { _sendSequence = value; OnPropertyChanged(nameof(SendSequence)); } }

        public IEnumerable<t_result> ResultValues
        {
            get
            {
                return Enum.GetValues(typeof(t_result)).Cast<t_result>();
            }
        }

        public void Clear()
        {
            Result = t_result.success;
            Message = "Completed";
            Progress = 0;
            Function = "bonusCommit";
            RequestId = 0;
            RequestIdStr = "0";
            Sequence = 0;
        }
    }
}
