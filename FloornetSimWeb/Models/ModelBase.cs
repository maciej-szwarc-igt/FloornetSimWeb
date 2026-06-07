using System.ComponentModel;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool _respondToRPC = true;

        public bool RespondToRPC
        {
            get => _respondToRPC;
            set
            {
                _respondToRPC = value; OnPropertyChanged(nameof(RespondToRPC));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
