using IGT.FloorNet.EX.Auth.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class AuthEventHandler : IBusEventHandler<AuthEntry>
    {
        private readonly ResponseViewModel _responseViewModel;
        public IConfiguration Configuration { get; }
        public AuthEventHandler(ResponseViewModel responseViewModel/*, iAuth AuthRpcProxy*/)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(AuthEntry busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "authEvent", eventCallContext);
            return await Task.FromResult(true);
        }
    }
}
