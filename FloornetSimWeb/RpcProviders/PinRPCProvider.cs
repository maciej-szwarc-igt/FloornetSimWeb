using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.Player;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPin;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class PinRPCProvider : iPin
    {

        private readonly ResponseViewModel _responseViewModel;
        public PinRPCProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<getPublicKeyResp> getPublicKey()
        {

            if (!IKeysViewModel.keysModelCard.RespondToRPC)
                return Task.FromResult<getPublicKeyResp>(null);

            var req = new Dictionary<string, object>();
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelCard.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelCard.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };
            _responseViewModel.LogRpc(nameof(getPublicKeyResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<validatePinResp> validatePin(string cardId, long cardInCount, DateTime startTime, long currentKeyNumber, string smib_key, string iv, string encpin)
        {

            if (!ValidatePinViewModel.ValidatePinModel.RespondToRPC)
                return Task.FromResult<validatePinResp>(null);

            string jwt = "";
            string message = "";

            var req = new Dictionary<string, object>
            {
                {nameof(cardId), cardId},
                {nameof(cardInCount), cardInCount},
                {nameof(startTime), startTime},
                {nameof(currentKeyNumber), currentKeyNumber},
                {nameof(smib_key), smib_key},
                {nameof(iv), iv},
                {nameof(encpin), encpin},

            };

            if (ValidatePinViewModel.ValidatePinModel.IsValidPin)
            {

                jwt = GenerateJsonWebToken(RpcCallContext.Current, cardId, cardInCount, currentKeyNumber,ValidatePinViewModel.ValidatePinModel.PlayerId,ValidatePinViewModel.ValidatePinModel.PlayerName,ValidatePinViewModel.ValidatePinModel.PlayerLastName);
                message = null;
            }
            else
            {
                jwt = null;
                message = ValidatePinViewModel.ValidatePinModel.Message;
            }

            var resp = new validatePinResp()
            {
                status = ValidatePinViewModel.ValidatePinModel.Status,
                message = message,
                jwt = jwt
            };

            ValidatePinViewModel.ValidatePinModel.Jwt = jwt;

            _responseViewModel.LogRpc(nameof(validatePinResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        private string GenerateJsonWebToken(RpcCallContext currentContext,string cardId, long cardInCount, long currentKeyNumber,string playerid,string playerName,string playerLastName)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss,"ServiceSimulator"),
                new Claim(JwtRegisteredClaimNames.Sub,playerid),
                new Claim(JwtRegisteredClaimNames.Exp,DateTime.Now.AddMinutes(30).ToString()),
                new Claim("igt.pin","true"),
                new Claim("igt.cid",cardId),
                new Claim("igt.sid",currentContext.SiteId),
                new Claim("igt.emp","false"),
                new Claim("igt.ses",cardInCount.ToString()),
                new Claim("igt.loc",currentContext.MachineLoc),
                new Claim("igt.num",currentContext.MachineNum.ToString()),
                new Claim("igt.fn",playerName),
                new Claim("igt.ln",playerLastName)
            };

            var header = new JwtHeader()
            {
                { JwtHeaderParameterNames.Alg, SecurityAlgorithms.EcdsaSha256 },
                { JwtHeaderParameterNames.Typ, "JWT" },
                { "kid", currentKeyNumber.ToString() }
            };

            var securityToken = new JwtSecurityToken(header,new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
