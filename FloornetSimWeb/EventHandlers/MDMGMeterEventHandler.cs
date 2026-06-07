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
    public class MDMGMeterEventHandler : IBusEventHandler<mdmgMeterEvent>
    {
        private readonly ResponseViewModel _responseViewModel;
        public MDMGMeterEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(mdmgMeterEvent busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "mdmgMeterEvent", eventCallContext);
            return await Task.FromResult(true);
        }
    }
}
