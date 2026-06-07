using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.Player;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Card.iSmibPlr;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class PlayerRPCProvider : iPlayer
    {

        private readonly ResponseViewModel _responseViewModel;
        public PlayerRPCProvider(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public Task<cardInResp> cardIn(string cardId, t_idReaderType idReaderType, long cardInCount, DateTime startTime, bool isCardUnregistered, bool xcTransEnabled)
        {

            if (!CardInViewModel.CardInModel.RespondToRPC)
                return Task.FromResult<cardInResp>(null);

            long playerId = CardInViewModel.CardInModel.PlayerId;
            string firstName = CardInViewModel.CardInModel.FirstName;
            t_cardType cardType = CardInViewModel.CardInModel.CardType;

            var req = new Dictionary<string, object>
            {
                {nameof(cardId), cardId },
                {nameof(idReaderType), idReaderType },
                {nameof(cardInCount),  cardInCount},
                {nameof(startTime),  startTime},
                {nameof(isCardUnregistered),  isCardUnregistered },
                {nameof(xcTransEnabled),  xcTransEnabled }
            };

            if (CardInViewModel.CardInModel.IsUnknownPlayer)
            {
                firstName = "";
                cardType = t_cardType.invalid;
            }

            if (CardInViewModel.CardInModel.IsUnknownPlayer || CardInViewModel.CardInModel.IsEmployee)
            {
                playerId = 0;
            }

            var resp = new cardInResp()
            {
                cardId = cardId,
                cardInCount = cardInCount,
                cardType = cardType,
                playerId = playerId,
                firstName = firstName
            };

            CardViewModel.forceCardInModel.CardIdFromCardInRPC = cardId;

            CardViewModel.forceCardOutModel.CardIdFromCardInRPC = cardId;
            CardViewModel.forceCardOutModel.CardInCountFromCardInRPC = cardInCount;

            CardViewModel.employeeCardDataModel.CardId = cardId;
            CardViewModel.employeeCardDataModel.IdReaderType = idReaderType;
            CardViewModel.employeeCardDataModel.CardInCount = cardInCount;


            CardViewModel.playerCardDataModel.CardId = cardId;
            CardViewModel.playerCardDataModel.IdReaderType = idReaderType;
            CardViewModel.playerCardDataModel.CardInCount = cardInCount;

            _responseViewModel.LogRpc(nameof(cardInResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<employeeCardOutResp> employeeCardOut(string cardId, t_idReaderType idReaderType, long cardInCount, DateTime startTime, DateTime endTime)
        {
            if (!EmployeeCardOutViewModel.EmployeeCardOutModel.RespondToRPC)
                return Task.FromResult<employeeCardOutResp>(null);

            long? sequence = EmployeeCardOutViewModel.EmployeeCardOutModel.Sequence;

            var req = new Dictionary<string, object>
            {
                {nameof(cardId), cardId },
                {nameof(idReaderType), idReaderType },
                {nameof(cardInCount),  cardInCount},
                {nameof(startTime),  startTime},
                {nameof(endTime),  endTime }
            };

            if (!EmployeeCardOutViewModel.EmployeeCardOutModel.SendSequence)
            {
                sequence = null;
            }

            var resp = new employeeCardOutResp()
            {
                result = EmployeeCardOutViewModel.EmployeeCardOutModel.Result,
                message = EmployeeCardOutViewModel.EmployeeCardOutModel.Message,
                progress = EmployeeCardOutViewModel.EmployeeCardOutModel.Progress,
                function = EmployeeCardOutViewModel.EmployeeCardOutModel.Function,
                requestId = EmployeeCardOutViewModel.EmployeeCardOutModel.RequestId,
                requestIdStr = EmployeeCardOutViewModel.EmployeeCardOutModel.RequestIdStr,
                sequence = sequence
            };

            _responseViewModel.LogRpc(nameof(employeeCardOutResp), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
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

        public Task<playerCardOutResp> playerCardOut(string cardId, t_idReaderType idReaderType, long cardInCount, DateTime startTime, DateTime endTime, long pointAward, long carryOver, long coinIn, long coinOut, long jackpotHandpay, long gamesPlayed, long gamesWon, long weightedTheo, long voucherIn, long voucherOut, bool offlinePlayerSession, double spXier, double spXierPoints, double spRankXier, double spRankPoints, long xcUsed, long xcRPEarned, long xcPPEarned, long xcBSEarned, long xcPTPEarned, long xcRREarned, long ppPoolBalance, long ppLuckyNumber, long ppTotalWon, long ptpSPUsed, long ptpSPUsedCents, long rpPointAdjustment, long rpEarnedDay, long srpPointsEarned, bool pinLock, long rrPointsEarned, long xcAdjustments, long ptAdjustments, long totalBills, bool abandoned, string vendorName, long reportDenomId, long basePercentage, bool xcTransUsed, bool isCardUnregistered, string signature)
        {

            if (!PlayerCardOutViewModel.PlayerCardOutModel.RespondToRPC)
                return Task.FromResult<playerCardOutResp>(null);

            long? sequence = PlayerCardOutViewModel.PlayerCardOutModel.Sequence;
            var req = new Dictionary<string, object>
            {
                {nameof(cardId), cardId },
                {nameof(idReaderType), idReaderType },
                {nameof(cardInCount),cardInCount },
                {nameof(startTime),startTime },
                {nameof(endTime),endTime },
                {nameof(pointAward),pointAward },
                {nameof(carryOver),carryOver },
                {nameof(coinIn),coinIn },
                {nameof(coinOut),coinOut },
                {nameof(jackpotHandpay),jackpotHandpay },
                {nameof(gamesPlayed),gamesPlayed },
                {nameof(gamesWon),gamesWon },
                {nameof(weightedTheo),weightedTheo },
                {nameof(voucherIn),voucherIn },
                {nameof(voucherOut),voucherOut },
                {nameof(offlinePlayerSession),offlinePlayerSession },
                {nameof(spXier),spXier },
                {nameof(spXierPoints),spXierPoints },
                {nameof(spRankXier),spRankXier },
                {nameof(spRankPoints),spRankPoints },
                {nameof(xcUsed),xcUsed },
                {nameof(xcRPEarned),xcRPEarned },
                {nameof(xcPPEarned),xcPPEarned },
                {nameof(xcBSEarned),xcBSEarned },
                {nameof(xcPTPEarned),xcPTPEarned },
                {nameof(xcRREarned),xcRREarned },
                {nameof(ppPoolBalance),ppPoolBalance },
                {nameof(ppLuckyNumber),ppLuckyNumber },
                {nameof(ppTotalWon),ppTotalWon },
                {nameof(ptpSPUsed),ptpSPUsed },
                {nameof(ptpSPUsedCents),ptpSPUsedCents },
                {nameof(rpPointAdjustment),rpPointAdjustment },
                {nameof(rpEarnedDay),rpEarnedDay },
                {nameof(srpPointsEarned),srpPointsEarned },
                {nameof(pinLock),pinLock },
                {nameof(rrPointsEarned),rrPointsEarned },
                {nameof(xcAdjustments),xcAdjustments },
                {nameof(ptAdjustments),ptAdjustments },
                {nameof(totalBills),totalBills },
                {nameof(abandoned),abandoned },
                {nameof(vendorName),vendorName },
                {nameof(reportDenomId),reportDenomId },
                {nameof(basePercentage),basePercentage },
                {nameof(xcTransUsed),xcTransUsed },
                {nameof(isCardUnregistered),isCardUnregistered },
                {nameof(signature),signature }
            };

            if(!PlayerCardOutViewModel.PlayerCardOutModel.SendSequence)
            {
                sequence = null;
            }

            var resp = new playerCardOutResp()
            {
                result = PlayerCardOutViewModel.PlayerCardOutModel.Result,
                message = PlayerCardOutViewModel.PlayerCardOutModel.Message,
                progress = PlayerCardOutViewModel.PlayerCardOutModel.Progress,
                function = PlayerCardOutViewModel.PlayerCardOutModel.Function,
                requestId = PlayerCardOutViewModel.PlayerCardOutModel.RequestId,
                requestIdStr = PlayerCardOutViewModel.PlayerCardOutModel.RequestIdStr,
                sequence = sequence
            };

            _responseViewModel.LogRpc(nameof(playerCardOut), req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }
    }
}
