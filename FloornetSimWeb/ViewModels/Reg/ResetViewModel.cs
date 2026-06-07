using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Reg;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg
{
    public class ResetViewModel
    {
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _reset { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        private readonly iReg _iRegProxy;

        public ResetViewModel(iReg regProxy, ResponseViewModel responseViewModel)
        {
            _iRegProxy = regProxy;
            _responseViewModel = responseViewModel;
        }

        public RelayCommand ResetCommand
        {
            get
            {
                _reset = new RelayCommand(
                    RPCReset,
                    param => !string.IsNullOrEmpty(ResetModel.uId)
                );

                return _reset;
            }
        }

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                     param => !string.IsNullOrEmpty(ResetModel.uId)
                );

                return _clearCommand;
            }
        }

        public void Clear(object obj)
        {
            ResetModel.Clear();
        }


        public ResetModel ResetModel { get; } = new ResetModel();

        private async void RPCReset(object obj)
        {
            var req = new Dictionary<string, object>
            {
                {nameof(ResetModel.Hard), ResetModel.Hard}                
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(ResetModel.uId);

            var resp = await _iRegProxy.reset(ResetModel.Hard);

            _responseViewModel.LogRpcResponse(Constants.Reset, req, resp, RpcCallContext.Current);

        }
    }
}
