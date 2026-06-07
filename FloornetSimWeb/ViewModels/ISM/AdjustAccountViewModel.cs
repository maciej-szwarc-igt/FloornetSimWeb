using System.Collections.Generic;
using IGT.FloorNet.EX.ISM;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.Models.ISM;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.ISM
{
    public class AdjustAccountViewModel
    {
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _sendAdjustAccount { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        private readonly iSmibISM _iSmibISMProxy;

        public AdjustAccountViewModel(iSmibISM iSmibISMProxy, ResponseViewModel responseViewModel)
        {
            _iSmibISMProxy = iSmibISMProxy;
            _responseViewModel = responseViewModel;            
        }

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => true
                );

                return _clearCommand;
            }
        }

        public RelayCommand SendAdjustAccountCommand
        {
            get
            {
                _sendAdjustAccount = new RelayCommand(
                    RPCAdjustAccount,
                    param => !string.IsNullOrEmpty(AdjustAccountModel.UID) &&
                             !string.IsNullOrEmpty(IKeysViewModel.keysModelISM.PublicKey) &&
                             !string.IsNullOrEmpty(IKeysViewModel.keysModelISM.PrivateKey)
                );

                return _sendAdjustAccount;
            }
        }

        public AdjustAccountModel AdjustAccountModel { get; } = new AdjustAccountModel();

        public void Clear(object obj)
        {
            AdjustAccountModel.Clear();
        }

        private async void RPCAdjustAccount(object obj)
        {

            if (AdjustAccountModel.SendInvalidSignature)
            {
                AdjustAccountModel.Signature = IKeysViewModel.keysModelISM.FloornetECDsaProvider.ComputeSignature(
                IKeysViewModel.keysModelISM.ECDsa,
                AdjustAccountModel.CardId,
                AdjustAccountModel.AmountCents,
                AdjustAccountModel.Account,
                AdjustAccountModel.MsgTimeSecs,
                AdjustAccountModel.MsgTone,
                AdjustAccountModel.Msg,
                AdjustAccountModel.Direction,
                AdjustAccountModel.Direction, // Adding Direction twice will cause the incorrect Signature to be generated.
                IKeysViewModel.keysModelISM.CurrentKeyNum
                );
            }
            else
            {
                AdjustAccountModel.Signature = IKeysViewModel.keysModelISM.FloornetECDsaProvider.ComputeSignature(
                IKeysViewModel.keysModelISM.ECDsa,
                AdjustAccountModel.CardId,
                AdjustAccountModel.AmountCents,
                AdjustAccountModel.Account,
                AdjustAccountModel.MsgTimeSecs,
                AdjustAccountModel.MsgTone,
                AdjustAccountModel.Msg,
                AdjustAccountModel.Direction,
                IKeysViewModel.keysModelISM.CurrentKeyNum
                );
            }

            var req = new Dictionary<string, object>
            {
                {nameof(AdjustAccountModel.CardId), AdjustAccountModel.CardId},
                {nameof(AdjustAccountModel.AmountCents), AdjustAccountModel.AmountCents},
                {nameof(AdjustAccountModel.Account), AdjustAccountModel.Account},
                {nameof(AdjustAccountModel.MsgTimeSecs), AdjustAccountModel.MsgTimeSecs},
                {nameof(AdjustAccountModel.MsgTone), AdjustAccountModel.MsgTone},
                {nameof(AdjustAccountModel.Msg), AdjustAccountModel.Msg},
                {nameof(AdjustAccountModel.Direction), AdjustAccountModel.Direction},
                {"KeyNumber", IKeysViewModel.keysModelISM.CurrentKeyNum},
                {nameof(AdjustAccountModel.Signature), AdjustAccountModel.Signature},
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(AdjustAccountModel.UID);

            var resp = await _iSmibISMProxy.AdjustAccount(AdjustAccountModel.CardId, AdjustAccountModel.AmountCents, AdjustAccountModel.Account,
                                                          AdjustAccountModel.MsgTimeSecs, AdjustAccountModel.MsgTone, AdjustAccountModel.Msg,
                                                          AdjustAccountModel.Direction, IKeysViewModel.keysModelISM.CurrentKeyNum, AdjustAccountModel.Signature);

            _responseViewModel.LogRpcResponse(Constants.AdjustAccount, req, resp, RpcCallContext.Current);
        }

    }
}
