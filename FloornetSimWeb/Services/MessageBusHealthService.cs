using System;
using System.Threading;
using System.Threading.Tasks;
using IGT.FloorNet.MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services
{
    /// <summary>
    /// Watches the RabbitMQ/MessageBus consumer health and recovers it automatically.
    ///
    /// Background: the broker enforces a per-consumer delivery-acknowledgement timeout
    /// (RabbitMQ <c>consumer_timeout</c>, default 30&#160;min). If a delivered message is not
    /// acked within that window — e.g. the dev box sleeps, or a handler stalls — RabbitMQ raises
    /// <c>PRECONDITION_FAILED - delivery acknowledgement on channel N timed out</c>, closes the
    /// channel and <b>cancels all consumers</b>. The <c>IGT.FloorNet.MessageBus</c> library logs
    /// the cancellation but does not always re-establish the consumers, leaving the service
    /// silently unable to answer RPCs (observed: <c>iDiscovery.getAllServiceInterfaces</c> stops
    /// responding, which blocks SMIB TITO validation-ID seeding / cashout).
    ///
    /// This service polls <see cref="IMessageBus.IsConnected"/> and
    /// <see cref="IMessageBus.IsConsumerHealthy"/> and, when the consumer has died while the
    /// connection is up, calls <see cref="IMessageBus.StartConsumer"/> to re-register the
    /// consumers (RPC providers and event subscriptions remain registered on the bus, so a
    /// restarted consumer resumes serving them). It is intentionally conservative: it only acts
    /// on an unhealthy-consumer transition and logs every recovery attempt.
    /// </summary>
    public sealed class MessageBusHealthService : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);

        private readonly IMessageBus _bus;
        private readonly ILogger<MessageBusHealthService> _logger;

        // Track last-known health so we only log/recover on transitions, not every tick.
        private bool _lastConsumerHealthy = true;
        private bool _lastConnected = true;

        public MessageBusHealthService(IMessageBus bus, ILogger<MessageBusHealthService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "MessageBusHealthService started (poll={Poll}s). Recovers RabbitMQ consumers after a broker-side ack timeout.",
                PollInterval.TotalSeconds);

            // Give the bus time to make its initial connection before the first probe.
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Probe();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MessageBusHealthService probe failed.");
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("MessageBusHealthService stopping.");
        }

        private void Probe()
        {
            bool connected = _bus.IsConnected;
            bool consumerHealthy = _bus.IsConsumerHealthy;

            // Log connection-state transitions for observability.
            if (connected != _lastConnected)
            {
                if (connected)
                    _logger.LogInformation("MessageBus connection restored.");
                else
                    _logger.LogWarning("MessageBus connection is DOWN; the library should auto-reconnect.");
                _lastConnected = connected;
            }

            // Only attempt consumer recovery once the underlying connection is up — restarting a
            // consumer on a dead connection would just fail; the library reconnects the connection
            // itself, after which a dead consumer needs an explicit StartConsumer.
            if (connected && !consumerHealthy)
            {
                if (_lastConsumerHealthy)
                {
                    _logger.LogWarning(
                        "MessageBus consumer is UNHEALTHY (likely a broker ack timeout cancelled the consumers). Attempting recovery via StartConsumer().");
                }

                try
                {
                    _bus.StartConsumer();
                    _logger.LogInformation("MessageBus StartConsumer() invoked to recover consumers.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MessageBus StartConsumer() recovery attempt failed; will retry next poll.");
                }
            }
            else if (connected && consumerHealthy && !_lastConsumerHealthy)
            {
                _logger.LogInformation("MessageBus consumer health restored.");
            }

            _lastConsumerHealthy = consumerHealthy;
        }
    }
}
