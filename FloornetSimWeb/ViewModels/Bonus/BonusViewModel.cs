using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.BIS;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus
{
    public class BonusViewModel
    {
        private iAuthSub _authSub { get; set; }
        public ICommand SendRpc
        {
            get;
            set;
        }

        public ICommand AuthoriseRpc
        {
            get;
            set;
        }
        public ICommand DenyRpc
        {
            get;
            set;
        }

        public BonusViewModel()
        {
            iBonusInfo BonusInfo = new iBonusInfo();
            SendRpc = new RelayCommand(ExecuteMyMethod, CanExecuteMyMethod);

        }


        private bool CanExecuteMyMethod(object parameter)
        {
            return true;
        }

        private void ExecuteMyMethod(object parameter)
        {
            if (parameter.ToString().Contains("getUiFunctions"))
                RpcGetUiFunctions();
            else if (parameter.ToString().Contains("getLoggedOnUsers"))
                RpcGetLoggedonUserMethod();
            else if (parameter.ToString().Contains("getPoolMeters"))
                RpcGetPoolMeters();
            else if (parameter.ToString().Contains("getPoolConfig"))
                RpcGetPoolConfig();
            else if (parameter.ToString().Contains("getBonusHistory"))
                RpcGetBonusHistory();
            else if (parameter.ToString().Contains("getRegisteredPermissionManagers"))
                RpcGetRegisteredPermissionManagers();
            else if (parameter.ToString().Contains("registerPermissionManager"))
            {
                if (string.IsNullOrEmpty(SetFunctionDataInfo.Message))
                {
                    return;

                }
                else
                {
                    var myObject = JsonConvert.DeserializeObject<SetUIFunctionInfo>(SetFunctionDataInfo.Message);
                    RpcRegisterPermissionManager(myObject.name, myObject.functions);
                }
            }
            else if (parameter.ToString().Contains("setHostState"))
                RpcSetHostState();
            else if (parameter.ToString().Contains("setPoolState"))
                RpcSetPoolState();
            else if (parameter.ToString().Contains("getHostStatus"))
                RpcGetHostStatus();
        }


        private object JsonResponse(object resp)
        {
            var json = JsonConvert.SerializeObject(resp);
            return JToken.Parse(json).ToString(Formatting.Indented);
        }



        private async void RpcGetUiFunctions()
        {

            RpcProxyContext.Current = RpcProxyContext.ToService("BonusSvc", 15);

            getUiFunctionsResp _getUiFunctionsResp = await Startup._iAuthSub.getUiFunctions();

            //Startup.responseViewModel.LogRpc("getUiFunctionsResp", Startup._imessageBusRpcProxy, _getUiFunctionsResp, RpcCallContext.Current);

        }

        private async void RpcGetLoggedonUserMethod()
        {

            RpcProxyContext.Current = RpcProxyContext.ToService("BonusSvc", 15);
            getLoggedOnUsersResp _getLoggedOnUsersResp = await Startup._iAuthSub.getLoggedOnUsers();
           // Startup.responseViewModel.LogRpc("getLoggedOnUsersResp", Startup._imessageBusRpcProxy, _getLoggedOnUsersResp, RpcCallContext.Current);


        }

        private async void RpcGetPoolMeters()
        {

            if (Startup.levelID.Count() > 0)
            {
                RpcProxyContext.Current = RpcProxyContext.ToService(15);
                getPoolMetersResp _getPoolMetersResp = await Startup._iBnsMgr.getPoolMeters(Startup.levelID);
               // Startup.responseViewModel.LogRpc("getPoolMetersResp", Startup._imessageBusRpcProxy, _getPoolMetersResp, RpcCallContext.Current);
            }
            else
            {
                // MessageBox removed (no WPF)
            }
        }

        private async void RpcGetPoolConfig()
        {

            if (Startup.levelID.Count() > 0)
            {
                RpcProxyContext.Current = RpcProxyContext.ToService(15);
                getPoolConfigResp _getPoolConfigResp = await Startup._iBnsMgr.getPoolConfig(Startup.levelID);
               //Startup.responseViewModel.LogRpc("getPoolConfigResp", Startup._imessageBusRpcProxy, _getPoolConfigResp, RpcCallContext.Current);

            }
            else
            {
                // MessageBox removed (no WPF)
            }
        }
        private async void RpcGetBonusHistory()
        {
            if (BonusHistoryInfo.BonusId < 20 || BonusHistoryInfo.BonusId > 127)
            {
                // MessageBox removed (no WPF)
                return;
            }
            else
            {
                RpcProxyContext.Current = RpcProxyContext.ToService(15);
                getBonusHistoryResp _getBonusHistoryResp = await Startup._iBnsMgr.getBonusHistory(BonusHistoryInfo.BonusId, BonusHistoryInfo.HitSeqNum);
               // Startup.responseViewModel.LogRpc("getBonusHistoryResp", Startup._imessageBusRpcProxy, _getBonusHistoryResp, RpcCallContext.Current);
            }

        }
        private async void RpcRegisterPermissionManager(string authkey, List<string> uiFunctions)
        {

            RpcProxyContext.Current = RpcProxyContext.ToService("BonusSvc", 15);
            registerPermissionManagerResp _registerPermissionManagerResp = await Startup._iAuthSub.registerPermissionManager(authkey, uiFunctions);

           // Startup.responseViewModel.LogRpc("registerPermissionManagerResp", Startup._imessageBusRpcProxy, _registerPermissionManagerResp, RpcCallContext.Current);

        }

        private async void RpcSetHostState()
        {
            if (SetHostStateInfo.BonusId < 20 || SetHostStateInfo.BonusId > 127)
            {
                // MessageBox removed (no WPF)
                return;
            }
            else
            {
                RpcProxyContext.Current = RpcProxyContext.ToService(15);
                setHostStateResp _setHostStateResp = await Startup._iBnsMgr.setHostState(SetHostStateInfo.BonusId, SetHostStateInfo.HostState);
              //  Startup.responseViewModel.LogRpc("setHostStateResp", Startup._imessageBusRpcProxy, _setHostStateResp, RpcCallContext.Current);

            }

        }
        private async void RpcSetPoolState()
        {
            if (Startup.levelID.Count() > 0)
            {
                RpcProxyContext.Current = RpcProxyContext.ToService(15);
                setPoolStateResp _setPoolStateResp = await Startup._iBnsMgr.setPoolState(Startup.levelID, SetHostStateInfo.HostState);
               // Startup.responseViewModel.LogRpc("setPoolStateResp", Startup._imessageBusRpcProxy, _setPoolStateResp, RpcCallContext.Current);
            }
            else
            {
                // MessageBox removed (no WPF)
            }
        }

        private async void RpcGetHostStatus()
        {

            RpcProxyContext.Current = RpcProxyContext.ToService(15);
            getHostStatusResp _getHostStatusResp = await Startup._iBnsMgr.getHostStatus();
           // Startup.responseViewModel.LogRpc("getHostStatusResp", Startup._imessageBusRpcProxy, _getHostStatusResp, RpcCallContext.Current);
        }

        private async void RpcGetRegisteredPermissionManagers()
        {

            RpcProxyContext.Current = RpcProxyContext.ToService("BonusSvc", 15);
            getRegisteredPermissionManagersResp _getRegisteredPermissionManagersResp = await Startup._iAuthSub.getRegisteredPermissionManagers();
           // Startup.responseViewModel.LogRpc("getRegisteredPermissionManagersResp", Startup._imessageBusRpcProxy, _getRegisteredPermissionManagersResp, RpcCallContext.Current);

        }

    }

}


        

    

