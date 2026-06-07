using IGT.FloorNet.EX.RG;
using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.RG
{
    public class EnableEGMWithLeaseModel : ModelBase
    {
        private string _uId = "5007100";
        private string _disableKey;
        private DateTime _disableAt;
        private double _seconds;
        private double _minutes;
        private double _hours;

        public DateTime DisableAt
        {
            get => _disableAt;
            set
            {
                _disableAt = value;
                OnPropertyChanged("DisableAt");
            }
        }

        public string DisableKey
        {
            get => _disableKey;
            set
            {
                _disableKey = value;
                OnPropertyChanged("DisableKey");
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

        public double Seconds
        {
            get => _seconds;
            set
            {
                if (value > 59)
                {
                    _seconds = 0;
                }
                else
                {
                    _seconds = value;
                }

                OnPropertyChanged("Seconds");
            }
        }

        public double Minutes
        {
            get => _minutes;
            set
            {
                if (value > 59)
                {
                    _minutes = 0;
                }
                else
                {
                    _minutes = value;
                }

                OnPropertyChanged("Minutes");
            }
        }

        public double Hours
        {
            get => _hours;
            set
            {
                if (value > 23)
                {
                    _hours = 0;
                }
                else
                {
                    _hours = value;
                }

                OnPropertyChanged("Hours");
            }
        }

        public EnableEGMWithLeaseModel()
        {
            clear();
        }

        public void clear()
        {
            DisableAt = DateTime.MinValue;
            DisableKey = string.Empty;
            Seconds = 0;
            Minutes = 0;
            Hours = 0;
            uId = string.Empty;            
        }

    }
}
