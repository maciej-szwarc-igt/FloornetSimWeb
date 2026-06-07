'use strict';

// --- API Helpers ---
async function api(method, path, body) {
    const opts = { method, headers: { 'Content-Type': 'application/json' } };
    if (body) opts.body = JSON.stringify(body);
    const resp = await fetch(path, opts);
    if (!resp.ok) {
        const err = await resp.text();
        console.error(`API ${method} ${path} failed:`, err);
        return null;
    }
    return resp.json();
}

function esc(str) {
    const d = document.createElement('div');
    d.textContent = str;
    return d.innerHTML;
}

// --- Tab Management ---
document.querySelectorAll('.tab').forEach(tab => {
    tab.addEventListener('click', () => {
        document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
        document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
        tab.classList.add('active');
        document.getElementById('panel-' + tab.dataset.tab).classList.add('active');
    });
});

// --- Polling ---
let lastLogText = '';

async function pollStatus() {
    const status = await api('GET', '/api/status');
    if (!status) return;
    const indicator = document.getElementById('status-indicator');
    if (status.isConnected) {
        indicator.textContent = 'Connected';
        indicator.className = 'status connected';
    } else {
        indicator.textContent = 'Disconnected';
        indicator.className = 'status disconnected';
    }
}

async function pollLog() {
    const data = await api('GET', '/api/status/log');
    if (!data) return;
    const text = data.text || '';
    if (text === lastLogText) return;
    lastLogText = text;
    parseAndRenderLog(text);
}

let logBlocks = [];

function parseLogBlocks(text) {
    if (!text) return [];
    const rawBlocks = text.split(/={20,}/).filter(b => b.trim());
    const results = [];

    for (const raw of rawBlocks) {
        const trimmed = raw.trim();
        const lines = trimmed.split('\n').filter(Boolean);

        // Separate leading info lines ([Checkin] etc.) from the main entry
        let infoLines = [];
        let entryStart = 0;
        for (let i = 0; i < lines.length; i++) {
            const lt = lines[i].trim();
            if (/^\[[A-Za-z]+\]\s/.test(lt) && !/^\[(RPC|EVT)/.test(lt)) {
                infoLines.push(lt);
                entryStart = i + 1;
            } else {
                break;
            }
        }

        // Add each info line as its own INFO block
        for (const info of infoLines) {
            results.push({ dir: 'INFO', timestamp: '', summary: info, uid: '', full: info });
        }

        // Process the main entry (if any remains)
        const entryLines = lines.slice(entryStart).filter(Boolean);
        if (entryLines.length === 0) continue;

        const firstLine = entryLines[0].trim();
        const isRpc = /^\[RPC[\]<>!]/.test(firstLine);
        const isEvt = /^\[EVT[\]>]/.test(firstLine);
        const isOutbound = /^\[RPC[<!>]/.test(firstLine);
        const dir = isRpc ? 'RPC' : isEvt ? 'EVT' : 'INFO';

        let timestamp = '';
        let uid = '';
        const summary = firstLine;

        // Second line has context: "06/01/2026 11:14:53 AM  Uid: IGT_...  DeviceType: ..."
        const ctxLine = entryLines[1] ? entryLines[1].trim() : '';
        const tsMatch = ctxLine.match(/^(\d{2}\/\d{2}\/\d{4}\s+\d{1,2}:\d{2}:\d{2}\s*(AM|PM)?)/i);
        if (tsMatch) timestamp = tsMatch[1];

        const uidMatch = ctxLine.match(/Uid:\s*(\S+)/);
        if (uidMatch) uid = uidMatch[1];

        // For outbound RPCs, extract UID from the "→ TARGET" in the name
        if (isOutbound && !uid) {
            const targetMatch = firstLine.match(/→\s*(\S+)/);
            if (targetMatch) uid = targetMatch[1];
        }

        // Skip unrecognized blocks with no prefix and no timestamp
        if (!isRpc && !isEvt && !timestamp && !firstLine.startsWith('[')) continue;

        const full = entryLines.map(l => l.trim()).join('\n');
        results.push({ dir, timestamp, summary, uid, full, outbound: isOutbound });
    }
    return results;
}

function parseAndRenderLog(text) {
    logBlocks = parseLogBlocks(text);
    renderLogEntries();
}

function renderLogEntries() {
    const el = document.getElementById('message-log');
    if (logBlocks.length === 0) {
        el.innerHTML = '<div class="log-line" style="color:#888">No messages yet.</div>';
        return;
    }

    const filter = document.getElementById('log-filter').value;
    const filtered = logBlocks.filter(b => {
        if (!filter) return true;
        if (filter === 'RPC Only') return b.dir === 'RPC';
        if (filter === 'Events Only') return b.dir === 'EVT';
        if (filter === 'Info Only') return b.dir === 'INFO';
        return true;
    });

    el.innerHTML = filtered.map((block, idx) => {
        const dirClass = block.dir === 'RPC' ? 'log-dir-rpc' : block.dir === 'EVT' ? 'log-dir-evt' : 'log-dir-info';
        const ts = block.timestamp ? `<span class="log-time">${esc(block.timestamp)}</span> ` : '';
        const uidHtml = block.uid ? `<span class="log-uid">${esc(block.uid)}</span> ` : '';

        // Direction arrow: → inbound (SMIB→us), ← outbound (us→SMIB)
        const arrow = block.dir === 'INFO' ? '' : block.outbound ? '<span class="log-arrow out">←</span> ' : '<span class="log-arrow in">→</span> ';

        // Normalize tag display: [RPC], [RPC!], [RPC<] → [RPC] for inbound, [RPC→] for outbound success, [RPC✗] for outbound fail
        let tag;
        if (block.outbound) {
            tag = block.summary.match(/^\[RPC!\]/) ? '[RPC✗]' : '[RPC]';
        } else {
            tag = block.summary.match(/^\[[^\]]*\]/)?.[0] || `[${block.dir}]`;
        }

        // Strip the [XXX] prefix and any "→ TARGET" suffix from name (UID already shown separately)
        let name = block.summary.replace(/^\[[^\]]*\]\s*/, '');
        if (block.outbound) name = name.replace(/\s*→\s*\S+$/, '');

        return `<div class="log-line" data-idx="${idx}" title="Double-click for details">` +
               arrow +
               `<span class="${dirClass}">${esc(tag)}</span> ` +
               uidHtml +
               ts +
               `<span class="log-summary">${esc(name)}</span>` +
               `</div>`;
    }).join('');

    if (document.getElementById('log-autoscroll').checked) {
        el.scrollTop = el.scrollHeight;
    }
}

