using IGT.FloorNet.EX.EFT;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Eft;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Eft
{
    public class ISmibEftViewModel
    {
        private readonly iSmibEft iSmibEftProxy;
        private readonly ResponseViewModel responseViewModel;

        public RelayCommand SendRequestDebit { get; }
        public RelayCommand Clear { get; }
        private RelayCommand _PrintTicket;
        public RelayCommand PrintTicket
        {
            get
            {
                _PrintTicket = new RelayCommand(
                    IsPrintTicketChecked,
                    param => true);
                return _PrintTicket;
            }
        }
        
        private void IsPrintTicketChecked(object obj)
        {
            // CommandManager removed (no WPF)
        }

        public ISmibEftModel iSmibEftModel { get; set; } = new ISmibEftModel();

        public ISmibEftViewModel(iSmibEft iSmibEftProxy, ResponseViewModel responseViewModel)
        {
            this.iSmibEftProxy = iSmibEftProxy;
            this.responseViewModel = responseViewModel;
            SendRequestDebit = new RelayCommand(RequestDebit);
            Clear = new RelayCommand(ClearModel);
        }

        private async void RequestDebit(object obj)
        {
            if(!ValidSmibUid())
            {
                responseViewModel.Log(
                    Constants.BlockLine + "\n" +
                    Constants.EFTService + "\n" +
                    "\nInvalid ISmibUid provided, RPC not sent to SMIB" + "\n" +
                    Constants.BlockLine);
                return;
            }

            iSmibEftModel.Signature = IKeysViewModel.keysModelEft.FloornetECDsaProvider.ComputeSignature(
                IKeysViewModel.keysModelEft.ECDsa,
                iSmibEftModel.ResourceId,
                iSmibEftModel.ReqCashableAmt,
                iSmibEftModel.PrintTicket,
                iSmibEftModel.PlayerId,
                IKeysViewModel.keysModelEft.CurrentKeyNum);
            
            var req = new Dictionary<string, object>
            {
                {nameof(iSmibEftModel.ResourceId), iSmibEftModel.ResourceId },
                {nameof(iSmibEftModel.ReqCashableAmt), iSmibEftModel.ReqCashableAmt },
                {nameof(iSmibEftModel.PrintTicket), iSmibEftModel.PrintTicket},
                {nameof(iSmibEftModel.PlayerId), iSmibEftModel.PlayerId},
                {nameof(IKeysViewModel.keysModelEft.CurrentKeyNum), IKeysViewModel.keysModelEft.CurrentKeyNum},
                {nameof(iSmibEftModel.Signature), iSmibEftModel.Signature },
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(iSmibEftModel.SmibUid);
                
            var response = await iSmibEftProxy.RequestDebit(
                iSmibEftModel.ResourceId,
                iSmibEftModel.ReqCashableAmt,
                iSmibEftModel.PrintTicket,
                iSmibEftModel.PlayerId,
                IKeysViewModel.keysModelEft.CurrentKeyNum,
                iSmibEftModel.Signature
                );

            if (response == null || response.result != t_result.pending)
            {
                IncrementResource();
            }

            responseViewModel.LogRpcResponse(Constants.RequestDebit, req, response, RpcCallContext.Current);
        }
        private void ClearModel(object obj)
        {
            iSmibEftModel.Clear();
        }

        private bool ValidSmibUid()
        {
            bool valid = false;
            if (string.IsNullOrEmpty(iSmibEftModel.SmibUid))
            {
                // MessageBox removed (no WPF)
            }
            else if (!Regex.IsMatch(iSmibEftModel.SmibUid, Constants.SmibUidRegex))
            {
                // MessageBox removed (no WPF)
            }
            else
            {
                valid = true;
            }
            return valid;
        }

        public void IncrementResource()
        {
            var number = ulong.Parse(iSmibEftModel.ResourceId);
            ++number;
            iSmibEftModel.ResourceId = number.ToString();
        }
    }
}
