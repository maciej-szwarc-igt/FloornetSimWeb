using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Reg;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg
{
    public class LockEgmViewModel
    {
        private readonly iReg _iRegProxy;
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _sendLockEGMCommand { get; set; }
        private RelayCommand _getLockEGMKeysListCommand { get; set; }
        private readonly ResponseViewModel _responseViewModel;

        public LockEgmViewModel(iReg iRegProxy, ResponseViewModel responseViewModel)
        {
            _iRegProxy = iRegProxy;
            _responseViewModel = responseViewModel;
        }

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => !string.IsNullOrEmpty(LockEgmModel.uId) || !string.IsNullOrEmpty(LockEgmModel.LockKey)
                );

                return _clearCommand;
            }
        }

        public RelayCommand SendLockEGMCommand
        {
            get
            {
                _sendLockEGMCommand = new RelayCommand(
                    RPCSetLockEGM,
                    param => !string.IsNullOrEmpty(LockEgmModel.LockKey) && !string.IsNullOrEmpty(LockEgmModel.uId)
                );

                return _sendLockEGMCommand;
            }
        }

        public RelayCommand GetLockEGMKeysCommand
        {
            get
            {
                _getLockEGMKeysListCommand = new RelayCommand(
                    RPCGetLockEGMKeys,
                    param => !string.IsNullOrEmpty(LockEgmModel.uId)
                );

                return _getLockEGMKeysListCommand;
            }
        }

        public LockEgmModel LockEgmModel { get; } = new LockEgmModel();

        public void Clear(object obj)
        {
            LockEgmModel.Clear();
        }

        private async void RPCSetLockEGM(object obj)
        {
            var req = new Dictionary<string, object>
                {
                    {nameof(LockEgmModel.Timer), LockEgmModel.Timer},
                    {nameof(LockEgmModel.State), LockEgmModel.State},
                    {nameof(LockEgmModel.LockKey), LockEgmModel.LockKey}
                };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(LockEgmModel.uId);
            var resp = await _iRegProxy.lockEGM(LockEgmModel.Timer, LockEgmModel.State, LockEgmModel.LockKey);
            _responseViewModel.LogRpcResponse(Constants.LockEGM, req, resp, RpcCallContext.Current);
        }

        private async void RPCGetLockEGMKeys(object obj)
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(LockEgmModel.uId);

            var resp = await _iRegProxy.getLockKeys();

            _responseViewModel.LogRpcResponse(Constants.GetLockEGMKeys, req, resp, RpcCallContext.Current);
        }
    }
}
