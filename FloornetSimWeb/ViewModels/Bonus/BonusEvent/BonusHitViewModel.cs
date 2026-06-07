using System;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent;
using IGT.FloorNet.EX.Bonus.evt;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.MessageBus;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus.Common;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus.BonusEvent
{
    public class BonusHitViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        public BonusHitModel BonusHitModel { get; } = new BonusHitModel();
        private IMessageBus RabbitMQBus { get; set; }

        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishBonusHitCommand { get; }

        public BonusHitViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = Bus;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            PublishBonusHitCommand = new RelayCommand(
                PublishBonusHit,                    
                param => !string.IsNullOrEmpty(IKeysViewModel.keysModelBonus.PublicKey) &&
                            !string.IsNullOrEmpty(IKeysViewModel.keysModelBonus.PrivateKey)
            );
        }

        public void Clear(object obj)
        {
            BonusHitModel.Clear();
        }

        private void PublishBonusHit(object obj)
        {
            if (BonusHitModel.SendInvalidSignature)
            {
                BonusHitModel.Signature = IKeysViewModel.keysModelBonus.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelBonus.ECDsa,
                    BonusHitModel.LevelId,
                    BonusHitModel.HitSeqNum,
                    true,
                    BonusHitModel.ControlStringId,
                    IKeysViewModel.keysModelBonus.CurrentKeyNum,
                    BonusHitModel.Type,
                    BonusHitModel.MachineLevel,
                    BonusHitModel.DefaultAmt,
                    BonusHitModel.CardedAmt,
                    BonusHitModel.PreferredAmt,
                    BonusHitModel.PayTo,
                    BonusHitModel.PayMethod,
                    BonusHitModel.Carded,
                    BonusHitModel.AckRequired,
                    BonusHitModel.LockUntilAcked,
                    BonusHitModel.TestEligible,
                    BonusHitModel.PayAbandoned,
                    BonusHitModel.MjtDefaultLen,
                    BonusHitModel.MjtCardedLen,
                    BonusHitModel.MjtPreferredLen,
                    BonusHitModel.MjtExpireTime,
                    BonusHitModel.MjtMinWin,
                    BonusHitModel.MjtMaxWin,
                    BonusHitModel.CardId,
                    BonusHitModel.PlayerId,
                    BonusHitModel.MjtRollUp,
                    BonusHitModel.MjtRollUp, // Adding mjtRollup twice will cause the incorrect signature to be generated.
                    BonusHitModel.AckTimeout,
                    BonusHitModel.MessageTimeout,
                    BonusHitModel.Timestamp
                );
            }
            else
            {
                //Calculate Signature
                BonusHitModel.Signature = IKeysViewModel.keysModelBonus.FloornetECDsaProvider.ComputeSignature(
                    IKeysViewModel.keysModelBonus.ECDsa,
                    BonusHitModel.LevelId,
                    BonusHitModel.HitSeqNum,
                    true,
                    BonusHitModel.ControlStringId,
                    IKeysViewModel.keysModelBonus.CurrentKeyNum,
                    BonusHitModel.Type,
                    BonusHitModel.MachineLevel,
                    BonusHitModel.DefaultAmt,
                    BonusHitModel.CardedAmt,
                    BonusHitModel.PreferredAmt,
                    BonusHitModel.PayTo,
                    BonusHitModel.PayMethod,
                    BonusHitModel.Carded,
                    BonusHitModel.AckRequired,
                    BonusHitModel.LockUntilAcked,
                    BonusHitModel.TestEligible,
                    BonusHitModel.PayAbandoned,
                    BonusHitModel.MjtDefaultLen,
                    BonusHitModel.MjtCardedLen,
                    BonusHitModel.MjtPreferredLen,
                    BonusHitModel.MjtExpireTime,
                    BonusHitModel.MjtMinWin,
                    BonusHitModel.MjtMaxWin,
                    BonusHitModel.CardId,
                    BonusHitModel.PlayerId,
                    BonusHitModel.MjtRollUp,
                    BonusHitModel.AckTimeout,
                    BonusHitModel.MessageTimeout,
                    BonusHitModel.Timestamp
                );
            }

            bonusHit bonusHit = new()
            {
                levelId = BonusHitModel.LevelId,
                hitSeqNum = BonusHitModel.HitSeqNum,
                celebration = true,
                controlStringId = BonusHitModel.ControlStringId,
                keyNumber = IKeysViewModel.keysModelBonus.CurrentKeyNum,
                type = BonusHitModel.Type,
                machineLevel = BonusHitModel.MachineLevel,
                defaultAmt = BonusHitModel.DefaultAmt,
                cardedAmt = BonusHitModel.CardedAmt,
                preferredAmt = BonusHitModel.PreferredAmt,
                payTo = BonusHitModel.PayTo,
                payMethod = BonusHitModel.PayMethod,
                carded = BonusHitModel.Carded,
                ackRequired = BonusHitModel.AckRequired,
                lockUntilAcked = BonusHitModel.LockUntilAcked,
                testEligible = BonusHitModel.TestEligible,
                payAbandoned = BonusHitModel.PayAbandoned,
                mjtDefaultLen = BonusHitModel.MjtDefaultLen,
                mjtCardedLen = BonusHitModel.MjtCardedLen,
                mjtPreferredLen = BonusHitModel.MjtPreferredLen,
                mjtExpireTime = BonusHitModel.MjtExpireTime,
                mjtMinWin = BonusHitModel.MjtMinWin,
                mjtMaxWin = BonusHitModel.MjtMaxWin,
                cardId = BonusHitModel.CardId,
                playerId = BonusHitModel.PlayerId,
                mjtRollUp = BonusHitModel.MjtRollUp,
                ackTimeout = BonusHitModel.AckTimeout,
                messageTimeout = BonusHitModel.MessageTimeout,
                timestamp = BonusHitModel.Timestamp,
                signature = BonusHitModel.Signature
            };

            

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(bonusHit));

            busMessageEvent busMsg = new()
            {
                dateTime = DateTime.UtcNow,
                deviceType = t_deviceType.FN_BONUS_ID,
                machineLoc = "ServiceSimulator",
                machineNum = Convert.ToInt64(t_deviceType.FN_BONUS_ID),
                retryCnt = 0,
                siteId = "1",
                uid = "BonusServiceSimulator",
                body = jsonObj.ToObject<t_busEvent>()
            };

            _responseViewModel.LogOutBoundEvent(Constants.BonusHit, bonusHit);
            RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
        }

    }
}
