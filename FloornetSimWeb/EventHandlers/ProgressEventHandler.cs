using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.EX.evt;
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
    public class ProgressEventHandler : IBusEventHandler<Progress>
    {
        private readonly ResponseViewModel _responseViewModel;
        public IConfiguration Configuration { get; }

        public ProgressEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(Progress busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "progress", eventCallContext);
            _responseViewModel.LogProgress(busEvent, string.Empty, eventCallContext);

            return await Task.FromResult(true);
        }
    }
}