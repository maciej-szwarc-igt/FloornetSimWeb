using IGT.FloorNet.EX.Diagnostics;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Marker;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;
using System.Collections.Generic;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Marker
{
    public class MarkerViewModel
    {
        private RelayCommand _sendResponseCommand;
        public GetMarkerBalanceResp GetMarkerBalanceResp { get; set; } = new GetMarkerBalanceResp();

        private readonly ResponseViewModel _responseViewModel;

        public MarkerViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
        }

        public RelayCommand SendResponseCommand
        {
            get
            {
                _sendResponseCommand = new RelayCommand(
                    SendResponse,
                    param => true);
                return _sendResponseCommand;
            }
        }

        private async void SendResponse(object obj)
        {
            GetMarkerBalanceResp.ProceedResponse = true;
        }

        public void ResponseSent()
        {
            GetMarkerBalanceResp.ProceedResponse = false;
        }
    }
}