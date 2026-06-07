using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.HandPay;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.HandPay
{
    public class HandPayViewModel
    {
        private readonly ResponseViewModel responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static HandPayResponseModel HandPayResponseModel { get; } = new HandPayResponseModel();
        
        public HandPayViewModel( ResponseViewModel responseViewModel)
        {
            this.responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(ClearModel);
        }
        public void ClearModel(object obj)
        {
            HandPayResponseModel.Clear();
        }

    }
}
