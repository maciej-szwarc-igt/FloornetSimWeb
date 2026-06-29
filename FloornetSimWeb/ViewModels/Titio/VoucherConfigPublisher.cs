using IGT.FloorNet;
using IGT.FloorNet.EX.Tito.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio
{
    /// <summary>
    /// Publishes the TITO <c>voucherConfig</c> event on the FloorNet bus
    /// (RabbitMQ exchange <c>IGT.FloorNet.EX.Tito.evt</c>, routing key <c>voucherConfig</c>).
    ///
    /// The SMIB BE2 queue is bound to BOTH <c>voucherHeartbeat</c> AND <c>voucherConfig</c> on the
    /// Tito.evt exchange (confirmed via RabbitMQ bindings). The heartbeat only asserts the host is
    /// alive; the <c>voucherConfig</c> event carries the actual TITO validation configuration the
    /// SMIB needs before it will request validation IDs from the EGM.
    ///
    /// In Secure-Enhanced (SE) mode the critical flag is <c>enbSecEnhValidation = true</c>
    /// (together with <c>enbTicketRedemption = true</c>). Until the SMIB receives a voucherConfig
    /// that enables SE validation, its getValidationIds poll-loop gate (tito.c) stays closed, so
    /// LP 4C (validation-id configuration) is never sent to the EGM and the EGM keeps raising
    /// Exception 3F (Validation ID Not Configured) — blocking Secure-Enhanced cashout.
    ///
    /// We publish once shortly after startup and then re-publish periodically so the SMIB always
    /// has current config even across SMIB restarts.
    /// </summary>
    public class VoucherConfigPublisher
    {
        // Re-publish periodically so a restarted SMIB picks up the config without needing a
        // FloornetSimWeb restart. The config is small and idempotent.
        private const int PublishIntervalSeconds = 30;

        // Voucher expiration in days (0 = never expires). SAS §15 validation tickets.
        private const long ExpirationDays = 30;

        private readonly IMessageBus _bus;
        private readonly ResponseViewModel _responseViewModel;
        private readonly ILogger<VoucherConfigPublisher> _logger;
        private CancellationTokenSource _cts;

        public VoucherConfigPublisher(
            IMessageBus bus,
            ResponseViewModel responseViewModel,
            ILogger<VoucherConfigPublisher> logger = null)
        {
            _bus = bus;
            _responseViewModel = responseViewModel;
            _logger = logger;
        }

        /// <summary>
        /// Starts the periodic voucherConfig publish loop. Safe to call once at startup.
        /// </summary>
        public void Start()
        {
            if (_cts != null)
                return;

            _cts = new CancellationTokenSource();
            _ = Task.Run(() => PublishLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private async Task PublishLoopAsync(CancellationToken token)
        {
            // Publish immediately at startup so the SMIB's SE validation gate opens as soon as
            // possible, then keep it refreshed.
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await PublishConfigAsync();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to publish voucherConfig");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(PublishIntervalSeconds), token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task<bool> PublishConfigAsync()
        {
            voucherConfig config = new()
            {
                // Secure-Enhanced validation is the production mode the SMIB runs in.
                enbSecEnhValidation = true,
                // Allow the EGM to redeem (ticket-in) and to validate cashout (ticket-out) tickets.
                enbTicketRedemption = true,
                // Standard cashout via the printer.
                enbPrinterAsCashout = true,
                // Restricted / foreign-restricted promo tickets — not used by the basic cashout flow.
                enbRestrictedTickets = false,
                enbForeignRestrictedTicket = false,
                // Handpay receipt / validation features — leave off for the cashout TITO flow.
                enbHandpayReceipts = false,
                enbHandpayValidation = false,
                // Ticket print metadata.
                expiration = ExpirationDays,
                location = "FloornetSimWeb",
                address1 = "Service Simulator",
                address2 = "",
                debitTicketTitle = "CASHOUT TICKET",
                restrictedTicketTitle = "PROMO TICKET"
            };

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(config));

            busMessageEvent busMsg = new()
            {
                dateTime = DateTime.UtcNow,
                deviceType = t_deviceType.FN_TITO_ID,
                machineLoc = "TitoSvc",
                machineNum = Convert.ToInt64(t_deviceType.FN_TITO_ID),
                retryCnt = 0,
                siteId = "1",
                uid = "TitoServiceSimulator",
                body = jsonObj.ToObject<t_busEvent>()
            };

            bool success = await _bus.PublishEventWithConfirmAsync(busMsg);

            if (success)
                _responseViewModel.LogOutBoundEvent(Constants.VoucherConfig, config);

            return success;
        }
    }
}
