using System.Collections.Generic;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.RG;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.RG
{
    public  class DisableEgmOnCardOutViewModel
    {
        private readonly iRG _iRGProxy;
        private readonly ResponseViewModel _responseViewModel;
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _disableEGMOnCardOutCommand { get; set; }
        private RelayCommand _getDisableEGMOnCardOutKeysListCommand { get; set; }

        public DisableEgmOnCardOutViewModel(iRG iRGProxy, ResponseViewModel responseViewModel)
        {
            _iRGProxy = iRGProxy;
            _responseViewModel = responseViewModel;
        }

        public DisableEMGOnCardOutModel DisableEGMOnCardOutModel { get; } = new DisableEMGOnCardOutModel();

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => !string.IsNullOrEmpty(DisableEGMOnCardOutModel.uId) || !string.IsNullOrEmpty(DisableEGMOnCardOutModel.DisableKey)
                );

                return _clearCommand;
            }
        }

        public RelayCommand DisableEGMOnCardOutCommand
        {
            get
            {
                _disableEGMOnCardOutCommand = new RelayCommand(
                    RPCDisableEGMOnCardOut,
                    param => !string.IsNullOrEmpty(DisableEGMOnCardOutModel.DisableKey) && !string.IsNullOrEmpty(DisableEGMOnCardOutModel.uId)
                );

                return _disableEGMOnCardOutCommand;
            }
        }

        public RelayCommand GetDisableEGMOnCardOutKeysListCommand
        {
            get
            {
                _getDisableEGMOnCardOutKeysListCommand = new RelayCommand(
                    RPCGetDisableEGMOnCardOutKeysList,
                    param => !string.IsNullOrEmpty(DisableEGMOnCardOutModel.uId)
                );

                return _getDisableEGMOnCardOutKeysListCommand;
            }
        }
        private async void RPCDisableEGMOnCardOut(object obj)
        {
            var req = new Dictionary<string, object>
            {
                {nameof(DisableEGMOnCardOutModel.State), DisableEGMOnCardOutModel.State},
                {nameof(DisableEGMOnCardOutModel.DisableKey), DisableEGMOnCardOutModel.DisableKey}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(DisableEGMOnCardOutModel.uId);

            var resp = await _iRGProxy.DisableEGMOnCardOut(DisableEGMOnCardOutModel.State, DisableEGMOnCardOutModel.DisableKey);

            _responseViewModel.LogRpcResponse(Constants.DisableEGMOnCardOut, req, resp, RpcCallContext.Current);
        }

        private async void RPCGetDisableEGMOnCardOutKeysList(object obj)
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(DisableEGMOnCardOutModel.uId);

            var resp = await _iRGProxy.GetDisableEGMOnCardOutKeys();

            _responseViewModel.LogRpcResponse(Constants.GetDisableEGMOnCardOutKeys, req, resp, RpcCallContext.Current);
        }


        public void Clear(object obj)
        {
            DisableEGMOnCardOutModel.Clear();
        }

    }
}
