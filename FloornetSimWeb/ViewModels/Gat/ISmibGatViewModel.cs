using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using IGT.FloorNet.EX.Gat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Gat;
using System.Runtime.ConstrainedExecution;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.MessageBus.Rpc;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Gat
{
    public class ISmibGatViewModel
    {
        private readonly iSmibGat _iSmibGatProxy;
        private readonly ResponseViewModel _responseViewModel;
        public RelayCommand ClearCommand { get; }
        public RelayCommand SendDoVerificationCommand { get; }
        public RelayCommand SendGetPackageListCommand { get; }

        public ISmibGatModel ISmibGatModel { get; } = new ISmibGatModel();
        public ISmibGatViewModel(iSmibGat iSmibGatProxy, ResponseViewModel responseViewModel)
        {
            _iSmibGatProxy = iSmibGatProxy;
            _responseViewModel = responseViewModel;
            ClearCommand = new RelayCommand(Clear);
            SendDoVerificationCommand = new RelayCommand(
                SendDoVerification,
                param => !string.IsNullOrEmpty(ISmibGatModel.DoVerificationUID)
            );
            SendGetPackageListCommand = new RelayCommand(
                SendGetPackageList,
                param => !string.IsNullOrEmpty(ISmibGatModel.GetPackageListUID)
            );
        }

        private async void SendDoVerification(object obj)
        {
            List<t_verifyComponent> verifyComponentList = new List<t_verifyComponent>();
            if (ISmibGatModel.SendVerifyComponentData1)
                verifyComponentList.Add(new t_verifyComponent() {
                    name = ISmibGatModel.Name1,
                    isHardware = ISmibGatModel.IsHardware1,
                    algorithm = ISmibGatModel.Algorithm1,
                    seed = ISmibGatModel.Seed1,
                    salt = ISmibGatModel.Salt1,
                    offset = ISmibGatModel.Offset1
                });
            if (ISmibGatModel.SendVerifyComponentData2)
                verifyComponentList.Add(new t_verifyComponent()
                {
                    name = ISmibGatModel.Name2,
                    isHardware = ISmibGatModel.IsHardware2,
                    algorithm = ISmibGatModel.Algorithm2,
                    seed = ISmibGatModel.Seed2,
                    salt = ISmibGatModel.Salt2,
                    offset = ISmibGatModel.Offset2
                });
            if (ISmibGatModel.SendVerifyComponentData3)
                verifyComponentList.Add(new t_verifyComponent()
                {
                    name = ISmibGatModel.Name3,
                    isHardware = ISmibGatModel.IsHardware3,
                    algorithm = ISmibGatModel.Algorithm3,
                    seed = ISmibGatModel.Seed3,
                    salt = ISmibGatModel.Salt3,
                    offset = ISmibGatModel.Offset3
                });

            var req = new Dictionary<string, object>
            {
                {nameof(ISmibGatModel.RequestId), ISmibGatModel.RequestId },
                {nameof(verifyComponentList), verifyComponentList }
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(ISmibGatModel.DoVerificationUID);

            var resp = await _iSmibGatProxy.doVerification(
                ISmibGatModel.RequestId,
                verifyComponentList);

            _responseViewModel.LogRpcResponse(Constants.DoVerification, req, resp, RpcCallContext.Current);
        }

        private async void SendGetPackageList(object obj)
        {
            var req = new Dictionary<string, object>{};

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(ISmibGatModel.GetPackageListUID);

            var resp = await _iSmibGatProxy.getPackageList();

            _responseViewModel.LogRpcResponse(Constants.GetPackageList, req, resp, RpcCallContext.Current);
        }

        public void Clear(object obj)
        {
            ISmibGatModel.Clear();
        }
    }
}
