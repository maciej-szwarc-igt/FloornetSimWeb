using IGT.FloorNet.EX.Cage.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class CageFillCreditEventHandler : IBusEventHandler<FillCredit>
    {
        private readonly ResponseViewModel _responseViewModel;
        
        public CageFillCreditEventHandler(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<bool> HandleAsync(FillCredit busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.Log($"Received new cage fill credit event \n {JsonConvert.SerializeObject(busEvent)}");
            return await Task.FromResult(true);
        }
    }
}
