using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Globalization;
using System.Text.Json;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Eft;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Gat;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.HandPay;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iSmibWat;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.AML;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Discovery;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Cardless;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.ISM;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.MetersSvc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.ProgressEvent;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.RG;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.IConf;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Download;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Diags;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Event;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Marker;

namespace FloornetSimWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly ResponseViewModel _response;

    public StatusController(ResponseViewModel response)
    {
        _response = response;
    }

    [HttpGet]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            IsConnected = true,
            Timestamp = DateTime.Now
        });
    }

    [HttpGet("log")]
    public IActionResult GetLog()
    {
        return Ok(new
        {
            Text = _response.Model.Response ?? ""
        });
    }

    [HttpPost("log/clear")]
    public IActionResult ClearLog()
    {
        _response.Clear(null);
        return Ok(new { message = "Log cleared" });
    }
}

[ApiController]
[Route("api/tito")]
public class TitoApiController : ControllerBase
{
    private readonly ValidationViewModel _validation;
    private readonly IssueViewModel _issue;
    private readonly CommitViewModel _commit;
    private readonly RedeemViewModel _redeem;
    private readonly SimFeatureState _sim;

    public TitoApiController(ValidationViewModel v, IssueViewModel i, CommitViewModel c, RedeemViewModel r, SimFeatureState sim)
    { _validation = v; _issue = i; _commit = c; _redeem = r; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var extra = _sim.Get("tito");
        return Ok(new
        {
            respondToValidation = _validation.Model.RespondToRPC,
            seedValue1 = _validation.Model.SeedValue1,
            seedValue2 = _validation.Model.SeedValue2,
            validationIdCount = extra.TryGetValue("validationIdCount", out var vic) ? vic : null,
            respondToIssue = _issue.Model.RespondToRPC,
            issueStatus = extra.TryGetValue("issueStatus", out var iss) ? iss : null,
            respondToCommit = _commit.Model.RespondToRPC,
            transactionId = _commit.Model.TransactionId,
            respondToRedeem = _redeem.Model.RespondToRPC,
            redeemStatus = extra.TryGetValue("redeemStatus", out var rs) ? rs : null,
            redeemAmount = _redeem.Model.VoucherAmount
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        if (u.TryGetValue("respondToValidation", out var rv)) _validation.Model.RespondToRPC = ToBool(rv);
        if (u.TryGetValue("seedValue1", out var s1) && s1 is not null) _validation.Model.SeedValue1 = ToInt64(s1);
        if (u.TryGetValue("seedValue2", out var s2) && s2 is not null) _validation.Model.SeedValue2 = ToInt64(s2);
        if (u.TryGetValue("respondToIssue", out var ri)) _issue.Model.RespondToRPC = ToBool(ri);
        if (u.TryGetValue("respondToCommit", out var rc)) _commit.Model.RespondToRPC = ToBool(rc);
        if (u.TryGetValue("transactionId", out var tid) && tid is not null) _commit.Model.TransactionId = ToInt64(tid);
        if (u.TryGetValue("respondToRedeem", out var rr)) _redeem.Model.RespondToRPC = ToBool(rr);
        if (u.TryGetValue("redeemAmount", out var ra) && ra is not null) _redeem.Model.VoucherAmount = ToInt64(ra);

        // Fields with no model home are retained in-memory for round-trip.
        var extra = new Dictionary<string, object?>();
        if (u.TryGetValue("validationIdCount", out var vic)) extra["validationIdCount"] = vic;
        if (u.TryGetValue("issueStatus", out var iss)) extra["issueStatus"] = iss;
        if (u.TryGetValue("redeemStatus", out var rs)) extra["redeemStatus"] = rs;
        if (extra.Count > 0) _sim.Set("tito", extra);

        return GetState();
    }

    [HttpPut("validation")]
    public IActionResult UpdateValidation([FromBody] Dictionary<string, object> update)
    {
        if (update.ContainsKey("respondToRPC")) _validation.Model.RespondToRPC = ToBool(update["respondToRPC"]);
        return Ok(new { message = "Updated" });
    }

    // System.Text.Json deserializes Dictionary<string, object?> values as JsonElement,
    // which is not IConvertible. These helpers coerce JsonElement (or primitive/string) safely.
    private static bool ToBool(object? value) => value switch
    {
        null => false,
        bool b => b,
        JsonElement je => je.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => je.GetInt64() != 0,
            JsonValueKind.String => bool.TryParse(je.GetString(), out var pb) && pb,
            _ => false
        },
        string s => bool.TryParse(s, out var pb) && pb,
        _ => Convert.ToBoolean(value, CultureInfo.InvariantCulture)
    };

    private static long ToInt64(object? value) => value switch
    {
        null => 0L,
        long l => l,
        int i => i,
        JsonElement je => je.ValueKind switch
        {
            JsonValueKind.Number => je.GetInt64(),
            JsonValueKind.String => long.Parse(je.GetString()!, CultureInfo.InvariantCulture),
            _ => 0L
        },
        string s => long.Parse(s, CultureInfo.InvariantCulture),
        _ => Convert.ToInt64(value, CultureInfo.InvariantCulture)
    };
}

[ApiController]
[Route("api/card")]
public class CardApiController : ControllerBase
{
    private readonly CardViewModel _card;
    private readonly SimFeatureState _sim;
    public CardApiController(CardViewModel card, SimFeatureState sim) { _card = card; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var cardIn = IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer.CardInViewModel.CardInModel;
        var cardOut = IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer.PlayerCardOutViewModel.PlayerCardOutModel;
        var pin = IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPin.ValidatePinViewModel.ValidatePinModel;
        var extra = _sim.Get("card");
        return Ok(new
        {
            _card.EnableCardService,
            respondToCardIn = cardIn.RespondToRPC,
            respondToCardOut = cardOut.RespondToRPC,
            respondToPinValidation = pin.RespondToRPC,
            playerCardNumber = extra.TryGetValue("playerCardNumber", out var pcn) ? pcn : null,
            playerName = cardIn.FirstName,
            pinValidationResult = pin.IsValidPin
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        var cardIn = IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer.CardInViewModel.CardInModel;
        var cardOut = IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer.PlayerCardOutViewModel.PlayerCardOutModel;
        var pin = IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPin.ValidatePinViewModel.ValidatePinModel;

        if (u.TryGetValue("respondToCardIn", out var rci)) cardIn.RespondToRPC = Convert.ToBoolean(rci);
        if (u.TryGetValue("respondToCardOut", out var rco)) cardOut.RespondToRPC = Convert.ToBoolean(rco);
        if (u.TryGetValue("respondToPinValidation", out var rpv)) pin.RespondToRPC = Convert.ToBoolean(rpv);
        if (u.TryGetValue("playerName", out var pn) && pn is not null) cardIn.FirstName = pn.ToString();
        if (u.TryGetValue("pinValidationResult", out var pvr)) pin.IsValidPin = Convert.ToBoolean(pvr);

        // No model home for the player card number on the response side.
        if (u.TryGetValue("playerCardNumber", out var pcn)) _sim.Set("card", "playerCardNumber", pcn);

        return GetState();
    }
}

[ApiController]
[Route("api/bonus")]
public class BonusApiController : ControllerBase
{
    private readonly BonusViewModel _bonus;
    private readonly SimFeatureState _sim;
    public BonusApiController(BonusViewModel bonus, SimFeatureState sim) { _bonus = bonus; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var m = IBonusViewModel.IBonusModel;
        var extra = _sim.Get("bonus");
        return Ok(new
        {
            respondToBonus = m.RespondToRPC,
            bonusAmount = extra.TryGetValue("bonusAmount", out var ba) ? ba : null,
            bonusMultiplier = extra.TryGetValue("bonusMultiplier", out var bm) ? bm : null,
            levelIds = extra.TryGetValue("levelIds", out var li) ? li : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        if (u.TryGetValue("respondToBonus", out var rb)) IBonusViewModel.IBonusModel.RespondToRPC = Convert.ToBoolean(rb);
        // bonusAmount/bonusMultiplier/levelIds are RPC parameters, not response-control model fields.
        var extra = new Dictionary<string, object?>();
        if (u.TryGetValue("bonusAmount", out var ba)) extra["bonusAmount"] = ba;
        if (u.TryGetValue("bonusMultiplier", out var bm)) extra["bonusMultiplier"] = bm;
        if (u.TryGetValue("levelIds", out var li)) extra["levelIds"] = li;
        if (extra.Count > 0) _sim.Set("bonus", extra);
        return GetState();
    }
}

[ApiController]
[Route("api/eft")]
public class EftApiController : ControllerBase
{
    private readonly IEftViewModel _eft;
    private readonly SimFeatureState _sim;
    public EftApiController(IEftViewModel eft, SimFeatureState sim) { _eft = eft; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var m = IEftViewModel.IEftModel;
        var extra = _sim.Get("eft");
        return Ok(new
        {
            respondToEft = m.RespondToRPC,
            transferStatus = m.HostException.ToString(),
            availableBalance = m.AuthCashableAmt
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        var m = IEftViewModel.IEftModel;
        if (u.TryGetValue("respondToEft", out var re)) m.RespondToRPC = Convert.ToBoolean(re);
        if (u.TryGetValue("transferStatus", out var ts) && ts is not null
            && Enum.TryParse<IGT.FloorNet.EX.Wat.t_watException>(ts.ToString(), true, out var ex))
            m.HostException = ex;
        if (u.TryGetValue("availableBalance", out var ab) && ab is not null) m.AuthCashableAmt = Convert.ToInt64(ab);
        return GetState();
    }
}

[ApiController]
[Route("api/gat")]
public class GatApiController : ControllerBase
{
    private readonly IGatViewModel _gat;
    private readonly SimFeatureState _sim;
    public GatApiController(IGatViewModel gat, SimFeatureState sim) { _gat = gat; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var m = IGatViewModel.IGatModel;
        var extra = _sim.Get("gat");
        return Ok(new
        {
            respondToGat = m.RespondToRPC,
            gatStatus = extra.TryGetValue("gatStatus", out var gs) ? gs : null,
            componentHash = extra.TryGetValue("componentHash", out var ch) ? ch : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        if (u.TryGetValue("respondToGat", out var rg)) IGatViewModel.IGatModel.RespondToRPC = Convert.ToBoolean(rg);
        // gatStatus/componentHash have no single model field (GAT uses per-component pass/fail).
        var extra = new Dictionary<string, object?>();
        if (u.TryGetValue("gatStatus", out var gs)) extra["gatStatus"] = gs;
        if (u.TryGetValue("componentHash", out var ch)) extra["componentHash"] = ch;
        if (extra.Count > 0) _sim.Set("gat", extra);
        return GetState();
    }
}

[ApiController]
[Route("api/handpay")]
public class HandpayApiController : ControllerBase
{
    private readonly HandPayViewModel _hp;
    private readonly SimFeatureState _sim;
    public HandpayApiController(HandPayViewModel hp, SimFeatureState sim) { _hp = hp; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var m = HandPayViewModel.HandPayResponseModel;
        var extra = _sim.Get("handpay");
        return Ok(new
        {
            respondToHandpay = m.RespondToRPC,
            identity = m.Identity,
            pouchPayEnable = m.PouchPayEnable,
            keyToCreditEnable = m.KeyToCreditEnable,
            selfServeOption = m.SelfServeOption.ToString(),
            autoKeyOff = extra.TryGetValue("autoKeyOff", out var ako) ? ako : null,
            keyOffType = extra.TryGetValue("keyOffType", out var kot) ? kot : null,
            resetAmount = extra.TryGetValue("resetAmount", out var ra) ? ra : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        var m = HandPayViewModel.HandPayResponseModel;
        if (u.TryGetValue("respondToHandpay", out var rh) && rh is not null) m.RespondToRPC = Convert.ToBoolean(rh);
        if (u.TryGetValue("keyToCreditEnable", out var kc) && kc is not null) m.KeyToCreditEnable = Convert.ToBoolean(kc);
        if (u.TryGetValue("pouchPayEnable", out var pp) && pp is not null) m.PouchPayEnable = Convert.ToBoolean(pp);

        // autoKeyOff/keyOffType/resetAmount have no direct model home; retain in-memory.
        var extra = new Dictionary<string, object?>();
        foreach (var key in new[] { "autoKeyOff", "keyOffType", "resetAmount" })
        {
            if (u.TryGetValue(key, out var v)) extra[key] = v;
        }
        if (extra.Count > 0) _sim.Set("handpay", extra);
        return GetState();
    }

    [HttpPost("reset")]
    public IActionResult Reset([FromBody] Dictionary<string, object?>? u)
    {
        // Handpay reset is local model state (no outbound RPC); clear the response model
        // and increment the identity sequence so the next response uses a fresh PK.
        var m = HandPayViewModel.HandPayResponseModel;
        m.Clear();
        m.IncrementIdentityPK();
        if (u is not null && u.TryGetValue("amount", out var amt)) _sim.Set("handpay", "resetAmount", amt);
        return GetState();
    }
}

[ApiController]
[Route("api/wat")]
public class WatApiController : ControllerBase
{
    private readonly RequestTransferViewModel _wat;
    private readonly ResponseViewModel _response;
    private readonly SimFeatureState _sim;
    public WatApiController(RequestTransferViewModel wat, ResponseViewModel response, SimFeatureState sim) { _wat = wat; _response = response; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var extra = _sim.Get("wat");
        return Ok(new
        {
            respondToWat = _wat.RpcProcess,
            accountId = extra.TryGetValue("accountId", out var a) ? a : null,
            watBalance = extra.TryGetValue("watBalance", out var b) ? b : null,
            transferResult = extra.TryGetValue("transferResult", out var t) ? t : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        if (u.TryGetValue("respondToWat", out var rw) && rw is not null) _wat.RpcProcess = Convert.ToBoolean(rw);

        var extra = new Dictionary<string, object?>();
        foreach (var key in new[] { "accountId", "watBalance", "transferResult" })
        {
            if (u.TryGetValue(key, out var v)) extra[key] = v;
        }
        if (extra.Count > 0) _sim.Set("wat", extra);
        return GetState();
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] WatTransferRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        var m = RequestTransferViewModel.RequestTransferModel;
        // Accept either the enum name ("from_EGM"/"to_EGM") or the numeric form
        // sent by the WAT panel dropdown (0 = To EGM, 1 = From EGM).
        var dir = (req.Direction ?? string.Empty).Trim();
        var direction =
            (string.Equals(dir, "from_EGM", StringComparison.OrdinalIgnoreCase) || dir == "1")
                ? IGT.FloorNet.EX.Wat.t_transferDirection.from_EGM
                : IGT.FloorNet.EX.Wat.t_transferDirection.to_EGM;

        var requestId = string.IsNullOrWhiteSpace(m.RequestId) ? Guid.NewGuid().ToString() : m.RequestId;
        var cardId = req.AccountId ?? m.CardId ?? string.Empty;

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iSmibWat.requestTransfer(
            requestId,
            m.ResourceId ?? string.Empty,
            cardId,
            m.PlayerId,
            m.CardInCount,
            direction,
            req.Amount,
            m.ReqPromoAmt,
            m.ReqNonCashAmt,
            m.PrintTicket,
            m.Jwt ?? string.Empty);

        _response.LogRpcResponse($"requestTransfer({direction}, {req.Amount}) → {req.Uid}",
            new { target = req.Uid, requestId, cardId, direction = direction.ToString(), amount = req.Amount }, resp, RpcCallContext.Current);
        return Ok(resp);
    }
}

public class WatTransferRequest
{
    public string Uid { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string? Direction { get; set; }
    public string? AccountId { get; set; }
}

[ApiController]
[Route("api/registration")]
public class RegApiController : ControllerBase
{
    private readonly SmibRegistrationTracker _tracker;
    public RegApiController(SmibRegistrationTracker tracker)
    {
        _tracker = tracker;
    }

    [HttpGet("smibs")]
    public IActionResult GetSmibs() => Ok(_tracker.GetAllSmibs());
}

[ApiController]
[Route("api/checkin")]
public class CheckinApiController : ControllerBase
{
    private readonly RegistrationViewModel _reg;
    private readonly SmibRegistrationTracker _tracker;
    public CheckinApiController(RegistrationViewModel reg, SmibRegistrationTracker tracker)
    {
        _reg = reg;
        _tracker = tracker;
    }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { autoRegister = Startup.AutoRegister });

    [HttpGet("defaults")]
    public IActionResult GetDefaults([FromQuery] string? uid = null)
    {
        var c = CheckinDefaultsResolver.Resolve(Startup.Configuration, uid);
        return Ok(new SetRegistrationRequest
        {
            Uid = uid ?? "",
            SiteId = c.SiteId,
            MachineNum = c.MachineNum,
            MachineLoc = c.MachineLoc,
            Enabled = c.Enabled,
            Registered = c.Registered,
            NotRegisteredReason = c.NotRegisteredReason,
            Vip = c.Vip,
            ReportDenomId = c.ReportDenomId,
            PointsCount = c.PointsCount,
            PointsAward = c.PointsAward,
            MachineStatus = c.MachineStatus,
            HaveInitialMeters = c.HaveInitialMeters,
            TitoEnabled = c.TitoEnabled,
            TruePlayerWinEnabled = c.TruePlayerWinEnabled,
            MdmgEnabled = c.MdmgEnabled,
            HotPlayerPeriod = c.HotPlayerPeriod,
            HotPlayerWagers = c.HotPlayerWagers,
            HotPlayerGames = c.HotPlayerGames,
            HotPlayerInactivityTimer = c.HotPlayerInactivityTimer,
            BonusEnabled = c.BonusEnabled
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] RegStateRequest body)
    {
        Startup.AutoRegister = body.AutoRegister;
        return Ok(new { autoRegister = Startup.AutoRegister });
    }

    [HttpPost("set")]
    public async Task<IActionResult> SetRegistration([FromBody] SetRegistrationRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iReg.setRegistration(
            req.Enabled,
            req.Registered,
            req.NotRegisteredReason,
            req.Vip,
            req.MachineNum,
            req.MachineLoc,
            req.SiteId,
            req.ReportDenomId,
            req.PointsCount,
            req.PointsAward,
            string.IsNullOrEmpty(req.MachineStatus) ? 'A' : req.MachineStatus[0],
            req.HaveInitialMeters,
            req.TitoEnabled,
            req.TruePlayerWinEnabled,
            req.MdmgEnabled,
            req.HotPlayerPeriod,
            req.HotPlayerWagers,
            req.HotPlayerGames,
            req.HotPlayerInactivityTimer,
            req.BonusEnabled);

        _tracker.MarkRegistered(req.Uid);
        return Ok(new { success = true, registered = resp?.registered, uid = req.Uid });
    }

    [HttpPost("get")]
    public async Task<IActionResult> GetRegistration([FromBody] Dictionary<string, string> body)
    {
        var uid = body.ContainsKey("uid") ? body["uid"] : null;
        if (string.IsNullOrWhiteSpace(uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(uid);
        var resp = await Startup._iReg.getRegistration();
        return Ok(resp);
    }
}

public class SetRegistrationRequest
{
    public string Uid { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public bool Registered { get; set; } = true;
    public string? NotRegisteredReason { get; set; }
    public bool Vip { get; set; }
    public long MachineNum { get; set; }
    public string? MachineLoc { get; set; }
    public string? SiteId { get; set; }
    public long ReportDenomId { get; set; }
    public long PointsCount { get; set; }
    public long PointsAward { get; set; }
    public string MachineStatus { get; set; } = "A";
    public bool HaveInitialMeters { get; set; }
    public bool TitoEnabled { get; set; }
    public bool TruePlayerWinEnabled { get; set; }
    public bool MdmgEnabled { get; set; }
    public long HotPlayerPeriod { get; set; }
    public long HotPlayerWagers { get; set; }
    public long HotPlayerGames { get; set; }
    public long HotPlayerInactivityTimer { get; set; }
    public bool BonusEnabled { get; set; }
}

public class RegStateRequest
{
    public bool AutoRegister { get; set; } = true;
}

[ApiController]
[Route("api/rg")]
public class RgApiController : ControllerBase
{
    private readonly SimFeatureState _sim;
    private readonly ResponseViewModel _response;
    public RgApiController(SimFeatureState sim, ResponseViewModel response) { _sim = sim; _response = response; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var extra = _sim.Get("rg");
        return Ok(new
        {
            respondToRg = extra.TryGetValue("respondToRg", out var rr) ? rr : null,
            disableOnCardOut = extra.TryGetValue("disableOnCardOut", out var d) ? d : null,
            lockBillValidator = extra.TryGetValue("lockBillValidator", out var l) ? l : null,
            leaseMinutes = extra.TryGetValue("leaseMinutes", out var lm) ? lm : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        // RG is an outbound (to-SMIB) interface with no response-control model in this sim;
        // retain configured flags in-memory so the UI round-trips. RG actions are POST endpoints.
        var extra = new Dictionary<string, object?>();
        foreach (var key in new[] { "respondToRg", "disableOnCardOut", "lockBillValidator", "leaseMinutes" })
        {
            if (u.TryGetValue(key, out var v)) extra[key] = v;
        }
        if (extra.Count > 0) _sim.Set("rg", extra);
        return GetState();
    }

    [HttpPost("disable-on-card-out")]
    public async Task<IActionResult> DisableOnCardOut([FromBody] RgDeviceRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            return BadRequest(new { error = "deviceId is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.DeviceId);
        var resp = await Startup._iRG.DisableEGMOnCardOut(true, req.DisableKey ?? string.Empty);
        _response.LogRpcResponse($"DisableEGMOnCardOut(true) → {req.DeviceId}", new { target = req.DeviceId, disableKey = req.DisableKey }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("enable-with-lease")]
    public async Task<IActionResult> EnableWithLease([FromBody] RgLeaseRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            return BadRequest(new { error = "deviceId is required" });

        var disableAt = DateTime.UtcNow.AddMinutes(req.LeaseMinutes);
        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.DeviceId);
        var resp = await Startup._iRG.EnableEGMwithLease(disableAt, req.DisableKey ?? string.Empty);
        _response.LogRpcResponse($"EnableEGMwithLease(lease={req.LeaseMinutes}m, at={disableAt:o}) → {req.DeviceId}", new { target = req.DeviceId, leaseMinutes = req.LeaseMinutes, disableAt }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("lock-bv")]
    public async Task<IActionResult> LockBv([FromBody] RgDeviceRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            return BadRequest(new { error = "deviceId is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.DeviceId);
        var resp = await Startup._iRG.LockBV(true, req.LockKey ?? string.Empty);
        _response.LogRpcResponse($"LockBV(true) → {req.DeviceId}", new { target = req.DeviceId, lockKey = req.LockKey }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("lock-bv-keys")]
    public async Task<IActionResult> LockBvKeys([FromBody] RgDeviceRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            return BadRequest(new { error = "deviceId is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.DeviceId);
        var resp = await Startup._iRG.GetLockBVKeys();
        _response.LogRpcResponse($"GetLockBVKeys → {req.DeviceId}", new { target = req.DeviceId }, resp, RpcCallContext.Current);
        return Ok(resp);
    }
}

public class RgDeviceRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public string? LockKey { get; set; }
    public string? DisableKey { get; set; }
}

public class RgLeaseRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public int LeaseMinutes { get; set; }
    public string? DisableKey { get; set; }
}

[ApiController]
[Route("api/discovery")]
public class DiscoveryApiController : ControllerBase
{
    private readonly DiscoveryViewModel _disc;
    private readonly IGT.FloorNet.EX.Registration.iDiscovery _discovery;
    public DiscoveryApiController(DiscoveryViewModel disc, IGT.FloorNet.EX.Registration.iDiscovery discovery)
    {
        _disc = disc;
        _discovery = discovery;
    }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { _disc.DiscoveryModel });

    [HttpGet("smib-interfaces")]
    public async Task<IActionResult> GetSmibInterfaces([FromQuery] string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
        {
            return BadRequest(new { error = "uid is required" });
        }

        var resp = await _discovery.getSmibInterfaces(uid);
        return Ok(resp);
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, bool> u)
    {
        var m = _disc.DiscoveryModel;
        if (u.ContainsKey("cardlessInterface")) m.CardlessInterface = u["cardlessInterface"];
        if (u.ContainsKey("eftInterface")) m.EftInterface = u["eftInterface"];
        if (u.ContainsKey("playerInterface")) m.PlayerInterface = u["playerInterface"];
        if (u.ContainsKey("bonusInterface")) m.BonusInterface = u["bonusInterface"];
        if (u.ContainsKey("diagsInterface")) m.DiagsInterface = u["diagsInterface"];
        if (u.ContainsKey("gatInterface")) m.GatInterface = u["gatInterface"];
        if (u.ContainsKey("confInterface")) m.ConfInterface = u["confInterface"];
        if (u.ContainsKey("downloadInterface")) m.DownloadInterface = u["downloadInterface"];
        if (u.ContainsKey("amlInterface")) m.AMLInterface = u["amlInterface"];
        if (u.ContainsKey("titoInterface")) m.TitoInterface = u["titoInterface"];
        if (u.ContainsKey("pinInterface")) m.PinInterface = u["pinInterface"];
        if (u.ContainsKey("markerInterface")) m.MarkerInterface = u["markerInterface"];
        if (u.ContainsKey("rgInterface")) m.RGInterface = u["rgInterface"];
        if (u.ContainsKey("regInterface")) m.RegInterface = u["regInterface"];
        if (u.ContainsKey("handpayInterface")) m.HandpayInterface = u["handpayInterface"];
        if (u.ContainsKey("pcsInterface")) m.PCSInterface = u["pcsInterface"];
        if (u.ContainsKey("ismInterface")) m.IsmInterface = u["ismInterface"];
        if (u.ContainsKey("watInterface")) m.WatInterface = u["watInterface"];
        return Ok(new { _disc.DiscoveryModel });
    }

    [HttpPost("clear")]
    public IActionResult Clear()
    {
        _disc.DiscoveryModel.Clear();
        return Ok(new { _disc.DiscoveryModel });
    }
}

[ApiController]
[Route("api/aml")]
public class AmlApiController : ControllerBase
{
    private readonly AMLViewModel _aml;
    private readonly SimFeatureState _sim;
    public AmlApiController(AMLViewModel aml, SimFeatureState sim) { _aml = aml; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var extra = _sim.Get("aml");
        return Ok(new
        {
            _aml.DailyCashLimit,
            _aml.DailyCashAggregate,
            _aml.LargestBillDenom,
            respondToAml = extra.TryGetValue("respondToAml", out var ra) ? ra : null,
            cashAggregationThreshold = _aml.DailyCashLimit,
            sessionThreshold = extra.TryGetValue("sessionThreshold", out var st) ? st : null
        });
    }

    [HttpPut("state")]
    public IActionResult Update([FromBody] Dictionary<string, object?> u)
    {
        if (u.TryGetValue("dailyCashLimit", out var dcl) && dcl is not null) _aml.DailyCashLimit = Convert.ToInt64(dcl);
        // cashAggregationThreshold maps onto the same DailyCashLimit when dailyCashLimit isn't sent
        else if (u.TryGetValue("cashAggregationThreshold", out var cat) && cat is not null) _aml.DailyCashLimit = Convert.ToInt64(cat);
        if (u.TryGetValue("largestBillDenom", out var lbd) && lbd is not null) _aml.LargestBillDenom = Convert.ToInt64(lbd);

        // respondToAml/sessionThreshold have no model home; retain in-memory.
        var extra = new Dictionary<string, object?>();
        if (u.TryGetValue("respondToAml", out var ra)) extra["respondToAml"] = ra;
        if (u.TryGetValue("sessionThreshold", out var st)) extra["sessionThreshold"] = st;
        if (extra.Count > 0) _sim.Set("aml", extra);

        return GetState();
    }
}

[ApiController]
[Route("api/meters")]
public class MetersApiController : ControllerBase
{
    private readonly MetersSvcViewModel _meters;
    private readonly ResponseViewModel _response;
    public MetersApiController(MetersSvcViewModel meters, ResponseViewModel response) { _meters = meters; _response = response; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Meters Service active" });

    [HttpPost("request")]
    public async Task<IActionResult> Request([FromBody] MetersRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long meterTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;
        char meterType = string.IsNullOrEmpty(req.MeterType) ? 'A' : req.MeterType[0];

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iMeters.getMeters(meterType, meterTime);
        _response.LogRpcResponse($"getMeters('{meterType}') → {req.Uid}", new { target = req.Uid, meterType = meterType.ToString(), meterTime }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("mga-descriptions")]
    public async Task<IActionResult> MgaDescriptions([FromBody] MetersRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iMeters.getMgaDescriptions();
        _response.LogRpcResponse($"getMgaDescriptions → {req.Uid}", new { target = req.Uid }, resp, RpcCallContext.Current);
        return Ok(resp);
    }
}

public class MetersRequest
{
    public string Uid { get; set; } = string.Empty;
    public string? MeterType { get; set; }
}

[ApiController]
[Route("api/config")]
public class ConfigApiController : ControllerBase
{
    private readonly ResponseViewModel _response;
    public ConfigApiController(ResponseViewModel response) { _response = response; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Config Service active" });

    [HttpPost("getOptions")]
    public async Task<IActionResult> GetOptionList([FromBody] ConfigGetRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var categories = new List<string> { req.Category };
        var resp = await Startup._iConfig.getOptionList(categories);
        _response.LogRpcResponse($"getOptionList({req.Category}) → {req.Uid}", new { target = req.Uid, category = req.Category }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("getOptionsShort")]
    public async Task<IActionResult> GetOptionListShort([FromBody] Dictionary<string, string> body)
    {
        var uid = body.ContainsKey("uid") ? body["uid"] : null;
        if (string.IsNullOrWhiteSpace(uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(uid);
        var resp = await Startup._iConfig.getOptionListShort();
        _response.LogRpcResponse($"getOptionListShort → {uid}", new { target = uid }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("setOptions")]
    public async Task<IActionResult> SetOptionChange([FromBody] ConfigSetRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var categoryOptions = new List<IGT.FloorNet.EX.OptionConfig.categoryOption>
        {
            new IGT.FloorNet.EX.OptionConfig.categoryOption
            {
                messageCategory = req.Category,
                optionItems = req.Options.Select(o => new IGT.FloorNet.EX.OptionConfig.t_optionItem
                {
                    name = o.Name,
                    value = ParseOptionValue(o.Value)
                }).ToList()
            }
        };
        var resp = await Startup._iConfig.setOptionChange(categoryOptions);
        _response.LogRpcResponse($"setOptionChange({req.Category}) → {req.Uid}", new { target = req.Uid, category = req.Category, options = req.Options }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    private static object ParseOptionValue(string raw)
    {
        if (bool.TryParse(raw, out var b)) return b;
        if (long.TryParse(raw, out var l)) return l;
        if (double.TryParse(raw, out var d)) return d;
        return raw;
    }
}

public class ConfigGetRequest
{
    public string Uid { get; set; } = string.Empty;
    public string Category { get; set; } = "EgmSettings";
}

public class ConfigSetRequest
{
    public string Uid { get; set; } = string.Empty;
    public string Category { get; set; } = "EgmSettings";
    public List<ConfigOptionItem> Options { get; set; } = new();
}

public class ConfigOptionItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

[ApiController]
[Route("api/progress")]
public class ProgressApiController : ControllerBase
{
    private readonly ResponseViewModel _response;
    private readonly SimFeatureState _sim;
    public ProgressApiController(ResponseViewModel response, SimFeatureState sim) { _response = response; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var extra = _sim.Get("progress");
        return Ok(new
        {
            _response.ProgressList,
            levels = extra.TryGetValue("levels", out var lv) ? lv : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        // Progressive levels are event-only; retain configured levels in-memory for the UI.
        if (u.TryGetValue("levels", out var lv)) _sim.Set("progress", "levels", lv);
        return GetState();
    }
}

[ApiController]
[Route("api/download")]
public class DownloadApiController : ControllerBase
{
    private readonly DownloadViewModel _dl;
    private readonly ResponseViewModel _response;
    public DownloadApiController(DownloadViewModel dl, ResponseViewModel response) { _dl = dl; _response = response; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Download Service active" });

    [HttpPost("notify")]
    public IActionResult Notify([FromBody] DownloadNotifyRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            return BadRequest(new { error = "deviceId is required" });
        if (string.IsNullOrWhiteSpace(req.Url))
            return BadRequest(new { error = "url is required" });

        var requestId = string.IsNullOrWhiteSpace(req.RequestId) ? Guid.NewGuid().ToString() : req.RequestId;
        var fileName = string.IsNullOrWhiteSpace(req.FileName)
            ? System.IO.Path.GetFileName(new Uri(req.Url, UriKind.RelativeOrAbsolute).IsAbsoluteUri ? new Uri(req.Url).AbsolutePath : req.Url)
            : req.FileName;

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.DeviceId);
        // addPackage is fire-and-forget (void) on the iDownload proxy.
        Startup._iDownload.addPackage(req.Url, req.User ?? string.Empty, req.Password ?? string.Empty,
            requestId, fileName, req.FileSize, req.Crc ?? string.Empty);

        _response.LogRpcResponse($"addPackage({fileName}) → {req.DeviceId}",
            new { target = req.DeviceId, url = req.Url, requestId, fileName, fileGroup = req.FileGroup },
            new { queued = true }, RpcCallContext.Current);
        return Ok(new { queued = true, requestId, fileName });
    }
}

public class DownloadNotifyRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? FileGroup { get; set; }
    public string? FileName { get; set; }
    public string? RequestId { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public long FileSize { get; set; }
    public string? Crc { get; set; }
}

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsApiController : ControllerBase
{
    private readonly DiagsViewModel _diags;
    private readonly ResponseViewModel _response;
    public DiagnosticsApiController(DiagsViewModel diags, ResponseViewModel response) { _diags = diags; _response = response; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Diagnostics Service active" });

    [HttpPost("run")]
    public async Task<IActionResult> Run([FromBody] DiagnosticsRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iDiags.diagnostics();
        _response.LogRpcResponse($"diagnostics → {req.Uid}", new { target = req.Uid }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] DiagnosticsRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        var resetType = req.Hard
            ? IGT.FloorNet.EX.Diagnostics.t_resetTypes.hard
            : IGT.FloorNet.EX.Diagnostics.t_resetTypes.soft;

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iDiags.reset(resetType, 0);
        _response.LogRpcResponse($"reset({resetType}) → {req.Uid}", new { target = req.Uid, resetType = resetType.ToString() }, resp, RpcCallContext.Current);
        return Ok(resp);
    }
}

public class DiagnosticsRequest
{
    public string Uid { get; set; } = string.Empty;
    public bool Hard { get; set; }
}

[ApiController]
[Route("api/events")]
public class EventsApiController : ControllerBase
{
    private readonly EventViewModel _events;
    private readonly ResponseViewModel _response;
    public EventsApiController(EventViewModel events, ResponseViewModel response) { _events = events; _response = response; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Events Service active" });

    [HttpGet("audit")]
    public IActionResult GetAudit() => Ok(new { log = _response.Model.Response ?? string.Empty });
}

[ApiController]
[Route("api/cardless")]
public class CardlessApiController : ControllerBase
{
    private readonly GetNonceRespViewModel _cardless;
    private readonly SimFeatureState _sim;
    public CardlessApiController(GetNonceRespViewModel cardless, SimFeatureState sim) { _cardless = cardless; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var m = GetNonceRespViewModel.NonceResponseModel;
        return Ok(new
        {
            respondToCardless = m.RespondToRPC,
            nonce = m.CustomNonce,
            sendCustomNonce = m.SendCustomNonce
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        var m = GetNonceRespViewModel.NonceResponseModel;
        if (u.TryGetValue("respondToCardless", out var rc)) m.RespondToRPC = Convert.ToBoolean(rc);
        if (u.TryGetValue("nonce", out var n) && n is not null)
        {
            m.CustomNonce = Convert.ToInt64(n);
            m.SendCustomNonce = true; // a supplied nonce only takes effect when this flag is set
        }
        return GetState();
    }
}

[ApiController]
[Route("api/ism")]
public class IsmApiController : ControllerBase
{
    private readonly AdjustAccountViewModel _ism;
    private readonly SimFeatureState _sim;
    public IsmApiController(AdjustAccountViewModel ism, SimFeatureState sim) { _ism = ism; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var s = _sim.Get("ism");
        return Ok(new
        {
            respondToIsm = s.TryGetValue("respondToIsm", out var r) ? r : true,
            accountBalance = s.TryGetValue("accountBalance", out var b) ? b : 0,
            publicKey = s.TryGetValue("publicKey", out var k) ? k : string.Empty
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        // ISM response config has no direct desktop model property; persist to the in-memory feature bag.
        if (u.TryGetValue("respondToIsm", out var r)) _sim.Set("ism", "respondToIsm", r);
        if (u.TryGetValue("accountBalance", out var b)) _sim.Set("ism", "accountBalance", b);
        if (u.TryGetValue("publicKey", out var k)) _sim.Set("ism", "publicKey", k);
        return GetState();
    }
}

[ApiController]
[Route("api/marker")]
public class MarkerApiController : ControllerBase
{
    private readonly MarkerViewModel _marker;
    private readonly SimFeatureState _sim;
    public MarkerApiController(MarkerViewModel marker, SimFeatureState sim) { _marker = marker; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var m = _marker.GetMarkerBalanceResp;
        var extra = _sim.Get("marker");
        return Ok(new
        {
            respondToMarker = m.RespondToRPC,
            markerBalance = m.MarkerBalance,
            creditLimit = m.CreditLimit,
            statusCode = m.StatusCode,
            immediateResponse = m.InmediateResponse,
            proceedResponse = m.ProceedResponse,
            repayResult = extra.TryGetValue("repayResult", out var rr) ? rr : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        var m = _marker.GetMarkerBalanceResp;
        if (u.TryGetValue("respondToMarker", out var rm) && rm is not null) m.RespondToRPC = Convert.ToBoolean(rm);
        if (u.TryGetValue("markerBalance", out var mb) && mb is not null) m.MarkerBalance = Convert.ToInt32(mb);
        if (u.TryGetValue("creditLimit", out var cl) && cl is not null) m.CreditLimit = Convert.ToInt32(cl);
        if (u.TryGetValue("statusCode", out var sc) && sc is not null) m.StatusCode = Convert.ToInt32(sc);
        if (u.TryGetValue("immediateResponse", out var ir) && ir is not null) m.InmediateResponse = Convert.ToBoolean(ir);
        if (u.TryGetValue("repayResult", out var rr)) _sim.Set("marker", "repayResult", rr);
        return GetState();
    }

    [HttpPost("repay")]
    public IActionResult Repay([FromBody] Dictionary<string, object?> body)
    {
        // Marker repay is a synchronous response toggle (model-only, no outbound RPC).
        if (body.TryGetValue("amount", out var amt)) _sim.Set("marker", "lastRepayAmount", amt);
        _marker.GetMarkerBalanceResp.ProceedResponse = true;
        return GetState();
    }
}

[ApiController]
[Route("api/pcs")]
public class PcsApiController : ControllerBase
{
    private readonly PCSViewModel _pcs;
    private readonly SimFeatureState _sim;
    public PcsApiController(PCSViewModel pcs, SimFeatureState sim) { _pcs = pcs; _sim = sim; }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        var extra = _sim.Get("pcs");
        return Ok(new
        {
            respondToPcs = extra.TryGetValue("respondToPcs", out var rp) ? rp : null,
            pointsBalance = extra.TryGetValue("pointsBalance", out var pb) ? pb : null,
            cashbackBalance = extra.TryGetValue("cashbackBalance", out var cb) ? cb : null,
            sessionLimit = extra.TryGetValue("sessionLimit", out var sl) ? sl : null,
            timeLimit = extra.TryGetValue("timeLimit", out var tl) ? tl : null
        });
    }

    [HttpPut("state")]
    public IActionResult UpdateState([FromBody] Dictionary<string, object?> u)
    {
        // PCS uses a limit collection (PCSContainer); these scalar fields have no direct
        // response-control model property, so retain them in-memory for UI round-tripping.
        var extra = new Dictionary<string, object?>();
        foreach (var key in new[] { "respondToPcs", "pointsBalance", "cashbackBalance", "sessionLimit", "timeLimit" })
        {
            if (u.TryGetValue(key, out var v)) extra[key] = v;
        }
        if (extra.Count > 0) _sim.Set("pcs", extra);
        return GetState();
    }
}

[ApiController]
[Route("api/egmcontrol")]
public class EgmControlApiController : ControllerBase
{
    private readonly ResponseViewModel _response;
    public EgmControlApiController(ResponseViewModel response) { _response = response; }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableEgm([FromBody] DisableEgmRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });
        if (string.IsNullOrWhiteSpace(req.DisableKey))
            return BadRequest(new { error = "disableKey is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iReg.disableEGM(req.State, req.DisableKey);
        _response.LogRpcResponse($"disableEGM(state={req.State}, key={req.DisableKey}) → {req.Uid}", new { target = req.Uid, state = req.State, disableKey = req.DisableKey }, resp, RpcCallContext.Current);
        var result = resp ?? (object)new { success = true, message = $"disableEGM({req.State}) sent to {req.Uid}" };
        return Ok(result);
    }

    [HttpPost("disable-keys")]
    public async Task<IActionResult> GetDisableKeys([FromBody] UidRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iReg.getDisableKeys();
        _response.LogRpcResponse($"getDisableKeys → {req.Uid}", new { target = req.Uid }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("lock")]
    public async Task<IActionResult> LockEgm([FromBody] LockEgmRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });
        if (string.IsNullOrWhiteSpace(req.LockKey))
            return BadRequest(new { error = "lockKey is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iReg.lockEGM(req.Timer, req.State, req.LockKey);
        _response.LogRpcResponse($"lockEGM(timer={req.Timer}, state={req.State}, key={req.LockKey}) → {req.Uid}", new { target = req.Uid, timer = req.Timer, state = req.State, lockKey = req.LockKey }, resp, RpcCallContext.Current);
        var result = resp ?? (object)new { success = true, message = $"lockEGM({req.State}) sent to {req.Uid}" };
        return Ok(result);
    }

    [HttpPost("lock-keys")]
    public async Task<IActionResult> GetLockKeys([FromBody] UidRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iReg.getLockKeys();
        _response.LogRpcResponse($"getLockKeys → {req.Uid}", new { target = req.Uid }, resp, RpcCallContext.Current);
        return Ok(resp);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetEgm([FromBody] ResetEgmRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Uid))
            return BadRequest(new { error = "uid is required" });

        RpcProxyContext.Current = RpcProxyContext.ToSMIB(req.Uid);
        var resp = await Startup._iReg.reset(req.Hard);
        _response.LogRpcResponse($"reset(hard={req.Hard}) → {req.Uid}", new { target = req.Uid, hard = req.Hard }, resp, RpcCallContext.Current);
        var result = resp ?? (object)new { success = true, message = $"reset(hard={req.Hard}) sent to {req.Uid}" };
        return Ok(result);
    }
}

public class DisableEgmRequest
{
    public string Uid { get; set; } = string.Empty;
    public bool State { get; set; } = true;
    public string DisableKey { get; set; } = string.Empty;
}

public class LockEgmRequest
{
    public string Uid { get; set; } = string.Empty;
    public long Timer { get; set; }
    public bool State { get; set; } = true;
    public string LockKey { get; set; } = string.Empty;
}

public class ResetEgmRequest
{
    public string Uid { get; set; } = string.Empty;
    public bool Hard { get; set; }
}

public class UidRequest
{
    public string Uid { get; set; } = string.Empty;
}
