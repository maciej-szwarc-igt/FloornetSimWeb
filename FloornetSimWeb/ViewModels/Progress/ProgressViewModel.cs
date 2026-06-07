using IGT.FloorNet.EX.Cage.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.EventHandlers;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Event;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using IGT.FloorNet.EX.evt;
using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.ProgressEvent
{
    public class ProgressViewModel
    {
        public ResponseViewModel ResponseView { get; set; }

        /// <summary>
        /// RabbitMQ Queue Bus
        /// </summary>
        private IMessageBus RabbitMQBus { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Bus"></param>
        public ProgressViewModel(IMessageBus Bus, ResponseViewModel responseViewModel)
        {
            RabbitMQBus = Bus;
            ResponseView = responseViewModel;
        }

        public RelayCommand SubscribeEventCommand
        {
            get
            {
                return new RelayCommand(
                    SubscribeToEvent,
                    param => true
                );
            }
        }

        public RelayCommand UnSubscribeEventCommand
        {
            get
            {
                return new RelayCommand(
                    UnSubscribeToEvent,
                    param => true
                );
            }
        }

        public RelayCommand Clear
        {
            get
            {
                return new RelayCommand(
                    ClearTable,
                    param => true
                );
            }
        }

        private void SubscribeToEvent(object obj)
        {
            try
            {
                //Subscribed
                RabbitMQBus.SubscribeEvent<Progress, ProgressEventHandler>();
            }
            catch (Exception ex)
            {
                RabbitMQBus.UnsubscribeEvent<Progress, ProgressEventHandler>();
                RabbitMQBus.SubscribeEvent<Progress, ProgressEventHandler>();
            }
        }

        private void UnSubscribeToEvent(object obj)
        {
            RabbitMQBus.UnsubscribeEvent<Progress, ProgressEventHandler>();
        }

        //Cleaing Data from ProgressTable
        private void ClearTable(object obj)
        {
            ResponseView.ProgressList.Clear();
        }
    }
}