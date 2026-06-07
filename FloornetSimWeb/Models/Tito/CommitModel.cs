namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Tito
{
    public class CommitModel : ModelBase
    {
        private long? _transactionId;

        public CommitModel()
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
