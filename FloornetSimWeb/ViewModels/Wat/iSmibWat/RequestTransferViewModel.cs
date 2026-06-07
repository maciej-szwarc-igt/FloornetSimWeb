using IGT.FloorNet.EX.Wat;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Wat.iSmibWat;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iSmibWat
{
    public class RequestTransferViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        private bool _rpcProcess = true;
        public ResponseModel Model { get; } = new ResponseModel();
        private RelayCommand _requestTransferCommand;
        public RelayCommand ClearCommand { get; }
        public static RequestTransferModel RequestTransferModel { get; } = new RequestTransferModel();
        public RequestTransferViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
        }

        public bool RpcProcess
        {
            get => _rpcProcess;
            set
            {
                _rpcProcess = value;
                if (_rpcProcess)
                    Model.Response = string.Empty;
            }
        }

        public RelayCommand RequestTransferCommand
        {
            get
            {
                _requestTransferCommand = new RelayCommand(
                  RequestTransfer,
                  param => true

                );

                return _requestTransferCommand;
            }
        }

        private void RequestTransfer(Object obj)
        {
            if (!RpcProcess) return;
            RPCRequestTransfer();
        }

        private async void RPCRequestTransfer()
        {

            string requestId = RequestTransferModel.RequestId;
            string resourceId = RequestTransferModel.ResourceId;
            string cardId = RequestTransferModel.CardId;
            long playerId = RequestTransferModel.PlayerId;
            long cardInCount = RequestTransferModel.CardInCount;
            t_transferDirection transferDirection = RequestTransferModel.TransferDirection;
            long reqCashableAmt = RequestTransferModel.RequestCashableAmt;
            long reqPromoAmt = RequestTransferModel.ReqPromoAmt;
            long reqNonCashAmt = RequestTransferModel.ReqNonCashAmt;
            bool printTicket = RequestTransferModel.PrintTicket;

            string jwt = GenerateJsonWebToken(RequestTransferModel.SiteId, RequestTransferModel.MachineLocation, RequestTransferModel.MachineNum, cardId, cardInCount, playerId);

            //string jwt = "fewwefwf";

            var req = new Dictionary<string, object> {
                {nameof(requestId), requestId},
                {nameof(resourceId), resourceId},
                {nameof(cardId), cardId},
                {nameof(playerId), playerId},
                {nameof(cardInCount), cardInCount},
                {nameof(transferDirection), transferDirection},
                {nameof(reqCashableAmt), reqCashableAmt},
                {nameof(reqPromoAmt), reqPromoAmt},
                {nameof(reqNonCashAmt), reqNonCashAmt},
                {nameof(printTicket), printTicket},
                {nameof(jwt), jwt}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(RequestTransferModel.RpcSmibUid);

            var resp = await Startup._iSmibWat.requestTransfer(requestId,resourceId,cardId,playerId,cardInCount,transferDirection,reqCashableAmt,reqPromoAmt,reqNonCashAmt,printTicket,jwt);

            RequestTransferModel.Jwt = jwt;

            _responseViewModel.LogRpc(Constants.RequestTransfer, req, resp, RpcCallContext.Current);

        }

        private string GenerateJsonWebToken(string SiteId,string MachineLoc,long MachineNum, string cardId, long cardInCount, long playerid)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss,"ServiceSimulator"),
                new Claim(JwtRegisteredClaimNames.Sub,playerid.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,DateTime.Now.AddMinutes(30).ToString()),
                new Claim("igt.pin","true"),
                new Claim("igt.cid",cardId),
                new Claim("igt.sid",SiteId),
                new Claim("igt.emp","false"),
                new Claim("igt.ses",cardInCount.ToString()),
                new Claim("igt.loc",MachineLoc),
                new Claim("igt.num",MachineNum.ToString()),
            };

            var header = new JwtHeader()
            {
                { JwtHeaderParameterNames.Alg, SecurityAlgorithms.EcdsaSha256 },
                { JwtHeaderParameterNames.Typ, "JWT" }
                //{ "kid", currentKeyNumber.ToString() }
            };

            var securityToken = new JwtSecurityToken(header, new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public void Clear(object obj)
        {
            RequestTransferModel.Clear();
        }
    }
}
