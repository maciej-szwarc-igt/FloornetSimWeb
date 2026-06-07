using IGT.FloorNet.EX.Cardless;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Cardless;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class CardlessRpcProvider : iCardless
    {

        private readonly ResponseViewModel _responseViewModel;
        public CardlessRpcProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<GetNonceResp> GetNonce()
        {
            if (!GetNonceRespViewModel.NonceResponseModel.RespondToRPC)
                return Task.FromResult<GetNonceResp>(null);

            long _nonce = (GetNonceRespViewModel.NonceResponseModel.SendCustomNonce) ? GetNonceRespViewModel.NonceResponseModel.CustomNonce : generateNonce();

            var req = new Dictionary<string, object>
            {

            };

            var resp = new GetNonceResp()
            {
                nonce = _nonce,
                expireDateTime = DateTime.Now.AddSeconds(GetNonceRespViewModel.NonceResponseModel.ExpireTimeSec)
            };

            GetNonceRespViewModel.NonceResponseModel.SmibUIDReq = RpcCallContext.Current.Uid;
            _responseViewModel.LogRpc(nameof(GetNonceResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        private long generateNonce()
        {
            Random random = new Random();

            long nonce = (long)(random.Next(100000, 1000000));

            return nonce;
        }
    }
}
