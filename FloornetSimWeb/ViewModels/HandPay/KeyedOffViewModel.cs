using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.HandPay
{
    public class KeyedOffViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static StatusResponseModel KeyedOffModel { get; } = new StatusResponseModel();
        public KeyedOffViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            KeyedOffModel.Clear();
        }

    }
}
