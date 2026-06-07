namespace IGT.FloorNet.Tools.ServiceSimulator.Models.ProgressEvent
{
    public class ProgressStatus : ModelBase
    {
        private string _deviceType;

        private string _requestIdStr;

        private t_result _result { get; set; }
        private string _message { get; set; }
        private string _function { get; set; }
        private long _requestId { get; set; }
        private long? _sequence { get; set; }

        private long _progress { get; set; }

        private string _datetime { get; set; }

        public t_result result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged("result");
            }
        }

        public string message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged("message");
            }
        }
        
        public string DeviceType
        {
            get => _deviceType;
            set
            {
                _deviceType = value;
                OnPropertyChanged("DeviceType");
            }
        }

        public long progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged("progress");
            }
        }

        public string function
        {
            get => _function;
            set
            {
                _function = value;
                OnPropertyChanged("function");
            }
        }

        public long requestId
        {
            get => _requestId;
            set
            {
                _requestId = value;
                OnPropertyChanged("requestId");
            }
        }

        public string requestIdStr
        {
            get => _requestIdStr;
            set
            {
                _requestIdStr = value;
                OnPropertyChanged("requestIdStr");
            }
        }

        public long? sequence
        {
            get => _sequence;
            set
            {
                _sequence = value;
                OnPropertyChanged("sequence");
            }
        }

        public string? datetimme
        {
            get => _datetime;
            set
            {
                _datetime = value;
                OnPropertyChanged("datetimme");
            }
        }
    }
}