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
    public class StartCelebrationViewModel
    {
        public StartCelebrationModel StartCelebrationModel { get; } = new StartCelebrationModel();
        private IMessageBus RabbitMQBus { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishStartCelebrationCommand { get; }

        public StartCelebrationViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = Bus;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            PublishStartCelebrationCommand = new RelayCommand(PublishStartCelebration);
        }

        public void Clear(object obj)
        {
            StartCelebrationModel.Clear();
        }

        private void PublishStartCelebration(object obj)
        {
            startCelebration startCelebrationEvt = new () 
            {
                levelId = StartCelebrationModel.LevelId,
                hitSeqNum = StartCelebrationModel.HitSeqNum,
                controlStringId = StartCelebrationModel.ControlStringId,
                testEligible = StartCelebrationModel.TestEligible,
                carded = StartCelebrationModel.Carded,
                lockEgm = StartCelebrationModel.LockEgm
            };

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(startCelebrationEvt));

            busMessageEvent busMsg = new ()
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

            _responseViewModel.LogOutBoundEvent(Constants.StartCelebration, startCelebrationEvt);
            RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
        }

    }
}
