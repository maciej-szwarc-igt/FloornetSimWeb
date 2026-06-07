using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.Wat;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat;
using IGT.FloorNet.EX.EZPay;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class WatRpcProvider : iWat
    {

        private readonly ResponseViewModel _responseViewModel;

        public WatRpcProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<commitTransferResp> commitTransfer(string requestId, string resourceId, t_idReaderType idReaderType, string cardId, t_transferDirection transferDirection, long transCashableAmt, long transPromoAmt, long transNonCashAmt, long cashoutTicketAmt, long promoTicketAmt, t_watEgmException egmException, DateTime transDateTime, string jwt, string signature)
        {

            var req = new Dictionary<string, object>
            {
                {nameof(requestId), requestId },
                {nameof(resourceId), resourceId },
                {nameof(idReaderType), idReaderType },
                {nameof(cardId),  cardId},
                {nameof(transferDirection),  transferDirection},
                {nameof(transCashableAmt),  transCashableAmt },
                {nameof(transPromoAmt),  transPromoAmt },
                {nameof(transNonCashAmt),  transNonCashAmt },
                {nameof(cashoutTicketAmt),  cashoutTicketAmt },
                {nameof(promoTicketAmt),  promoTicketAmt },
                {nameof(egmException),  egmException },
                {nameof(transDateTime),  transDateTime },
                {nameof(jwt),  jwt },
                {nameof(signature),  signature }
            };

            var resp = new commitTransferResp()
            {
                requestId = requestId,
                resourceId = resourceId,
            };

            _responseViewModel.LogRpc(nameof(commitTransferResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<getPublicKeyResp> getPublicKey()
        {
            if (!IKeysViewModel.keysModelWat.RespondToRPC)
                return Task.FromResult<getPublicKeyResp>(null);

            var req = new Dictionary<string, object>();
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelWat.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelWat.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };
            _responseViewModel.LogRpc(nameof(getPublicKeyResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<getWatAccountsResp> getWatAccounts(t_idReaderType idReaderType, string cardId, long playerId, long cardInCount)
        {
            if(!getWatAccountViewModel.getWatAccountModel.RespondToRPC)
            {
                return Task.FromResult<getWatAccountsResp>(null);
            }
            t_watAccount watAccount = new t_watAccount();
            watAccount.accountId = getWatAccountViewModel.getWatAccountModel.accountId;
            watAccount.authRequired = getWatAccountViewModel.getWatAccountModel.authRequired;
            watAccount.creditType = getWatAccountViewModel.getWatAccountModel.creditType;
            watAccount.withdrawOk = getWatAccountViewModel.getWatAccountModel.withdrawOk;
            watAccount.depositOk = getWatAccountViewModel.getWatAccountModel.depositOk;
            watAccount.selectAmt = getWatAccountViewModel.getWatAccountModel.selectAmt;
            watAccount.defaultAmt = getWatAccountViewModel.getWatAccountModel.defaultAmt;
            watAccount.withdrawMax = getWatAccountViewModel.getWatAccountModel.withdrawMax;
            watAccount.withdrawMin = getWatAccountViewModel.getWatAccountModel.withdrawMin;
            watAccount.depositMax = getWatAccountViewModel.getWatAccountModel.depositMax;
            watAccount.depositMin = getWatAccountViewModel.getWatAccountModel.depositMin;
            watAccount.balance = getWatAccountViewModel.getWatAccountModel.balance;
            watAccount.accountState = getWatAccountViewModel.getWatAccountModel.accountState;
            watAccount.expirationDate = getWatAccountViewModel.getWatAccountModel.expirationDate;

            List<t_watAccount> watAccountList = getWatAccountViewModel.getWatAccountModel.WatAccountsList;
            watAccountList.Clear();
            watAccountList.Add(watAccount);

            var req = new Dictionary<string, object>
            {
                {nameof(idReaderType), idReaderType },
                {nameof(cardId), cardId },
                {nameof(playerId), playerId },
                {nameof(cardInCount), cardInCount }
            };

            var resp = new getWatAccountsResp()
            {
                cardId = getWatAccountViewModel.getWatAccountModel.CardId,
                watAccountList = watAccountList,
                hostException = getWatAccountViewModel.getWatAccountModel.HostException,
            };

            _responseViewModel.LogRpc(nameof(getWatAccountsResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<initiateTransferResp> initiateTransfer(string requestId, string resourceId, t_idReaderType idReaderType, string cardId, t_transferDirection transferDirection, long reqCashableAmt, long reqPromoAmt, long reqNonCashAmt, bool printTicket, bool expireCredits, DateTime? expireDateTime, bool reduceAmts, long playerId, long cardInCount, string jwt, string signature)
        {

            initiateTransferModel model = initiateTransferViewModel.initiateTransferModel;

            model.signature = IKeysViewModel.keysModelWat.FloornetECDsaProvider.ComputeSignature(
                                IKeysViewModel.keysModelWat.ECDsa,
                                requestId,
                                model.resourceId,
                                idReaderType,
                                cardId,
                                model.transferDirection,
                                model.authCashableAmt,
                                model.authPromoAmt,
                                model.authNonCashAmt,
                                model.hostException,
                                IKeysViewModel.keysModelWat.CurrentKeyNum
                            );


            var req = new Dictionary<string, object>
            {
                {nameof(requestId), requestId },
                {nameof(resourceId), resourceId },
                {nameof(idReaderType), idReaderType },
                {nameof(cardId), cardId },
                {nameof(transferDirection), transferDirection },
                {nameof(reqCashableAmt), reqCashableAmt },
                {nameof(reqPromoAmt), reqPromoAmt },
                {nameof(reqNonCashAmt), reqNonCashAmt },
                {nameof(printTicket), printTicket },
                {nameof(expireCredits), expireCredits },
                {nameof(expireDateTime), expireDateTime },
                {nameof(reduceAmts), reduceAmts },
                {nameof(playerId), playerId },
                {nameof(cardInCount), cardInCount },
                {nameof(jwt), jwt },
                {nameof(signature), signature }
            };

            var resp = new initiateTransferResp()
            {
                requestId = requestId,
                resourceId = model.resourceId,
                idReaderType = idReaderType,
                cardId = cardId,
                transferDirection = model.transferDirection,
                authCashableAmt = model.authCashableAmt,
                authPromoAmt = model.authPromoAmt,
                authNonCashAmt = model.authNonCashAmt,
                hostException = model.hostException,
                currentKeyNumber = IKeysViewModel.keysModelWat.CurrentKeyNum,
                signature = model.signature
            };

            _responseViewModel.LogRpc(nameof(initiateTransferResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }
    }
}
