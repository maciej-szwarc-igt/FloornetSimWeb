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
    /// Periodically publishes the TITO <c>voucherHeartbeat</c> event on the FloorNet bus
    /// (RabbitMQ topic <c>IGT.SMIB.TITO.voucherHeartbeat</c>) with <c>hostAvailable = true</c>.
    ///
    /// The SMIB BE2 firmware (tito.c) requires this heartbeat: its getValidationIds poll-loop
    /// gate keeps <c>TITO_HeartbeatTimeout</c> in the future ONLY while the host keeps sending
    /// this event. The firmware defines <c>TITO_HEARTBEAT_TIMEOUT = 30</c> seconds and the
    /// FloorNet TITO spec states the service sends the heartbeat every 15 seconds, disabling
    /// voucher functions if none is received within 30 seconds. Without this heartbeat the SMIB
    /// never calls getValidationIds, so LP 4C (validation-id configuration) is never sent to the
    /// EGM and Secure-Enhanced cashout (Ticket-Out) can never complete.
    ///
    /// We publish every 10 seconds (well under the 30 s timeout) and advertise rateSec = 15.
    /// </summary>
    public class VoucherHeartbeatPublisher
    {
        private const int PublishIntervalSeconds = 10;
        private const long AdvertisedRateSec = 15;

        private readonly IMessageBus _bus;
        private readonly ResponseViewModel _responseViewModel;
        private readonly ILogger<VoucherHeartbeatPublisher> _logger;
        private CancellationTokenSource _cts;

        public VoucherHeartbeatPublisher(
            IMessageBus bus,
            ResponseViewModel responseViewModel,
            ILogger<VoucherHeartbeatPublisher> logger = null)
        {
            _bus = bus;
            _responseViewModel = responseViewModel;
            _logger = logger;
        }

        /// <summary>
        /// Starts the periodic heartbeat publish loop. Safe to call once at startup.
        /// </summary>
        public void Start()
        {
            if (_cts != null)
                return;

            _cts = new CancellationTokenSource();
            _ = Task.Run(() => PublishLoopAsync(_cts.Token));
        }

        /// <summary>
        /// Stops the heartbeat loop and publishes a final hostAvailable = false so the SMIB
        /// can promptly disable voucher functions instead of waiting for the timeout.
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
            try
            {
                _ = PublishHeartbeatAsync(hostAvailable: false);
            }
            catch
            {
                // best-effort shutdown notification
            }
        }

        private async Task PublishLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await PublishHeartbeatAsync(hostAvailable: true);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to publish voucherHeartbeat");
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

        private async Task<bool> PublishHeartbeatAsync(bool hostAvailable)
        {
            voucherHeartbeat heartbeat = new()
            {
                hostAvailable = hostAvailable,
                rateSec = AdvertisedRateSec
            };

            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(heartbeat));

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
                _responseViewModel.LogOutBoundEvent(Constants.VoucherHeartbeat, heartbeat);

            return success;
        }
    }
}
