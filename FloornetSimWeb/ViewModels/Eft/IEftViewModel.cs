using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Eft;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Eft
{
    public class IEftViewModel
    {
        private readonly ResponseViewModel responseViewModel;
        public RelayCommand ClearCommand { get;}
        public static IEftModel IEftModel { get; } = new IEftModel();
        public IEftViewModel(ResponseViewModel responseViewModel)
        {
            this.responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }
        public void Clear(object obj)
        {
            IEftModel.Clear();
        }
    }
}
