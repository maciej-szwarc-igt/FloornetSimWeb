using IGT.FloorNet.EX.Wat.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iWat.WatEvents;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat.WatEvents
{
    public class WatConfigViewModel
    {
        public WatConfigModel WatConfigModel { get; set; } = new WatConfigModel();
        private IMessageBus RabbitMQBus { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand PublishWatConfigCommand { get; }

        public WatConfigViewModel(IMessageBus messageBus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = messageBus;
            _responseViewModel = responseViewModel;

            ClearCommand = new RelayCommand(Clear);
            PublishWatConfigCommand = new RelayCommand(PublishWatConfig);
        }

        public void Clear(object obj)
        {
            WatConfigModel.Clear();
        }

        public async void PublishWatConfig(object obj)
        {            

            bool isOnline = false;

            while (!isOnline)
            {

                WatConfig watConfig = getValuesFromView();

                var jsonObj = JObject.Parse(JsonConvert.SerializeObject(watConfig));

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

                bool eventSuccess = await RabbitMQBus.PublishEventWithConfirmAsync(busMsg);

                if (eventSuccess)
                    isOnline = false;
                else
                    break;

                _responseViewModel.LogOutBoundEvent(Constants.WatConfig, watConfig);
                await Task.Delay(TimeSpan.FromSeconds(30));
            }


        }

        private WatConfig getValuesFromView()
        {
            return new()
            {
                enable = WatConfigModel.Enable,
                cashoutInterceptEnable = WatConfigModel.CashoutInterceptEnable,
                autoTransferOnCardIn = WatConfigModel.AutoTransferOnCardIn,
                autoTransferOnCardOut = WatConfigModel.AutoTransferOnCardOut,
                noCardBvDisable = WatConfigModel.NoCardBvDisable,
                autoPlayDisable = WatConfigModel.AutoPlayDisable,
                noPinAtEGM = WatConfigModel.NoPinAtEGM
            };

        }


    }
}
