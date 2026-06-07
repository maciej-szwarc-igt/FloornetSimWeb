using IGT.FloorNet.Tools.ServiceSimulator.Models.Tito;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio
{
    public class CommitViewModel
    {
        private RelayCommand _clearCommand;

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => true
                );

                return _clearCommand;
            }
        }

        public CommitModel Model { get; } = new CommitModel();

        public void Clear(object obj)
        {
            Model.Clear();
        }
    }
}
