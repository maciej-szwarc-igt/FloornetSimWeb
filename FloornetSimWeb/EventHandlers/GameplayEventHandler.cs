using IGT.FloorNet.EX.Gameplay.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class GameplayStartedEventHandler : IBusEventHandler<gameStarted>
    {
        private readonly ResponseViewModel _responseViewModel;

        public GameplayStartedEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<bool> HandleAsync(gameStarted busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "gameStarted", eventCallContext);
            return Task.FromResult(true);
        }
    }

    public class GameplayEndedEventHandler : IBusEventHandler<gameEnded>
    {
        private readonly ResponseViewModel _responseViewModel;

        public GameplayEndedEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<bool> HandleAsync(gameEnded busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "gameEnded", eventCallContext);
            return Task.FromResult(true);
        }
    }

    public class GameplaySelectedEventHandler : IBusEventHandler<gameSelected>
    {
        private readonly ResponseViewModel _responseViewModel;

        public GameplaySelectedEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<bool> HandleAsync(gameSelected busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "gameSelected", eventCallContext);
            return Task.FromResult(true);
        }
    }
}
