using IGT.FloorNet.EX.OptionConfig;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.EX.Registration.evt;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.EventHandlers
{
    public class KeepAliveEventHandler : IBusEventHandler<keepAlive>
    {
        private readonly ResponseViewModel _responseViewModel;
        private readonly SmibRegistrationTracker _tracker;

        public KeepAliveEventHandler(ResponseViewModel responseViewModel, SmibRegistrationTracker tracker)
        {
            _responseViewModel = responseViewModel;
            _tracker = tracker;
        }

        public async Task<bool> HandleAsync(keepAlive busEvent, EventCallContext eventCallContext)
        {
            _responseViewModel.LogEvent(busEvent, "keepAlive", eventCallContext);

            // Only handle SMIB keepAlives (not service keepAlives)
            if (eventCallContext.DeviceType != t_deviceType.FN_SMIB_ID)
                return true;

            var uid = eventCallContext.Uid;

            if (!_tracker.IsSmibKnown(uid))
            {
                _tracker.MarkOnline(uid);
                if (Startup.AutoRegister)
                {
                    // Unknown SMIB sending keepAlive — trigger registration sync
                    _responseViewModel.Log($"[Checkin] Unknown SMIB {uid} detected via keepAlive — triggering registration sync");
                    _ = SyncSmibRegistrationAsync(uid);
                }
                else
                {
                    _responseViewModel.Log($"[Checkin] Unknown SMIB {uid} detected via keepAlive — auto-checkin disabled");
                }
            }
            else
            {
                _tracker.UpdateKeepAlive(uid);
                if (!_tracker.IsRegistered(uid) && Startup.AutoRegister)
                {
                    _responseViewModel.Log($"[Checkin] Known SMIB {uid} still unregistered — re-syncing registration");
                    _ = SyncSmibRegistrationAsync(uid);
                }
            }

            return true;
        }

        private async Task SyncSmibRegistrationAsync(string uid)
        {
            try
            {
                // Step 1: Call getRegistration() to read current state from SMIB
                RpcProxyContext.Current = RpcProxyContext.ToSMIB(uid);
                var regResp = await Startup._iReg.getRegistration();
                _responseViewModel.LogRpcResponse($"getRegistration → {uid}", new { target = uid }, regResp, RpcCallContext.Current);

                if (regResp != null)
                {
                    if (!regResp.registered)
                    {
                        // SMIB says it's not registered — send setRegistration
                        _responseViewModel.Log($"[Checkin] SMIB {uid} not registered (registered={regResp.registered}) — sending setRegistration");
                        await AutoRegisterSmibAsync(uid);
                    }
                    else
                    {
                        // SMIB already considers itself registered (persisted from a prior run),
                        // but its persisted RegistrationFlags may NOT include the TITO single-wire
                        // ticket-system bit (REG_SINGLE_WIRE_TICKET_SYSTEM). The only way to set that
                        // flag — which the BE2 firmware uses to enable TITO_SASEGMEnhValidation (the
                        // gate for Secure Enhanced getValidationIds / LP 0x4C) — is via setRegistration.
                        // So push setRegistration once to (re)assert the current host config
                        // (titoEnabled=true, etc.) regardless of the self-reported registered flag.
                        _responseViewModel.Log($"[Checkin] SMIB {uid} already registered — re-asserting host config via setRegistration (ensures TitoEnabled/single-wire-ticket flag)");
                        await AutoRegisterSmibAsync(uid);
                    }
                }
                else
                {
                    _responseViewModel.Log($"[Checkin] getRegistration returned null for {uid} — sending setRegistration");
                    await AutoRegisterSmibAsync(uid);
                }
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"[Checkin] Registration sync failed for SMIB {uid}: {ex.Message}");
            }
        }

        private async Task AutoRegisterSmibAsync(string uid)
        {
            try
            {
                var c = CheckinDefaultsResolver.Resolve(Startup.Configuration, uid);
                var siteId = c.SiteId;
                var machineNum = c.MachineNum;
                var machineLoc = c.MachineLoc;
                var enabled = c.Enabled;
                var registered = c.Registered;
                var notRegisteredReason = c.NotRegisteredReason;
                var vip = c.Vip;
                var reportDenomId = c.ReportDenomId;
                var pointsCount = c.PointsCount;
                var pointsAward = c.PointsAward;
                var machineStatus = c.MachineStatus ?? "A";
                var haveInitialMeters = c.HaveInitialMeters;
                var titoEnabled = c.TitoEnabled;
                var truePlayerWinEnabled = c.TruePlayerWinEnabled;
                var mdmgEnabled = c.MdmgEnabled;
                var hotPlayerPeriod = c.HotPlayerPeriod;
                var hotPlayerWagers = c.HotPlayerWagers;
                var hotPlayerGames = c.HotPlayerGames;
                var hotPlayerInactivityTimer = c.HotPlayerInactivityTimer;
                var bonusEnabled = c.BonusEnabled;

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
                _tracker.MarkRegistered(uid);
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"[Checkin] Auto-register failed for SMIB {uid}: {ex.Message}");
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
                await Startup._iConfig.setOptionChange(categoryOptions);
                _responseViewModel.LogRpcResponse($"setOptionChange → {uid}", new { target = uid, DISABLE_OL = true }, new { success = true }, RpcCallContext.Current);
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"[Checkin] Failed to push DISABLE_OL config to {uid}: {ex.Message}");
            }
        }
    }
}
