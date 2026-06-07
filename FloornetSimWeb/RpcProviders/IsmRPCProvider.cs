using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.ISM;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class IsmRPCProvider : iISM
    {
        private readonly ResponseViewModel _responseViewModel;
        public IsmRPCProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<getPublicKeyResp> getPublicKey()
        {
            if (!IKeysViewModel.keysModelISM.RespondToRPC)
                return Task.FromResult<getPublicKeyResp>(null);

            var req = new Dictionary<string, object>();
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelISM.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelISM.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };
            _responseViewModel.LogRpc(nameof(getPublicKeyResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }
    }
}
