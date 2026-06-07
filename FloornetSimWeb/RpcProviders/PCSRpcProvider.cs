using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.AML;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using IGT.FloorNet.EX.Player;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class PCSRpcProvider : iPCS
    {
        private readonly PCSViewModel _PCSViewModel;

        private readonly ResponseViewModel _responseViewModel;

        public PCSRpcProvider(PCSViewModel amlViewModel, ResponseViewModel responseViewModel)
        {
            _PCSViewModel = amlViewModel;
            _responseViewModel = responseViewModel;
        }

        public async Task<getPublicKeyResp> getPublicKey()
        {
            var req = new Dictionary<string, object>
            {

            };
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelEft.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelEft.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };
            _responseViewModel.LogRpc("getPublicKey", req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
        public Task<StartPCSSessionResp> StartPCSSession(string cardId, t_idReaderType idReaderType, long cardInCount, string smib_key, string iv, string encpin)
        {
            var req = new Dictionary<string, object>
            {
                {nameof(cardId), cardId},
                {nameof(cardInCount), cardInCount},
                {nameof(idReaderType), idReaderType},
                {nameof(smib_key), smib_key},
                {nameof(iv), iv},
                {nameof(encpin), encpin},

            };

            _PCSViewModel.Cardid = cardId;
            _PCSViewModel.CardInCount = cardInCount;

            var resp = new StartPCSSessionResp()
            {
                cardId = _PCSViewModel.Cardid,
                alertMsg = _PCSViewModel.AlertMsg,
                cardInCount = _PCSViewModel.CardInCount,
                noLimitTime = _PCSViewModel.NoLimitTime,
                pinValid = _PCSViewModel.PinValid,
                activityStatementReady = _PCSViewModel.ActivityStatement,
                accountInUse = _PCSViewModel.AccountInUse,
                pcsDown = _PCSViewModel.PcsDown,
                pinLocked = _PCSViewModel.PinLocked,
                accountCanceled = _PCSViewModel.AccountCanceled,
                invalidPCSID = _PCSViewModel.InvalidPCSID,
                limits = new List<t_PCSLimitDetail>(),
                pinDetails = new t_pinDetails {status = (t_pinStatus)_PCSViewModel.PinDetailsSelectedIndex }

            };

            var myEnumerator = _PCSViewModel.PCSContainer.GetEnumerator();
            while (myEnumerator.MoveNext()) 
            {
                resp.limits.Add(
                    new t_PCSLimitDetail() 
                    {
                    type=myEnumerator.Current.type,
                    threshold=myEnumerator.Current.threshold,
                    period=myEnumerator.Current.period,
                    currentValue=myEnumerator.Current.currentValue,
                    limitReached=myEnumerator.Current.limitReached,
                    });
            }

            _responseViewModel.LogRpc(nameof(StartPCSSessionResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }
    }
}
