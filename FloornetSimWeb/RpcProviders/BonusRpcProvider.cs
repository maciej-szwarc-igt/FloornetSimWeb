using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus;
using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.MessageBus.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{    
    public class BonusRpcProvider : iBonus
    {
        private readonly ResponseViewModel _responseViewModel;
        public BonusRpcProvider( ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<getPublicKeyResp> getPublicKey()
        {
            if (!IKeysViewModel.keysModelBonus.RespondToRPC)
                return await Task.FromResult<getPublicKeyResp>(null);

            var req = new Dictionary<string, object>
            {

            };
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelBonus.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelBonus.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };

            _responseViewModel.LogRpc(nameof(getPublicKey), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

        public async Task<bonusCommitResp> bonusCommit(long levelId, long hitSeqNum, long amount, DateTime timestamp, bool celebration, long mjtMultiplier, bool ackRequiredTimeout, t_commitStatus status, t_bonusCode type, long machineLevel, t_payTo payTo, t_payMethod payMethod, t_payTo paidAs)
        {

            if (!IBonusViewModel.IBonusModel.RespondToRPC)
                return await Task.FromResult<bonusCommitResp>(null);

            var req = new Dictionary<string, object>
            {
                {nameof(levelId), levelId },
                {nameof(hitSeqNum), hitSeqNum },
                {nameof(amount),  amount},
                {nameof(timestamp),  timestamp},
                {nameof(celebration),  celebration },
                {nameof(mjtMultiplier),  mjtMultiplier },
                {nameof(ackRequiredTimeout),ackRequiredTimeout},
                {nameof(status), status },
                {nameof(type), type },
                {nameof(machineLevel), machineLevel},
                {nameof(payTo), payTo },
                {nameof(payMethod), payMethod },
                {nameof(paidAs), paidAs },
            };
            var resp = new bonusCommitResp()
            {
                result = IBonusViewModel.IBonusModel.Result,
                message = IBonusViewModel.IBonusModel.Message,
                progress = IBonusViewModel.IBonusModel.Progress,
                function = IBonusViewModel.IBonusModel.Function,
                requestId = IBonusViewModel.IBonusModel.RequestId,
                requestIdStr = IBonusViewModel.IBonusModel.RequestIdStr,
                sequence = null
            };

            if (IBonusViewModel.IBonusModel.SendSequence)
            {
                resp.sequence = IBonusViewModel.IBonusModel.Sequence;
            }

            _responseViewModel.LogRpc(nameof(bonusCommit), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

    }
}
