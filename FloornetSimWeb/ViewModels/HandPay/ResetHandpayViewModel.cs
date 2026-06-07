using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Handpay;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.EX.Handpay;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using IGT.FloorNet.MessageBus.Rpc;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.HandPay
{
    public class ResetHandpayViewModel
    {
        private readonly iSmibHandpay _iSmibHandpayProxy;
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand SendResetHandpayCommand { get; }

        public ResetHandpayModel ResetHandpayModel { get; } = new ResetHandpayModel();

        public ResetHandpayViewModel(iSmibHandpay iSmibHandpayProxy, ResponseViewModel responseViewModel)
        {
            _iSmibHandpayProxy = iSmibHandpayProxy;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            SendResetHandpayCommand = new RelayCommand(
                RPCSendResetHandpay,
                param => !string.IsNullOrEmpty(ResetHandpayModel.UID) &&
                            !string.IsNullOrEmpty(IKeysViewModel.keysModelHandpay.PublicKey) &&
                            !string.IsNullOrEmpty(IKeysViewModel.keysModelHandpay.PrivateKey)
            );
        }

        public void Clear(object obj)
        {
            ResetHandpayModel.Clear();
        }

        private async void RPCSendResetHandpay(object obj)
        {
            if (ResetHandpayModel.SendInvalidSignature)
            {
                // Compute invalid signature
                ResetHandpayModel.Signature = IKeysViewModel.keysModelHandpay.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelHandpay.ECDsa,
                    ResetHandpayModel.RequestId,
                    ResetHandpayModel.Identity,
                    ResetHandpayModel.Remote,
                    ResetHandpayModel.KeyToCredit,
                    ResetHandpayModel.KeyToCredit, // Adding keyToCredit twice will cause the incorrect signature to be generated.
                    IKeysViewModel.keysModelHandpay.CurrentKeyNum
                );
            }
            else
            {
                // Compute valid signature
                ResetHandpayModel.Signature = IKeysViewModel.keysModelHandpay.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelHandpay.ECDsa,
                    ResetHandpayModel.RequestId,
                    ResetHandpayModel.Identity,
                    ResetHandpayModel.Remote,
                    ResetHandpayModel.KeyToCredit,
                    IKeysViewModel.keysModelHandpay.CurrentKeyNum
                );
            }

            var req = new Dictionary<string, object>
                {
                    {nameof(ResetHandpayModel.RequestId), ResetHandpayModel.RequestId},
                    {nameof(ResetHandpayModel.Identity), ResetHandpayModel.Identity},
                    {nameof(ResetHandpayModel.Remote), ResetHandpayModel.Remote},
                    {nameof(ResetHandpayModel.KeyToCredit), ResetHandpayModel.KeyToCredit},
                    {"KeyNumber", IKeysViewModel.keysModelHandpay.CurrentKeyNum},
                    {nameof(ResetHandpayModel.Signature), ResetHandpayModel.Signature},
                };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(ResetHandpayModel.UID);

            var resp = await _iSmibHandpayProxy.resetHandpay(
                ResetHandpayModel.RequestId,
                ResetHandpayModel.Identity,
                ResetHandpayModel.Remote,
                ResetHandpayModel.KeyToCredit,
                IKeysViewModel.keysModelHandpay.CurrentKeyNum,
                ResetHandpayModel.Signature);

            _responseViewModel.LogRpcResponse(Constants.ResetHandpay, req, resp, RpcCallContext.Current);
        }
    }
}
