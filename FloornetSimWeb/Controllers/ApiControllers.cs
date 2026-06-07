using Microsoft.AspNetCore.Mvc;
using System.Linq;
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

    public TitoApiController(ValidationViewModel v, IssueViewModel i, CommitViewModel c, RedeemViewModel r)
    { _validation = v; _issue = i; _commit = c; _redeem = r; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new
    {
        Validation = new { _validation.Model.RespondToRPC, _validation.Model.SeedValue1, _validation.Model.SeedValue2, _validation.Model.IsValid },
        Issue = new { _issue.Model.RespondToRPC },
        Commit = new { _commit.Model.RespondToRPC, _commit.Model.TransactionId },
        Redeem = new { _redeem.Model.RespondToRPC }
    });

    [HttpPut("validation")]
    public IActionResult UpdateValidation([FromBody] Dictionary<string, object> update)
    {
        if (update.ContainsKey("respondToRPC")) _validation.Model.RespondToRPC = Convert.ToBoolean(update["respondToRPC"]);
        return Ok(new { message = "Updated" });
    }
}

[ApiController]
[Route("api/card")]
public class CardApiController : ControllerBase
{
    private readonly CardViewModel _card;
    public CardApiController(CardViewModel card) { _card = card; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { _card.EnableCardService });
}

[ApiController]
[Route("api/bonus")]
public class BonusApiController : ControllerBase
{
    private readonly BonusViewModel _bonus;
    public BonusApiController(BonusViewModel bonus) { _bonus = bonus; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Bonus Service active" });
}

[ApiController]
[Route("api/eft")]
public class EftApiController : ControllerBase
{
    private readonly IEftViewModel _eft;
    public EftApiController(IEftViewModel eft) { _eft = eft; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "EFT Service active" });
}

[ApiController]
[Route("api/gat")]
public class GatApiController : ControllerBase
{
    private readonly IGatViewModel _gat;
    public GatApiController(IGatViewModel gat) { _gat = gat; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "GAT Service active" });
}

[ApiController]
[Route("api/handpay")]
public class HandpayApiController : ControllerBase
{
    private readonly HandPayViewModel _hp;
    public HandpayApiController(HandPayViewModel hp) { _hp = hp; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Handpay Service active" });
}

[ApiController]
[Route("api/wat")]
public class WatApiController : ControllerBase
{
    private readonly RequestTransferViewModel _wat;
    public WatApiController(RequestTransferViewModel wat) { _wat = wat; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "WAT Service active" });
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
            CheckinDefaultsResolver.ToMachineStatusChar(req.MachineStatus),
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
    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "RG Service active" });
}

[ApiController]
[Route("api/discovery")]
public class DiscoveryApiController : ControllerBase
{
    private readonly DiscoveryViewModel _disc;
    public DiscoveryApiController(DiscoveryViewModel disc) { _disc = disc; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { _disc.DiscoveryModel });

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
    public AmlApiController(AMLViewModel aml) { _aml = aml; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new
    {
        _aml.DailyCashLimit,
        _aml.DailyCashAggregate,
        _aml.LargestBillDenom
    });

    [HttpPut("state")]
    public IActionResult Update([FromBody] Dictionary<string, object> u)
    {
        if (u.ContainsKey("dailyCashLimit")) _aml.DailyCashLimit = Convert.ToInt64(u["dailyCashLimit"]);
        if (u.ContainsKey("largestBillDenom")) _aml.LargestBillDenom = Convert.ToInt64(u["largestBillDenom"]);
        return Ok(new { message = "Updated" });
    }
}

[ApiController]
[Route("api/meters")]
public class MetersApiController : ControllerBase
{
    private readonly MetersSvcViewModel _meters;
    public MetersApiController(MetersSvcViewModel meters) { _meters = meters; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Meters Service active" });
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
    public ProgressApiController(ResponseViewModel response) { _response = response; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { _response.ProgressList });
}

[ApiController]
[Route("api/download")]
public class DownloadApiController : ControllerBase
{
    private readonly DownloadViewModel _dl;
    public DownloadApiController(DownloadViewModel dl) { _dl = dl; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Download Service active" });
}

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsApiController : ControllerBase
{
    private readonly DiagsViewModel _diags;
    public DiagnosticsApiController(DiagsViewModel diags) { _diags = diags; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Diagnostics Service active" });
}

[ApiController]
[Route("api/events")]
public class EventsApiController : ControllerBase
{
    private readonly EventViewModel _events;
    public EventsApiController(EventViewModel events) { _events = events; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Events Service active" });
}

[ApiController]
[Route("api/cardless")]
public class CardlessApiController : ControllerBase
{
    private readonly GetNonceRespViewModel _cardless;
    public CardlessApiController(GetNonceRespViewModel cardless) { _cardless = cardless; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Cardless Service active" });
}

[ApiController]
[Route("api/ism")]
public class IsmApiController : ControllerBase
{
    private readonly AdjustAccountViewModel _ism;
    public IsmApiController(AdjustAccountViewModel ism) { _ism = ism; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "ISM/Random Riches Service active" });
}

[ApiController]
[Route("api/marker")]
public class MarkerApiController : ControllerBase
{
    private readonly MarkerViewModel _marker;
    public MarkerApiController(MarkerViewModel marker) { _marker = marker; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "Marker Service active" });
}

[ApiController]
[Route("api/pcs")]
public class PcsApiController : ControllerBase
{
    private readonly PCSViewModel _pcs;
    public PcsApiController(PCSViewModel pcs) { _pcs = pcs; }

    [HttpGet("state")]
    public IActionResult GetState() => Ok(new { message = "PCS Service active" });
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
