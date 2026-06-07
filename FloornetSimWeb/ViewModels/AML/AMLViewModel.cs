using IGT.FloorNet.Tools.ServiceSimulator.RpcProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Download;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.AML
{
    public class AMLViewModel : INotifyPropertyChanged
    {

        private ResponseViewModel _responseViewModel;
        public event PropertyChangedEventHandler PropertyChanged;

        private string _uid;
        public string Uid
        {
            get { return _uid; }

            set
            {
                _uid = value;
                OnPropertyChanged(nameof(Uid));
            }
        }

        private long _largestBillDenom = 5000;

        private long _dailyCashLimit = 50000;

        private long _dailyCashAggregate;

        private long _playerId;
        public long PlayerId
        {
            get { return _playerId; }

            set
            {
                _playerId = value;
                OnPropertyChanged(nameof(PlayerId));
            }
        }

        private long _billDenom;
        public long BillDenom
        {
            get { return _billDenom; }

            set
            {
                _billDenom = value;
                OnPropertyChanged(nameof(BillDenom));
            }
        }

        public long LargestBillDenom
        {
            get { return _largestBillDenom; }

            set
            {
                _largestBillDenom = value;
                OnPropertyChanged(nameof(LargestBillDenom));
            }
        }

        public long DailyCashLimit
        {
            get { return _dailyCashLimit; }

            set
            {
                _dailyCashLimit = value;
                OnPropertyChanged(nameof(DailyCashLimit));
            }
        }

        public long DailyCashAggregate
        {
            get { return _dailyCashAggregate; }

            set
            {
                _dailyCashAggregate = value;
                OnPropertyChanged(nameof(DailyCashAggregate));
            }
        }
        public void SendData()
        {
            Startup._iAML.CashAccepted(15354, 500);
            Startup._iAML.GetPlayerLimits(15354);
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
