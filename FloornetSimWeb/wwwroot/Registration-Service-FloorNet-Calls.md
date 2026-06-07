# FloorNet.Registration — FloorNet Message Interface

This document describes all incoming and outgoing FloorNet calls (events and RPCs) handled by the **Registration** service.

## Incoming Events (Subscribed)

| Event | Method | Handler | Action | Simulator Behavior | Source File |
|-------|--------|---------|--------|-------------------|-------------|
| `auditEvent` | `regSmibOnline` | `RegSmibOnlineEventHandler` | Upserts SMIB registration data into Redis cache; sets SMIB status to online | Extracts SMIB key from event; marks SMIB online in `SmibRegistrationTracker` (in-memory); if AutoRegister=true, calls `iReg.setRegistration()` + `iConfig.setOptionChange(DISABLE_OL=true)` to the SMIB | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/EventHandlers/RegSmibOnlineEventHandler.cs` |
| `auditEvent` | `regSmibOffline` | `RegSmibOfflineEventHandler` | Sets SMIB status to offline in Redis cache | Marks SMIB offline in `SmibRegistrationTracker` via `MarkOffline()`; logs state change to response view | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/EventHandlers/RegSmibOfflineEventHandler.cs` |
| `auditEvent` | `regNodeOnline` | `RegNodeOnlineEventHandler` | Upserts service node registration into Redis cache; sets node online | Logs the event to response view; no node tracking implemented | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/EventHandlers/RegNodeOnlineEventHandler.cs` |
| `auditEvent` | `regNodeOffline` | `RegNodeOfflineEventHandler` | Sets service node status to offline in Redis cache | Logs the event to response view; no node tracking implemented | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/EventHandlers/RegNodeOfflineEventHandler.cs` |
| `keepAlive` | *(heartbeat)* | `KeepAliveEventHandler` | Updates last-seen timestamp in Redis; if SMIB/node was marked offline, triggers `regSmibOnline`/`regNodeOnline` re-publish | Filters for SMIB keepAlives only; if SMIB is unknown or unregistered and AutoRegister=true, calls `iReg.getRegistration()` then `iReg.setRegistration()` to auto-checkin; updates `SmibRegistrationTracker` timestamps in-memory | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/EventHandlers/KeepAliveEventHandler.cs` |

## Outgoing Events (Published)

| Event | Method | Publisher / Service | Trigger | Simulator Behavior | Source File |
|-------|--------|---------------------|---------|-------------------|-------------|
| `auditEvent` | `regSmibOffline` | `SmibOfflineDetectService` | SMIB missed heartbeat threshold exceeded (periodic scan) | Not implemented — no periodic offline detection scan runs in the simulator | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/SmibOfflineDetectService.cs` |
| `auditEvent` | `regNodeOnline` | `KeepAliveEventHandler` | KeepAlive received from a node currently marked offline in Redis | Not implemented — no node online/offline state tracking | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/EventHandlers/KeepAliveEventHandler.cs` |
| `auditEvent` | `regNodeOffline` | `NodeOfflineDetectService` | Service node missed heartbeat threshold exceeded (periodic scan) | Not implemented — no periodic offline detection scan runs in the simulator | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/NodeOfflineDetectService.cs` |

## Outgoing RPCs (Caller)

| Interface | Method | Caller Service | Purpose | Simulator Behavior | Source File |
|-----------|--------|----------------|---------|-------------------|-------------|
| `iReg` | `getRegistration()` | `SmibRegistrationSyncService` | Fetches current registration data from SMIBs that are online but not yet cached in Redis | Called from `KeepAliveEventHandler` when an unknown/unregistered SMIB sends keepAlive; reads SMIB's `registered` flag to decide if `setRegistration()` is needed | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/SmibRegistrationSyncService.cs` |

## Incoming RPCs (Provider)

| Interface | Method | Provider | Action | Simulator Behavior | Source File |
|-----------|--------|----------|--------|-------------------|-------------|
| `iDiscovery` | `getAllServiceInterfaces()` | `DiscoveryRpcProvider` | Returns all registered service node interfaces from Redis cache | Returns a list of interfaces based on `servicesettings.json[Registration:Discovery]` toggles (hot-reloadable; also adjustable via UI and REST API `PUT api/discovery/state`) | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/DiscoveryRpcProvider.cs` |
| `iDiscovery` | `getServiceInterfaces(interfaces)` | `DiscoveryRpcProvider` | Returns service nodes matching the requested interface list | Returns matching interfaces from the configured set; returns `NoService` descriptor for unconfigured interfaces | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/DiscoveryRpcProvider.cs` |
| `iDiscovery` | `getSmibInterfaces(uid)` | `DiscoveryRpcProvider` | Returns interfaces registered by a specific SMIB (by UID) | `throw new NotImplementedException()` — not supported in the simulator | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/DiscoveryRpcProvider.cs` |
| `iDiags` | *(various)* | `DiagsRpcProvider` | Diagnostic/health-check RPC interface | Not implemented in the simulator | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/DiagsRpcProvider.cs` |

## Background Services

| Service | Mechanism | Purpose | Simulator Behavior | Source File |
|---------|-----------|---------|-------------------|-------------|
| `SmibOfflineDetectService` | `PeriodicActionManager` (Redis election) | Scans Redis for SMIBs whose last keepAlive exceeds threshold; publishes `regSmibOffline` | Not implemented — no periodic background scan | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/SmibOfflineDetectService.cs` |
| `NodeOfflineDetectService` | `PeriodicActionManager` (Redis election) | Scans Redis for service nodes whose last keepAlive exceeds threshold; publishes `regNodeOffline` | Not implemented — no periodic background scan | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/NodeOfflineDetectService.cs` |
| `SmibRegistrationSyncService` | `PeriodicActionManager` (Redis election) | Identifies online SMIBs without cached registration; calls `iReg.getRegistration()` to sync | Handled inline in `KeepAliveEventHandler` — triggered reactively on each keepAlive rather than by periodic scan | `IGT.FloorNet.Registration/IGT.FloorNet.Registration/Services/SmibRegistrationSyncService.cs` |

## Notes

- **Device type**: `FN_CMS_ID` — the Registration service registers itself as a CMS-type node on the FloorNet bus.
- **Redis**: Used as the primary store for all SMIB and node registration state (no SQL database).
- **Distributed election**: Background periodic services use `PeriodicActionManager` with Redis-based leader election to ensure only one instance runs the scan across a cluster.
- **Event bus**: All events flow through RabbitMQ (`auditEvent` exchange for registration events, `keepAlive` exchange for heartbeats).

### Simulator Differences Summary

| Aspect | Real Service | Simulator |
|--------|-------------|-----------|
| State store | Redis (distributed, persistent) | `SmibRegistrationTracker` — in-memory `ConcurrentDictionary` (lost on restart) |
| Offline detection | Periodic background scan with Redis election | Reacts to `regSmibOffline` events only; no periodic heartbeat-timeout scan |
| Node tracking | Full service node lifecycle (online/offline) | Not tracked |
| Registration sync | Periodic `SmibRegistrationSyncService` | Reactive — triggered by keepAlive or regSmibOnline |
| Auto-register toggle | Always registers (production CMS) | Controlled by `servicesettings.json[Registration:AutoRegister]` (hot-reloadable) |
| Config push | Sends full machine config from accounting DB | Sends hardcoded `DISABLE_OL=true` only |
