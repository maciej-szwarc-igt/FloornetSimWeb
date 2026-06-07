using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class AuthRpcProvider : iAuth
    {
        private readonly BonusViewModel _BonusViewModel;
        private readonly ResponseViewModel _ResponseViewModel;
        private bool flag = true;

        public AuthRpcProvider(
            BonusViewModel bonusViewModel,
            ResponseViewModel responseViewModel)
        {
            _BonusViewModel = bonusViewModel;
            _ResponseViewModel = responseViewModel;
        }
        public async Task<authorizeUiFunctionResp> authorizeUiFunction(string invocationId, string uiFunction, string sessionId, string user1Id, string user2Id, string serviceName, object[] uiFunctionParams)
        {
            var req = new Dictionary<string, object>
            {
             {nameof(invocationId), invocationId},
                {nameof(uiFunction), uiFunction},
                 {nameof(sessionId), sessionId},
                  {nameof(user1Id), user1Id},
                   {nameof(user2Id), user2Id},
                   {nameof(serviceName), serviceName},
                   {nameof(uiFunctionParams), uiFunctionParams}

            };

            authorizeUiFunctionResp resp;

            if (flag)
            {
                resp = new authorizeUiFunctionResp()
                {
                    authorized = true,
                    denyReason = null

                };
                flag = false;
            }
            else
            {
                resp = new authorizeUiFunctionResp()
                {
                    authorized = false,
                    denyReason = "Denied by simulator."

                };
                flag = true;
            }

            _ResponseViewModel.LogRpc("Authorize Ui Function", req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
    }
}
