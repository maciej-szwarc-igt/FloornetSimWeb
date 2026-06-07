using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System.Collections.Generic;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus
{
    public class IBonusViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public static IBonusModel IBonusModel { get; } = new IBonusModel();
        public IBonusViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            IBonusModel.Clear();
        }
    }
}
