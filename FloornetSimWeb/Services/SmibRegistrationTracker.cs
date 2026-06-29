using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Services
{
    /// <summary>
    /// In-memory tracker of known SMIB registrations.
    /// Replaces Redis from the real Registration service for development use.
    /// </summary>
    public class SmibRegistrationTracker
    {
        private readonly ConcurrentDictionary<string, SmibRegistrationState> _smibs = new();

        public bool IsSmibKnown(string uid) => _smibs.ContainsKey(uid);

        public bool IsRegistered(string uid) => _smibs.TryGetValue(uid, out var state) && state.Registered;

        public void MarkRegistered(string uid)
        {
            _smibs.AddOrUpdate(uid,
                _ => new SmibRegistrationState { Uid = uid, Online = true, Registered = true, LastKeepAlive = DateTime.UtcNow },
                (_, existing) => { existing.Online = true; existing.Registered = true; existing.LastKeepAlive = DateTime.UtcNow; return existing; });
        }

        public void MarkOnline(string uid)
        {
            _smibs.AddOrUpdate(uid,
                _ => new SmibRegistrationState { Uid = uid, Online = true, Registered = false, LastKeepAlive = DateTime.UtcNow },
                (_, existing) => { existing.Online = true; existing.LastKeepAlive = DateTime.UtcNow; return existing; });
        }

        public void MarkOffline(string uid)
        {
            if (_smibs.TryGetValue(uid, out var state))
            {
                state.Online = false;
            }
        }

        public void UpdateKeepAlive(string uid)
        {
            if (_smibs.TryGetValue(uid, out var state))
            {
                // A fresh keepAlive proves the SMIB is alive again. If the offline
                // sweep (or a prior gap) had marked it offline, bring it back online —
                // this mirrors the real Registration service re-publishing regSmibOnline
                // when a keepAlive resumes. Without this, a SMIB stays "Offline" in the
                // GUI forever even though traffic is flowing.
                state.Online = true;
                state.LastKeepAlive = DateTime.UtcNow;
            }
        }

        public IReadOnlyList<SmibRegistrationState> GetAllSmibs() => _smibs.Values.ToList();
    }

    public class SmibRegistrationState
    {
        public string Uid { get; set; } = string.Empty;
        public bool Online { get; set; }
        public bool Registered { get; set; }
        public DateTime LastKeepAlive { get; set; }
    }
}
