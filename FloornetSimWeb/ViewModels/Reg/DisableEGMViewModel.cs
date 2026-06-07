using IGT.FloorNet.MessageBus.Rpc;
using System.Collections.Generic;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.EX.Player.evt;
using System.Transactions;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Reg;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg
{
    public class DisableEgmViewModel
    {
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _getDisableKeys { get; set; }
        private RelayCommand _disableEGMCommand { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        private readonly iReg _iRegProxy;


        public DisableEgmViewModel(iReg regProxy, ResponseViewModel responseViewModel)
        {
            _iRegProxy = regProxy;
            _responseViewModel = responseViewModel;
        }

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => !string.IsNullOrEmpty(DisableEGMModel.uId) || !string.IsNullOrEmpty(DisableEGMModel.DisableKey)
                );

                return _clearCommand;
            }
        }

        public RelayCommand DisableEGMCommand
        {
            get
            {
                _disableEGMCommand = new RelayCommand(
                    RPCDisableEGM,
                    param => !string.IsNullOrEmpty(DisableEGMModel.DisableKey) && !string.IsNullOrEmpty(DisableEGMModel.uId)
                );

                return _disableEGMCommand;
            }
        }

        public RelayCommand GetDisableKeysCommand
        {
            get
            {
                _getDisableKeys = new RelayCommand(
                    RPCGetDisableKeys,
                    param => !string.IsNullOrEmpty(DisableEGMModel.uId)
                );

                return _getDisableKeys;
            }
        }

        public DisableEGMModel DisableEGMModel { get; } = new DisableEGMModel();

        public void Clear(object obj)
        {
            DisableEGMModel.Clear();
        }

        private async void RPCDisableEGM(object obj)
        {
            var req = new Dictionary<string, object>
            {
                {nameof(DisableEGMModel.State), DisableEGMModel.State},
                {nameof(DisableEGMModel.DisableKey), DisableEGMModel.DisableKey}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(DisableEGMModel.uId);

            var resp = await _iRegProxy.disableEGM(DisableEGMModel.State, DisableEGMModel.DisableKey);

            _responseViewModel.LogRpcResponse(Constants.DisableEGM, req, resp, RpcCallContext.Current);
        }

        private async void RPCGetDisableKeys(object obj)
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(DisableEGMModel.uId);

            var resp = await _iRegProxy.getDisableKeys();

            _responseViewModel.LogRpcResponse(Constants.GetDisableKeys, req, resp, RpcCallContext.Current);
        }
    }
}
