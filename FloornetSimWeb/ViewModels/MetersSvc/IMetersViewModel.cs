using IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.EX.Meters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.EX.Gat;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Gat;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.MetersSvc
{
    public class IMetersViewModel
    {
        private readonly iMeters _iMetersProxy;
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand SendGetMetersCommand { get; }
        public RelayCommand SendGetMgaDescriptionsCommand { get; }

        public IMetersModel IMetersModel { get; } = new IMetersModel();
        public IMetersViewModel(iMeters iMetersProxy, ResponseViewModel responseViewModel)
        {
            _iMetersProxy = iMetersProxy;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            SendGetMetersCommand = new RelayCommand(
                SendGetMeters,
                param => !string.IsNullOrEmpty(IMetersModel.GetMetersUID)
            );
            SendGetMgaDescriptionsCommand = new RelayCommand(
                SendGetMgaDescriptions,
                param => !string.IsNullOrEmpty(IMetersModel.GetMgaDescriptionsUID)
            );
        }
        public void Clear(object obj)
        {
            IMetersModel.Clear();
        }

        private async void SendGetMeters(object obj)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long meterTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;

            var req = new Dictionary<string, object>
            {
                {nameof(IMetersModel.MeterType), (char)IMetersModel.MeterType },
                {"MeterTime", meterTime }
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(IMetersModel.GetMetersUID);

            var resp = await _iMetersProxy.getMeters(
                IMetersModel.MeterType,
                meterTime);

            _responseViewModel.LogRpcResponse(Constants.GetMeters, req, resp, RpcCallContext.Current);
        }
        private async void SendGetMgaDescriptions(object obj)
        {
            var req = new Dictionary<string, object>{};

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(IMetersModel.GetMgaDescriptionsUID);

            var resp = await _iMetersProxy.getMgaDescriptions();

            _responseViewModel.LogRpcResponse(Constants.GetMgaDescriptions, req, resp, RpcCallContext.Current);
        }
    }
}