// Double-click log line to show full detail in modal
document.getElementById('message-log').addEventListener('dblclick', (e) => {
    const line = e.target.closest('.log-line');
    if (!line) return;
    const idx = parseInt(line.dataset.idx);
    const filter = document.getElementById('log-filter').value;
    const filtered = logBlocks.filter(b => {
        if (!filter) return true;
        if (filter === 'RPC Only') return b.dir === 'RPC';
        if (filter === 'Events Only') return b.dir === 'EVT';
        if (filter === 'Info Only') return b.dir === 'INFO';
        return true;
    });
    const block = filtered[idx];
    if (!block) return;
    showDetailModal(block);
});

function showDetailModal(block) {
    // Build a clean title: "→ [RPC] getRegistration" or "← [EVT] keepAlive"
    let title;
    if (block.dir === 'INFO') {
        title = block.summary;
    } else {
        const arrow = block.outbound ? '←' : '→';
        let tag;
        if (block.outbound) {
            tag = block.summary.match(/^\[RPC!\]/) ? '[RPC✗]' : '[RPC]';
        } else {
            tag = block.summary.match(/^\[[^\]]*\]/)?.[0] || `[${block.dir}]`;
        }
        let name = block.summary.replace(/^\[[^\]]*\]\s*/, '');
        if (block.outbound) name = name.replace(/\s*→\s*\S+$/, '');
        title = `${arrow} ${tag} ${name}`;
        if (block.uid) title += ` (${block.uid})`;
    }
    document.getElementById('detail-modal-title').textContent = title;
    document.getElementById('detail-body').textContent = block.full;
    document.getElementById('detail-modal').classList.remove('hidden');
}

function closeDetailModal() {
    document.getElementById('detail-modal').classList.add('hidden');
}

document.getElementById('log-filter').addEventListener('change', renderLogEntries);

async function clearLog() {
    await api('POST', '/api/status/log/clear');
    lastLogText = '';
    logBlocks = [];
    document.getElementById('message-log').innerHTML = '';
}

// --- Service State Save Functions ---

async function saveTitoState() {
    await api('PUT', '/api/tito/state', {
        respondToValidation: document.getElementById('tito-respond-validation').checked,
        seedValue1: parseInt(document.getElementById('tito-seed1').value) || 0,
        seedValue2: parseInt(document.getElementById('tito-seed2').value) || 0,
        validationIdCount: parseInt(document.getElementById('tito-id-count').value) || 10,
        respondToIssue: document.getElementById('tito-respond-issue').checked,
        issueStatus: parseInt(document.getElementById('tito-issue-status').value),
        respondToCommit: document.getElementById('tito-respond-commit').checked,
        transactionId: parseInt(document.getElementById('tito-txn-id').value) || null,
        respondToRedeem: document.getElementById('tito-respond-redeem').checked,
        redeemStatus: parseInt(document.getElementById('tito-redeem-status').value),
        redeemAmount: parseInt(document.getElementById('tito-redeem-amount').value) || 0
    });
}

