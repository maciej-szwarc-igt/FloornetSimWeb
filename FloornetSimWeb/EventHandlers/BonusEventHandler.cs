using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class BonusEventHandler : IBusEventHandler<PoolMeters>
    {
        private readonly ResponseViewModel _responseViewModel;
        public IConfiguration Configuration { get; }
        public BonusEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(PoolMeters busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "bonusEvent", eventCallContext);
             return await Task.FromResult(true);
        }
    }
}
