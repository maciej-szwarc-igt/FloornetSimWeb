using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iPlayer;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer
{
    public class PlayerCardOutViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static StatusResponseModel PlayerCardOutModel { get; } = new StatusResponseModel();
        public PlayerCardOutViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            PlayerCardOutModel.Clear();
        }
    }
}