async function saveCardState() {
    await api('PUT', '/api/card/state', {
        respondToCardIn: document.getElementById('card-respond-in').checked,
        respondToCardOut: document.getElementById('card-respond-out').checked,
        respondToPinValidation: document.getElementById('card-respond-pin').checked,
        playerCardNumber: document.getElementById('card-number').value,
        playerName: document.getElementById('card-player-name').value,
        pinValidationResult: parseInt(document.getElementById('card-pin-result').value)
    });
}

async function savePcsState() {
    await api('PUT', '/api/pcs/state', {
        respondToPcs: document.getElementById('pcs-respond').checked,
        pointsBalance: parseInt(document.getElementById('pcs-points').value) || 0,
        cashbackBalance: parseInt(document.getElementById('pcs-cashback').value) || 0,
        sessionLimit: parseInt(document.getElementById('pcs-session-limit').value) || 0,
        timeLimit: parseInt(document.getElementById('pcs-time-limit').value) || 0
    });
}

async function saveBonusState() {
    const levels = document.getElementById('bonus-levels').value.split(',').map(s => parseInt(s.trim())).filter(n => !isNaN(n));
    await api('PUT', '/api/bonus/state', {
        respondToBonus: document.getElementById('bonus-respond').checked,
        bonusAmount: parseInt(document.getElementById('bonus-amount').value) || 0,
        bonusMultiplier: parseInt(document.getElementById('bonus-multiplier').value) || 1,
        levelIds: levels
    });
}

async function saveEftState() {
    await api('PUT', '/api/eft/state', {
        respondToEft: document.getElementById('eft-respond').checked,
        transferStatus: parseInt(document.getElementById('eft-status').value),
        availableBalance: parseInt(document.getElementById('eft-balance').value) || 0
    });
}

async function saveGatState() {
    await api('PUT', '/api/gat/state', {
        respondToGat: document.getElementById('gat-respond').checked,
        gatStatus: parseInt(document.getElementById('gat-status').value),
        componentHash: document.getElementById('gat-hash').value
    });
}

async function saveHandpayState() {
    await api('PUT', '/api/handpay/state', {
        respondToHandpay: document.getElementById('hp-respond').checked,
        autoKeyOff: document.getElementById('hp-auto-keyoff').checked,
        keyOffType: parseInt(document.getElementById('hp-keyoff-type').value),
        resetAmount: parseInt(document.getElementById('hp-reset-amount').value) || 0
    });
}

async function resetHandpay() {
    const amount = parseInt(document.getElementById('hp-action-amount').value) || 0;
    await api('POST', '/api/handpay/reset', { amount });
}

async function saveWatState() {
    await api('PUT', '/api/wat/state', {
        respondToWat: document.getElementById('wat-respond').checked,
        accountId: document.getElementById('wat-account-id').value,
        watBalance: parseInt(document.getElementById('wat-balance').value) || 0,
        transferResult: parseInt(document.getElementById('wat-result').value)
    });
}

async function requestWatTransfer() {
    const amount = parseInt(document.getElementById('wat-transfer-amount').value) || 0;
    const direction = parseInt(document.getElementById('wat-transfer-dir').value);
    await api('POST', '/api/wat/transfer', { amount, direction, accountId: document.getElementById('wat-account-id').value });
}

async function saveAmlState() {
    await api('PUT', '/api/aml/state', {
        respondToAml: document.getElementById('aml-respond').checked,
        cashAggregationThreshold: parseInt(document.getElementById('aml-cash-threshold').value) || 0,
        sessionThreshold: parseInt(document.getElementById('aml-session-threshold').value) || 0
    });
}

async function savePcsState() {
    await api('PUT', '/api/pcs/state', {
        respondToPcs: document.getElementById('pcs-respond').checked,
        pointsBalance: parseInt(document.getElementById('pcs-points').value) || 0,
        cashbackBalance: parseInt(document.getElementById('pcs-cashback').value) || 0,
        sessionLimit: parseInt(document.getElementById('pcs-session-limit').value) || 0,
        timeLimit: parseInt(document.getElementById('pcs-time-limit').value) || 0
    });
}

