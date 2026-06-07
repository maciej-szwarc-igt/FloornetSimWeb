using IGT.FloorNet.EX.EZPay;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System.Collections.Generic;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat
{
    public class getWatAccountViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static getWatAccountModel getWatAccountModel { get; set; } = new getWatAccountModel();
        public getWatAccountViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            getWatAccountModel.WatAccountsList = new List<t_watAccount>();
        }
        public void Clear(object obj)
        {
            getWatAccountModel.Clear();
        }
    }
}
