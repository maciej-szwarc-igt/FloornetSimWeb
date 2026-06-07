using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Marker
{
    public class GetMarkerBalanceResp : ModelBase
    {
        private int _markerBalance { get; set; }
        private int _creditLimit { get; set; }
        private int _statusCode { get; set; }
        private bool _inmediateResponse { get; set; }
        private bool _proceedResponse { get; set; }

        private IEnumerable<int> _statusCodes;

        public GetMarkerBalanceResp()
        {
            _markerBalance = 0;
            _creditLimit = 0;
            _statusCode = 200;
            _inmediateResponse = true;
            _proceedResponse = false;

            _statusCodes = new List<int>() { 200, 400, 401, 404, 501, 503 };
        }

        public int MarkerBalance
        {
            get => _markerBalance;
            set
            {
                _markerBalance = value;
                OnPropertyChanged("MarkerBalance");
            }
        }

        public int CreditLimit
        {
            get => _creditLimit;
            set
            {
                _creditLimit = value;
                OnPropertyChanged("CreditLimit");
            }
        }

        public int StatusCode
        {
            get => _statusCode;
            set
            {
                _statusCode = value;
                OnPropertyChanged("StatusCode");
            }
        }

        public bool InmediateResponse
        {
            get => _inmediateResponse;
            set
            {
                _inmediateResponse = value;
                OnPropertyChanged("InmediateResponse");
            }
        }

        public bool ProceedResponse
        {
            get => _proceedResponse;
            set
            {
                _proceedResponse = value;
                OnPropertyChanged("ProceedResponse");
            }
        }

        public IEnumerable<int> StatusCodes
        {
            get => _statusCodes;
        }
    }
}