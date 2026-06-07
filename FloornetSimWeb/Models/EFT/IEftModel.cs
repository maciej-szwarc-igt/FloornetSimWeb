using IGT.FloorNet.EX.Wat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Eft
{
    public class IEftModel : ModelBase
    {
        private string resourceId;
        private long authCashableAmt;
        private long reqCashableAmt;
        private t_watException hostException;
        private long currentKeyNumber;
        private string signature;
        private HashSet<string> usedResourceIds;
        private int initiateDebitDelaySeconds;
        private int remainingNoResponseTimeMs;


        public string ResourceId { get => resourceId; set { resourceId = value; OnPropertyChanged(nameof(resourceId)); } }
        public long AuthCashableAmt { get => authCashableAmt; set { authCashableAmt = value; OnPropertyChanged(nameof(authCashableAmt)); } }
        public long ReqCashableAmt { get => authCashableAmt; set { authCashableAmt = value; OnPropertyChanged(nameof(authCashableAmt)); } }
        public t_watException HostException { get => hostException; set { hostException = value; OnPropertyChanged(nameof(hostException)); } }
        public long CurrentKeyNumber { get => currentKeyNumber; set { currentKeyNumber = value; OnPropertyChanged(nameof(currentKeyNumber)); } }
        public string Signature { get => signature; set { signature = value; OnPropertyChanged(nameof(signature)); } }
        public HashSet<string> UsedResourceIds{ get => usedResourceIds; set { usedResourceIds = value; OnPropertyChanged(nameof(usedResourceIds)); } }
        public int InitiateDebitDelaySeconds { get => initiateDebitDelaySeconds; set { initiateDebitDelaySeconds = value < 0 ? 0 : value; OnPropertyChanged(nameof(InitiateDebitDelaySeconds)); } }
        public int RemainingNoResponseTimeMs { get => remainingNoResponseTimeMs; private set { remainingNoResponseTimeMs = value; OnPropertyChanged(nameof(RemainingNoResponseTimeMs)); } }
        public IEftModel()
        {
            resourceId = string.Empty;
            authCashableAmt = 0;
            reqCashableAmt = 0;
            hostException = t_watException.transfer_denied;
            currentKeyNumber = 0;
            signature = string.Empty;
            usedResourceIds = new HashSet<string>();
            initiateDebitDelaySeconds = 0;
        }

        public void Clear()
        {
            ResourceId = string.Empty;
            AuthCashableAmt = 0;
            ReqCashableAmt = 0;
            HostException = t_watException.transfer_denied;
            CurrentKeyNumber = 0;
            Signature = string.Empty;
            InitiateDebitDelaySeconds = 0;
        }

        public IEnumerable<t_watException> GetWatExceptions
        {
            get
            {
                return Enum.GetValues(typeof(t_watException)).Cast<t_watException>();
            }
        }

        // Shared countdown gate
        private readonly object _countdownLock = new();
        private Task _delayGate = Task.CompletedTask;
        private CancellationTokenSource? _delayCts;
        private DateTime _delayEndsAtUtc = DateTime.MinValue;

        /// <summary>
        /// Starts/restarts the countdown using InitiateDebitDelaySeconds. All callers can await WaitForNoResponseDelayAsync().
        /// </summary>
        public void StartNoResponseCountdown()
        {
            var durationMs = InitiateDebitDelaySeconds * 1000;
            if (durationMs <= 0)
            {
                lock (_countdownLock)
                {
                    _delayCts?.Cancel();
                    _delayCts?.Dispose();
                    _delayCts = null;
                    _delayEndsAtUtc = DateTime.MinValue;
                    _delayGate = Task.CompletedTask;
                    RemainingNoResponseTimeMs = 0;
                }
                return;
            }

            lock (_countdownLock)
            {
                _delayCts?.Cancel();
                _delayCts?.Dispose();

                _delayCts = new CancellationTokenSource();
                var token = _delayCts.Token;

                _delayEndsAtUtc = DateTime.UtcNow.AddMilliseconds(durationMs);
                RemainingNoResponseTimeMs = durationMs;

                _delayGate = RunCountdownAsync(token);
            }
        }

        public Task WaitForNoResponseDelayAsync()
        {
            Task gate;
            lock (_countdownLock)
            {
                gate = _delayGate;
            }
            return gate;
        }

        private async Task RunCountdownAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    var remaining = (int)Math.Ceiling((_delayEndsAtUtc - DateTime.UtcNow).TotalMilliseconds);
                    if (remaining <= 0)
                    {
                        RemainingNoResponseTimeMs = 0;
                        return;
                    }

                    RemainingNoResponseTimeMs = remaining;

                    await Task.Delay(Math.Min(100, remaining), token);
                }
            }
            catch (OperationCanceledException)
            {
                // if cancelled, just exit
            }
        }
    }
}
