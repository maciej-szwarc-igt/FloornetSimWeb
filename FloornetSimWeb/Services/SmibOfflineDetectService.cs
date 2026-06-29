using System;
using System.Threading;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services
{
    /// <summary>
    /// Background hosted service that periodically sweeps <see cref="SmibRegistrationTracker"/>
    /// and marks SMIBs offline when they have not sent a keepAlive within the configured window.
    /// This is a standalone-web feature (the real CMS uses Redis-backed presence); the simulator
    /// keeps presence purely in-memory.
    /// </summary>
    public sealed class SmibOfflineDetectService : BackgroundService
    {
        private readonly SmibRegistrationTracker _tracker;
        private readonly ResponseViewModel _response;
        private readonly ILogger<SmibOfflineDetectService> _logger;

        // Sweep cadence and offline threshold. The SMIB keepAlive cadence is ~60s,
        // and the production Registration service only flags a SMIB offline after
        // KeepAliveInterval (1 min) * OfflineDetectWindows (2) = 2 minutes of silence.
        // Mirror that here: a 120s threshold prevents a single late/jittered keepAlive
        // from flapping the SMIB offline (and disrupting the SAS poll loop) every minute.
        private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan OfflineThreshold = TimeSpan.FromSeconds(120);

        public SmibOfflineDetectService(
            SmibRegistrationTracker tracker,
            ResponseViewModel response,
            ILogger<SmibOfflineDetectService> logger)
        {
            _tracker = tracker;
            _response = response;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SmibOfflineDetectService started (sweep={Sweep}s, threshold={Threshold}s).",
                SweepInterval.TotalSeconds, OfflineThreshold.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Sweep();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SmibOfflineDetectService sweep failed.");
                }

                try
                {
                    await Task.Delay(SweepInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("SmibOfflineDetectService stopping.");
        }

        private void Sweep()
        {
            var now = DateTime.UtcNow;
            foreach (var smib in _tracker.GetAllSmibs())
            {
                if (!smib.Online)
                {
                    continue;
                }

                if (now - smib.LastKeepAlive > OfflineThreshold)
                {
                    _tracker.MarkOffline(smib.Uid);
                    _logger.LogWarning("SMIB {Uid} marked OFFLINE (last keepAlive {Age:F0}s ago).",
                        smib.Uid, (now - smib.LastKeepAlive).TotalSeconds);
                    _response.Log($"SMIB {smib.Uid} marked OFFLINE — no keepAlive for {(now - smib.LastKeepAlive).TotalSeconds:F0}s");
                }
            }
        }
    }
}
