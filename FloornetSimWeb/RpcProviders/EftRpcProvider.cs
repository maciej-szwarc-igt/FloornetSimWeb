using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.EFT;
using IGT.FloorNet.EX.Wat;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Eft;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class EftRpcProvider : iEft
    {
        private readonly ResponseViewModel responseViewModel;
        private readonly ISmibEftViewModel smibEftViewModel;
        public EftRpcProvider(ResponseViewModel responseViewModel, ISmibEftViewModel smibEftViewModel)
        {
            this.responseViewModel = responseViewModel;
            this.smibEftViewModel = smibEftViewModel;
        }

        public async Task<CommitDebitResp> CommitDebit(string resourceId, long transCashableAmt, long cashoutTicketAmt, t_watEgmException egmException, DateTime transDateTime, string signature)
        {
            if (!IEftViewModel.IEftModel.RespondToRPC)
                return await Task.FromResult<CommitDebitResp>(null);

            var req = new Dictionary<string, object>
            {
                { nameof(resourceId), resourceId},
                { nameof(transCashableAmt), transCashableAmt},
                { nameof(cashoutTicketAmt), cashoutTicketAmt},
                { nameof(egmException), egmException},
                { nameof(transDateTime), transDateTime},
                { nameof(signature), signature}
            };
            //string debugCorrectSignature = /*String for debug since there is no implementation at the smib*/
            //            IKeysViewModel.keysModelEft.FloornetECDsaProvider.ComputeSignature(IKeysViewModel.keysModelEft.ECDsa,
            //            resourceId, transCashableAmt, cashoutTicketAmt, egmException, transDateTime);
            
            if(!IEftViewModel.IEftModel.UsedResourceIds.Contains(resourceId))//If its already added, just acknowledge it
            {
                IEftViewModel.IEftModel.UsedResourceIds.Add(resourceId);
            }

            var resp = new CommitDebitResp()
            {
                resourceId = IEftViewModel.IEftModel.ResourceId
            };
            smibEftViewModel.IncrementResource();

            responseViewModel.LogRpc(nameof(CommitDebitResp), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

        public Task<CommitDebitResp> CommitDebit(string resourceId, long transCashableAmt, long cashoutTicketAmt, t_watEgmException egmException, DateTime transDateTime, t_idReaderType idReaderType, string cardId, long playerId, long cardInCount, string signature)
        {
            return CommitDebit(resourceId, transCashableAmt, cashoutTicketAmt, egmException, transDateTime, signature);
        }

        public async Task<getPublicKeyResp> getPublicKey()
        {
            var req = new Dictionary<string, object>();
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelEft.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelEft.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };
            responseViewModel.LogRpc(nameof(getPublicKey), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
        public async Task<InitiateDebitResp> InitiateDebit(string resourceId, long reqCashableAmt, bool printTicket, t_idReaderType idReaderType, string cardId, long playerId, long cardInCount, string signature)
        {
            if (!IEftViewModel.IEftModel.RespondToRPC)
                return await Task.FromResult<InitiateDebitResp>(null);

            if (IEftViewModel.IEftModel.InitiateDebitDelaySeconds > 0 &&
                IEftViewModel.IEftModel.RemainingNoResponseTimeMs == 0 &&
                IEftViewModel.IEftModel.ResourceId != resourceId)
            {
                IEftViewModel.IEftModel.StartNoResponseCountdown();
            }

            // All calls wait for the same decreasing timer
            await IEftViewModel.IEftModel.WaitForNoResponseDelayAsync();

            //Default value for CurrentKeyNumber & saving the reqCashableAmt
            IEftViewModel.IEftModel.CurrentKeyNumber = IKeysViewModel.keysModelEft.CurrentKeyNum;
            IEftViewModel.IEftModel.ReqCashableAmt = reqCashableAmt;
            IEftViewModel.IEftModel.AuthCashableAmt = 0;
            IEftViewModel.IEftModel.Signature = string.Empty;
            IEftViewModel.IEftModel.ResourceId = resourceId;

            //Flag to validate the signature matches the parameters received
            bool isSameSignature = false;
            try
            {
                isSameSignature = IKeysViewModel.keysModelEft.FloornetECDsaProvider.VerifySignature(
                 IKeysViewModel.GetSmibKey(RpcCallContext.Current.Uid) ?? "", signature, resourceId, reqCashableAmt, printTicket, idReaderType, cardId, playerId, cardInCount);
            }
            catch (Exception ex)
            {
                responseViewModel.Log($"VerifySignature failed with exception: {ex.ToString()}");
            }
            var req = new Dictionary<string, object>
            {
                { nameof(resourceId), resourceId},
                { nameof(reqCashableAmt), reqCashableAmt},
                { nameof(printTicket), printTicket},
                { nameof(signature), signature}
            };

            //Validates if the resourceId has been already used
            if (IEftViewModel.IEftModel.UsedResourceIds.Contains(resourceId))
            {
                IEftViewModel.IEftModel.HostException = t_watException.unable_to_accept_transfer;
            }
            else if (!isSameSignature)
            {
                IEftViewModel.IEftModel.HostException = t_watException.signature_failed;
                //string debugCorrectSignature = /*String for debug since there is no implementation at the smib*/
                //    IKeysViewModel.keysModelEft.FloornetECDsaProvider.ComputeSignature(IKeysViewModel.keysModelEft.ECDsa,
                //    resourceId, reqCashableAmt, printTicket);
            }
            else if (IEftViewModel.IEftModel.HostException == IGT.FloorNet.EX.Wat.t_watException.authorized)//If authorized is selected, then the reqAmount is allowed
            {
                IEftViewModel.IEftModel.AuthCashableAmt = reqCashableAmt;
            }

            IEftViewModel.IEftModel.Signature = IKeysViewModel.keysModelEft.FloornetECDsaProvider.ComputeSignature(IKeysViewModel.keysModelEft.ECDsa,
                IEftViewModel.IEftModel.ResourceId,
                IEftViewModel.IEftModel.AuthCashableAmt,
                IEftViewModel.IEftModel.HostException,
                IKeysViewModel.keysModelEft.CurrentKeyNum);

            var resp = new InitiateDebitResp()
            {
                resourceId = IEftViewModel.IEftModel.ResourceId,
                authCashableAmt = IEftViewModel.IEftModel.AuthCashableAmt,
                hostException = IEftViewModel.IEftModel.HostException,
                currentKeyNumber = IEftViewModel.IEftModel.CurrentKeyNumber,
                signature = IEftViewModel.IEftModel.Signature
            };

            responseViewModel.LogRpc(nameof(InitiateDebit), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
    }
}
