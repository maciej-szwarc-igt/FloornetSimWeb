using System;
using System.Collections.Generic;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.RG;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.RG
{
    public class EnableEgmWithLeaseViewModel
    {
        private readonly iRG _iRGProxy;
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _sendEnableEGMWithLeaseCommand { get; set; }
        private RelayCommand _getEnableEGMWithLeaseKeysListCommand { get; set; }
        private readonly ResponseViewModel _responseViewModel;

        public EnableEgmWithLeaseViewModel(iRG iRGProxy, ResponseViewModel responseViewModel)
        {
            _iRGProxy = iRGProxy;
            _responseViewModel = responseViewModel;
        }


        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => !string.IsNullOrEmpty(EnableEGMWithLeaseModel.uId) || !string.IsNullOrEmpty(EnableEGMWithLeaseModel.DisableKey) || EnableEGMWithLeaseModel.Hours != 0 || EnableEGMWithLeaseModel.Minutes != 0 || EnableEGMWithLeaseModel.Seconds != 0
                );

                return _clearCommand;
            }
        }

        public RelayCommand SendEnableEGMWithLeaseCommand
        {
            get
            {
                _sendEnableEGMWithLeaseCommand = new RelayCommand(
                    RPCSendEnableEGMWithLease,
                    param => (!string.IsNullOrEmpty(EnableEGMWithLeaseModel.uId) && !string.IsNullOrEmpty(EnableEGMWithLeaseModel.DisableKey))
                );

                return _sendEnableEGMWithLeaseCommand;
            }
        }

        public RelayCommand GetEnableEGMWithLeaseKeysListCommand
        {
            get
            {
                _getEnableEGMWithLeaseKeysListCommand = new RelayCommand(
                    RPCGetEnableEGMWithLeaseKeysList,
                    param => !string.IsNullOrEmpty(EnableEGMWithLeaseModel.uId)
                );

                return _getEnableEGMWithLeaseKeysListCommand;
            }
        }

        public EnableEGMWithLeaseModel EnableEGMWithLeaseModel { get; } = new EnableEGMWithLeaseModel();

        public void Clear(object obj)
        {
            EnableEGMWithLeaseModel.clear();
        }

        private async void RPCSendEnableEGMWithLease(object obj)
        {
            EnableEGMWithLeaseModel.DisableAt = DateTime.UtcNow;         
            EnableEGMWithLeaseModel.DisableAt = EnableEGMWithLeaseModel.DisableAt.AddHours(EnableEGMWithLeaseModel.Hours);
            EnableEGMWithLeaseModel.DisableAt = EnableEGMWithLeaseModel.DisableAt.AddMinutes(EnableEGMWithLeaseModel.Minutes);
            EnableEGMWithLeaseModel.DisableAt = EnableEGMWithLeaseModel.DisableAt.AddSeconds(EnableEGMWithLeaseModel.Seconds);

            var req = new Dictionary<string, object>
                {
                    {nameof(EnableEGMWithLeaseModel.DisableAt), EnableEGMWithLeaseModel.DisableAt},
                    {nameof(EnableEGMWithLeaseModel.DisableKey), EnableEGMWithLeaseModel.DisableKey}
                };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(EnableEGMWithLeaseModel.uId);

            var resp = await _iRGProxy.EnableEGMwithLease(EnableEGMWithLeaseModel.DisableAt, EnableEGMWithLeaseModel.DisableKey);

            _responseViewModel.LogRpcResponse(Constants.EnableEGMWithLease, req, resp, RpcCallContext.Current);

        }

        private async void RPCGetEnableEGMWithLeaseKeysList(object obj)
        {
            var req = new Dictionary<string, object>{};

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(EnableEGMWithLeaseModel.uId);

            var resp = await _iRGProxy.GetEnableEGMLeaseKeys();

            _responseViewModel.LogRpcResponse(Constants.GetEnableEGMWithLease, req, resp, RpcCallContext.Current);
        }

    }
}
