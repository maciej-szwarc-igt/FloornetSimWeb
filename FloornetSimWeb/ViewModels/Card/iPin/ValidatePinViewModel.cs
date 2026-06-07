using IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iPin;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPin
{
    public class ValidatePinViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static ValidatePinModel ValidatePinModel { get; } = new ValidatePinModel();
        public ValidatePinViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            ValidatePinModel.Clear();
        }
    }
}
