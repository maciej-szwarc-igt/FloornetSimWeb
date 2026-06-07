using IGT.FloorNet.EX.evt;
using IGT.FloorNet.EX.Tito.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.EventProcessors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class AuditEventHandler : IBusEventHandler<auditEvent>
    {
        private readonly ResponseViewModel _responseViewModel;
        private Dictionary<t_eventCode, AuditEventProcessor> _auditEventProcessors = new Dictionary<t_eventCode, AuditEventProcessor>();

        public AuditEventHandler(ResponseViewModel responseViewModel, SmibRegistrationTracker tracker)
        {
            _responseViewModel = responseViewModel;
            InitProcessors(tracker);
        }

        public Task<bool> HandleAsync(auditEvent busEvent, EventCallContext eventCallContext)
        {
            if (_auditEventProcessors.TryGetValue(busEvent.code, out var processor))
            {
                processor.Process(busEvent, eventCallContext);
            }

            _responseViewModel.LogAuditEvent(busEvent, eventCallContext);
            return Task.FromResult(true);
        }
        
        private void InitProcessors(SmibRegistrationTracker tracker)
        {
            _auditEventProcessors.Add(IGT.FloorNet.t_eventCode.regSmibOnline, new RegSmibOnlineHandler(_responseViewModel, tracker));
            _auditEventProcessors.Add(IGT.FloorNet.t_eventCode.regSmibOffline, new RegSmibOfflineHandler(_responseViewModel, tracker));
        }

    }
}