async function saveDiscoveryState() {
    await api('PUT', '/api/discovery/state', {
        bonusInterface: document.getElementById('disc-iBonus').checked,
        rgInterface: document.getElementById('disc-iRG').checked,
        pcsInterface: document.getElementById('disc-iPCS').checked,
        diagsInterface: document.getElementById('disc-iDiags').checked,
        titoInterface: document.getElementById('disc-iTito').checked,
        watInterface: document.getElementById('disc-iWat').checked,
        gatInterface: document.getElementById('disc-iGat').checked,
        markerInterface: document.getElementById('disc-iMarker').checked,
        ismInterface: document.getElementById('disc-iISM').checked,
        pinInterface: document.getElementById('disc-iPin').checked,
        regInterface: document.getElementById('disc-iReg').checked,
        amlInterface: document.getElementById('disc-iAML').checked,
        confInterface: document.getElementById('disc-iConfig').checked,
        eftInterface: document.getElementById('disc-iEft').checked,
        downloadInterface: document.getElementById('disc-iDownload').checked,
        playerInterface: document.getElementById('disc-iPlayer').checked,
        cardlessInterface: document.getElementById('disc-iCardless').checked,
        handpayInterface: document.getElementById('disc-iHandpay').checked
    });
}

async function clearDiscoveryState() {
    const data = await api('POST', '/api/discovery/clear');
    if (data && data.discoveryModel) applyDiscoveryState(data.discoveryModel);
}

async function loadDiscoveryState() {
    const data = await api('GET', '/api/discovery/state');
    if (data && data.discoveryModel) applyDiscoveryState(data.discoveryModel);
}

function applyDiscoveryState(m) {
    document.getElementById('disc-iBonus').checked = m.bonusInterface;
    document.getElementById('disc-iRG').checked = m.rgInterface;
    document.getElementById('disc-iPCS').checked = m.pcsInterface;
    document.getElementById('disc-iDiags').checked = m.diagsInterface;
    document.getElementById('disc-iTito').checked = m.titoInterface;
    document.getElementById('disc-iWat').checked = m.watInterface;
    document.getElementById('disc-iGat').checked = m.gatInterface;
    document.getElementById('disc-iMarker').checked = m.markerInterface;
    document.getElementById('disc-iISM').checked = m.ismInterface;
    document.getElementById('disc-iPin').checked = m.pinInterface;
    document.getElementById('disc-iReg').checked = m.regInterface;
    document.getElementById('disc-iAML').checked = m.amlInterface;
    document.getElementById('disc-iConfig').checked = m.confInterface;
    document.getElementById('disc-iEft').checked = m.eftInterface;
    document.getElementById('disc-iDownload').checked = m.downloadInterface;
    document.getElementById('disc-iPlayer').checked = m.playerInterface;
    document.getElementById('disc-iCardless').checked = m.cardlessInterface;
    document.getElementById('disc-iHandpay').checked = m.handpayInterface;
}

async function discoveryGetAll() {
    const data = await api('GET', '/api/discovery/state');
    if (data && data.discoveryModel) {
        document.getElementById('disc-response').textContent = JSON.stringify(data.discoveryModel, null, 2);
    }
}

async function discoveryGetSelected() {
    const checkboxes = [
        { id: 'disc-iBonus', name: 'iBonus' },
        { id: 'disc-iRG', name: 'iRG' },
        { id: 'disc-iPCS', name: 'iPCS' },
        { id: 'disc-iDiags', name: 'iDiags' },
        { id: 'disc-iTito', name: 'iTito' },
        { id: 'disc-iWat', name: 'iWat' },
        { id: 'disc-iGat', name: 'iGat' },
        { id: 'disc-iMarker', name: 'iMarker' },
        { id: 'disc-iISM', name: 'iISM' },
        { id: 'disc-iPin', name: 'iPin' },
        { id: 'disc-iReg', name: 'iReg' },
        { id: 'disc-iAML', name: 'iAML' },
        { id: 'disc-iConfig', name: 'iConfig' },
        { id: 'disc-iEft', name: 'iEft' },
        { id: 'disc-iDownload', name: 'iDownload' },
        { id: 'disc-iPlayer', name: 'iPlayer' },
        { id: 'disc-iCardless', name: 'iCardless' },
        { id: 'disc-iHandpay', name: 'iHandpay' }
    ];
    const selected = checkboxes.filter(c => document.getElementById(c.id).checked).map(c => c.name);
    document.getElementById('disc-response').textContent = 'Active interfaces:\n' + selected.join(', ');
}

async function saveCardlessState() {
    await api('PUT', '/api/cardless/state', {
        respondToCardless: document.getElementById('cardless-respond').checked,
        nonce: document.getElementById('cardless-nonce').value
    });
}

async function saveIsmState() {
    await api('PUT', '/api/ism/state', {
        respondToIsm: document.getElementById('ism-respond').checked,
        accountBalance: parseInt(document.getElementById('ism-balance').value) || 0,
        publicKey: document.getElementById('ism-pubkey').value
    });
}

