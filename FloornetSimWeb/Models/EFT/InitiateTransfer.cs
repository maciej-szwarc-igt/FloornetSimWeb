using IGT.FloorNet.EX.Wat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.EFT
{
    public class InitiateTransfer : ModelBase
    {

        private string _resourceId;
        private long _transCashableAmt;
        private long _cashoutTicketAmt;
        private t_watEgmException _watEgmException;
        private DateTime _transDateTime;
        private string _clientSignature;

        public string ResourceId
        {
            get => _resourceId;
            set
            {
                _resourceId = value;
                OnPropertyChanged("ResourceId");
            }
        }

        public long TransCashableAmt
        {
            get => _transCashableAmt;
            set
            {
                _transCashableAmt = value;
                OnPropertyChanged("TransCashableAmt");
            }
        }

        public long CashoutTicketAmt
        {
            get => _cashoutTicketAmt;
            set
            {
                _cashoutTicketAmt = value;
                OnPropertyChanged("CashoutTicketAmt");
            }
        }

        public t_watEgmException WatEgmException
        {
            get => _watEgmException;
            set
            {
                _watEgmException = value;
                OnPropertyChanged("WatEgmException");
            }
        }

        public DateTime TransDateTime
        {
            get => _transDateTime;
            set
            {
                _transDateTime = value;
                OnPropertyChanged("TransDateTime");
            }
        }
        public string ClientSignature
        {
            get => _clientSignature;
            set
            {
                _clientSignature = value;
                OnPropertyChanged("ClientSignature");
            }
        }
    }
}
