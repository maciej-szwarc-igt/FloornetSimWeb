using Newtonsoft.Json;
using System.Collections.Generic;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus
{
    public class iBonusInfo : ModelBase
    {
        public iBonusInfo()
        {
            //Init();
        }

        private Dictionary<string, string> _EventList =
                new Dictionary<string, string> {
                { "getUiFunctions", GenerateJsonData("getUiFunctions") },
                { "getLoggedOnUsers", GenerateJsonData("getLoggedOnUsers") },
                { "registerPermissionManager", GenerateJsonData("registerPermissionManager") },
                { "getRegisteredPermissionManagers", GenerateJsonData("getRegisteredPermissionManagers") },
                };

        private List<string> _CategoriesList =
                 new List<string> {
                 { "iAuth" },
                 { "iBnsMgr" },
                 };

        private Dictionary<string, string> _iBnsMgrEventList =
                new Dictionary<string, string> {
                { "getHostStatus", GenerateJsonData("getHostStatus") },
                { "getPoolConfig", GenerateJsonData("getPoolConfig") },
                { "getPoolMeters", GenerateJsonData("getPoolMeters") },
                { "getBonusHistory", GenerateJsonData("getBonusHistory") },
                { "setHostState", GenerateJsonData("setHostState") },
                { "setPoolState", GenerateJsonData("setPoolState") },

                
                };

        public List<string> CategoriesList
        {
            get { return this._CategoriesList; }
            set
            {
                this._CategoriesList = value;
                OnPropertyChanged("CategoriesList");
            }
        }

        public Dictionary<string, string> EventList
        {
            get { return this._EventList; }
            set
            {
                this._EventList = value;
                OnPropertyChanged("EventList");
            }
        }

        public Dictionary<string, string> IBnsMgrEventList
        {
            get { return this._iBnsMgrEventList; }
            set
            {
                this._EventList = value;
                OnPropertyChanged("IBnsMgrEventList");
            }
        }

        public static string GenerateJsonData(string EventType)
        {
            string json = "";
            GetDataInfo getDataInfo = new GetDataInfo();
            switch (EventType)
            {
                case "getUiFunctions":
                    getDataInfo.message = "GetUiFunctions";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "getLoggedOnUsers":
                    getDataInfo.message = "GetLoggedOnUsersInfo";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "registerPermissionManager":
                    getDataInfo.message = "registerPermissionManager";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "getRegisteredPermissionManagers":
                    getDataInfo.message = "getRegisteredPermissionManagers";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;


                case "getHostStatus":
                    getDataInfo.message = "GetHostStatusInfo";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "getPoolConfig":
                    getDataInfo.message = "GetPoolConfigInfo";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "getPoolMeters":
                    getDataInfo.message = "GetPoolMetersInfo";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "getBonusHistory":
                    getDataInfo.message = "GetBonusHistoryInfo";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "setHostState":
                    getDataInfo.message = "SetHostState";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;
                case "setPoolState":
                    getDataInfo.message = "SetPoolState";
                    json = JsonConvert.SerializeObject(getDataInfo, Formatting.Indented);
                    break;

               
            }
            return json;
        }
      
    }

 }

