namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Eft
{
    public class ISmibEftModel : ModelBase
    {
        private string resourceId;
        private long reqCashableAmt;
        private bool printTicket;
        private string signature;
        private string smibUid;
        private long playerId;

        public string ResourceId { get => resourceId; set { resourceId = value; OnPropertyChanged(nameof(resourceId));  } }
        public long ReqCashableAmt { get => reqCashableAmt; set { reqCashableAmt = value; OnPropertyChanged(nameof(reqCashableAmt)); } }
        public bool PrintTicket { get => printTicket; set { printTicket = value; OnPropertyChanged(nameof(printTicket)); } }
        public string Signature { get => signature; set { signature = value; OnPropertyChanged(nameof(signature)); } }
        public string SmibUid { get => smibUid; set { smibUid = value; OnPropertyChanged(nameof(smibUid)); } }
        public long PlayerId { get => playerId; set { playerId = value; OnPropertyChanged(nameof(playerId)); } }
        public ISmibEftModel()
        {
            resourceId = "0";
            reqCashableAmt = 0;
            printTicket = false;
            signature = string.Empty;
            smibUid = string.Empty;
            playerId = 0;
        }

        public void Clear()
        {
            ReqCashableAmt = 1;
            PrintTicket = false;
            Signature = string.Empty;
            SmibUid = string.Empty;
            playerId = 0;
        }
    }
}
