using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.Handpay;
using IGT.FloorNet.EX.Player;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.HandPay;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class HandpayRPCProvider : iHandpay
    {

        private readonly ResponseViewModel _responseViewModel;
        public HandpayRPCProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }


        public async Task<getPublicKeyResp> getPublicKey()
        {
            if (!IKeysViewModel.keysModelHandpay.RespondToRPC)
                return await Task.FromResult<getPublicKeyResp>(null);

            var req = new Dictionary<string, object>
            {

            };
            var resp = new getPublicKeyResp()
            {
                currentKeyNumber = IKeysViewModel.keysModelHandpay.CurrentKeyNum,
                currentKey = IKeysViewModel.keysModelHandpay.PublicKey,
                currentKeyExpireDate = DateTime.Now.AddDays(30),
                previousKeyOverlapPeriod = 5000
            };

            _responseViewModel.LogRpc(nameof(getPublicKey), req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }

        public Task<handpayResp> handpay(string requestId, char type, long amount, long hopperPaidAmount, DateTime dateTime, bool keyToCreditAvailable, long gameWin, long progWin, long bonusWin, long levelId, long hitSeqNum, long coinsBet, long wagerDenom, string cardId, long playerId, long[,] meters, string signature)
        {
            if (!HandPayViewModel.HandPayResponseModel.RespondToRPC)
                return Task.FromResult<handpayResp>(null);

            string Ridentity = HandPayViewModel.HandPayResponseModel.Identity;
            bool RpouchPayEnable = HandPayViewModel.HandPayResponseModel.PouchPayEnable;
            bool RkeyToCreditEnable = HandPayViewModel.HandPayResponseModel.KeyToCreditEnable;
            t_selfServeOption RselfServeOption = HandPayViewModel.HandPayResponseModel.SelfServeOption;

            bool isSameSignature = IKeysViewModel.keysModelHandpay.FloornetECDsaProvider.VerifySignature(
                    IKeysViewModel.keysModelHandpay.PublicKey,
                    signature,
                    requestId,
                    type,
                    amount,
                    hopperPaidAmount,
                    dateTime,
                    keyToCreditAvailable,
                    gameWin,
                    progWin,
                    bonusWin,
                    levelId,
                    hitSeqNum,
                    coinsBet,
                    wagerDenom,
                    cardId,
                    playerId,
                    meters
                    );

            var req = new Dictionary<string, object>
             {
                 {nameof(requestId), requestId },
                 {nameof(type), type },
                 {nameof(amount),  amount},
                 {nameof(hopperPaidAmount),  hopperPaidAmount},
                 {nameof(dateTime),  dateTime},
                 {nameof(keyToCreditAvailable),  keyToCreditAvailable},
                 {nameof(gameWin),  gameWin},
                 {nameof(progWin),  progWin},
                 {nameof(bonusWin),  bonusWin},
                 {nameof(levelId),  levelId},
                 {nameof(hitSeqNum),  hitSeqNum},
                 {nameof(coinsBet),  coinsBet},
                 {nameof(wagerDenom),  wagerDenom},
                 {nameof(cardId),  cardId},
                 {nameof(playerId),  playerId},
                 {nameof(meters),  meters},
                 {nameof(signature),  signature}
             };

            if (!isSameSignature)
            {
                Ridentity = "";
                RpouchPayEnable = false;
                RkeyToCreditEnable = false;
                RselfServeOption = t_selfServeOption.none;
                _responseViewModel.Log("\b**********Signature Fails*********");
            }

            var resp = new handpayResp()
            {
                identity = Ridentity,
                pouchPayEnable = RpouchPayEnable,
                keyToCreditEnable = RkeyToCreditEnable,
                selfServeOption = RselfServeOption
            };

            HandPayViewModel.HandPayResponseModel.IncrementIdentityPK();//Since is a primaryKey, I'll fake this incrementing this value

            _responseViewModel.LogRpc(nameof(handpayResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<keyedOffResp> keyedOff(string requestId, char type, long amount, DateTime dateTime, string cardId, bool keyToCredit, bool canceled, string identity, long playerId, string signature)
        {
            if (!KeyedOffViewModel.KeyedOffModel.RespondToRPC)
                return Task.FromResult<keyedOffResp>(null);

            t_result Rresult = KeyedOffViewModel.KeyedOffModel.Result;
            string Rmessage = KeyedOffViewModel.KeyedOffModel.Message;
            long Rprogress = KeyedOffViewModel.KeyedOffModel.Progress;
            string Rfunction = KeyedOffViewModel.KeyedOffModel.Function;
            long RrequestId = KeyedOffViewModel.KeyedOffModel.RequestId;
            string RrequestIdStr = KeyedOffViewModel.KeyedOffModel.RequestIdStr;
            long? sequence = KeyedOffViewModel.KeyedOffModel.Sequence;

            bool isSameSignature = IKeysViewModel.keysModelHandpay.FloornetECDsaProvider.VerifySignature(
                IKeysViewModel.keysModelHandpay.PublicKey,
                signature,
                requestId,
                type,
                amount,
                dateTime,
                cardId,
                keyToCredit,
                canceled,
                identity,
                playerId
                );


            var req = new Dictionary<string, object>
             {
                 {nameof(requestId), requestId },
                 {nameof(type), type },
                 {nameof(amount),  amount},
                 {nameof(dateTime),  dateTime},
                 {nameof(cardId),  cardId},
                 {nameof(keyToCredit),  keyToCredit},
                 {nameof(canceled),  canceled},
                 {nameof(identity),  identity},
                 {nameof(playerId),  playerId},
                 {nameof(signature),  signature }
             };

            if (!KeyedOffViewModel.KeyedOffModel.SendSequence || !isSameSignature)
            {
                sequence = null;
            }

            if (!isSameSignature)
            {
                Rresult = t_result.failed;
                Rmessage = "Signature Fail";
                Rprogress = 0;
                Rfunction = "iHandPay.KeyedOff";
                RrequestId = KeyedOffViewModel.KeyedOffModel.RequestId;
                RrequestIdStr = "";
            }

            var resp = new keyedOffResp()
            {
                result = Rresult,
                message = Rmessage,
                progress = Rprogress,
                function = Rfunction,
                requestId = RrequestId,
                requestIdStr = RrequestIdStr,
                sequence = sequence
            };

            _responseViewModel.LogRpc(nameof(keyedOffResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }
    }
}
