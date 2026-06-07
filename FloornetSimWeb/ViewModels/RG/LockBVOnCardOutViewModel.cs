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
    public partial class LockBVOnCardOutViewModel
    {
        private readonly iRG _iRGProxy;
        private readonly ResponseViewModel _responseViewModel;
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _LockBVOnCardOutCommand { get; set; }
        private RelayCommand _getLockBVKeysListCommand { get; set; }

        public LockBVOnCardOutViewModel(iRG iRGProxy, ResponseViewModel responseViewModel)
        {
            _iRGProxy = iRGProxy;
            _responseViewModel = responseViewModel;
        }

        public LockBVOnCardOut LockBVOnCardOutModel { get; } = new LockBVOnCardOut();

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => !string.IsNullOrEmpty(LockBVOnCardOutModel.uId) || !string.IsNullOrEmpty(LockBVOnCardOutModel.LockKey)
                );

                return _clearCommand;
            }
        }

        public RelayCommand LockBVOnCardOutCommand
        {
            get
            {
                _LockBVOnCardOutCommand = new RelayCommand(
                    RPCLockBVOnCardOut,
                    param => !string.IsNullOrEmpty(LockBVOnCardOutModel.LockKey) && !string.IsNullOrEmpty(LockBVOnCardOutModel.uId)
                );

                return _LockBVOnCardOutCommand;
            }
        }

        public RelayCommand GetLockBVOnCardOutCommand
        {
            get
            {
                _getLockBVKeysListCommand = new RelayCommand(
                    RPCGetLockBVOnCardOutKeys,
                    param => !string.IsNullOrEmpty(LockBVOnCardOutModel.uId)
                );

                return _getLockBVKeysListCommand;
            }
        }

        private async void RPCLockBVOnCardOut(object obj)
        {
            var req = new Dictionary<string, object>
            {
                {nameof(LockBVOnCardOutModel.Disable), LockBVOnCardOutModel.Disable},
                {nameof(LockBVOnCardOutModel.LockKey), LockBVOnCardOutModel.LockKey}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(LockBVOnCardOutModel.uId);

            var resp = await _iRGProxy.LockBVOnCardOut(LockBVOnCardOutModel.Disable, LockBVOnCardOutModel.LockKey);

            _responseViewModel.LogRpcResponse(Constants.LockBVOnCardOut, req, resp, RpcCallContext.Current);
        }

        private async void RPCGetLockBVOnCardOutKeys(object obj)
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(LockBVOnCardOutModel.uId);

            var resp = await _iRGProxy.GetLockBVOnCardOutKeys();

            _responseViewModel.LogRpcResponse(Constants.GetLockBVKeys, req, resp, RpcCallContext.Current);
        }

        public void Clear(object obj)
        {
            LockBVOnCardOutModel.Clear();
        }
    }
}
