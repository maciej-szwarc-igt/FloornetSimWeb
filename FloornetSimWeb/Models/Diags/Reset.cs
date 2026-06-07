using System.Collections.Generic;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Diags
{
    public class Reset : ModelBase
    {
        private string _resetUid;
        private string _serviceName;
        private string _resetHard;
        private IEnumerable<string> _hardValues;
        private bool _sendToSMIB;

        public Reset()
        {
            ResetHard = "false";
            _serviceName = string.Empty;
            _hardValues = new List<string> { "false", "true" };
            _sendToSMIB = false;
        }

        public string ResetUid
        {
            get => _resetUid;
            set
            {
                _resetUid = value;
                OnPropertyChanged("ResetUid");
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

        public string ResetHard
        {
            get => _resetHard;
            set
            {
                _resetHard = value;
                OnPropertyChanged("ResetHard");
            }
        }

        public IEnumerable<string> HardValues
        {
            get => _hardValues;
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