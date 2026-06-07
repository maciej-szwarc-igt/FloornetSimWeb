namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Diags
{
    public class Diagnostics : ModelBase
    {
        private string _uid;
        private string _serviceName;
        private bool _sendToSMIB;

        public Diagnostics()
        {
            _uid = string.Empty;
            _serviceName = string.Empty;
            _sendToSMIB = false;
        }

        public string Uid
        {
            get => _uid;
            set
            {
                _uid = value;
                OnPropertyChanged("Uid");
            }
        }

        public string ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;
                OnPropertyChanged("ServiceName");
            }
        }

        public bool SendToSMIB
        {
            get => _sendToSMIB;
            set
            {
                _sendToSMIB = value;
                OnPropertyChanged("SendToSMIB");
            }
        }
    }
}