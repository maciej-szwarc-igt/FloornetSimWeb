using IGT.FloorNet.EX.BIS;
using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class BnsMgrProvider : iBnsMgr
    {
        private readonly IBonusMeterMockService _bonusMeterMockService;
        public BnsMgrProvider(IBonusMeterMockService bonusMeterMockService)
        {
            _bonusMeterMockService = bonusMeterMockService;
        }
        public Task<getBonusHistoryResp> getBonusHistory(long bonusId, long hitSeqNum)
        {
            var bonusKey = Convert.ToByte(bonusId);
            var resp = new getBonusHistoryResp();
            resp.records = new List<PoolHit>();
            if(_bonusMeterMockService.PoolHitMap.ContainsKey(bonusKey))
                resp.records = _bonusMeterMockService.PoolHitMap[bonusKey];
            return Task.FromResult(resp);
        }

        public Task<getHostStatusResp> getHostStatus()
        {
            var resp = new getHostStatusResp();
            resp.hosts = new List<BonusHostStatus>();

            if (_bonusMeterMockService.BonusHostsMap.Count > 0)
                resp.hosts = _bonusMeterMockService.BonusHostsMap.Values.ToList<BonusHostStatus>();

            return Task.FromResult(resp);

        }

        public Task<getPoolConfigResp> getPoolConfig(List<long> levelIds)
        {
            var resp = new getPoolConfigResp();
            resp.pools = new List<PoolConfig>();

            foreach( long levelId in levelIds)
            {
                if (_bonusMeterMockService.PoolConfigMap.ContainsKey(levelId))
                    resp.pools.Add(_bonusMeterMockService.PoolConfigMap[levelId]);
            }

            return Task.FromResult(resp);
        }

        public Task<getPoolMetersResp> getPoolMeters(List<long> levelIds)
        {
            var resp = new getPoolMetersResp();
            resp.meters = new List<PoolMeters>();

            foreach (long levelId in levelIds)
            {
                if (_bonusMeterMockService.PoolMetersMap.ContainsKey(levelId))
                    resp.meters.Add(_bonusMeterMockService.PoolMetersMap[levelId]);
            }

            return Task.FromResult(resp);
        }

        public Task<setHostStateResp> setHostState(long bonusId, bool suspended)
        {
            throw new NotImplementedException();
        }

        public Task<setPoolStateResp> setPoolState(List<long> levelIds, bool suspended)
        {
            throw new NotImplementedException();
        }
    }
}
