namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Tito
{
    public class IssueModel : ModelBase
    {
        private long? _transactionId;

        public IssueModel()
        {
            Clear();
        }

        public long? TransactionId
        {
            get => _transactionId;
            set
            {
                _transactionId = value;
                OnPropertyChanged("TransactionId");
            }
        }

        public void Clear()
        {
            TransactionId = null;
        }
    }
}
