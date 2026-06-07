using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Gat;
using IGT.FloorNet.EX.Gat;
using IGT.FloorNet.MessageBus.Rpc;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class GatRpcProvider : iGat
    {
        private readonly ResponseViewModel _responseViewModel;
        public GatRpcProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public async Task<verificationResultResp> verificationResult(long requestId, List<t_resultComponent> resultComponentList)
        {
            if (!IGatViewModel.IGatModel.RespondToRPC)
                return await Task.FromResult<verificationResultResp>(null);


            var req = new Dictionary<string, object>
            {
                {nameof(requestId), requestId },
                {nameof(resultComponentList), resultComponentList }
            };

            List<t_passFailComponent> passFailComponentList = new List<t_passFailComponent>();
            if (IGatViewModel.IGatModel.SendPassFailComponentData1)
                passFailComponentList.Add(new t_passFailComponent(){ name = IGatViewModel.IGatModel.Name1, pass = IGatViewModel.IGatModel.Pass1 });
            if (IGatViewModel.IGatModel.SendPassFailComponentData2)
                passFailComponentList.Add(new t_passFailComponent(){ name = IGatViewModel.IGatModel.Name2, pass = IGatViewModel.IGatModel.Pass2 });
            if (IGatViewModel.IGatModel.SendPassFailComponentData3)
                passFailComponentList.Add(new t_passFailComponent(){ name = IGatViewModel.IGatModel.Name3, pass = IGatViewModel.IGatModel.Pass3 });

            var resp = new verificationResultResp()
            {
                requestId = IGatViewModel.IGatModel.SendCustomRequestId ? IGatViewModel.IGatModel.RequestId : requestId,
                passFailComponentList = passFailComponentList
            };

            _responseViewModel.LogRpc(nameof(verificationResult), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
    }
}
