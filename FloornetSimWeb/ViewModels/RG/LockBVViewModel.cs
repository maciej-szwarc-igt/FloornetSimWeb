using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.RG;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.RG
{
    public class LockBVViewModel
    {
        private readonly iRG _iRGProxy;
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _sendLockBVCommand { get; set; }
        private RelayCommand _getLockBVKeysListCommand { get; set; }
        private readonly ResponseViewModel _responseViewModel;

        public LockBVViewModel(iRG iRGProxy, ResponseViewModel responseViewModel)
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
                    param => !string.IsNullOrEmpty(LockBVModel.uId) || !string.IsNullOrEmpty(LockBVModel.LockKey)
                );

                return _clearCommand;
            }
        }

        public RelayCommand SendLockBVCommand
        {
            get
            {
                _sendLockBVCommand = new RelayCommand(
                    RPCSetLockBV,
                    param => !string.IsNullOrEmpty(LockBVModel.LockKey) && !string.IsNullOrEmpty(LockBVModel.uId)
                );

                return _sendLockBVCommand;
            }
        }

        public RelayCommand GetLockBVKeysCommand
        {
            get
            {
                _getLockBVKeysListCommand = new RelayCommand(
                    RPCGetLockBVKeys,
                    param => !string.IsNullOrEmpty(LockBVModel.uId)
                );

                return _getLockBVKeysListCommand;
            }
        }

        public LockBVModel LockBVModel { get; } = new LockBVModel();

        public void Clear(object obj)
        {
            LockBVModel.Clear();
        }

        private async void RPCSetLockBV(object obj)
        {
            var req = new Dictionary<string, object>
                {
                    {nameof(LockBVModel.Disable), LockBVModel.Disable},
                    {nameof(LockBVModel.LockKey), LockBVModel.LockKey}
                };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(LockBVModel.uId);

            var resp = await _iRGProxy.LockBV(LockBVModel.Disable, LockBVModel.LockKey);

            _responseViewModel.LogRpcResponse(Constants.LockBV, req, resp, RpcCallContext.Current);
        }

        private async void RPCGetLockBVKeys(object obj)
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(LockBVModel.uId);

            var resp = await _iRGProxy.GetLockBVKeys();

            _responseViewModel.LogRpcResponse(Constants.GetLockBVKeys, req, resp, RpcCallContext.Current);
        }
    }
}
