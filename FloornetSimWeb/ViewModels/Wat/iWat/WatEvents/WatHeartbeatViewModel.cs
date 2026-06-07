using IGT.FloorNet.EX.Bonus.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus.BonusEvent;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat.WatEvents;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using IGT.FloorNet.EX.Wat.evt;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat.WatEvents
{
    public class WatHeartbeatViewModel
    {
        public WatHeartbeatModel watHeartbeatModel { get; set; } = new WatHeartbeatModel();
        private IMessageBus RabbitMQBus { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishWatHearbeatCommand { get; }

        public WatHeartbeatViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {

            RabbitMQBus = Bus;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            PublishWatHearbeatCommand = new RelayCommand(PublishWatHearbeat);
        }

        public void Clear(object obj)
        {
            watHeartbeatModel.Clear();
        }

        public async void PublishWatHearbeat(object obj)
        {

            WatHeartbeat watHeartbeat = new()
            {
                hostAvailable = watHeartbeatModel.hostAvailable,
                rateSec = 60
            };

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(watHeartbeat));

            busMessageEvent busMsg = new()
            {
                dateTime = DateTime.UtcNow,
                deviceType = t_deviceType.FN_WAT_ID,
                machineLoc = "ServiceSimulator",
                machineNum = Convert.ToInt64(t_deviceType.FN_WAT_ID),
                retryCnt = 0,
                siteId = "1",
                uid = "WatServiceSimulator",
                body = jsonObj.ToObject<t_busEvent>()
            };

            bool isOnline=false;

            while (!isOnline )
            {
                bool eventSuccess = await RabbitMQBus.PublishEventWithConfirmAsync(busMsg);

                if (eventSuccess)
                    isOnline = false;
                else
                    break;

                _responseViewModel.LogOutBoundEvent(Constants.WatHeartbeat, watHeartbeat);
                await Task.Delay(TimeSpan.FromSeconds((double) watHeartbeat.rateSec));
            }

        }

    }
}