async function saveProgressState() {
    await api('PUT', '/api/progress/state', {
        levels: [
            { levelId: 1, name: 'Minor', amount: parseInt(document.getElementById('prog-level-1').value) || 0 },
            { levelId: 2, name: 'Major', amount: parseInt(document.getElementById('prog-level-2').value) || 0 },
            { levelId: 3, name: 'Grand', amount: parseInt(document.getElementById('prog-level-3').value) || 0 }
        ]
    });
}

async function saveRegState() {
    await api('PUT', '/api/registration/state', {
        autoRegister: document.getElementById('reg-auto').checked
    });
}

// Toggle manual checkin fields based on Auto Checkin checkbox
function toggleRegAuto() {
    const auto = document.getElementById('reg-auto').checked;
    const panel = document.getElementById('checkin-panel');
    // Disable all form inputs except the auto checkbox itself
    panel.querySelectorAll('input:not(#reg-auto), select').forEach(el => {
        el.disabled = auto;
        el.style.opacity = auto ? '0.5' : '1';
    });
    // Disable the action buttons at the bottom
    document.querySelectorAll('#panel-checkin > button').forEach(el => {
        el.disabled = auto;
        el.style.opacity = auto ? '0.5' : '1';
    });
}

async function refreshSmibList() {
    const resp = await api('GET', '/api/registration/smibs');
    const list = document.getElementById('reg-smib-list');
    const select = document.getElementById('reg-uid');
    const confSelect = document.getElementById('conf-uid');
    const disableSelect = document.getElementById('disable-uid');
    const lockSelect = document.getElementById('lock-uid');
    const resetSelect = document.getElementById('reset-uid');
    if (!resp || !resp.length) {
        list.innerHTML = '<em>No SMIBs detected yet</em>';
        return;
    }
    select.innerHTML = '<option value="">-- select --</option>';
    confSelect.innerHTML = '<option value="">-- select --</option>';
    disableSelect.innerHTML = '<option value="">-- select --</option>';
    lockSelect.innerHTML = '<option value="">-- select --</option>';
    resetSelect.innerHTML = '<option value="">-- select --</option>';
    list.innerHTML = resp.map(s =>
        `<div>${s.uid} | ${s.registered ? '✓ Reg' : '✗ Unreg'} | ${s.online ? 'Online' : 'Offline'} | ${new Date(s.lastKeepAlive).toLocaleTimeString()}</div>`
    ).join('');
    resp.forEach(s => {
        const opt = document.createElement('option');
        opt.value = s.uid;
        opt.textContent = s.uid;
        select.appendChild(opt);
        confSelect.appendChild(opt.cloneNode(true));
        disableSelect.appendChild(opt.cloneNode(true));
        lockSelect.appendChild(opt.cloneNode(true));
        resetSelect.appendChild(opt.cloneNode(true));
    });
}

function getRegTargetUid() {
    const selectVal = document.getElementById('reg-uid').value;
    const textVal = document.getElementById('reg-uid-text').value.trim();
    return textVal || selectVal || '';
}

async function sendSetRegistration() {
    const uid = getRegTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    await api('POST', '/api/checkin/set', {
        uid,
        enabled: document.getElementById('reg-enabled').checked,
        registered: document.getElementById('reg-registered').checked,
        vip: document.getElementById('reg-vip').checked,
        bonusEnabled: document.getElementById('reg-bonus-enabled').checked,
        notRegisteredReason: document.getElementById('reg-not-registered-reason').value || null,
        machineNum: parseInt(document.getElementById('reg-machine-num').value) || 0,
        machineLoc: document.getElementById('reg-machine-loc').value || null,
        siteId: document.getElementById('reg-site-id').value || null,
        reportDenomId: parseInt(document.getElementById('reg-report-denom').value) || 0,
        machineStatus: document.getElementById('reg-machine-status').value,
        haveInitialMeters: document.getElementById('reg-have-initial-meters').checked,
        titoEnabled: document.getElementById('reg-tito-enabled').checked,
        truePlayerWinEnabled: document.getElementById('reg-tpw-enabled').checked,
        mdmgEnabled: document.getElementById('reg-mdmg-enabled').checked,
        pointsCount: parseInt(document.getElementById('reg-points-count').value) || 0,
        pointsAward: parseInt(document.getElementById('reg-points-award').value) || 0,
        hotPlayerPeriod: parseInt(document.getElementById('reg-hp-period').value) || 0,
        hotPlayerWagers: parseInt(document.getElementById('reg-hp-wagers').value) || 0,
        hotPlayerGames: parseInt(document.getElementById('reg-hp-games').value) || 0,
        hotPlayerInactivityTimer: parseInt(document.getElementById('reg-hp-inactivity').value) || 0
    });
}

