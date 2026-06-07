using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Reg;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg
{
    public class GetServiceRegViewModel
    {
        private readonly ResponseViewModel _responseViewModel;
        private readonly iSvcReg _iSvcReg;
        public GetServiceRegModel GetServiceRegModel { get; } = new GetServiceRegModel();
        private RelayCommand _getServiceRegCommand { get; set; }

        private RelayCommand _clearGetServiceRegCommand { get; set; }

        public GetServiceRegViewModel(iSvcReg svcReg, ResponseViewModel responseViewModel)
        {
            _iSvcReg = svcReg;
            _responseViewModel = responseViewModel;
        }

        public RelayCommand GetServiceRegCommand
        {
            get
            {
                _getServiceRegCommand = new RelayCommand(
                    GetServiceReg,
                    param => GetServiceRegModel.IsValid()
                );

                return _getServiceRegCommand;
            }
        }

        public RelayCommand ClearGetServiceRegCommand
        {
            get
            {
                _clearGetServiceRegCommand = new RelayCommand(
                    ClearGetServiceReg,
                    param => true
                );
                return _clearGetServiceRegCommand;
            }
        }

        private void GetServiceReg(object obj)
        {
            RPCGetServiceReg();
        }

        private async void RPCGetServiceReg()
        {
            RpcProxyContext.Current = RpcProxyContext.ToService(GetServiceRegModel.ServiceName, GetServiceRegModel.UId);

            var resp = await _iSvcReg.GetServiceReg();

            _responseViewModel.LogRpcResponse("GetServiceReg", _iSvcReg, resp, RpcCallContext.Current);
        }

        private void ClearGetServiceReg(object obj)
        {
            GetServiceRegModel.Clear();
        }
    }
}
