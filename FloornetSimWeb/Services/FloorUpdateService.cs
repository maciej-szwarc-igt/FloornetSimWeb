using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services
{
    /// <summary>
    /// Floor update job codes mirrored from the real MACheckin accounting-DB queue.
    /// See <c>wwwroot/MACheckin-Service-FloorNet-Calls.md</c>.
    /// </summary>
    public enum FloorUpdateJobCode
    {
        RegistrationUpdate = 2,
        EgmEnableDisable = 19,
        RgControlRequest = 25
    }

    /// <summary>A single queued floor-update job targeted at a SMIB uid.</summary>
    public sealed class FloorUpdateJob
    {
        public FloorUpdateJobCode Code { get; init; }
        public string Uid { get; init; } = string.Empty;

        /// <summary>For Job 19: true = disable EGM, false = enable EGM.</summary>
        public bool Disable { get; init; }

        /// <summary>For Job 25: RG lockState (0 = unlock, 2 = lock BV, 3 = lock EGM).</summary>
        public int LockState { get; init; }

        /// <summary>Check-in / RG key forwarded to the SMIB.</summary>
        public string Key { get; init; } = string.Empty;
    }

    /// <summary>
    /// In-memory floor-update job queue. The real MACheckin polls a SQL accounting DB with Redis
    /// leader election; the standalone simulator instead lets operators enqueue jobs via REST and
    /// drains them here, executing the same <c>iReg.disableEGM</c> / <c>iRG.LockBV</c> RPC paths
    /// (Jobs 2 / 19 / 25) documented in <c>MACheckin-Service-FloorNet-Calls.md</c>.
    /// </summary>
    public sealed class FloorUpdateService : BackgroundService
    {
        private static readonly ConcurrentQueue<FloorUpdateJob> Queue = new();
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

        private readonly ResponseViewModel _response;
        private readonly ILogger<FloorUpdateService> _logger;

        public FloorUpdateService(ResponseViewModel response, ILogger<FloorUpdateService> logger)
        {
            _response = response;
            _logger = logger;
        }

        /// <summary>Enqueue a floor-update job for the background drainer to process.</summary>
        public static void Enqueue(FloorUpdateJob job) => Queue.Enqueue(job);

        /// <summary>Current number of pending jobs (for status surfacing).</summary>
        public static int PendingCount => Queue.Count;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FloorUpdateService started (poll={Poll}s).", PollInterval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                while (Queue.TryDequeue(out var job))
                {
                    try
                    {
                        await ProcessAsync(job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "FloorUpdateService job {Code} for {Uid} failed.", job.Code, job.Uid);
                        _response.Log($"FloorUpdate Job {(int)job.Code} for {job.Uid} FAILED: {ex.Message}");
                    }
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

            _logger.LogInformation("FloorUpdateService stopping.");
        }

        private async Task ProcessAsync(FloorUpdateJob job)
        {
            if (string.IsNullOrWhiteSpace(job.Uid))
            {
                _response.Log($"FloorUpdate Job {(int)job.Code} skipped — no uid.");
                return;
            }

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(job.Uid);

            switch (job.Code)
            {
                case FloorUpdateJobCode.EgmEnableDisable:
                    // Job 19: enable/disable EGM at the SMIB.
                    await Startup._iReg.disableEGM(job.Disable, job.Key);
                    _response.Log($"FloorUpdate Job 19 (EGMEnableDisable) → {job.Uid}: disableEGM({job.Disable})");
                    break;

                case FloorUpdateJobCode.RgControlRequest:
                    // Job 25: Responsible Gaming lock/unlock based on lockState.
                    // lockState 0 = unlock all; 2 = lock BV; 3 = lock EGM.
                    switch (job.LockState)
                    {
                        case 0:
                            await Startup._iReg.disableEGM(false, job.Key);
                            await Startup._iRG.LockBV(false, job.Key);
                            _response.Log($"FloorUpdate Job 25 (RGControlRequest, unlock) → {job.Uid}: disableEGM(false)+LockBV(false)");
                            break;
                        case 2:
                            await Startup._iRG.LockBV(true, job.Key);
                            _response.Log($"FloorUpdate Job 25 (RGControlRequest, lock BV) → {job.Uid}: LockBV(true)");
                            break;
                        case 3:
                            await Startup._iReg.disableEGM(true, job.Key);
                            _response.Log($"FloorUpdate Job 25 (RGControlRequest, lock EGM) → {job.Uid}: disableEGM(true)");
                            break;
                        default:
                            _response.Log($"FloorUpdate Job 25 → {job.Uid}: unknown lockState {job.LockState}");
                            break;
                    }
                    break;

                case FloorUpdateJobCode.RegistrationUpdate:
                    // Job 2: would re-push registration. The simulator has no per-machine DB config,
                    // so this is surfaced as a no-op log to keep parity visibility.
                    _response.Log($"FloorUpdate Job 2 (RegistrationUpdate) → {job.Uid}: no DB config in simulator (no-op).");
                    break;

                default:
                    _response.Log($"FloorUpdate unknown job code {(int)job.Code} → {job.Uid}");
                    break;
            }
        }
    }
}