async function sendGetRegistration() {
    const uid = getRegTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    const resp = await api('POST', '/api/checkin/get', { uid });
    if (!resp) return;

    // Populate text/number fields from SMIB response
    if (resp.machineNum != null) document.getElementById('reg-machine-num').value = resp.machineNum;
    if (resp.machineLoc != null) document.getElementById('reg-machine-loc').value = resp.machineLoc;
    if (resp.siteId != null) document.getElementById('reg-site-id').value = resp.siteId;
    if (resp.reportDenomId != null) document.getElementById('reg-report-denom').value = resp.reportDenomId;
    if (resp.pointsCount != null) document.getElementById('reg-points-count').value = resp.pointsCount;
    if (resp.pointsAward != null) document.getElementById('reg-points-award').value = resp.pointsAward;
    if (resp.hotPlayerPeriod != null) document.getElementById('reg-hp-period').value = resp.hotPlayerPeriod;
    if (resp.hotPlayerWagers != null) document.getElementById('reg-hp-wagers').value = resp.hotPlayerWagers;
    if (resp.hotPlayerGames != null) document.getElementById('reg-hp-games').value = resp.hotPlayerGames;
    if (resp.hotPlayerInactivityTimer != null) document.getElementById('reg-hp-inactivity').value = resp.hotPlayerInactivityTimer;

    // Select fields
    if (resp.machineStatus) document.getElementById('reg-machine-status').value = resp.machineStatus;

    // Checkbox fields
    if (resp.enabled != null) document.getElementById('reg-enabled').checked = !!resp.enabled;
    if (resp.registered != null) document.getElementById('reg-registered').checked = !!resp.registered;
    if (resp.vip != null) document.getElementById('reg-vip').checked = !!resp.vip;
    if (resp.bonusEnabled != null) document.getElementById('reg-bonus-enabled').checked = !!resp.bonusEnabled;
    if (resp.titoEnabled != null) document.getElementById('reg-tito-enabled').checked = !!resp.titoEnabled;
    if (resp.haveInitialMeters != null) document.getElementById('reg-have-initial-meters').checked = !!resp.haveInitialMeters;
    if (resp.truePlayerWinEnabled != null) document.getElementById('reg-tpw-enabled').checked = !!resp.truePlayerWinEnabled;
    if (resp.mdmgEnabled != null) document.getElementById('reg-mdmg-enabled').checked = !!resp.mdmgEnabled;
}

// Save checkin auto-register state to backend
async function saveCheckinState() {
    const auto = document.getElementById('reg-auto').checked;
    await api('PUT', '/api/checkin/state', { autoRegister: auto });
}

// Load checkin defaults from servicesettings.json via API and populate UI fields.
// When a SMIB UID is selected, per-UID overrides (Checkin:Machines[]) are merged
// over the global defaults so each machine auto-fills with its own values.
async function loadCheckinDefaults() {
    const uid = getRegTargetUid();
    const qs = uid ? ('?uid=' + encodeURIComponent(uid)) : '';
    const defaults = await api('GET', '/api/checkin/defaults' + qs);
    if (!defaults) return;

    // Text/number fields
    if (defaults.machineNum != null) document.getElementById('reg-machine-num').value = defaults.machineNum;
    if (defaults.machineLoc != null) document.getElementById('reg-machine-loc').value = defaults.machineLoc;
    if (defaults.siteId != null) document.getElementById('reg-site-id').value = defaults.siteId;
    if (defaults.reportDenomId != null) document.getElementById('reg-report-denom').value = defaults.reportDenomId;
    if (defaults.pointsCount != null) document.getElementById('reg-points-count').value = defaults.pointsCount;
    if (defaults.pointsAward != null) document.getElementById('reg-points-award').value = defaults.pointsAward;
    if (defaults.hotPlayerPeriod != null) document.getElementById('reg-hp-period').value = defaults.hotPlayerPeriod;
    if (defaults.hotPlayerWagers != null) document.getElementById('reg-hp-wagers').value = defaults.hotPlayerWagers;
    if (defaults.hotPlayerGames != null) document.getElementById('reg-hp-games').value = defaults.hotPlayerGames;
    if (defaults.hotPlayerInactivityTimer != null) document.getElementById('reg-hp-inactivity').value = defaults.hotPlayerInactivityTimer;
    document.getElementById('reg-not-registered-reason').value = defaults.notRegisteredReason || '';

    // Select fields
    if (defaults.machineStatus) document.getElementById('reg-machine-status').value = defaults.machineStatus;

    // Checkbox fields
    document.getElementById('reg-enabled').checked = !!defaults.enabled;
    document.getElementById('reg-registered').checked = !!defaults.registered;
    document.getElementById('reg-vip').checked = !!defaults.vip;
    document.getElementById('reg-bonus-enabled').checked = !!defaults.bonusEnabled;
    document.getElementById('reg-tito-enabled').checked = !!defaults.titoEnabled;
    document.getElementById('reg-have-initial-meters').checked = !!defaults.haveInitialMeters;
    document.getElementById('reg-tpw-enabled').checked = !!defaults.truePlayerWinEnabled;
    document.getElementById('reg-mdmg-enabled').checked = !!defaults.mdmgEnabled;
}

