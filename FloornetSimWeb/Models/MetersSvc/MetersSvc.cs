using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.EX.Bonus.evt;
using IGT.FloorNet.EX.evt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc
{
    public class MetersSvc : ModelBase
    {
        private string _eventBody;

        private BNS_Hit_Events? _hitEventCode;

        private BNS_Pool_Events? _poolEventCode;

        private bool _isInitialized;

        private bool _isPoolIdSelected = false;

        private readonly IBonusMeterMockService _bonusMeterMockService;

        public MetersSvc(IBonusMeterMockService bonusMeterMockService)
        {
            _bonusMeterMockService = bonusMeterMockService;
        }

        public BNS_Hit_Events? BNS_Hit_Events
        {
            get => _hitEventCode;
            set
            {
                _hitEventCode = value;
                _poolEventCode = null;
                OnPropertyChanged("BNS_Hit_Events");
                OnPropertyChanged("BNS_Pool_Events");
                GenerateMetersAuditEventJson(true);
            }
        }

        public BNS_Pool_Events? BNS_Pool_Events
        {
            get => _poolEventCode;
            set
            {
                _poolEventCode = value;
                _hitEventCode = null;
                OnPropertyChanged("BNS_Pool_Events");
                OnPropertyChanged("BNS_Hit_Events");
                GenerateMetersAuditEventJson(BNS_Pool_Events == null);
            }
        }

        public string EventBody
        {
            get => _eventBody;
            set
            {
                _eventBody = value;
                OnPropertyChanged("EventBody");
            }
        }

        private string _bonusId;
        private ObservableCollection<string> _bonusIds;
        private string _poolId;
        private ObservableCollection<string> _poolIds;

        public string BonusId
        {
            get
            {
                return this._bonusId;
            }
            set
            {
                if (this._bonusId != value)
                {
                    this._bonusId = value;
                    OnPropertyChanged("BonusId");
                    SetPoolId(this._bonusId);
                    EventBody = string.Empty;
                    BNS_Hit_Events = null; BNS_Pool_Events = null;
                }
            }
        }

        public bool IsInitialized
        {
            get
            {
                return this._isInitialized;
            }
            set
            {
                if (this._isInitialized != value)
                {
                    this._isInitialized = value;
                    OnPropertyChanged("IsInitialized");
                }
            }
        }

        public bool IsPoolIdSelected
        {
            get
            {
                return this._isPoolIdSelected;
            }
            set
            {
                if (this._isPoolIdSelected != value)
                {
                    this._isPoolIdSelected = value;
                    OnPropertyChanged("IsPoolIdSelected");
                }
            }
        }

        public ObservableCollection<string> BonusIds
        {
            get
            {
                return this._bonusIds;
            }
            set
            {
                if (this._bonusIds != value)
                {
                    this._bonusIds = value;
                    OnPropertyChanged("BonusIds");
                }
            }
        }

        public string PoolId
        {
            get
            {
                return this._poolId;
            }
            set
            {
                if (this._poolId != value)
                {
                    this._poolId = value;
                    OnPropertyChanged("PoolId");
                    EventBody = string.Empty;
                    BNS_Hit_Events = null; BNS_Pool_Events = null;
                    IsPoolIdSelected = true;
                }
            }
        }

        public ObservableCollection<string> PoolIds
        {
            get
            {
                return this._poolIds;
            }
            set
            {
                if (this._poolIds != value)
                {
                    this._poolIds = value;
                    OnPropertyChanged("PoolIds");
                }
            }
        }

        private auditEvent BuildAuditEvent(t_busResp eventData, t_eventCode eventCode)
        {
            return new auditEvent
            {
                code = eventCode,
                seq = 0,
                amount = 0,
                amount2 = 0,
                value1 = 0,
                value2 = 0,
                cardId = null,
                playerId = 0,
                data = eventData,
                meters = null,
            };
        }

        private void GenerateMetersAuditEventJson(bool IsBnsHitEvent = false)
        {
            t_busResp obj;

            t_eventCode selectedEventCode;

            if (IsBnsHitEvent && BNS_Hit_Events != null && BonusId != null)
            {
                obj = _bonusMeterMockService.PoolHitMap[ConvertToByte(BonusId)].FirstOrDefault();

                selectedEventCode = (t_eventCode)Enum.Parse(typeof(t_eventCode), BNS_Hit_Events.ToString());

                var json = JsonConvert.SerializeObject(BuildAuditEvent(obj, selectedEventCode));
                EventBody = JToken.Parse(json).ToString(Formatting.Indented);
            }
            else if (BNS_Pool_Events != null && BonusId != null && PoolId != null)
            {
                BonusUID bonusUID = new BonusUID(ConvertToByte(BonusId), ConvertToByte(PoolId));
                obj = _bonusMeterMockService.PoolConfigMap[bonusUID.LevelId];
                selectedEventCode = (t_eventCode)Enum.Parse(typeof(t_eventCode), BNS_Pool_Events.ToString());

                var json = JsonConvert.SerializeObject(BuildAuditEvent(obj, selectedEventCode));
                EventBody = JToken.Parse(json).ToString(Formatting.Indented);
            }
        }

        private void SetPoolId(string bonusId)
        {
            PoolIds = new ObservableCollection<string>();
            var bonusHostsMap = _bonusMeterMockService.BonusHostsMap.Where(x => x.Key.ToString() == bonusId).Select(x => x.Value);

            foreach (var item in bonusHostsMap.SelectMany(x => x.poolList).Select(p => p.levelId))
            {
                BonusUID bonusUID = new BonusUID(item);
                PoolIds.Add(bonusUID.PoolId.ToString());
            }
        }

        private byte ConvertToByte(string val)
        {
            return byte.Parse(val);
        }
    }

    public enum BNS_Hit_Events
    {
        bnsSvcHit = t_eventCode.bnsSvcHit,
        bnsSvcHitPaid = t_eventCode.bnsSvcHitPaid,
        bnsSvcCelebration_no_winner = t_eventCode.bnsSvcCelebration_no_winner
    }

    public enum BNS_Pool_Events
    {
        bnsSvcCfgChanged = t_eventCode.bnsSvcCfgChanged,
        bnsSvcCfgNewBonus = t_eventCode.bnsSvcCfgNewBonus
    }
}
