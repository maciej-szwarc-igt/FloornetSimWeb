using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Gat;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Gat
{
    public class IGatViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static IGatModel IGatModel { get; } = new IGatModel();
        public IGatViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            IGatModel.Clear();
        }
    }
}