// Initialize checkin tab state on load
document.addEventListener('DOMContentLoaded', () => {
    loadCheckinDefaults();
    toggleRegAuto();
    refreshSmibList();
    loadDiscoveryState();
});

async function saveRgState() {
    await api('PUT', '/api/rg/state', {
        disableOnCardOut: document.getElementById('rg-disable-card-out').checked,
        lockBillValidator: document.getElementById('rg-lock-bv').checked,
        leaseMinutes: parseInt(document.getElementById('rg-lease').value) || 60
    });
}

async function rgDisableOnCardOut() {
    const deviceId = parseInt(document.getElementById('rg-device-id').value) || 0;
    await api('POST', '/api/rg/disable-on-card-out', { deviceId });
}

async function rgEnableWithLease() {
    const deviceId = parseInt(document.getElementById('rg-device-id').value) || 0;
    const leaseMinutes = parseInt(document.getElementById('rg-lease').value) || 60;
    await api('POST', '/api/rg/enable-with-lease', { deviceId, leaseMinutes });
}

async function rgLockBV() {
    const deviceId = parseInt(document.getElementById('rg-device-id').value) || 0;
    await api('POST', '/api/rg/lock-bv', { deviceId });
}

async function requestMeters() {
    await api('POST', '/api/meters/request');
}

// --- Config Tab ---
function getConfUid() {
    return document.getElementById('conf-uid').value || document.getElementById('conf-uid-text').value.trim();
}

// Available option names for current category (populated by getOptionList)
let _optionNames = [];
let _optionValues = {}; // name -> value map

async function getOptionList() {
    const uid = getConfUid();
    if (!uid) { alert('Select a SMIB'); return; }
    const category = document.getElementById('conf-category').value;
    const resp = await api('POST', '/api/config/getOptions', { uid, category });
    if (resp && resp.optionGroups && resp.optionGroups.length > 0) {
        const group = resp.optionGroups[0];
        _optionNames = (group.optionItems || []).map(i => i.name);
        _optionValues = {};
        for (const item of (group.optionItems || [])) {
            _optionValues[item.name] = String(item.value ?? '');
        }
        const sel = document.getElementById('conf-option-select');
        sel.innerHTML = _optionNames.map(n =>
            `<option value="${escHtml(n)}">${escHtml(n)}</option>`).join('');
        const valInput = document.getElementById('conf-option-value');
        valInput.disabled = false;
        if (_optionNames.length > 0) {
            valInput.value = _optionValues[_optionNames[0]] || '';
        }
        sel.onchange = function() {
            valInput.value = _optionValues[sel.value] || '';
        };
        valInput.oninput = function() {
            if (sel.value) _optionValues[sel.value] = valInput.value;
        };
        // Show summary of total items
        const summary = document.getElementById('conf-option-summary');
        summary.textContent = `${_optionNames.length} option(s) loaded for ${category}`;
    }
}

async function getOptionListShort() {
    const uid = getConfUid();
    if (!uid) { alert('Select a SMIB'); return; }
    const resp = await api('POST', '/api/config/getOptionsShort', { uid });
    if (resp && resp.optionGroups) {
        _optionNames = resp.optionGroups.map(g => g.messageCategory);
        _optionValues = {};
        for (const g of resp.optionGroups) {
            _optionValues[g.messageCategory] = `seq=${g.configSeq}${g.setby ? ' [' + g.setby + ']' : ''}`;
        }
        const sel = document.getElementById('conf-option-select');
        sel.innerHTML = _optionNames.map(n =>
            `<option value="${escHtml(n)}">${escHtml(n)}</option>`).join('');
        const valInput = document.getElementById('conf-option-value');
        valInput.disabled = false;
        if (_optionNames.length > 0) {
            valInput.value = _optionValues[_optionNames[0]] || '';
        }
        sel.onchange = function() {
            valInput.value = _optionValues[sel.value] || '';
        };
        valInput.oninput = function() {
            if (sel.value) _optionValues[sel.value] = valInput.value;
        };
        const summary = document.getElementById('conf-option-summary');
        summary.textContent = `${_optionNames.length} category(ies) loaded`;
    }
}

