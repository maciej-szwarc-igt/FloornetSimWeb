using System;
using System.Collections.Generic;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent;
using IGT.FloorNet.EX.Bonus.evt;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.MessageBus;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IGT.FloorNet.Tools.ServiceSimulator.Models;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus.BonusEvent
{
    public class LevelUpdateViewModel
    {
        public LevelUpdateModel LevelUpdateModel { get; } = new LevelUpdateModel();
        private IMessageBus RabbitMQBus { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishLevelUpdateCommand { get; }


        public LevelUpdateViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = Bus;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            PublishLevelUpdateCommand = new RelayCommand(PublishLevelUpdate);
        }

        public void Clear(object obj)
        {
            LevelUpdateModel.Clear();
        }

        public void PublishLevelUpdate(object obj)
        {
            t_level level = new()
            {
                levelId = LevelUpdateModel.LevelId,
                hitSeqNum = LevelUpdateModel.HitSeqNum,
                machineLevel = LevelUpdateModel.MachineLevel,
                amount = LevelUpdateModel.Amount,
                bbpgData = null,
                isProg = LevelUpdateModel.IsProg
            };

            if (LevelUpdateModel.IsBbpgLevel)
            {
                level.bbpgData = new t_bbpgData() {BbPGIdx = LevelUpdateModel.BbPGIdx, BbPGName = LevelUpdateModel.BbPGName};
            }

            List<t_level> levelList = new();
            levelList.Add(level);

            levelUpdate levelUpdate = new()
            {
                levels = levelList,
            };

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(levelUpdate));

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

            _responseViewModel.LogOutBoundEvent(Constants.LevelUpdate, levelUpdate);
            RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
        }
    }
}
