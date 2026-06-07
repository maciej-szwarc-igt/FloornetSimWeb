
namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Reg
{
    public class RegistrationModel : ModelBase
    {
        private string _uId;

        public RegistrationModel() {
            Clear();
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

        public void Clear()
        {
            uId = string.Empty;
        }

    }
}
