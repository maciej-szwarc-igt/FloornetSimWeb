using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.EX.Bonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc
{
    /// <summary>
    /// Mock Bonus Meter Service to provide Bonuses, Pools, PoolConfig, etc
    /// </summary>
    public interface IBonusMeterMockService
    {
        public Dictionary<byte, BonusHostStatus> BonusHostsMap { get; }
        public Dictionary<long, PoolConfig> PoolConfigMap { get; }
        public Dictionary<long, PoolMeters> PoolMetersMap { get; }
        public Dictionary<byte, List<PoolHit>> PoolHitMap { get; }

        public bool IsInitialized { get; set; }

        public void Initialize();

        public void Clear();
    }

    public class BonusMeterMockService : ModelBase, IBonusMeterMockService
    {
        //GET CONFIG FROM THE UI
        private string _bonusHosts;

        private byte[] _bonusHostIds;
        private byte _noofPools;
        private byte _noOfMachines;
        private byte _noOfHits;

        public string BonusHosts
        {
            get => _bonusHosts;
            set
            {
                _bonusHosts = value;
                if (BonusHosts.Length > 0)
                {
                    string[] val = BonusHosts.Split(',');
                    _bonusHostIds = val.Where(s => s.Trim().Length > 0).Select(s => byte.Parse(s)).ToArray();
                    OnPropertyChanged("BonusHostIds");
                }
                OnPropertyChanged("BonusHosts");
            }
        }

        public byte[] BonusHostIds
        {
            get => _bonusHostIds;
            set
            {
                _bonusHostIds = value;
                OnPropertyChanged("BonusHostIds");
            }
        }

        public byte NoofPools
        {
            get => _noofPools;
            set
            {
                _noofPools = value;
                OnPropertyChanged("NoofPools");
            }
        }

        public byte NoOfMachines
        {
            get => _noOfMachines;
            set
            {
                _noOfMachines = value;
                OnPropertyChanged("NoOfMachines");
            }
        }

        public byte NoOfHits
        {
            get => _noOfHits;
            set
            {
                _noOfHits = value;
                OnPropertyChanged("NoOfHits");
            }
        }

        public Dictionary<byte, BonusHostStatus> BonusHostsMap { get; }
        public Dictionary<long, PoolConfig> PoolConfigMap { get; }
        public Dictionary<long, PoolMeters> PoolMetersMap { get; }
        public Dictionary<byte, List<PoolHit>> PoolHitMap { get; }
        public bool IsInitialized { get; set; }

        private readonly Random rng;

        public BonusMeterMockService()
        {
            Init();
            rng = new Random();
            BonusHostsMap = new Dictionary<byte, BonusHostStatus>();
            PoolConfigMap = new Dictionary<long, PoolConfig>();
            PoolMetersMap = new Dictionary<long, PoolMeters>();
            PoolHitMap = new Dictionary<byte, List<PoolHit>>();
        }

        private void Init()
        {
            BonusHosts = "95";
            string[] val = BonusHosts.Split(',');
            BonusHostIds = val.Select(s => byte.Parse(s)).ToArray();
            NoofPools = 1;
            NoOfMachines = 100;
            NoOfHits = 1;
        }

        public void Initialize()
        {
            InitializeBonusHostStatus();
            InitializePoolConfig();
            InitializePoolMeters();
            InitializePoolHitMap();
        }

        public void Clear()
        {
            BonusHosts = "";
            NoofPools = 0;
            NoOfMachines = 0;
            NoOfHits = 0;

            BonusHostsMap.Clear();
            PoolConfigMap.Clear();
            PoolMetersMap.Clear();
            PoolHitMap.Clear();
        }

        private void InitializeBonusHostStatus()
        {
            BonusHostsMap.Clear();
            for (byte i = 0; i < BonusHostIds.Length; i++)
            {
                BonusHostStatus bonusHostStatus = new BonusHostStatus()
                {
                    bisVersion = "3.1",
                    bonusId = BonusHostIds[i],
                    bonusType = t_bonusType.PROG,
                    gmid = "23",
                    hostState = t_hostState.online,
                    powerUps = 3,
                    serialNo = "12",
                    ubsVersion = "3.2",
                    poolList = new List<t_poolSummary>()
                };

                for (byte j = 0; j < NoofPools; j++)
                {
                    t_poolSummary pool = new t_poolSummary()
                    {
                        status = t_bonusStatus.poolRunning,
                        levelId = new BonusUID(BonusHostIds[i], j).LevelId
                    };
                    bonusHostStatus.poolList.Add(pool);
                }

                BonusHostsMap[BonusHostIds[i]] = bonusHostStatus;
            }
        }

        private void InitializePoolConfig()
        {
            PoolConfigMap.Clear();
            for (byte i = 0; i < BonusHostIds.Length; i++)
            {
                for (byte j = 0; j < NoofPools; j++)
                {
                    BonusUID bonusUID = new BonusUID(BonusHostIds[i], j);

                    var poolConfig = new PoolConfig()
                    {
                        levelId = bonusUID.LevelId,
                        reason = t_configChangeReason.newBonus,
                        hitSeqNum = 1,
                        numConfig = NoOfMachines,
                        bonusCode = t_bonusCode.cardedMyst,
                        pool = 3000,
                        hiddenPool = 2000,
                        delayedPool = 1000,
                        startupPool = 100,
                        overflow = 20,
                        poolInc = 1000,
                        hiddenInc = 2000,
                        baseValue = 0,
                        mysteryMin = 200,
                        mysteryMax = 20000,
                        totalStartup = 100,
                        totalContributed = 2000,
                        maxAllowable = 10,
                        numberOfHits = 5,
                        status = t_bonusStatus.poolRunning,
                        cashlessPrize = false,
                        Unused = 0,
                        Member_Card_Award = 100,
                        Number_Card_Award = 50,
                        Uncarded_Award = 20,
                        bonusType = t_bonusType.CLC,
                        bonusName = "Pool " + bonusUID.ToString(),
                        Deductible_Flag = true,
                        CurrentSetPoints = new List<long>(),
                        CurrentSetPoints_IncPct = new List<long>(),
                        HiddenSetPoints = new List<long>(),
                        HiddenSetPoints_IncPct = new List<long>(),
                        Casino_Contrib = 1000,
                        Sum_of_Casino_Contrib = 50000,
                        CLD_Player_Count = 1234,
                        machines = new List<t_machineDetails> { },
                        machineLevel = 1,
                        variationName = "MYST1234",
                        trtp = 93,
                        memoryErrors = new List<t_bonusMemoryError>()
                    };

                    for (int k = 0; k < NoOfMachines; k++)
                    {
                        var machine = new t_machineDetails()
                        {
                            accumulatedTurnover = 1,
                            delayedTurnover = 2,
                            machineLoc = "XXXXX" + k.ToString(),
                            machineNum = 12345678 + k,
                            uid = "XXXXX" + k.ToString(),
                            enabled = true,
                            turnover = 1
                            
                        };
                        poolConfig.machines.Add(machine);
                    }

                    
                    
                    PoolConfigMap[bonusUID.LevelId] = poolConfig;
                    
                }
            }
        }

        private void InitializePoolMeters()
        {
            PoolMetersMap.Clear();
            for (byte i = 0; i < BonusHostIds.Length; i++)
            {
                for (byte j = 0; j < NoofPools; j++)
                {
                    BonusUID bonusUID = new BonusUID(BonusHostIds[i], j);
                    var poolMeter = new PoolMeters()
                    {
                        levelId = bonusUID.LevelId,
                        contributionSinceDoorOpen = 100,
                        contributionSincePowerUp = 200,
                        totalContributions = 300,
                        totalPaid = 100,
                        totalResets = 5,
                        totalTransferToMachines = 34,
                        totalTransferToMachinesRounding = 45,
                        turnoverSinceConfigChange = 2,
                        turnoverSinceStartUp = 4
                    };


                    PoolMetersMap[bonusUID.LevelId] = poolMeter;
                }
            }
        }

        private void InitializePoolHitMap()

        {
            PoolHitMap.Clear();
            for (byte i = 0; i < BonusHostIds.Length; i++)
                {
                    List<PoolHit> poolHitList = new List<PoolHit>();
                    for (int j = 0; j < NoOfHits; j++)
                    {
                        var poolHitData = new PoolHit()
                        {
                            BonusAttemptedPayAmount = 100,
                            bonusCode = t_bonusCode.cardedMyst,
                            cashlessPrize = false,
                            Casino_Contrib = 10000,
                            CelCashlessPrize = false,
                            CelDeductible_Flag = false,
                            CelPayToCreditMeter = 1,
                            CityIndex = 1,
                            ConsolationBE2DidntPayCnt = 20,
                            ConsolationFailureAmt = 10,
                            ConsolationFailureCnt = 5,
                            ConsolationNoResponseCnt = 3,
                            ConsolationSuccessAmt = 2,
                            ConsolationSuccessCnt = 1,
                            DCNPaidAmount = 10,
                            Deductible_Flag = false,
                            delayedPool = 100,
                            EGMsNotCommunicating = 4,
                            eventReason = t_bonusEventReason.hit,
                            eventSeqNum = 23,
                            hiddenPool = 12,
                            hitSeqNum = 143,
                            hitAmount = 200,
                            levelId = new BonusUID(BonusHostIds[i], Convert.ToByte(rng.Next(0, NoofPools))).LevelId,
                            newPool = 0,
                            paidAs = t_payTo.cashable,
                            payToCreditMeter = 1,
                            startupPool = 123,
                            systemTimeStamp = DateTime.UtcNow,
                            uid = "123",
                            WinningThreshold = 12345
                        };
                        poolHitList.Add(poolHitData);
                    }


                    PoolHitMap[BonusHostIds[i]] = poolHitList;                
            }
        }

        public enum BNS_Hit_Events
        {
            bnsSvcHit = t_eventCode.bnsSvcHit,
            bnsSvcHitPaid = t_eventCode.bnsSvcHitPaid,
            bnsSvcHitPaymentFailure = t_eventCode.bnsSvcHitPaymentFailure,
            bnsSvcPoolDeleted = t_eventCode.bnsSvcPoolDeleted,
            bnsSvcSeatRefusedPayment = t_eventCode.bnsSvcSeatRefusedPayment,
            bnsSvcCelebration_no_winner = t_eventCode.bnsSvcCelebration_no_winner
        }
    }
}
