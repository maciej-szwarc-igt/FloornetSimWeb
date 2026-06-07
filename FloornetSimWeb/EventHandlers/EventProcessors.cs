using IGT.FloorNet.EX.evt;
using IGT.FloorNet.EX.OptionConfig;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class AuditEventProcessor
{
    public abstract void Process(auditEvent evt, EventCallContext context);
}

namespace IGT.FloorNet.Tools.ServiceSimulator.EventProcessors
{
    public class RegSmibOnlineHandler : AuditEventProcessor
    {
        private readonly ResponseViewModel _responseViewModel;
        private readonly SmibRegistrationTracker _tracker;

        public RegSmibOnlineHandler(ResponseViewModel responseViewModel, SmibRegistrationTracker tracker)
        {
            _responseViewModel = responseViewModel;
            _tracker = tracker;
        }

        public override void Process(auditEvent evt, EventCallContext context)
        {
            try
            {
                var regData = evt.data as IGT.FloorNet.EX.Registration.evt.registration;
                if (regData != null)
                {
                    IKeysViewModel.SetSmibKey(context.Uid, regData.smib_key ?? "");
                }
            }
            catch(Exception exception)
            {
                _responseViewModel.Log($"RegSmibOnlineHandler::Process exception thrown {exception.ToString()}, could not parse regSmibOnline event.");
            }

            // Track this SMIB as online
            _tracker.MarkOnline(context.Uid);

            // Auto-register the SMIB if auto mode is enabled
            if (Startup.AutoRegister)
            {
                _ = AutoRegisterAndConfigureAsync(context.Uid);
            }
            else
            {
                _responseViewModel.Log($"[Checkin] SMIB {context.Uid} online — auto-checkin disabled, waiting for manual setRegistration");
            }
        }

        private async Task AutoRegisterAndConfigureAsync(string uid)
        {
            try
            {
                var regConfig = Startup.Configuration?.GetSection("Checkin");
                var siteId = regConfig?["SiteId"];
                var machineNum = long.TryParse(regConfig?["MachineNum"], out var mn) ? mn : 0;
                var machineLoc = regConfig?["MachineLoc"];
                var enabled = bool.TryParse(regConfig?["Enabled"], out var e) ? e : true;
                var registered = bool.TryParse(regConfig?["Registered"], out var r) ? r : true;
                var notRegisteredReason = regConfig?["NotRegisteredReason"];
                var vip = bool.TryParse(regConfig?["Vip"], out var v) ? v : false;
                var reportDenomId = long.TryParse(regConfig?["ReportDenomId"], out var rd) ? rd : 0;
                var pointsCount = long.TryParse(regConfig?["PointsCount"], out var pc) ? pc : 0;
                var pointsAward = long.TryParse(regConfig?["PointsAward"], out var pa) ? pa : 0;
                var machineStatus = regConfig?["MachineStatus"] ?? "A";
                var haveInitialMeters = bool.TryParse(regConfig?["HaveInitialMeters"], out var him) ? him : false;
                var titoEnabled = bool.TryParse(regConfig?["TitoEnabled"], out var te) ? te : false;
                var truePlayerWinEnabled = bool.TryParse(regConfig?["TruePlayerWinEnabled"], out var tpw) ? tpw : false;
                var mdmgEnabled = bool.TryParse(regConfig?["MdmgEnabled"], out var me) ? me : false;
                var hotPlayerPeriod = long.TryParse(regConfig?["HotPlayerPeriod"], out var hpp) ? hpp : 0;
                var hotPlayerWagers = long.TryParse(regConfig?["HotPlayerWagers"], out var hpw) ? hpw : 0;
                var hotPlayerGames = long.TryParse(regConfig?["HotPlayerGames"], out var hpg) ? hpg : 0;
                var hotPlayerInactivityTimer = long.TryParse(regConfig?["HotPlayerInactivityTimer"], out var hpit) ? hpit : 0;
                var bonusEnabled = bool.TryParse(regConfig?["BonusEnabled"], out var be) ? be : false;

                // Send setRegistration with the correct SMIB UID
                RpcProxyContext.Current = RpcProxyContext.ToSMIB(uid);
                var resp = await Startup._iReg.setRegistration(
                    enabled,
                    registered,
                    notRegisteredReason,
                    vip,
                    machineNum,
                    machineLoc,
                    siteId,
                    reportDenomId,
                    pointsCount,
                    pointsAward,
                    CheckinDefaultsResolver.ToMachineStatusChar(machineStatus),
                    haveInitialMeters,
                    titoEnabled,
                    truePlayerWinEnabled,
                    mdmgEnabled,
                    hotPlayerPeriod,
                    hotPlayerWagers,
                    hotPlayerGames,
                    hotPlayerInactivityTimer,
                    bonusEnabled
                );
                _responseViewModel.LogRpcResponse($"setRegistration → {uid}", new { target = uid, enabled, registered, machineNum, siteId }, resp, RpcCallContext.Current);
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"[Checkin] Auto-register failed for {uid}: {ex.Message}");
            }

            try
            {
                // Push DISABLE_OL=true to enable FloorNet-only event forwarding
                RpcProxyContext.Current = RpcProxyContext.ToSMIB(uid);
                var categoryOptions = new List<categoryOption>
                {
                    new categoryOption
                    {
                        messageCategory = "EgmSettings",
                        optionItems = new List<t_optionItem>
                        {
                            new t_optionItem { name = "DISABLE_OL", value = true }
                        }
                    }
                };
                var configResp = await Startup._iConfig.setOptionChange(categoryOptions);
                _responseViewModel.LogRpcResponse($"setOptionChange → {uid}", new { target = uid, DISABLE_OL = true }, configResp, RpcCallContext.Current);
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"[Checkin] Failed to push DISABLE_OL config to {uid}: {ex.Message}");
            }
        }
    };

    public class RegSmibOfflineHandler : AuditEventProcessor
    {
        private readonly ResponseViewModel _responseViewModel;
        private readonly SmibRegistrationTracker _tracker;

        public RegSmibOfflineHandler(ResponseViewModel responseViewModel, SmibRegistrationTracker tracker)
        {
            _responseViewModel = responseViewModel;
            _tracker = tracker;
        }

        public override void Process(auditEvent evt, EventCallContext context)
        {
            _tracker.MarkOffline(context.Uid);
            _responseViewModel.Log($"[Registration] SMIB {context.Uid} marked offline (regSmibOffline)");
        }
    };
}