async function setOptionChange() {
    const uid = getConfUid();
    if (!uid) { alert('Select a SMIB'); return; }
    const category = document.getElementById('conf-category').value;
    const options = [];
    for (const name of _optionNames) {
        const value = _optionValues[name];
        if (name && value !== undefined) options.push({ name, value: value || '' });
    }
    if (options.length === 0) { alert('No options to set'); return; }
    await api('POST', '/api/config/setOptions', { uid, category, options });
}

function escHtml(s) { return s.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;'); }

async function notifyDownload() {
    const deviceId = parseInt(document.getElementById('dl-device-id').value) || 0;
    const fileGroup = document.getElementById('dl-filegroup').value;
    const url = document.getElementById('dl-url').value;
    await api('POST', '/api/download/notify', { deviceId, fileGroup, url });
}

async function runDiags() {
    const deviceId = parseInt(document.getElementById('diag-device-id').value) || 0;
    await api('POST', '/api/diagnostics/run', { deviceId });
}

async function resetDiags() {
    const deviceId = parseInt(document.getElementById('diag-device-id').value) || 0;
    await api('POST', '/api/diagnostics/reset', { deviceId });
}

async function refreshAuditEvents() {
    const events = await api('GET', '/api/events/audit');
    if (!events) return;
    const el = document.getElementById('audit-events-list');
    if (events.length === 0) {
        el.textContent = 'No audit events received yet.';
        return;
    }
    el.innerHTML = events.map(e => {
        const time = new Date(e.timestamp).toLocaleTimeString();
        return `<div>${esc(time)} [${esc(e.eventType)}] ${esc(e.description)}</div>`;
    }).join('');
}

async function saveMarkerState() {
    await api('PUT', '/api/marker/state', {
        markerBalance: parseInt(document.getElementById('marker-balance').value) || 0,
        repayResult: parseInt(document.getElementById('marker-repay-result').value)
    });
}

async function repayMarker() {
    const amount = parseInt(document.getElementById('marker-repay-amount').value) || 0;
    await api('POST', '/api/marker/repay', { amount });
}

// --- EGM Control (Disable / Lock / Reset) ---

function getDisableTargetUid() {
    const textVal = document.getElementById('disable-uid-text').value.trim();
    const selectVal = document.getElementById('disable-uid').value;
    return textVal || selectVal || '';
}

function getLockTargetUid() {
    const textVal = document.getElementById('lock-uid-text').value.trim();
    const selectVal = document.getElementById('lock-uid').value;
    return textVal || selectVal || '';
}

function getResetTargetUid() {
    const textVal = document.getElementById('reset-uid-text').value.trim();
    const selectVal = document.getElementById('reset-uid').value;
    return textVal || selectVal || '';
}

async function sendDisableEgm() {
    const uid = getDisableTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    const disableKey = document.getElementById('disable-key').value;
    if (!disableKey) { alert('Please select a Disable Key'); return; }
    const state = document.getElementById('disable-state').checked;
    const resp = await api('POST', '/api/egmcontrol/disable', { uid, state, disableKey });
    document.getElementById('disable-response').textContent = JSON.stringify(resp, null, 2);
}

async function getDisableKeys() {
    const uid = getDisableTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    const resp = await api('POST', '/api/egmcontrol/disable-keys', { uid });
    document.getElementById('disable-response').textContent = JSON.stringify(resp, null, 2);
}

async function sendLockEgm() {
    const uid = getLockTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    const lockKey = document.getElementById('lock-key').value.trim();
    if (!lockKey) { alert('Please enter a Lock Key'); return; }
    const timer = parseInt(document.getElementById('lock-timer').value) || 0;
    const state = document.getElementById('lock-state').checked;
    const resp = await api('POST', '/api/egmcontrol/lock', { uid, timer, state, lockKey });
    document.getElementById('lock-response').textContent = JSON.stringify(resp, null, 2);
}

async function getLockKeys() {
    const uid = getLockTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    const resp = await api('POST', '/api/egmcontrol/lock-keys', { uid });
    document.getElementById('lock-response').textContent = JSON.stringify(resp, null, 2);
}

async function sendResetEgm() {
    const uid = getResetTargetUid();
    if (!uid) { alert('Please select or enter a target SMIB UID'); return; }
    const hard = document.getElementById('reset-hard').checked;
    if (!confirm(`Are you sure you want to ${hard ? 'HARD' : 'soft'} reset EGM ${uid}?`)) return;
    const resp = await api('POST', '/api/egmcontrol/reset', { uid, hard });
    document.getElementById('reset-response').textContent = JSON.stringify(resp, null, 2);
}

// --- Initialization ---
pollStatus();
pollLog();
setInterval(() => { pollStatus(); pollLog(); }, 1000);
