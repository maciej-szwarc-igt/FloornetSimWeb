using IGT.FloorNet.EX.Meters.evt;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Persistence;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class MeterEventHandler : IBusEventHandler<meterEvent>
    {
        private readonly ResponseViewModel _responseViewModel;
        private readonly SimDbStore _db;
        public MeterEventHandler(ResponseViewModel responseViewModel, SimDbStore db)
        {
            _responseViewModel = responseViewModel;
            _db = db;
        }

        public async Task<bool> HandleAsync(meterEvent busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "meterEvent", eventCallContext);
            MeterPersistence.Persist(_db, busEvent.meterType, busEvent.meterTime, busEvent.meters, eventCallContext);
            return await Task.FromResult(true);
        }
    }
}
