using System;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent;
using IGT.FloorNet.EX.Bonus.evt;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.MessageBus;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IGT.FloorNet.Tools.ServiceSimulator.Models;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus.BonusEvent
{
    public class LevelResetViewModel
    {
        public LevelResetModel LevelResetModel { get; } = new LevelResetModel();
        private IMessageBus RabbitMQBus { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishLevelResetCommand { get; }

        public LevelResetViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = Bus;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            PublishLevelResetCommand = new RelayCommand(PublishLevelReset);
        }

        public void Clear(object obj)
        {
            LevelResetModel.Clear();
        }

        public void PublishLevelReset(object obj)
        {
            levelReset levelReset = new()
            {
                levelId = LevelResetModel.LevelId,
                hitSeqNum = LevelResetModel.HitSeqNum,
                clcData = null
            };

            if (LevelResetModel.IsClcLevel)
            {
                levelReset.clcData = new t_clcData()
                {
                    controlStringId = LevelResetModel.ControlStringId,
                    newPoolAmount = LevelResetModel.NewPoolAmount,
                    dontPlayUID = LevelResetModel.DontPlayUID,
                    timeToDisplay = LevelResetModel.TimeToDisplay
                };
            }

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(levelReset));

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

            _responseViewModel.LogOutBoundEvent(Constants.LevelReset, levelReset);
            RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
        }
    }
}
