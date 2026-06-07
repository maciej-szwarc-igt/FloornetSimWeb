using IGT.FloorNet.EX.Cage.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.EventHandlers;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Event;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Event
{
    /// <summary>
    /// Event ViewModel
    /// </summary>
    public class EventViewModel
    {
        /// <summary>
        /// RabbitMQ Queue Bus
        /// </summary>
        private IMessageBus RabbitMQBus { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Bus"></param>
        public EventViewModel(IMessageBus Bus)
        {
            EventMdl = new EventModel();
            RabbitMQBus = Bus;
        }

        /// <summary>
        /// Event Model
        /// </summary>
        public EventModel EventMdl { get; set; }

        /// <summary>
        /// Binding to the Publish Button
        /// </summary>
        public RelayCommand PublishEventCommand
        {
            get
            {
                return new RelayCommand(
                    PublishEvent,
                    param => true
                );
            }
        }

        /// <summary>
        /// Binding to the Subscribe to an Event
        /// </summary>
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

        /// <summary>
        /// Publishes an Event to the RabbitMQ Bus
        /// </summary>
        /// <param name="obj"></param>
        public void PublishEvent(object obj)
        {
            t_inventoryBreakdown inventoryBreakdown = new t_inventoryBreakdown()
            {
                denomId = 3,
                quantity = 5,
                chipSetId = "1234"
            };
            FillCredit evnt = new FillCredit()
            {
                siteId = "IGT",
                locationId = 1,
                reason = t_fillCreditReason.Filled,
                locationInfo = new t_locationData()
                {
                    locationId = "Test1",
                    locationType = t_locationType.Bingo,
                    locationName = "Test2",
                    gameDate = DateTime.UtcNow,
                    shift = "Test3",
                    pitNumber = 4,
                    gameType = "bingo",
                    defaultCageId = 5,
                    markerPrinter = "Test4",
                    receiptPrinter = "Test5",
                    locationStatus = t_locationStatus.Open
                },
                fillCreditInfo = new t_fillCreditInfo()
                {
                    transactionInfo = new t_transactionInfo()
                    {
                        postedBy = "Marco",
                        approvedBy = "Rodolfo",
                        transactionTime = DateTime.UtcNow,
                        locationName = "IGT",
                        comment = "Test"
                    },
                    inventory = new t_inventoryBreakdown[1],
                    priority = t_fillCreditPriority.Normal,
                    fillCreditType = t_fillCreditType.Fill,
                    documentId = 78,
                    status = t_fillCreditStatus.Acknowledged
                }
            };
            evnt.fillCreditInfo.inventory[0] = inventoryBreakdown;
            busMessageEvent busMsg = new busMessageEvent() { body = evnt };
            RabbitMQBus.PublishEvent(busMsg, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Publishes an Event to the RabbitMQ Bus
        /// </summary>
        /// <param name="obj"></param>
        public void SubscribeToEvent(object obj)
        {
            try
            {
                //Subscribed
                RabbitMQBus.SubscribeEvent<FillCredit, CageFillCreditEventHandler>();
            }
            catch (Exception ex)
            {
                RabbitMQBus.UnsubscribeEvent<FillCredit, CageFillCreditEventHandler>();
                RabbitMQBus.SubscribeEvent<FillCredit, CageFillCreditEventHandler>();
            }
        }
    }
}