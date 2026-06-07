using IGT.FloorNet.EX.Diagnostics;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Diags;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Diags
{
    public class DiagsViewModel
    {
        private readonly iDiags _diagsProxy;
        private RelayCommand _getDiagnosticCommand;
        private RelayCommand _resetCommand;
        private RelayCommand _clearResetCommand;
        public Diagnostics Diagnostics { get; set; } = new Diagnostics();
        public Reset Reset { get; set; } = new Reset();
        private readonly ResponseViewModel _responseViewModel;

        public DiagsViewModel(iDiags diagsProxy, ResponseViewModel responseViewModel)
        {
            _diagsProxy = diagsProxy;
            _responseViewModel = responseViewModel;
        }

        public RelayCommand GetDiagnosticCommand
        {
            get
            {
                _getDiagnosticCommand = new RelayCommand(
                    RpcGetDiagnostic,
                    param => true);
                return _getDiagnosticCommand;
            }
        }

        private async void RpcGetDiagnostic(object obj)
        {
            try
            {
                RpcProxyContext.Current = Diagnostics.SendToSMIB ? RpcProxyContext.ToSMIB(Diagnostics.Uid) : RpcProxyContext.ToService(Diagnostics.ServiceName, Diagnostics.Uid);

                var response = await _diagsProxy.diagnostics();

                if (response == null) throw new Exception("Null response from Sevice instance");

                _responseViewModel.LogRpc("diagnostics", $"Uid: {Diagnostics.Uid}. Service Name: {Diagnostics.ServiceName}.", response, RpcCallContext.Current);
            }
            catch (Exception e)
            {
                _responseViewModel.LogRpc("diagnostics", $"Uid: {Diagnostics.Uid}. Service Name: {Diagnostics.ServiceName}.", $"Get Diagnostics Exception: {e.Message}", RpcCallContext.Current);
            }
        }

        public RelayCommand ClearDiagnosticsCommand
        {
            get
            {
                _clearResetCommand = new RelayCommand(
                    ClearDiagnostics,
                    param => true);
                return _clearResetCommand;
            }
        }

        private void ClearDiagnostics(object obj)
        {
            Diagnostics.Uid = string.Empty;
            Diagnostics.ServiceName = string.Empty;
        }

        public RelayCommand ResetCommand
        {
            get
            {
                _resetCommand = new RelayCommand(
                    RpcReset,
                    param => true);
                return _resetCommand;
            }
        }

        private async void RpcReset(object obj)
        {
            try
            {
                t_resetTypes resetType = Reset.ResetHard == "true" ? t_resetTypes.hard : t_resetTypes.soft;

                RpcProxyContext.Current = Reset.SendToSMIB ? RpcProxyContext.ToSMIB(Reset.ResetUid) : RpcProxyContext.ToService(Reset.ServiceName, Reset.ResetUid);

                var response = await _diagsProxy.reset(resetType, 0);

                _responseViewModel.LogRpc("reset", $"Uid: {Reset.ResetUid}. Service Name: {Reset.ServiceName}. Reset Type: {resetType}.", response, RpcCallContext.Current);
            }
            catch (Exception e)
            {
                _responseViewModel.LogRpc("reset", $"Uid: {Reset.ResetUid}. Service Name: {Reset.ServiceName}. Hard: {Reset.ResetHard}.", $"Reset Exception: {e.Message}", RpcCallContext.Current);
            }
        }

        public RelayCommand ClearResetCommand
        {
            get
            {
                _clearResetCommand = new RelayCommand(
                    ClearReset,
                    param => true);
                return _clearResetCommand;
            }
        }

        private void ClearReset(object obj)
        {
            Reset.ResetUid = string.Empty;
            Reset.ResetHard = "false";
            Reset.ServiceName = string.Empty;
        }
    }
}