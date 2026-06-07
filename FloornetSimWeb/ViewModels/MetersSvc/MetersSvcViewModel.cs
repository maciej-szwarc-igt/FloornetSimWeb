using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.MetersSvc
{
    public class MetersSvcViewModel
    {
        public Models.MetersSvc.MetersSvc MeterSvcModel { get; set; }
        public IBonusMeterMockService BonusMeterMockSvcModel { get; set; }

        /// <summary>
        /// RabbitMQ Queue Bus
        /// </summary>
        private IMessageBus RabbitMQBus { get; set; }

        public RelayCommand PublishMetersEventCommand
        {
            get
            {
                return new RelayCommand(
                    PublishEvent,
                    param => !string.IsNullOrEmpty(MeterSvcModel.EventBody)
                );
            }
        }

        public RelayCommand InitializeConfigCommand
        {
            get
            {
                return new RelayCommand(
                    InitializeConfiguration,
                    param => !MeterSvcModel.IsInitialized
                );
            }
        }

        public RelayCommand ClearConfigCommand
        {
            get
            {
                return new RelayCommand(
                    ClearConfiguration,
                    param => true
                );
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Bus"></param>
        public MetersSvcViewModel(IMessageBus Bus, IBonusMeterMockService bonusMeterMockService)
        {
            BonusMeterMockSvcModel = bonusMeterMockService;
            MeterSvcModel = new Models.MetersSvc.MetersSvc(BonusMeterMockSvcModel);
            RabbitMQBus = Bus;
        }

        /// <summary>
        /// Publishes an Event to the RabbitMQ Bus
        /// </summary>
        /// <param name="obj"></param>
        public void PublishEvent(object obj)
        {
            if (MeterSvcModel.EventBody != null)
            {
                var jsonObj = JObject.Parse(MeterSvcModel.EventBody);
                busMessageEvent busMsg = new busMessageEvent()
                {
                    dateTime = DateTime.UtcNow,
                    deviceType = t_deviceType.FN_BONUS_ID,
                    machineLoc = "BonusServiceSim",
                    machineNum = Convert.ToInt64(t_deviceType.FN_BONUS_ID),
                    retryCnt = 0,
                    siteId = "1",
                    uid = "BonusServiceSim1",
                    body = jsonObj.ToObject<t_busEvent>()
                };
                RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
            }
            else
            {
                // MessageBox removed (no WPF)
            }
        }

        public void InitializeConfiguration(object obj)
        {
            BonusMeterMockSvcModel?.Initialize();
            var bonusHostsMap = BonusMeterMockSvcModel.BonusHostsMap;

            MeterSvcModel.BonusIds = new ObservableCollection<string>();

            foreach (var item in bonusHostsMap)
            {
                MeterSvcModel.BonusIds.Add(item.Key.ToString());
            }
            MeterSvcModel.IsInitialized = true;
        }

        public void ClearConfiguration(object obj)
        {
            BonusMeterMockSvcModel?.Clear();
            MeterSvcModel.BNS_Hit_Events = null;
            MeterSvcModel.BNS_Pool_Events = null;
            MeterSvcModel.IsInitialized = false;
            BonusMeterMockSvcModel.IsInitialized = false;
            MeterSvcModel.IsPoolIdSelected = false;
            MeterSvcModel.PoolId = null;
            MeterSvcModel.BonusId = null;
            MeterSvcModel.EventBody = null;
        }
    }
}