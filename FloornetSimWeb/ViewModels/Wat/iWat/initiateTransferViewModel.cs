using IGT.FloorNet.EX.EZPay;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat
{
    public class initiateTransferViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static initiateTransferModel initiateTransferModel { get; set; } = new initiateTransferModel();
        public initiateTransferViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            initiateTransferModel.Clear();
        }
    }
}
