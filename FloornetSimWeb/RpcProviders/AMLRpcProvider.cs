using IGT.FloorNet.EX.Cardless;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.AML;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Cardless;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Discovery;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class AMLRpcProvider : iAML
    {

        private readonly AMLViewModel _AMLViewModel;

        private readonly ResponseViewModel _responseViewModel;

        public AMLRpcProvider(AMLViewModel amlViewModel, ResponseViewModel responseViewModel)
        {
            _AMLViewModel = amlViewModel;
            _responseViewModel = responseViewModel;
        }

        private string GetEGMID()
        {
            string EGMUid = "IGT_";
            if (RpcCallContext.Current != null)
                EGMUid += RpcCallContext.Current.Uid;

            return EGMUid;
        }

        public Task<CashAcceptedResp> CashAccepted(long playerId, long billDenom)
        {
            var req = new Dictionary<string, object>
            {
                { "uid", GetEGMID() },
                { "playerId", playerId },
                { "billDenom", billDenom },

            };

            _AMLViewModel.PlayerId = playerId;
            _AMLViewModel.Uid = GetEGMID();
            _AMLViewModel.BillDenom = billDenom;
            _AMLViewModel.DailyCashAggregate += billDenom;
            var resp = new CashAcceptedResp()
            {
                largestBillDenom = _AMLViewModel.LargestBillDenom,
                dailyCashLimit = _AMLViewModel.DailyCashLimit ,
                dailyCashAggregate = _AMLViewModel.DailyCashAggregate,

            };

            _responseViewModel.LogRpc("CashAccepted", req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }

        public Task<GetPlayerLimitsResp> GetPlayerLimits(long playerId)
        {
            var req = new Dictionary<string, object>
            {
                { "uid", GetEGMID() },
                { "playerId", playerId },
            };

            _AMLViewModel.PlayerId = playerId;
            _AMLViewModel.Uid = GetEGMID();
            _AMLViewModel.BillDenom = 0;
            var resp = new GetPlayerLimitsResp()
            {
                largestBillDenom = _AMLViewModel.LargestBillDenom,
                dailyCashLimit = _AMLViewModel.DailyCashLimit,
                dailyCashAggregate = _AMLViewModel.DailyCashAggregate,

            };
            
            _responseViewModel.LogRpc("GetPlayerLimits", req, resp, RpcCallContext.Current);
            return Task.FromResult(resp);
        }
        

    }
}
