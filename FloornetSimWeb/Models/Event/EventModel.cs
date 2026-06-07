namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Event
{
    public class EventModel : ModelBase
    {
        private string _topic = "IGT.FloorNet.EX.Cage.evt.FillCredit";
        public string Topic
        {
            get { return _topic; }
            set
            {
                _topic = value;
                OnPropertyChanged(Topic);
            }
        }

        private string _json;
        public string EventJson
        {
            get { return _json; }
            set
            {
                _json = value;
                OnPropertyChanged(EventJson);
            }
        }
    }
}
