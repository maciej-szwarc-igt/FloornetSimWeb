using IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iPlayer;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer
{
    public class CardInViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static CardInModel CardInModel { get; } = new CardInModel();
        public CardInViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            CardInModel.Clear();
        }
    }
}
