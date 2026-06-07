# FloorNet.MACheckin — FloorNet Message Interface

This document describes all incoming and outgoing FloorNet calls (events and RPCs) handled by the **MACheckin** (Machine Accounting Check-In) service.

## Incoming Events (Subscribed)

| Event | Method | Handler | Action | Simulator Behavior | Source File |
|-------|--------|---------|--------|-------------------|-------------|
| `auditEvent` | `regSmibOnline` | `RegSmibOnlineEventHandler` | Triggers SMIB registration check-in flow: calls `iReg.setRegistration()` to push machine config to SMIB | Handled by shared `AuditEventHandler` → `RegSmibOnlineHandler`: marks SMIB online in `SmibRegistrationTracker`; if AutoRegister=true, calls `iReg.setRegistration()` with config from `servicesettings.json` (SiteId, MachineNum, MachineLoc) + pushes `DISABLE_OL=true` via `iConfig.setOptionChange()` | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/EventHandlers/RegSmibOnlineEventHandler.cs` |
| `auditEvent` | `regNodeOnline` | `RegNodeOnlineEventHandler` | Triggers service-device-to-machine sync; ensures device↔machine mappings are current | Logs the event to response view; no device-to-machine sync implemented | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/EventHandlers/RegNodeOnlineEventHandler.cs` |

## Outgoing RPCs (Caller)

| Interface | Method | Caller Service | Trigger / Job Code | Purpose | Simulator Behavior | Source File |
|-----------|--------|----------------|-------------------|---------|-------------------|-------------|
| `iReg` | `setRegistration()` | `SmibRegistrationService` | `regSmibOnline` event | Pushes machine registration/config data to SMIB during check-in | Called from `RegSmibOnlineHandler` with fixed values from `servicesettings.json[Checkin]` — no DB lookup, no per-machine config | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/SmibRegistrationService.cs` |
| `iReg` | `setRegistration()` | `MaConfigurationService` | Floor update Job 2 (RegistrationUpdate) | Pushes updated registration/config to SMIB after DB config change | Not implemented — no accounting DB polling; no Job 2 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/MaConfigurationService.cs` |
| `iReg` | `disableEGM(false, checkinKey)` | `FloorUpdateService` | Floor update Job 19 (EGMEnableDisable, enable) | Enables the EGM at the SMIB | Not implemented — no accounting DB polling; no Job 19 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `iReg` | `disableEGM(true, checkinKey)` | `FloorUpdateService` | Floor update Job 19 (EGMEnableDisable, disable) | Disables the EGM at the SMIB | Not implemented — no accounting DB polling; no Job 19 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `iReg` | `disableEGM(false, rgKey)` | `FloorUpdateService` | Floor update Job 25 (RGControlRequest, lockState=0) | Unlocks EGM via Responsible Gaming control | Not implemented — no accounting DB polling; no Job 25 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `iReg` | `disableEGM(true, rgKey)` | `FloorUpdateService` | Floor update Job 25 (RGControlRequest, lockState=3) | Locks EGM via Responsible Gaming control | Not implemented — no accounting DB polling; no Job 25 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `iRG` | `LockBV(false, rgKey)` | `FloorUpdateService` | Floor update Job 25 (RGControlRequest, lockState=0) | Unlocks the Bill Validator via Responsible Gaming | Not implemented — no accounting DB polling; no Job 25 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `iRG` | `LockBV(true, rgKey)` | `FloorUpdateService` | Floor update Job 25 (RGControlRequest, lockState=2) | Locks the Bill Validator via Responsible Gaming | Not implemented — no accounting DB polling; no Job 25 processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `iConfig` | `setOptionChange()` | `RegSmibOnlineHandler` | `regSmibOnline` event (after setRegistration) | *(not part of real MACheckin — done by separate config service)* | Pushes `DISABLE_OL=true` option to the SMIB immediately after registration to enable FloorNet-only event forwarding | `FloornetSimWeb/EventHandlers/EventProcessors.cs` |

## Background Services

