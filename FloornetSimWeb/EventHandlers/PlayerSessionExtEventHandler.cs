using IGT.FloorNet.EX.Player.evt;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class PlayerSessionExtEventHandler : IBusEventHandler<PlayerSessionExt>
    {
        private readonly ResponseViewModel _responseViewModel;

        public PlayerSessionExtEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }
        public async Task<bool> HandleAsync(PlayerSessionExt busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "playerSessionExt", eventCallContext);
            return await Task.FromResult(true);
        }
    }
}
