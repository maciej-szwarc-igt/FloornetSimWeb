using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models
{
    public class ResponseModel : ModelBase
    {
        public const int MaxResponseStrSize = 500000;

        private string _response;
        private bool _filterByUID;
        private string _uId;
        private bool _limitResponseWindowSize = true;

        public string Response
        {
            get => _response;
            set
            {
                _response = value;
                OnPropertyChanged("Response");
            }
        }

        public bool FilterByUID
        {
            get => _filterByUID;
            set
            {
                _filterByUID = value;
                OnPropertyChanged("FilterByUID");
            }
        }
        public string uId
        {
            get => _uId;
            set
            {
                _uId = value;
                OnPropertyChanged("UID");
            }
        }

        public bool LimitResponseWindowSize
        {
            get => _limitResponseWindowSize;
            set
            {
                _limitResponseWindowSize = value;
                OnPropertyChanged("LimitLogSize");
            }
        }
    }
}
