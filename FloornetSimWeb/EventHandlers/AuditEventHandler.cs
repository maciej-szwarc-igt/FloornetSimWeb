using IGT.FloorNet.EX.evt;
using IGT.FloorNet.EX.Tito.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Persistence;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.EventProcessors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class AuditEventHandler : IBusEventHandler<auditEvent>
    {
        private readonly ResponseViewModel _responseViewModel;
        private readonly SimDbStore _db;
        private Dictionary<t_eventCode, AuditEventProcessor> _auditEventProcessors = new Dictionary<t_eventCode, AuditEventProcessor>();

        public AuditEventHandler(ResponseViewModel responseViewModel, SmibRegistrationTracker tracker, SimDbStore db)
        {
            _responseViewModel = responseViewModel;
            _db = db;
            InitProcessors(tracker);
        }

        public Task<bool> HandleAsync(auditEvent busEvent, EventCallContext eventCallContext)
        {
            if (_auditEventProcessors.TryGetValue(busEvent.code, out var processor))
            {
                processor.Process(busEvent, eventCallContext);
            }

            _responseViewModel.LogAuditEvent(busEvent, eventCallContext);
            PersistAuditEvent(busEvent, eventCallContext);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Persists the audit event to SQLite, including any meters carried on the event and, when the
        /// event is a TITO voucher (issue/redeem/commit), a derived cashout row. Best-effort: a DB
        /// failure never affects event handling.
        /// </summary>
        private void PersistAuditEvent(auditEvent busEvent, EventCallContext eventCallContext)
        {
            if (!_db.Enabled)
            {
                return;
            }

            var category = AuditEventCategoryResolver.Resolve(busEvent.code);

            _db.InsertEvent(new EventRecord
            {
                ReceivedUtc = DateTime.UtcNow,
                EventName = nameof(auditEvent),
                EventCode = busEvent.code.ToString(),
                Category = category,
                Uid = eventCallContext?.Uid,
                MachineNum = eventCallContext?.MachineNum,
                SiteId = eventCallContext?.SiteId,
                DeviceType = eventCallContext?.DeviceType.ToString(),
                PayloadJson = JsonConvert.SerializeObject(busEvent)
            });

            // auditEvents may carry a meter snapshot relevant to the event (e.g. on door open / card out).
            if (busEvent.meters is not null)
            {
                MeterPersistence.Persist(_db, '\0', 0, busEvent.meters, eventCallContext);
            }

            // TITO voucher activity arrives as auditEvent data; capture it as a cashout too.
            switch (busEvent.data)
            {
                case issueVoucher issued:
                    _db.InsertCashout(new CashoutRecord
                    {
                        ReceivedUtc = DateTime.UtcNow,
                        Operation = nameof(issueVoucher),
                        VoucherId = issued.voucherId,
                        AmountCents = issued.voucherAmt,
                        TransactionId = issued.transactionId,
                        CardId = issued.cardId,
                        Uid = eventCallContext?.Uid,
                        MachineNum = eventCallContext?.MachineNum,
                        SiteId = eventCallContext?.SiteId,
                        RequestJson = JsonConvert.SerializeObject(issued)
                    });
                    break;

                case redeemVoucher redeemed:
                    _db.InsertCashout(new CashoutRecord
                    {
                        ReceivedUtc = DateTime.UtcNow,
                        Operation = nameof(redeemVoucher),
                        VoucherId = redeemed.voucherId,
                        AmountCents = busEvent.amount ?? 0,
                        TransactionId = redeemed.transactionId,
                        CardId = redeemed.cardId,
                        Uid = eventCallContext?.Uid,
                        MachineNum = eventCallContext?.MachineNum,
                        SiteId = eventCallContext?.SiteId,
                        RequestJson = JsonConvert.SerializeObject(redeemed)
                    });
                    break;

                case commitVoucher committed:
                    _db.InsertCashout(new CashoutRecord
                    {
                        ReceivedUtc = DateTime.UtcNow,
                        Operation = nameof(commitVoucher),
                        VoucherId = committed.voucherId,
                        AmountCents = committed.transferAmount,
                        TransactionId = committed.transactionId,
                        Uid = eventCallContext?.Uid,
                        MachineNum = eventCallContext?.MachineNum,
                        SiteId = eventCallContext?.SiteId,
                        RequestJson = JsonConvert.SerializeObject(committed)
                    });
                    break;
            }
        }

        private void InitProcessors(SmibRegistrationTracker tracker)
        {
            _auditEventProcessors.Add(IGT.FloorNet.t_eventCode.regSmibOnline, new RegSmibOnlineHandler(_responseViewModel, tracker));
            _auditEventProcessors.Add(IGT.FloorNet.t_eventCode.regSmibOffline, new RegSmibOfflineHandler(_responseViewModel, tracker));
        }

    }
}
