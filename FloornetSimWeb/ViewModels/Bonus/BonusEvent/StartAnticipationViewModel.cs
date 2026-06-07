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
    public class StartAnticipationViewModel
    {
        public StartAnticipationModel StartAnticipationModel { get; } = new StartAnticipationModel();
        private IMessageBus RabbitMQBus { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishStartAnticipationCommand { get; }

        public StartAnticipationViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = Bus;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            PublishStartAnticipationCommand = new RelayCommand(PublishStartAnticipation);
        }

        public void Clear(object obj)
        {
            StartAnticipationModel.Clear();
        }

        private void PublishStartAnticipation(object obj)
        {
            startAnticipation startAnticipationEvt = new()
            {
                levelId = StartAnticipationModel.LevelId,
                hitSeqNum = StartAnticipationModel.HitSeqNum,
                controlStringId = StartAnticipationModel.ControlStringId,
                timeout = StartAnticipationModel.Timeout
            };

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(startAnticipationEvt));

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

            _responseViewModel.LogOutBoundEvent(Constants.StartAnticipation, startAnticipationEvt);
            RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
        }
    }
}
