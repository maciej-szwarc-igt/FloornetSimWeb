using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.SecurityUtils;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Discovery;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Discovery
{
    public class DiscoveryViewModel
    {
        private RelayCommand _cardlessCommand;
        private RelayCommand _eftCommand;
        private RelayCommand _bonusCommand;
        private RelayCommand _diagsCommand;
        private RelayCommand _gatCommand;
        private RelayCommand _titoCommand;
        private RelayCommand _pinCommand;
        private RelayCommand _markerCommand;
        private RelayCommand _RGCommand;
        private RelayCommand _RegCommand;
        private RelayCommand _getAllServiceInterfaceCommand;
        private RelayCommand _getServiceInterfaceCommand;
        private RelayCommand _clearServiceInterfaceCommand;
        private RelayCommand _playerCommand;
        private RelayCommand _handpayCommand;
        private RelayCommand _PCSCommand;
        private RelayCommand _watCommand;

        string _interfaceName = string.Empty;
        string _diags = string.Empty;
        string _tito = string.Empty;
        string _pin = string.Empty;
        string _marker = string.Empty;
        string _rg = string.Empty;

        private bool _rpcProcess = true;

        public ResponseModel Model { get; } = new ResponseModel();

        public DiscoveryModel DiscoveryModel { get; set; } = new DiscoveryModel();

        public bool RpcProcess
        {
            get => _rpcProcess;
            set
            {
                _rpcProcess = value;
                if (_rpcProcess)
                    Model.Response = string.Empty;
            }
        }

        private object JsonResponse(object resp)
        {
            var json = JsonConvert.SerializeObject(resp);
            return JToken.Parse(json).ToString(Formatting.Indented);
        }
        #region Discovery private methods

        public RelayCommand CardlessCommand
        {
            get
            {
                _cardlessCommand = new RelayCommand(
                    IsCardlessChecked,
                    param => true);
                return _cardlessCommand;
            }
        }
        public RelayCommand EftCommand
        {
            get
            {
                _eftCommand = new RelayCommand(
                    IsEftChecked,
                    param => true);
                return _eftCommand;
            }
        }
        public RelayCommand PlayerCommand
        {
            get
            {
                _playerCommand = new RelayCommand(
                    IsPlayerChecked,
                    param => true);
                return _playerCommand;
            }
        }
        public RelayCommand BonusCommand
        {
            get
            {
                _bonusCommand = new RelayCommand(
                    IsBonusChecked,
                    param => true);
                return _bonusCommand;
            }
        }
        public RelayCommand DiagsCommand
        {
            get
            {
                _diagsCommand = new RelayCommand(
                    IsDiagsChecked,
                    param => true) ;
                return _diagsCommand;
            }
        }
        public RelayCommand GatCommand
        {
            get
            {
                _gatCommand = new RelayCommand(
                    IsGatChecked,
                    param => true);
                return _gatCommand;
            }
        }
        public RelayCommand ConfCommand
        {
            get
            {
                _gatCommand = new RelayCommand(
                    IsGatChecked,
                    param => true);
                return _gatCommand;
            }
        }

        public RelayCommand TitoCommand
        {
            get
            {
                _titoCommand = new RelayCommand(
                    IsTitoChecked,
                    param => true);
                return _titoCommand;
            }
        }
        public RelayCommand PinCommand
        {
            get
            {
                _pinCommand = new RelayCommand(
                    IsPinChecked,
                    param => true);
                return _pinCommand;
            }
        }
        public RelayCommand MarkerCommand
        {
            get
            {
                _markerCommand = new RelayCommand(
                    IsMarkerChecked,
                    param => true);
                return _markerCommand;
            }            
        }
        public RelayCommand RGCommand
        {
            get
            {
                _RGCommand = new RelayCommand(
                    IsRGChecked,
                    param => true);
                return _RGCommand;
            }
        }

        public RelayCommand RegCommand
        {
            get
            {
                _RegCommand = new RelayCommand(
                    IsRGChecked,
                    param => true);
                return _RegCommand;
            }
        }
        public RelayCommand HandpayCommand
        {
            get
            {
                _handpayCommand = new RelayCommand(
                    IsHandpayChecked,
                    param => true);
                return _handpayCommand;
            }
        }
        public RelayCommand GetAllServiceInterfaceCommand
        {
            get
            {
                _getAllServiceInterfaceCommand = new RelayCommand(
                    AllServiceInterface,
                    param => true
                );

                return _getAllServiceInterfaceCommand;
            }
        }
        public RelayCommand GetServiceInterfaceCommand
        {
            get
            {
                _getServiceInterfaceCommand = new RelayCommand(
                    ServiceInterface,
                    param => true
                );

                return _getServiceInterfaceCommand;
            }
        }
        public RelayCommand ClearServiceInterfaceCommand
        {
            get
            {
                _clearServiceInterfaceCommand = new RelayCommand(
                    ClearServiceInterface,
                    param => true);
                return _clearServiceInterfaceCommand;
            }
        }

        public RelayCommand PCSCommand
        {
            get
            {
                _PCSCommand = new RelayCommand(
                    IsPCSChecked,
                    param => true);
                return _PCSCommand;
            }
        }
        public RelayCommand WatCommand
        {
            get
            {
                _watCommand = new RelayCommand(
                    IsWatChecked,
                    param => true);
                return _watCommand;
            }
        }
        private void IsCardlessChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsEftChecked(object obj)
        {
            RefreshCommandBound();
        }     
        private void IsPlayerChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsBonusChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsDiagsChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsGatChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsTitoChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsPinChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsMarkerChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsRGChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsHandpayChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsPCSChecked(object obj)
        {
            RefreshCommandBound();
        }
        private void IsWatChecked(object obj)
        {
            RefreshCommandBound();
        }

        private void RefreshCommandBound()
        {
            // CommandManager removed (no WPF)
        }

        private void ClearServiceInterface(object obj)
        {
            DiscoveryModel?.Clear();
        }

        private void ServiceInterface(object obj)
        {
            Model.Response = string.Empty;
            Model.Response = $"{Model.Response}{Environment.NewLine}{Constants.Request.ToUpper()}";
            _interfaceName = DiscoveryModel?.GetActiveInterfaces();
            GetServiceInterface(_interfaceName);
        }

        private void GetServiceInterface(string InterfaceName)
        {
            if (!RpcProcess) return;            
            List<string> interfaceList = InterfaceName.Split(',').ToList();
            interfaceList = interfaceList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            RPCGetServiceInterfaces(interfaceList);
        }       

        private async void RPCGetServiceInterfaces(List<string> interfaces)
        {
            var resp = await Startup.discovery.getServiceInterfaces(interfaces);

            // Response
            Model.Response = string.Empty;
            Model.Response = resp != null
                ? $"{Model.Response}{Environment.NewLine}{Constants.Response.ToUpper()} :{Environment.NewLine}{JsonResponse(resp)}"
                : $"{Model.Response}{Environment.NewLine}{Constants.Response.ToUpper()} :{Environment.NewLine}{Constants.NoResponse.ToUpper()}";
            _interfaceName = string.Empty;
        }
        private void AllServiceInterface(Object obj)
        {
            Model.Response = string.Empty;
            Model.Response = $"{Model.Response}{Environment.NewLine}{Constants.Request.ToUpper()}";
            GetAllServiceInterface();
        }

        private void GetAllServiceInterface()
        {
            if (!RpcProcess) return;
            RPCGetAllServiceInterface();
        }
        private async void RPCGetAllServiceInterface()
        {
            var resp = await Startup.discovery.getAllServiceInterfaces();

            // Response
            Model.Response = string.Empty;
            Model.Response = resp != null
                ? $"{Model.Response}{Environment.NewLine}{Constants.Response.ToUpper()} :{Environment.NewLine}{JsonResponse(resp)}"
                : $"{Model.Response}{Environment.NewLine}{Constants.Response.ToUpper()} :{Environment.NewLine}{Constants.NoResponse.ToUpper()}";
        }

        #endregion Discovery private methods
    }
}
