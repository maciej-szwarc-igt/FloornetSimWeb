using IGT.FloorNet.EX.Meters.evt;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class SapMeterEventHandler : IBusEventHandler<sapMeterEvent>
    {
        private readonly ResponseViewModel _responseViewModel;
        public SapMeterEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(sapMeterEvent busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "sapMeterEvent", eventCallContext);
            return await Task.FromResult(true);
        }
    }
}
