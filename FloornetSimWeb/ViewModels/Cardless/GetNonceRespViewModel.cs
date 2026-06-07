using IGT.FloorNet.EX.Cardless;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Cardless;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Gat;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Cardless
{
    public class GetNonceRespViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public static NonceResponseModel NonceResponseModel { get; set; } = new NonceResponseModel();
        public GetNonceRespViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }
    }
}
