using IGT.FloorNet.EX.Tito;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class TitoRpcProvider : iTito
    {
        private readonly IssueViewModel _issueViewModel;
        private readonly CommitViewModel _commitViewModel;
        private readonly RedeemViewModel _redeemViewModel;
        private readonly ValidationViewModel _validationViewModel;
        private readonly ResponseViewModel _responseViewModel;

        public TitoRpcProvider(
            IssueViewModel issueViewModel,
            CommitViewModel commitViewModel,
            RedeemViewModel redeemViewModel,
            ValidationViewModel validationViewModel,
            ResponseViewModel responseViewModel)
        {
            _issueViewModel = issueViewModel;
            _commitViewModel = commitViewModel;
            _redeemViewModel = redeemViewModel;
            _validationViewModel = validationViewModel;
            _responseViewModel = responseViewModel;
        }

        public async Task<commitVoucherResp> commitVoucher(string voucherId, long transactionId, long transferAmount, t_action action, t_egmException egmException)
        {
            if (!_commitViewModel.Model.RespondToRPC)
                return await Task.FromResult<commitVoucherResp>(null);

            var req = new Dictionary<string, object>
            {
                {nameof(voucherId), voucherId},
                {nameof(transactionId), transactionId},
                {nameof(transferAmount), transferAmount},
                {nameof(action), action},
                {nameof(egmException), egmException}
            };

            var resp = new commitVoucherResp()
            {
                transactionId = _commitViewModel.Model.TransactionId.HasValue
                    ? _commitViewModel.Model.TransactionId.Value
                    : transactionId
            };

            _responseViewModel.LogRpc(Constants.CommitVoucher, req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

        public async Task<getValidationIdsResp> getValidationIds(long requestedCount, t_voucheringType voucheringType)
        {

            if (!_validationViewModel.Model.RespondToRPC)
                return await Task.FromResult<getValidationIdsResp>(null);

            var req = new Dictionary<string, object>
            {
                {nameof(requestedCount), requestedCount},
                {nameof(voucheringType), voucheringType }
            };

            getValidationIdsResp resp = null;
            if (_validationViewModel.Model.IsValid)
            {
                resp = new getValidationIdsResp()
                {
                    seedDateTime = _validationViewModel.Model.SeedDateTime.Value,
                    seedValue1 = _validationViewModel.Model.SeedValue1.Value,
                    seedValue2 = _validationViewModel.Model.SeedValue2.Value,
                    validationIds = new List<string>(),
                    voucheringType = voucheringType
                };
                if (!string.IsNullOrEmpty(_validationViewModel.Model.ValidationIds))
                {
                    resp.validationIds.AddRange(_validationViewModel.Model.ValidationIds.Split(Environment.NewLine));
                }
            }

            _responseViewModel.LogRpc(Constants.GetValidationId, req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

        public async Task<issueVoucherResp> issueVoucher(string voucherId, long voucherAmt, 
            DateTime voucherDateTime, long voucherSequence, bool largeWin, bool handpay, 
            t_creditType creditType, long poolId, DateTime? expirationDateTime, string cardId, 
            long transactionId)
        {
            if (!_issueViewModel.Model.RespondToRPC)
                return await Task.FromResult<issueVoucherResp>(null);

            var req = new Dictionary<string, object>
            {
                {nameof(voucherId), voucherId},
                {nameof(voucherAmt), voucherAmt},
                {nameof(voucherDateTime), voucherDateTime},
                {nameof(voucherSequence), voucherSequence},
                {nameof(largeWin), largeWin},
                {nameof(handpay), handpay},
                {nameof(creditType), creditType},
                {nameof(poolId), poolId},
                {nameof(expirationDateTime), expirationDateTime},
                {nameof(cardId), cardId},
                {nameof(transactionId), transactionId}
            };

            var resp = new issueVoucherResp()
            {
                transactionId = _issueViewModel.Model.TransactionId.HasValue
                 ? _issueViewModel.Model.TransactionId.Value
                 : transactionId
            };

            _responseViewModel.LogRpc(Constants.IssueVoucher, req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

        public async Task<redeemVoucherResp> redeemVoucher(string voucherId, string cardId, long transactionId)
        {
            if (!_redeemViewModel.Model.RespondToRPC)
                return await Task.FromResult<redeemVoucherResp>(null);

            var req = new Dictionary<string, object>
            {
                {nameof(voucherId), voucherId},
                {nameof(cardId), cardId},
                {nameof(transactionId), transactionId }
            };

            redeemVoucherResp resp = null;
            if (_redeemViewModel.Model.IsValid)
            {
                resp = new redeemVoucherResp()
                {
                    cardId = cardId,
                    creditType = _redeemViewModel.Model.CreditType,
                    transactionId = transactionId,
                    expirationDateTime = _redeemViewModel.Model.ExpirationDateTime,
                    hostException = _redeemViewModel.Model.HostException,
                    poolId = _redeemViewModel.Model.PoolId.Value,
                    voucherAmount = _redeemViewModel.Model.VoucherAmount.Value,
                    voucherId = voucherId
                };
            }

            _responseViewModel.LogRpc(Constants.RedeemVoucher, req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
    }
}