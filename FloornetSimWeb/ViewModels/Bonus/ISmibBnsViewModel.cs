using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System.Collections.Generic;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus
{
    public class ISmibBnsViewModel
    {
        private readonly iSmibBns _iSmibBnsProxy;
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand SendBonusHitCommand { get; }

        public ISmibBnsViewModel(iSmibBns iSmibBnsProxy, ResponseViewModel responseViewModel)
        {
            _iSmibBnsProxy = iSmibBnsProxy;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            SendBonusHitCommand = new RelayCommand(
                RPCSetBonusHit,
                param => !string.IsNullOrEmpty(ISmibBnsModel.UID) &&
                            !string.IsNullOrEmpty(IKeysViewModel.keysModelBonus.PublicKey) &&
                            !string.IsNullOrEmpty(IKeysViewModel.keysModelBonus.PrivateKey)
            );
        }

        public ISmibBnsModel ISmibBnsModel { get; } = new ISmibBnsModel();
        public void Clear(object obj)
        {
            ISmibBnsModel.Clear();
        }

        private async void RPCSetBonusHit(object obj)
        {
            if (ISmibBnsModel.SendInvalidSignature)
            {
                ISmibBnsModel.Signature = IKeysViewModel.keysModelBonus.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelBonus.ECDsa,
                    ISmibBnsModel.LevelId,
                    ISmibBnsModel.HitSeqNum,
                    ISmibBnsModel.Celebration,
                    ISmibBnsModel.ControlStringId,
                    IKeysViewModel.keysModelBonus.CurrentKeyNum,
                    ISmibBnsModel.Type,
                    ISmibBnsModel.MachineLevel,
                    ISmibBnsModel.DefaultAmt,
                    ISmibBnsModel.CardedAmt,
                    ISmibBnsModel.PreferredAmt,
                    ISmibBnsModel.PayTo,
                    ISmibBnsModel.PayMethod,
                    ISmibBnsModel.Carded,
                    ISmibBnsModel.AckRequired,
                    ISmibBnsModel.LockUntilAcked,
                    ISmibBnsModel.TestEligible,
                    ISmibBnsModel.PayAbandoned,
                    ISmibBnsModel.MjtDefaultLen,
                    ISmibBnsModel.MjtCardedLen,
                    ISmibBnsModel.MjtPreferredLen,
                    ISmibBnsModel.MjtExpireTime,
                    ISmibBnsModel.MjtMinWin,
                    ISmibBnsModel.MjtMaxWin,
                    ISmibBnsModel.CardId,
                    ISmibBnsModel.PlayerId,
                    ISmibBnsModel.MjtRollUp,
                    ISmibBnsModel.AckTimeout,
                    ISmibBnsModel.MessageTimeout,
                    ISmibBnsModel.Timestamp,
                    ISmibBnsModel.Timestamp // Adding Timestamp twice will cause the incorrect signature to be generated.
                );
            }
            else
            {
                //Calculate Signature
                ISmibBnsModel.Signature = IKeysViewModel.keysModelBonus.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelBonus.ECDsa,
                    ISmibBnsModel.LevelId,
                    ISmibBnsModel.HitSeqNum,
                    ISmibBnsModel.Celebration,
                    ISmibBnsModel.ControlStringId,
                    IKeysViewModel.keysModelBonus.CurrentKeyNum,
                    ISmibBnsModel.Type,
                    ISmibBnsModel.MachineLevel,
                    ISmibBnsModel.DefaultAmt,
                    ISmibBnsModel.CardedAmt,
                    ISmibBnsModel.PreferredAmt,
                    ISmibBnsModel.PayTo,
                    ISmibBnsModel.PayMethod,
                    ISmibBnsModel.Carded,
                    ISmibBnsModel.AckRequired,
                    ISmibBnsModel.LockUntilAcked,
                    ISmibBnsModel.TestEligible,
                    ISmibBnsModel.PayAbandoned,
                    ISmibBnsModel.MjtDefaultLen,
                    ISmibBnsModel.MjtCardedLen,
                    ISmibBnsModel.MjtPreferredLen,
                    ISmibBnsModel.MjtExpireTime,
                    ISmibBnsModel.MjtMinWin,
                    ISmibBnsModel.MjtMaxWin,
                    ISmibBnsModel.CardId,
                    ISmibBnsModel.PlayerId,
                    ISmibBnsModel.MjtRollUp,
                    ISmibBnsModel.AckTimeout,
                    ISmibBnsModel.MessageTimeout,
                    ISmibBnsModel.Timestamp
                );
            }

            var req = new Dictionary<string, object>
                {
                    {nameof(ISmibBnsModel.LevelId), ISmibBnsModel.LevelId},
                    {nameof(ISmibBnsModel.HitSeqNum), ISmibBnsModel.HitSeqNum},
                    {nameof(ISmibBnsModel.Celebration), ISmibBnsModel.Celebration},
                    {nameof(ISmibBnsModel.ControlStringId), ISmibBnsModel.ControlStringId},
                    {"KeyNumber", IKeysViewModel.keysModelBonus.CurrentKeyNum}, 
                    {nameof(ISmibBnsModel.Type), ISmibBnsModel.Type},
                    {nameof(ISmibBnsModel.MachineLevel), ISmibBnsModel.MachineLevel},
                    {nameof(ISmibBnsModel.DefaultAmt), ISmibBnsModel.DefaultAmt},
                    {nameof(ISmibBnsModel.CardedAmt), ISmibBnsModel.CardedAmt},
                    {nameof(ISmibBnsModel.PreferredAmt), ISmibBnsModel.PreferredAmt},
                    {nameof(ISmibBnsModel.PayTo), ISmibBnsModel.PayTo},
                    {nameof(ISmibBnsModel.PayMethod), ISmibBnsModel.PayMethod},
                    {nameof(ISmibBnsModel.Carded), ISmibBnsModel.Carded},
                    {nameof(ISmibBnsModel.AckRequired), ISmibBnsModel.AckRequired},
                    {nameof(ISmibBnsModel.LockUntilAcked), ISmibBnsModel.LockUntilAcked},
                    {nameof(ISmibBnsModel.TestEligible), ISmibBnsModel.TestEligible},
                    {nameof(ISmibBnsModel.PayAbandoned), ISmibBnsModel.PayAbandoned},
                    {nameof(ISmibBnsModel.MjtDefaultLen), ISmibBnsModel.MjtDefaultLen},
                    {nameof(ISmibBnsModel.MjtCardedLen), ISmibBnsModel.MjtCardedLen},
                    {nameof(ISmibBnsModel.MjtPreferredLen), ISmibBnsModel.MjtPreferredLen},
                    {nameof(ISmibBnsModel.MjtExpireTime), ISmibBnsModel.MjtExpireTime},
                    {nameof(ISmibBnsModel.MjtMinWin), ISmibBnsModel.MjtMinWin},
                    {nameof(ISmibBnsModel.MjtMaxWin), ISmibBnsModel.MjtMaxWin},
                    {nameof(ISmibBnsModel.CardId), ISmibBnsModel.CardId},
                    {nameof(ISmibBnsModel.PlayerId), ISmibBnsModel.PlayerId},
                    {nameof(ISmibBnsModel.MjtRollUp), ISmibBnsModel.MjtRollUp},
                    {nameof(ISmibBnsModel.AckTimeout), ISmibBnsModel.AckTimeout},
                    {nameof(ISmibBnsModel.MessageTimeout), ISmibBnsModel.MessageTimeout},
                    {nameof(ISmibBnsModel.Timestamp), ISmibBnsModel.Timestamp},
                    {nameof(ISmibBnsModel.Signature), ISmibBnsModel.Signature}, 
                };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(ISmibBnsModel.UID);

            var resp = await _iSmibBnsProxy.bonusHit(ISmibBnsModel.LevelId,
                ISmibBnsModel.HitSeqNum,
                ISmibBnsModel.Celebration,
                ISmibBnsModel.ControlStringId,
                IKeysViewModel.keysModelBonus.CurrentKeyNum,
                ISmibBnsModel.Type,
                ISmibBnsModel.MachineLevel,
                ISmibBnsModel.DefaultAmt,
                ISmibBnsModel.CardedAmt,
                ISmibBnsModel.PreferredAmt,
                ISmibBnsModel.PayTo,
                ISmibBnsModel.PayMethod,
                ISmibBnsModel.Carded,
                ISmibBnsModel.AckRequired,
                ISmibBnsModel.LockUntilAcked,
                ISmibBnsModel.TestEligible,
                ISmibBnsModel.PayAbandoned,
                ISmibBnsModel.MjtDefaultLen,
                ISmibBnsModel.MjtCardedLen,
                ISmibBnsModel.MjtPreferredLen,
                ISmibBnsModel.MjtExpireTime,
                ISmibBnsModel.MjtMinWin,
                ISmibBnsModel.MjtMaxWin,
                ISmibBnsModel.CardId,
                ISmibBnsModel.PlayerId,
                ISmibBnsModel.MjtRollUp,
                ISmibBnsModel.AckTimeout,
                ISmibBnsModel.MessageTimeout,
                ISmibBnsModel.Timestamp,
                ISmibBnsModel.Signature);

            _responseViewModel.LogRpcResponse(Constants.BonusHit, req, resp, RpcCallContext.Current);
        }
    }
}
