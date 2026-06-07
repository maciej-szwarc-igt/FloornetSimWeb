using System;
using System.Collections.Generic;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Reg;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg
{
    public class RegistrationViewModel
    {
        private RelayCommand _clearCommand { get; set; }
        private RelayCommand _setRegistrationClearCommand { get; set; }
        private RelayCommand _getRegistration { get; set; }
        private RelayCommand _setRegistration { get; set; }
        private readonly ResponseViewModel _responseViewModel;
        private readonly iReg _iRegProxy;

        public SetRegistrationModel setRegistrationModel { get; set; } = new SetRegistrationModel();

        public RegistrationViewModel(iReg regProxy, ResponseViewModel responseViewModel)
        {
            _iRegProxy = regProxy;
            _responseViewModel = responseViewModel;
        }

        public RelayCommand GetRegistrationCommand
        {
            get
            {
                _getRegistration = new RelayCommand(
                    RPCGetRegistration,
                    param => !string.IsNullOrEmpty(RegistrationModel.uId)
                );

                return _getRegistration;
            }
        }
        public RelayCommand SetRegistrationCommand
        {
            get
            {
                _setRegistration = new RelayCommand(
                    RPCSetRegistration,
                    param => !string.IsNullOrEmpty(setRegistrationModel.uId)
                );

                return _setRegistration;
            }
        }

        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                     param => !string.IsNullOrEmpty(RegistrationModel.uId)
                );

                return _clearCommand;
            }
        }
        public RelayCommand setRegistrationClearCommand
        {
            get
            {
                _setRegistrationClearCommand = new RelayCommand(
                    setRegistrationClear,
                     param => !string.IsNullOrEmpty(setRegistrationModel.uId)
                );

                return _setRegistrationClearCommand;
            }
        }

        public void Clear(object obj)
        {
            RegistrationModel.Clear();
        }
        public void setRegistrationClear(object obj)
        {
            setRegistrationModel.Clear();
        }


        public RegistrationModel RegistrationModel { get; } = new RegistrationModel();

        private async void RPCGetRegistration(object obj)
        {
            var req = new Dictionary<string, object> { };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(RegistrationModel.uId);

            var resp = await _iRegProxy.getRegistration();

            _responseViewModel.LogRpcResponse(Constants.GetRegistration, req,resp,RpcCallContext.Current);          

        }
        private async void RPCSetRegistration(object obj)
        {

            bool enabled = setRegistrationModel.Enable;
            bool registered = setRegistrationModel.Registered;
            string notRegisteredReason = setRegistrationModel.NotRegisteredReason;
            bool vip = setRegistrationModel.Vip;
            long machineNum = setRegistrationModel.MachineNum;
            string machineLoc = setRegistrationModel.MachineLoc;
            string siteId = setRegistrationModel.SiteId;
            long reportDenomId = setRegistrationModel.ReportDenomId;
            long pointsCount = setRegistrationModel.PointsCount;
            long pointsAward = setRegistrationModel.PointsAward;
            char machineStatus = Convert.ToChar(setRegistrationModel.MachineStatusesSelected);
            bool haveInitialMeters = setRegistrationModel.HaveInitialMeters;
            bool titoEnabled = setRegistrationModel.TitoEnabled;
            bool truePlayerWinEnabled = setRegistrationModel.TruePlayerWinEnabled;
            bool mdmgEnabled = setRegistrationModel.MdmgEnabled;
            long hotPlayerPeriod = setRegistrationModel.HotPlayerPeriod;
            long hotPlayerWagers = setRegistrationModel.HotPlayerWagers;
            long hotPlayerGames = setRegistrationModel.HotPlayerGames;
            long hotPlayerInactivityTimer = setRegistrationModel.HotPlayerInactivityTimer;
            bool bonusEnabled = setRegistrationModel.BonusEnabled;

            var req = new Dictionary<string, object> {
                {nameof(enabled), enabled},
                {nameof(registered), registered},
                {nameof(notRegisteredReason), notRegisteredReason},
                {nameof(vip), vip},
                {nameof(machineNum), machineNum},
                {nameof(machineLoc), machineLoc},
                {nameof(siteId), siteId},
                {nameof(reportDenomId), reportDenomId},
                {nameof(pointsCount), pointsCount},
                {nameof(pointsAward), pointsAward},
                {nameof(machineStatus), machineStatus},
                {nameof(haveInitialMeters), haveInitialMeters},
                {nameof(titoEnabled), titoEnabled},
                {nameof(truePlayerWinEnabled), truePlayerWinEnabled},
                {nameof(mdmgEnabled), mdmgEnabled},
                {nameof(hotPlayerPeriod), hotPlayerPeriod},
                {nameof(hotPlayerWagers), hotPlayerWagers},
                {nameof(hotPlayerGames), hotPlayerGames},
                {nameof(hotPlayerInactivityTimer), hotPlayerInactivityTimer},
                {nameof(bonusEnabled), bonusEnabled}
            };

            RpcProxyContext.Current = RpcProxyContext.ToSMIB(setRegistrationModel.uId);

            var resp = await _iRegProxy.setRegistration(enabled,
                                                        registered,
                                                        notRegisteredReason,
                                                        vip,
                                                        machineNum,
                                                        machineLoc,
                                                        siteId,
                                                        reportDenomId,
                                                        pointsCount,
                                                        pointsAward,
                                                        machineStatus,
                                                        haveInitialMeters,
                                                        titoEnabled,
                                                        truePlayerWinEnabled,
                                                        mdmgEnabled,
                                                        hotPlayerPeriod,
                                                        hotPlayerWagers,
                                                        hotPlayerGames,
                                                        hotPlayerInactivityTimer,
                                                        bonusEnabled);

            _responseViewModel.LogRpcResponse(Constants.SetRegistration, req,resp,RpcCallContext.Current);          

        }
    }
}