| Service | Mechanism | Purpose | Simulator Behavior | Source File |
|---------|-----------|---------|-------------------|-------------|
| `FloorUpdateService` | `PeriodicActionManager` (Redis election) | Polls accounting DB `FloorUpdate` table for pending jobs (codes 2, 19, 25); executes RPC calls to SMIBs; updates job status | Not implemented — no DB polling, no floor update job processing | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/FloorUpdateService.cs` |
| `SmibRegistrationService` | Called from `RegSmibOnlineEventHandler` | Performs full SMIB registration/check-in: builds registration payload from DB, calls `iReg.setRegistration()` | Simplified: `RegSmibOnlineHandler` sends `setRegistration()` with static config from `servicesettings.json` (no DB lookup) | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/SmibRegistrationService.cs` |
| `MaConfigurationService` | Called from `FloorUpdateService` (Job 2) | Builds updated config payload from DB and pushes to SMIB via `iReg.setRegistration()` | Not implemented | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/MaConfigurationService.cs` |
| `ServiceRegistrationService` | Startup / periodic | Registers this service node on the FloorNet bus and publishes keepAlive heartbeats | Simulator registers as CMS device at startup and publishes keepAlive automatically via the message bus library | `IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/IGT.FloorNet.MACheckin/Services/ServiceRegistrationService.cs` |

## Floor Update Job Codes (from Accounting DB)

| Job Code | Constant Name | Description | RPC Action | Simulator Support |
|----------|---------------|-------------|------------|-------------------|
| 2 | `RegistrationUpdate` | Machine configuration changed in CMS | `iReg.setRegistration()` — push new config to SMIB | ❌ Not implemented |
| 19 | `EGMEnableDisable` | Enable or disable EGM from CMS | `iReg.disableEGM(enable/disable, checkinKey)` | ❌ Not implemented |
| 25 | `RGControlRequest` | Responsible Gaming lock/unlock | `iReg.disableEGM()` + `iRG.LockBV()` based on `lockState` | ❌ Not implemented |

## Job Processing Flow

```
Accounting DB (FloorUpdate table)
    │
    ▼ (poll every N seconds, Redis leader election)
FloorUpdateService
    │
    ├─ Job 2  → MaConfigurationService → iReg.setRegistration()
    ├─ Job 19 → iReg.disableEGM(enable/disable)
    └─ Job 25 → iReg.disableEGM() + iRG.LockBV() based on lockState
    │
    ▼ (update job status in DB)
Mark job Completed / Failed
```

> ⚠️ **Simulator note**: The entire FloorUpdate job pipeline above is NOT implemented in the simulator. The simulator has no accounting database connection and does not poll for jobs.

## Notes

- **Device type**: `FN_CMS_ID` — the MACheckin service registers itself as a CMS-type node on the FloorNet bus.
- **Accounting DB**: SQL Server database `MachineAccounting` — only this service interacts with it (the Registration service does not).
- **Data Access**: `MaCheckinDal` (implements `IMaCheckinDal`) handles all SQL stored procedure calls to the accounting database.
- **Redis**: Used for distributed leader election (`PeriodicActionManager`) and caching machine-number-to-UID mappings.
- **No outgoing events**: Unlike the Registration service, MACheckin does not publish `auditEvent` messages — it only consumes them and makes outgoing RPC calls.

### Simulator Differences Summary

| Aspect | Real Service | Simulator |
|--------|-------------|-----------|
| Accounting DB | SQL Server with stored procedures for job queue | Not connected — no database |
| Floor update jobs | Periodic polling with Redis leader election | Not implemented |
| Registration payload | Built from DB per-machine (denom, TITO, meters, bonus, etc.) | Static values from `servicesettings.json[Checkin]` section |
| EGM enable/disable | Triggered by CMS operator via Job 19 | Not available |
| Responsible Gaming | Lock/unlock EGM + BV via Job 25 | Not available |
| Config push | Separate `MaConfigurationService` sends full config | Inline `setOptionChange(DISABLE_OL=true)` only |
| Auto-register toggle | Always registers (production CMS behavior) | Controlled by `servicesettings.json[Registration:AutoRegister]` (hot-reloadable) |
