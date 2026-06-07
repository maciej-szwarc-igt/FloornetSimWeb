using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class BonusHostStatusEventHandler : IBusEventHandler<BonusHostStatus>
    {
        private readonly ResponseViewModel _responseViewModel;
        public IConfiguration Configuration { get; }
        public BonusHostStatusEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(BonusHostStatus busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "bonusHostStatus", eventCallContext);
            return await Task.FromResult(true);
        }
    }
}
