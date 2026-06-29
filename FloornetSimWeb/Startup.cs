using IGT.FloorNet.EX.Auth;
using IGT.FloorNet.EX.Auth.evt;
using IGT.FloorNet.EX.BIS;
using IGT.FloorNet.EX.BIS.evt;
using IGT.FloorNet.EX.Bonus;
using IGT.FloorNet.EX.Cardless;
using IGT.FloorNet.EX.Diagnostics;
using IGT.FloorNet.EX.Download;
using IGT.FloorNet.EX.EFT;
using IGT.FloorNet.EX.evt;
using IGT.FloorNet.EX.Gameplay.evt;
using IGT.FloorNet.EX.Gat;
using IGT.FloorNet.EX.Handpay;
using IGT.FloorNet.EX.ISM;
using IGT.FloorNet.EX.Meters;
using IGT.FloorNet.EX.Meters.evt;
using IGT.FloorNet.EX.OptionConfig;
using IGT.FloorNet.EX.Player;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.EX.Registration.evt;
using IGT.FloorNet.EX.RG;
using IGT.FloorNet.EX.Tito;
using IGT.FloorNet.EX.Wat;
using IGT.FloorNet.MessageBus;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Authentication;
using IGT.FloorNet.Tools.ServiceSimulator.EventHandlers;
using IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc;
using IGT.FloorNet.Tools.ServiceSimulator.RpcProviders;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Bonus.BonusEvent;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPin;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Card.iPlayer;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Cardless;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Diags;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Discovery;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Download;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.AML;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Eft;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Event;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Gat;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.HandPay;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.IConf;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Marker;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.MetersSvc;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.ProgressEvent;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Reg;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.RG;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iSmibWat;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Wat.iWat.WatEvents;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using IGT.FloorNet.EX.Player.evt;
using IGT.FloorNet.Tools.ServiceSimulator.Services;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.ISM;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using Microsoft.Extensions.Options;

namespace IGT.FloorNet.Tools.ServiceSimulator
{
    public static class Startup
    {
        public static IConfiguration Configuration { get; set; }

        public static bool EnableCardService = false;
        public static bool EnableBonusMeters = false;
        public static bool EnableAuditEvents = true;
        public static bool EnableResponseView = true;
        public static bool AutoRegister = true;
        public static iDiscovery discovery;
        public static iAuthSub _iAuthSub;
        public static iReg _iReg;
        public static iRG _iRG;    
        public static iSvcReg _iSvcReg;        
        public static iSmibBns _iSmibBns;
        public static iSmibEft _iSmibEft;
        public static iSmibGat _iSmibGat;
        public static iSmibHandpay _iSmibHandpay;
        public static iSmibISM _iSmibISM;
        public static iSmibPlr _iSmibPlr;        
        public static iSmibWat _iSmibWat;
        public static iMeters _iMeters;
        public static iConfig _iConfig;
        public static iDownload _iDownload;
        public static iDiags _iDiags;
        public static iAML _iAML;
        public static iPCS _iPCS;
        //public static IMessageBusRpcProxy _imessageBusRpcProxy;
        public static ResponseViewModel responseViewModel;
        public static iBnsMgr _iBnsMgr;
        public static iAuth authProvider;
        public static Dictionary<long, string> dicAuthfun = new Dictionary<long, string>();

        private static List<Type> _viewModels = new List<Type> {
            typeof(ResponseViewModel),
            typeof(ValidationViewModel),
            typeof(IssueViewModel),
            typeof(CommitViewModel),
            typeof(RedeemViewModel),
            typeof(CardViewModel),
            typeof(ISmibBnsViewModel),
            typeof(DiscoveryViewModel),
            typeof(BonusViewModel),
            typeof(EventViewModel),
            typeof(MetersSvcViewModel),
            typeof(DiagsViewModel),
            typeof(MarkerViewModel),
            typeof(RegistrationViewModel),
            typeof(ResetViewModel),
            typeof(DisableEgmViewModel),
            typeof(GetServiceRegViewModel),
            typeof(DisableEgmOnCardOutViewModel),
            typeof(EnableEgmWithLeaseViewModel),
            typeof(LockBVViewModel),
            typeof(LockBVOnCardOutViewModel),
            typeof(LockEgmViewModel),
            typeof(BonusHitViewModel),
            typeof(LevelResetViewModel),
            typeof(LevelUpdateViewModel),
            typeof(StartAnticipationViewModel),
            typeof(StartCelebrationViewModel),
            typeof(IBonusViewModel),
            typeof(IGatViewModel),
            typeof(IMetersViewModel),
            typeof(ISmibGatViewModel),
            typeof(ProgressViewModel),
            typeof(LockEgmViewModel),
            typeof(ConfViewModel),
            typeof(DownloadViewModel),
            typeof(AMLViewModel),
            typeof(PCSViewModel),
            typeof(GetNonceRespViewModel),
            typeof(CardInViewModel),
            typeof(IEftViewModel),
            typeof(IKeysViewModel),
            typeof(ISmibEftViewModel),
            typeof(PlayerCardOutViewModel),
            typeof(EmployeeCardOutViewModel),
            typeof(ValidatePinViewModel),
            typeof(KeyedOffViewModel),
            typeof(HandPayViewModel),
            typeof(RequestTransferViewModel),
            typeof(getWatAccountViewModel),
            typeof(ResetHandpayViewModel),
            typeof(initiateTransferViewModel),
            typeof(WatHeartbeatViewModel),
            typeof(WatConfigViewModel),
            typeof(AdjustAccountViewModel)
        };

        public static List<long> levelID = new List<long>();


        public static IServiceCollection ConfigureApp(this IServiceCollection services)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true, true)
              .AddJsonFile("servicesettings.json", optional: true, reloadOnChange: true)
              .Build();
            Configuration = (IConfiguration)config;
            EnableBonusMeters = Convert.ToBoolean(Configuration["EnableBonusMeters"] ?? Configuration["Features:EnableBonusMeters"] ?? "false");
            EnableCardService = Convert.ToBoolean(Configuration["EnableCardService"] ?? Configuration["Features:EnableCardService"] ?? "false");
            EnableAuditEvents = Convert.ToBoolean(Configuration["EnableAuditEvents"] ?? Configuration["Features:EnableAuditEvents"] ?? "true");
            EnableResponseView = Convert.ToBoolean(Configuration["EnableResponseView"] ?? Configuration["Features:EnableResponseView"] ?? "true");
            AutoRegister = Convert.ToBoolean(Configuration["Registration:AutoRegister"] ?? "true");
            services.AddOptions().AddLogging().AddSingleton(config);

            // Bind strongly-typed settings sections for IOptionsMonitor support
            services.Configure<CheckinSettings>(config.GetSection(CheckinSettings.SectionName));
            services.Configure<DownloadSettings>(config.GetSection(DownloadSettings.SectionName));
            services.Configure<RegistrationSettings>(config.GetSection(RegistrationSettings.SectionName));
            services.Configure<DiscoverySettings>(config.GetSection(DiscoverySettings.SectionName));
            services.Configure<FeatureSettings>(config.GetSection(FeatureSettings.SectionName));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });

            _viewModels.ForEach(vm => services.AddSingleton(vm));

            services.AddMessageBus(config, FloorNetNodeTypes.Service, "ServiceSimulator", t_deviceType.FN_OPTIONCONFIG_ID, Convert.ToInt64(t_deviceType.FN_OPTIONCONFIG_ID));
            if (!EnableBonusMeters)
            {
                services.AddRpcProxy<iAuthSub>(_rpcProxyConfig);
                services.AddRpcProxy<iBnsMgr>(_rpcProxyConfig);
                services.AddSingleton<iAuth, AuthRpcProvider>();
                services.AddSingleton<AuthEventHandler>();
                services.AddSingleton<AuthExitEventHandler>();
                services.AddSingleton<BonusEventHandler>();
                services.AddSingleton<BonusHostStatusEventHandler>();
                services.AddSingleton<CageFillCreditEventHandler>();
            }

            services.AddSingleton(config);
            services.AddSingleton<ResponseViewModel>();
            services.AddSingleton<SmibRegistrationTracker>();
            // SQLite-backed persistence for events, cashouts, and meters (cross-platform, no server).
            services.AddSingleton<IGT.FloorNet.Tools.ServiceSimulator.Services.SimDbStore>();
            services.AddSingleton<IGT.FloorNet.Tools.ServiceSimulator.Services.SimFeatureState>();
            // TITO host-side publishers. The SMIB BE2 firmware (tito.c) only requests validation
            // IDs while its TITO_HeartbeatTimeout is in the future, which is refreshed exclusively
            // by the FloorNet "voucherHeartbeat" event; "voucherConfig" carries the SE-validation
            // enable. Without these, the EGM loops forever raising SAS exception 0x57 on cashout.
            services.AddSingleton<IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio.VoucherHeartbeatPublisher>();
            services.AddSingleton<IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio.VoucherConfigPublisher>();
            services.AddRpcProxy<iAuthSub>(_rpcProxyConfig);
            services.AddRpcProxy<iBnsMgr>(_rpcProxyConfig);
            services.AddRpcProxy<iMeters>(_rpcProxyConfig);
            services.AddRpcProxy<iReg>(_rpcProxyConfig);
            services.AddRpcProxy<iRG>(_rpcProxyConfig);
            services.AddRpcProxy<iSvcReg>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibBns>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibEft>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibGat>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibHandpay>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibPlr>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibWat>(_rpcProxyConfig);
            services.AddRpcProxy<iConfig>(_rpcProxyConfig);
            services.AddRpcProxy<iSmibISM>(_rpcProxyConfig);
            services.AddRpcProxy<iDownload>(_rpcProxyConfig);
            
            services.AddSingleton<iAuth, AuthRpcProvider>();
            services.AddSingleton<iTito, TitoRpcProvider>();
            services.AddSingleton<iBonus, BonusRpcProvider>();
            services.AddSingleton<iGat, GatRpcProvider>();
            services.AddSingleton<iCardless, CardlessRpcProvider>();
            services.AddSingleton<iEft, EftRpcProvider>();
            services.AddSingleton<iISM, IsmRPCProvider>();
            services.AddSingleton<iHandpay, HandpayRPCProvider>();
            services.AddSingleton<iWat, WatRpcProvider>();
            services.AddSingleton<iAML, AMLRpcProvider>();
            services.AddSingleton<iPCS, PCSRpcProvider>();

            if (EnableCardService)
            {
                services.AddSingleton<iPlayer, PlayerRPCProvider>();
                services.AddSingleton<iPin, PinRPCProvider>();
            }

            services.AddSingleton<iDiscovery, DiscoveryRpcProvider>();

            services.AddSingleton<KeepAliveEventHandler>();
            services.AddSingleton<AuditEventHandler>();
            services.AddSingleton<ProgressEventHandler>();
            services.AddSingleton<PlayerSessionExtEventHandler>();
            services.AddSingleton<GameplayStartedEventHandler>();
            services.AddSingleton<GameplayEndedEventHandler>();
            services.AddSingleton<GameplaySelectedEventHandler>();

            if (EnableBonusMeters)
            {
                services.AddSingleton<BBPGMeterEventHandler>();
                services.AddSingleton<MDMGMeterEventHandler>();
                services.AddSingleton<MeterEventHandler>();
                services.AddSingleton<MGAMeterEventHandler>();
                services.AddSingleton<SapMeterEventHandler>();
                services.AddSingleton<IBonusMeterMockService, BonusMeterMockService>();
                services.AddSingleton<iBnsMgr, BnsMgrProvider>();
            }

            var rpcProxyConfig = new RpcProxyConfig { SiteId = "1" };
            services.AddRpcProxy<iDiags>(rpcProxyConfig);

            // Reliability / background services (Phase 5):
            //  - SmibOfflineDetectService: sweeps SmibRegistrationTracker and marks SMIBs offline
            //    when keepAlive heartbeats stop.
            //  - FloorUpdateService: drains an in-memory floor-update job queue (Jobs 2/19/25),
            //    executing iReg.disableEGM / iRG.LockBV RPC paths against target SMIBs.
            //  - MessageBusHealthService: recovers RabbitMQ consumers if the broker cancels them
            //    after a delivery-ack timeout (PRECONDITION_FAILED), restoring RPC responsiveness
            //    (e.g. iDiscovery.getAllServiceInterfaces) without a manual app restart.
            services.AddHostedService<Services.SmibOfflineDetectService>();
            services.AddHostedService<Services.FloorUpdateService>();
            services.AddHostedService<Services.MessageBusHealthService>();

            return services;
        }

        public static void StartApp(this IServiceProvider services)
        {
            // Subscribe to runtime config changes (hot-reload from servicesettings.json)
            var featureMonitor = services.GetRequiredService<IOptionsMonitor<FeatureSettings>>();
            featureMonitor.OnChange(settings =>
            {
                EnableAuditEvents = settings.EnableAuditEvents;
                EnableResponseView = settings.EnableResponseView;
                // Note: EnableBonusMeters and EnableCardService affect DI registrations
                // and require an application restart to take effect.
            });

            var regMonitor = services.GetRequiredService<IOptionsMonitor<RegistrationSettings>>();
            regMonitor.OnChange(settings =>
            {
                AutoRegister = settings.AutoRegister;
            });

            // Apply initial Discovery settings from config to the DiscoveryModel
            var discoverySettings = services.GetRequiredService<IOptionsMonitor<DiscoverySettings>>().CurrentValue;

            var config = services.GetRequiredService<iConfig>();
            var messageBus = services.GetRequiredService<IMessageBus>();
            var titoProvider = services.GetRequiredService<iTito>();
            var bonusProvider = services.GetRequiredService<iBonus>();
            var gatProvider = services.GetRequiredService<iGat>();
            var cardlessProvider = services.GetRequiredService<iCardless>();
            var eftProvider = services.GetRequiredService<iEft>();
            var ismProvider = services.GetRequiredService<iISM>();
            var handpayProvider = services.GetRequiredService<iHandpay>();
            var watProvider = services.GetRequiredService<iWat>();
            var AMLProvider = services.GetRequiredService<iAML>();
            var PCSProvider = services.GetRequiredService<iPCS>();

            discovery = services.GetRequiredService<iDiscovery>();
            _iReg = services.GetRequiredService<iReg>();
            _iRG = services.GetRequiredService<iRG>();    
            _iSvcReg = services.GetRequiredService<iSvcReg>();            
            _iSmibBns = services.GetRequiredService<iSmibBns>();
            _iSmibEft = services.GetRequiredService<iSmibEft>();
            _iSmibGat = services.GetRequiredService<iSmibGat>();
            _iSmibHandpay = services.GetRequiredService<iSmibHandpay>();
            _iMeters = services.GetRequiredService<iMeters>();
            _iSmibPlr = services.GetRequiredService<iSmibPlr>();
            _iSmibWat = services.GetRequiredService<iSmibWat>();
            _iConfig = services.GetRequiredService<iConfig>();
            _iSmibISM = services.GetRequiredService<iSmibISM>();
            _iDownload = services.GetRequiredService<iDownload>();
            _iDiags = services.GetRequiredService<iDiags>();
            _iAML = services.GetRequiredService<iAML>();
            _iPCS = services.GetRequiredService<iPCS>();

            if (!EnableBonusMeters)
            {
                authProvider = services.GetRequiredService<iAuth>();
                _iBnsMgr = services.GetRequiredService<iBnsMgr>();
            }
            _iAuthSub = services.GetRequiredService<iAuthSub>();

            //_imessageBusRpcProxy = services.GetRequiredService<IMessageBusRpcProxy>();
            responseViewModel = services.GetRequiredService<ResponseViewModel>();

            // Apply Discovery config from servicesettings.json to the DiscoveryModel
            var discoveryViewModel = services.GetRequiredService<DiscoveryViewModel>();
            discoveryViewModel.DiscoveryModel.CardlessInterface = discoverySettings.CardlessInterface;
            discoveryViewModel.DiscoveryModel.EftInterface = discoverySettings.EftInterface;
            discoveryViewModel.DiscoveryModel.AMLInterface = discoverySettings.AMLInterface;
            discoveryViewModel.DiscoveryModel.PlayerInterface = discoverySettings.PlayerInterface;
            discoveryViewModel.DiscoveryModel.BonusInterface = discoverySettings.BonusInterface;
            discoveryViewModel.DiscoveryModel.DiagsInterface = discoverySettings.DiagsInterface;
            discoveryViewModel.DiscoveryModel.GatInterface = discoverySettings.GatInterface;
            discoveryViewModel.DiscoveryModel.ConfInterface = discoverySettings.ConfInterface;
            discoveryViewModel.DiscoveryModel.DownloadInterface = discoverySettings.DownloadInterface;
            discoveryViewModel.DiscoveryModel.TitoInterface = discoverySettings.TitoInterface;
            discoveryViewModel.DiscoveryModel.PinInterface = discoverySettings.PinInterface;
            discoveryViewModel.DiscoveryModel.MarkerInterface = discoverySettings.MarkerInterface;
            discoveryViewModel.DiscoveryModel.RGInterface = discoverySettings.RGInterface;
            discoveryViewModel.DiscoveryModel.RegInterface = discoverySettings.RegInterface;
            discoveryViewModel.DiscoveryModel.HandpayInterface = discoverySettings.HandpayInterface;
            discoveryViewModel.DiscoveryModel.PCSInterface = discoverySettings.PCSInterface;
            discoveryViewModel.DiscoveryModel.IsmInterface = discoverySettings.IsmInterface;
            discoveryViewModel.DiscoveryModel.WatInterface = discoverySettings.WatInterface;

            // Subscribe to Discovery config changes for runtime hot-reload
            var discoveryMonitor = services.GetRequiredService<IOptionsMonitor<DiscoverySettings>>();
            discoveryMonitor.OnChange(settings =>
            {
                discoveryViewModel.DiscoveryModel.CardlessInterface = settings.CardlessInterface;
                discoveryViewModel.DiscoveryModel.EftInterface = settings.EftInterface;
                discoveryViewModel.DiscoveryModel.AMLInterface = settings.AMLInterface;
                discoveryViewModel.DiscoveryModel.PlayerInterface = settings.PlayerInterface;
                discoveryViewModel.DiscoveryModel.BonusInterface = settings.BonusInterface;
                discoveryViewModel.DiscoveryModel.DiagsInterface = settings.DiagsInterface;
                discoveryViewModel.DiscoveryModel.GatInterface = settings.GatInterface;
                discoveryViewModel.DiscoveryModel.ConfInterface = settings.ConfInterface;
                discoveryViewModel.DiscoveryModel.DownloadInterface = settings.DownloadInterface;
                discoveryViewModel.DiscoveryModel.TitoInterface = settings.TitoInterface;
                discoveryViewModel.DiscoveryModel.PinInterface = settings.PinInterface;
                discoveryViewModel.DiscoveryModel.MarkerInterface = settings.MarkerInterface;
                discoveryViewModel.DiscoveryModel.RGInterface = settings.RGInterface;
                discoveryViewModel.DiscoveryModel.RegInterface = settings.RegInterface;
                discoveryViewModel.DiscoveryModel.HandpayInterface = settings.HandpayInterface;
                discoveryViewModel.DiscoveryModel.PCSInterface = settings.PCSInterface;
                discoveryViewModel.DiscoveryModel.IsmInterface = settings.IsmInterface;
                discoveryViewModel.DiscoveryModel.WatInterface = settings.WatInterface;
            });

            var cardViewModel = services.GetRequiredService<CardViewModel>();
            cardViewModel.EnableCardService = EnableCardService;

            if (EnableCardService)
            {
                var pinProvider = services.GetRequiredService<iPin>();
                var playerProvider = services.GetRequiredService<iPlayer>();

                messageBus.RegisterRpcProvider<iPin>(pinProvider);
                messageBus.RegisterRpcProvider<iPlayer>(playerProvider);
            }

            messageBus.RegisterRpcProvider<iTito>(titoProvider);
            messageBus.RegisterRpcProvider<iDiscovery>(discovery);
            messageBus.RegisterRpcProvider<iBonus>(bonusProvider);
            messageBus.RegisterRpcProvider<iGat>(gatProvider);
            messageBus.RegisterRpcProvider<iCardless>(cardlessProvider);
            messageBus.RegisterRpcProvider<iEft>(eftProvider);
            messageBus.RegisterRpcProvider<iISM>(ismProvider);
            messageBus.RegisterRpcProvider<iHandpay>(handpayProvider);
            messageBus.RegisterRpcProvider<iWat>(watProvider);
            messageBus.RegisterRpcProvider<iAML>(AMLProvider);
            messageBus.RegisterRpcProvider<iPCS>(PCSProvider);

            if (!EnableBonusMeters)
            {
                messageBus.RegisterRpcProvider<iAuth>(authProvider);
                messageBus.SubscribeEvent<AuthEntry, AuthEventHandler>();
                messageBus.SubscribeEvent<AuthExit, AuthExitEventHandler>();
                messageBus.SubscribeEvent<PoolMeters, BonusEventHandler>();
                messageBus.SubscribeEvent<BonusHostStatus, BonusHostStatusEventHandler>();

                // rpc proxy
                messageBus.RegisterRpcProxy<iAuthSub>();
                messageBus.RegisterRpcProxy<iBnsMgr>();
            }

            if (EnableBonusMeters)
            {
                messageBus.SubscribeEvent<bbpgMeterEvent, BBPGMeterEventHandler>();
                messageBus.SubscribeEvent<mdmgMeterEvent, MDMGMeterEventHandler>();
                messageBus.SubscribeEvent<meterEvent, MeterEventHandler>();
                messageBus.SubscribeEvent<mgaMeterEvent, MGAMeterEventHandler>();
                messageBus.SubscribeEvent<sapMeterEvent, SapMeterEventHandler>();
                _iBnsMgr = services.GetRequiredService<iBnsMgr>();
                messageBus.RegisterRpcProvider<iBnsMgr>(_iBnsMgr);

                //Initialize mock data
                var mockBonusMeterMockService = services.GetRequiredService<IBonusMeterMockService>();
                mockBonusMeterMockService.Initialize();
            }
            messageBus.SubscribeEvent<keepAlive, KeepAliveEventHandler>();
            messageBus.SubscribeEvent<Progress, ProgressEventHandler>();
            messageBus.SubscribeEvent<PlayerSessionExt, PlayerSessionExtEventHandler>();
            messageBus.SubscribeEvent<gameStarted, GameplayStartedEventHandler>();
            messageBus.SubscribeEvent<gameEnded, GameplayEndedEventHandler>();
            messageBus.SubscribeEvent<gameSelected, GameplaySelectedEventHandler>();

            if (EnableAuditEvents)
            {
                messageBus.SubscribeAuditEvent<AuditEventHandler>(GetAuditEventSubscriptions());
            }
            // rpc proxy
            messageBus.RegisterRpcProxy<iAuthSub>();
            messageBus.RegisterRpcProxy<iBnsMgr>();
            messageBus.RegisterRpcProxy<iDiags>();
            messageBus.RegisterRpcProxy<iSvcReg>();
            messageBus.RegisterRpcProxy<iReg>();
            messageBus.RegisterRpcProxy<iRG>();
            messageBus.RegisterRpcProxy<iSmibBns>();
            messageBus.RegisterRpcProxy<iSmibEft>();
            messageBus.RegisterRpcProxy<iSmibGat>();
            messageBus.RegisterRpcProxy<iSmibHandpay>();
            messageBus.RegisterRpcProxy<iMeters>();
            messageBus.RegisterRpcProxy<iSmibPlr>();
            messageBus.RegisterRpcProxy<iSmibWat>();
            messageBus.RegisterRpcProxy<iSmibISM>();
            messageBus.RegisterRpcProxy<iConfig>();
            messageBus.RegisterRpcProxy<iEft>();            
            messageBus.StartConsumer();

            // Start the TITO host-side publishers now that the bus consumer is running.
            // VoucherHeartbeatPublisher keeps the SMIB's TITO_HeartbeatTimeout alive (every 10s);
            // VoucherConfigPublisher enables Secure-Enhanced validation (every 30s). Both are
            // required for the SMIB to fire EVT_NEED_VALIDATION_ID -> iTito.getValidationIds, which
            // delivers SAS LP 57/58 to the EGM and stops the repeating exception-0x57 cashout loop.
            services.GetRequiredService<IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio.VoucherHeartbeatPublisher>().Start();
            services.GetRequiredService<IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Titio.VoucherConfigPublisher>().Start();
        }

        public static void StartWebServer(this IServiceProvider services)
        {
            var builder = WebHost.CreateDefaultBuilder();

            var port = Configuration.GetValue<Int32>("RESTServerPort", 5000);
            var secureport = Configuration.GetValue<Int32>("RESTServerSecurePort", 5002);
            var certPw = Configuration.GetValue<string>("HTTPSCertificatePassword", "localhost");

            if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\Download"))
                Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Download");

            string certPath = Path.Combine(Directory.GetCurrentDirectory(), "server.pfx");

            var app = builder
                .UseKestrel(options =>
                {
                    options.ListenAnyIP(port); // HTTP
                    if (Directory.Exists(certPath))
                    {
                        options.ListenAnyIP(secureport, listenOptions =>
                                            {
                                                listenOptions.UseHttps(certPath, certPw, httpsOptions =>
                                                {
                                                    httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls;
                                                });
                                            }); // HTTPS

                    } 
                })
                .ConfigureServices(servs =>
                {
                    servs.ConfigureServices(services);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseCors(builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                    app.UseAuthentication();
                    app.UseAuthorization();

                    // Custom middleware to enforce authentication for static files
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path.StartsWithSegments("/Download"))
                        {
                            if (!context.User.Identity.IsAuthenticated)
                            {
                                context.Response.Headers["WWW-Authenticate"] = "Basic";
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync("Unauthorized");
                                return;
                            }
                        }
                        await next();
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider($"{Directory.GetCurrentDirectory()}"),
                        ServeUnknownFileTypes = true,
                    });
                }).Build();

            app.Run();
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services, IServiceProvider appServiceProvider)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Add CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddAuthorization();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add(new AllowAnonymousFilter());
            });

            services.AddSingleton<MarkerViewModel>();
            return services;
        }

        public static IDictionary<string, List<t_eventCode>> GetAuditEventSubscriptions()
        {
            IDictionary<string, List<t_eventCode>> subscriptions = new Dictionary<string, List<t_eventCode>>();

            subscriptions.Add("Registration", new List<t_eventCode>()
            {
                t_eventCode.regGameDisabled,
                t_eventCode.regGameEnabled,
                t_eventCode.regGameLockStatus,
                t_eventCode.regNodeOffline,
                t_eventCode.regNodeOnline,
                t_eventCode.regSmibOffline,
                t_eventCode.regSmibOnline,
                t_eventCode.regSmibReset,
                t_eventCode.regTimeDiscrepancy
            });

            subscriptions.Add("AFT", new List<t_eventCode>()
            {
                t_eventCode.aftBonusToCashable,
                t_eventCode.aftBonusToHandpay,
                t_eventCode.aftBonusToNonrestricted,
                t_eventCode.aftFromEGM,
                t_eventCode.aftRestrictedToXC,
                t_eventCode.aftTaxTransferException,
                t_eventCode.aftToEGM,
                t_eventCode.aftTransferDelayed,
                t_eventCode.aftTransferToBillTax,
                t_eventCode.aftTransferToCelebration,
                t_eventCode.aftTransferToNonrestricted,
                t_eventCode.aftTransferToVoucher,
                t_eventCode.aftXCtoRestricted
            });

            subscriptions.Add("ADV", new List<t_eventCode>()
            {
                t_eventCode.browserStarted,
                t_eventCode.browserStopped
            });

            subscriptions.Add("Cabinet", new List<t_eventCode>()
            {
                t_eventCode.cabAuxiliary_Door_Closed,
                t_eventCode.cabAuxiliary_Door_Open,
                t_eventCode.cabAuxiliary_Door_Open_Illegal,
                t_eventCode.cabBackup_Battery_Low,
                t_eventCode.cabBelly_Door_Closed,
                t_eventCode.cabBelly_Door_Open,
                t_eventCode.cabBelly_Door_Open_Illegal,
                t_eventCode.cabCabinet_Door_Closed,
                t_eventCode.cabCabinet_Door_Open,
                t_eventCode.cabCabinet_Door_Open_Illegal,
                t_eventCode.cabCashOut_Button_Pressed,
                t_eventCode.cabConfiguration_Changed_By_Operator,
                t_eventCode.cabEGM_Disabled_By_Operator_Menu,
                t_eventCode.cabEGM_Power_Lost,
                t_eventCode.cabEGM_Power_Up_Restart,
                t_eventCode.cabGeneral_Cabinet_Fault,
                t_eventCode.cabGeneral_Memory_Fault,
                t_eventCode.cabLifeToDate_Meters_Reset,
                t_eventCode.cabLogic_Door_Closed,
                t_eventCode.cabLogic_Door_Open,
                t_eventCode.cabLogic_Door_Open_Illegal,
                t_eventCode.cabMachine_Link_Down,
                t_eventCode.cabMachine_Link_Up,
                t_eventCode.cabMeters_Audit_Mode_Exited,
                t_eventCode.cabMeters_Audit_Mode_Initiated,
                t_eventCode.cabNonVolatile_Storage_Fault,
                t_eventCode.cabOperatorMenu_Entered,
                t_eventCode.cabOperatorMenu_Exited,
                t_eventCode.cabPower_Off_Cabinet_Door_Open,
                t_eventCode.cabPower_Off_Logic_Door_Open,
                t_eventCode.cabService_Lamp_Off,
                t_eventCode.cabService_Lamp_On,
                t_eventCode.reel1Fault,
                t_eventCode.reel2Fault,
                t_eventCode.reel3Fault,
                t_eventCode.reel4Fault,
                t_eventCode.reel5Fault,
                t_eventCode.reelFault,
                t_eventCode.reelsDisconnected,
            });

            subscriptions.Add("CoinAcceptor", new List<t_eventCode>()
            {
                t_eventCode.caeAcceptor_Fault,
                t_eventCode.caeCoin_Acceptor_Lockout_Malfunction,
                t_eventCode.caeDiverter_Fault,
                t_eventCode.caeDrop_Door_Closed,
                t_eventCode.caeDrop_Door_Opened,
                t_eventCode.caeDrop_Door_Opened_Illegal,
                t_eventCode.caeIllegal_Activity_Detected,
                t_eventCode.caePower_Off_Drop_Door_Opened
            });

            subscriptions.Add("Cardless", new List<t_eventCode>()
            {
                t_eventCode.ccConnectBLE,
                t_eventCode.ccConnectNFC,
                t_eventCode.ccConnectWiFi,
                t_eventCode.ccDisconnectBLE,
                t_eventCode.ccDisconnectNFC,
                t_eventCode.ccDisconnectWiFi,
                t_eventCode.ccPinFailureBLE,
                t_eventCode.ccPinFailureNFC,
                t_eventCode.ccPinFailureWiFi,
                t_eventCode.ccRadioLockout,
                t_eventCode.ccTransferFailureBLE,
                t_eventCode.ccTransferFailureNFC,
                t_eventCode.ccTransferFailureWiFi,
                t_eventCode.ccTransferRequestBLE,
                t_eventCode.ccTransferRequestNFC,
                t_eventCode.ccTransferRequestWiFi
            });

            subscriptions.Add("Extra", new List<t_eventCode>()
            {
                t_eventCode.dldDownloadComplete,
                t_eventCode.dldDownloadFailed,
                t_eventCode.dldDownloadPending,
                t_eventCode.dldInstallFailed,
                t_eventCode.dldInstallSuccess
            });

            subscriptions.Add("FJP", new List<t_eventCode>()
            {
                t_eventCode.fjpAuxFill,
                t_eventCode.fjpAuxHopperFillTicket,
                t_eventCode.fjpAuxReplenish,
                t_eventCode.fjpAuxReplenishReportTicket,
                t_eventCode.fjpBonusHandpayTicket,
                t_eventCode.fjpCancelCreditTicket,
                t_eventCode.fjpCancelProcessingTicket,
                t_eventCode.fjpDispenserCheckDenom,
                t_eventCode.fjpDispenserFailed,
                t_eventCode.fjpDispenserLoadInventory,
                t_eventCode.fjpDispenserNoDispense,
                t_eventCode.fjpDispenserSetup,
                t_eventCode.fjpDispenserSuccess,
                t_eventCode.fjpDispenserUnknown,
                t_eventCode.fjpDoubleBagFill,
                t_eventCode.fjpExcessiveFills,
                t_eventCode.fjpHandpayTicket,
                t_eventCode.fjpHighFill,
                t_eventCode.fjpHopperBleedTicket,
                t_eventCode.fjpHopperFillTicket,
                t_eventCode.fjpOverrideFill,
                t_eventCode.fjpPouchPay,
                t_eventCode.fjpPreemptiveFillTicket,
                t_eventCode.fjpProgressiveTicket,
                t_eventCode.fjpSAS6BonusTicket,
                t_eventCode.fjpShortPayTicket,
                t_eventCode.fjpTicketPrinted,
                t_eventCode.fjpTicketReprint,
                t_eventCode.fjpUnknownTicket,
                t_eventCode.fjpVoidTicket
            });

            subscriptions.Add("Hardware", new List<t_eventCode>()
            {
                t_eventCode.hdwCardReaderConnected,
                t_eventCode.hdwCardReaderDisconnected,
                t_eventCode.hdwDisplayConnected,
                t_eventCode.hdwDisplayDisconnected,
                t_eventCode.hdwEncFailure,
                t_eventCode.hdwEncInvalid
            });

            subscriptions.Add("Host", new List<t_eventCode>()
            {
                t_eventCode.hostAttendantRequest,
                t_eventCode.hostCancelOrder,
                t_eventCode.hostChangeOrder,
                t_eventCode.hostDrinkOrder,
                t_eventCode.hostFoodOrder,
                t_eventCode.hostLaunchApp,
                t_eventCode.hostPlayerSignupOrder,
                t_eventCode.hostValetRequest,
                t_eventCode.PlayerRankingReq,
            });

            subscriptions.Add("Hopper", new List<t_eventCode>()
            {
                t_eventCode.hpeCoins_Dispensed,
                t_eventCode.hpeExtra_Coins_Paid,
                t_eventCode.hpeHopper_Below_Low_Water_Mark,
                t_eventCode.hpeHopper_Empty,
                t_eventCode.hpeHopper_Fault,
                t_eventCode.hpeHopper_Full
            });

            subscriptions.Add("Handpay", new List<t_eventCode>()
            {
                t_eventCode.jpeBonusHandpayPending,
                t_eventCode.jpeBonusHandpayPendingW2G,
                t_eventCode.jpeBonusHandpaySas6Pending,
                t_eventCode.jpeBonusHandpaySas6PendingW2G,
                t_eventCode.jpeBonusReset,
                t_eventCode.jpeCancelCreditPending,
                t_eventCode.jpeCancelCreditPendingW2G,
                t_eventCode.jpeCancelCreditRequestCanceled,
                t_eventCode.jpeCancelCreditReset,
                t_eventCode.jpeCustomerHandpayRequest,
                t_eventCode.jpeHandpayReset,
                t_eventCode.jpeJackpotPending,
                t_eventCode.jpeJackpotPendingW2G,
                t_eventCode.jpeProgressiveJackpotPending,
                t_eventCode.jpeProgressiveJackpotPendingW2G,
                t_eventCode.jpeProgressiveReset,
                t_eventCode.jpeW2GKeyoff,
                t_eventCode.jpeW2GResponse
            });

            subscriptions.Add("Logging", new List<t_eventCode>()
            {
                t_eventCode.logLogsCleared,
                t_eventCode.logUploadComplete,
                t_eventCode.logUploadFailed,
                t_eventCode.logUploadPending
            });

            subscriptions.Add("MOB", new List<t_eventCode>()
            {
                t_eventCode.mobFour_of_a_Kind_10s,
                t_eventCode.mobFour_of_a_Kind_10s_wJ,
                t_eventCode.mobFour_of_a_Kind_2s,
                t_eventCode.mobFour_of_a_Kind_2s_wJ,
                t_eventCode.mobFour_of_a_Kind_3s,
                t_eventCode.mobFour_of_a_Kind_3s_wJ,
                t_eventCode.mobFour_of_a_Kind_4s,
                t_eventCode.mobFour_of_a_Kind_4s_wJ,
                t_eventCode.mobFour_of_a_Kind_5s,
                t_eventCode.mobFour_of_a_Kind_5s_wJ,
                t_eventCode.mobFour_of_a_Kind_6s,
                t_eventCode.mobFour_of_a_Kind_6s_wJ,
                t_eventCode.mobFour_of_a_Kind_7s,
                t_eventCode.mobFour_of_a_Kind_7s_wJ,
                t_eventCode.mobFour_of_a_Kind_8s,
                t_eventCode.mobFour_of_a_Kind_8s_wJ,
                t_eventCode.mobFour_of_a_Kind_9s,
                t_eventCode.mobFour_of_a_Kind_9s_wJ,
                t_eventCode.mobFour_of_a_Kind_Aces,
                t_eventCode.mobFour_of_a_Kind_Aces_wJ,
                t_eventCode.mobFour_of_a_Kind_Jacks,
                t_eventCode.mobFour_of_a_Kind_Jacks_wJ,
                t_eventCode.mobFour_of_a_Kind_Jokers,
                t_eventCode.mobFour_of_a_Kind_Jokers_alt,
                t_eventCode.mobFour_of_a_Kind_Kings,
                t_eventCode.mobFour_of_a_Kind_Kings_wJ,
                t_eventCode.mobFour_of_a_Kind_Other,
                t_eventCode.mobFour_of_a_Kind_Other_alt,
                t_eventCode.mobFour_of_a_Kind_Queens,
                t_eventCode.mobFour_of_a_Kind_Queens_wJ,
                t_eventCode.mobNo_Card_Data,
                t_eventCode.mobRoyal_Flush_Clubs,
                t_eventCode.mobRoyal_Flush_Clubs_wJ,
                t_eventCode.mobRoyal_Flush_Diamonds,
                t_eventCode.mobRoyal_Flush_Diamonds_wJ,
                t_eventCode.mobRoyal_Flush_Hearts,
                t_eventCode.mobRoyal_Flush_Hearts_wJ,
                t_eventCode.mobRoyal_Flush_Jokers,
                t_eventCode.mobRoyal_Flush_Jokers_alt,
                t_eventCode.mobRoyal_Flush_Other,
                t_eventCode.mobRoyal_Flush_Other_alt,
                t_eventCode.mobRoyal_Flush_Spades,
                t_eventCode.mobRoyal_Flush_Spades_wJ
            });

            subscriptions.Add("NoteAcceptor", new List<t_eventCode>()
            {
                t_eventCode.naeAcceptor_Fault,
                t_eventCode.naeAcceptor_Jammed,
                t_eventCode.naeDisabled,
                t_eventCode.naeEnabled,
                t_eventCode.naeIllegal_Activity_Detected,
                t_eventCode.naeNote_or_Voucher_Rejected,
                t_eventCode.naeNote_Stacked,
                t_eventCode.naeNotResponding,
                t_eventCode.naePower_Off_Stacker_Door_Open,
                t_eventCode.naeStacker_Door_Closed,
                t_eventCode.naeStacker_Door_Opened,
                t_eventCode.naeStacker_Door_Opened_Illegal,
                t_eventCode.naeStacker_Full,
                t_eventCode.naeStacker_Inserted,
                t_eventCode.naeStacker_Nearly_Full,
                t_eventCode.naeStacker_Removed,
                t_eventCode.naeValidator_Totals_Reset
            });

            subscriptions.Add("PCS", new List<t_eventCode>()
            {
                t_eventCode.pcsAbandonedCard,
                t_eventCode.pcsLASRequested,
                t_eventCode.pcsSession,
                t_eventCode.pcsStartSession,
                t_eventCode.pcsThresholdReached
            });

            subscriptions.Add("Printer", new List<t_eventCode>()
            {
                t_eventCode.pteDevice_Connected,
                t_eventCode.pteDevice_Disconnected,
                t_eventCode.pteDevice_Fault,
                t_eventCode.ptePaper_Empty,
                t_eventCode.ptePaper_Jam,
                t_eventCode.ptePaper_Low
            });

            subscriptions.Add("RG", new List<t_eventCode>()
            {
                t_eventCode.rgAlert1,
                t_eventCode.rgAlert2,
                t_eventCode.rgAlert3,
                t_eventCode.rgAlert4,
                t_eventCode.rgAlert5,
                t_eventCode.rgAlert6,
                t_eventCode.rgAlert7,
                t_eventCode.rgAlert8
            });

            subscriptions.Add("Meters", new List<t_eventCode>()
            {
                t_eventCode.sapHit,
                t_eventCode.sapUpdate
            });

            subscriptions.Add("Tables", new List<t_eventCode>()
            {
                t_eventCode.tblCardIn,
                t_eventCode.tblCardOut,
                t_eventCode.tblTransferFromTable,
                t_eventCode.tblTransferToTable
            });

            subscriptions.Add("Tito", new List<t_eventCode>()
            {
                t_eventCode.titoAFTCouponIssued,
                t_eventCode.titoAFTDebitIssued,
                t_eventCode.titoAFTVoucherIssued,
                t_eventCode.titoCashoutReceipt,
                t_eventCode.titoCouponIssued,
                t_eventCode.titoHandpayReceipt,
                t_eventCode.titoHandpayRecord,
                t_eventCode.titoJackpotReceipt,
                t_eventCode.titoJackpotRecord,
                t_eventCode.titoRejectedAmtMismatch,
                t_eventCode.titoRejectedBadAmount,
                t_eventCode.titoRejectedBadFunction,
                t_eventCode.titoRejectedBadNumber,
                t_eventCode.titoRejectedByHost,
                t_eventCode.titoRejectedCommLinkDown,
                t_eventCode.titoRejectedCreditLimit,
                t_eventCode.titoRejectedDisabled,
                t_eventCode.titoRejectedNotMultiple,
                t_eventCode.titoRejectedOutOfCycle,
                t_eventCode.titoRejectedTimeout,
                t_eventCode.titoRejectedUnableToAccept,
                t_eventCode.titoRejectedValidatorFailed,
                t_eventCode.titoVoucherIssued,
                t_eventCode.titoVoucherRedeemed,
                t_eventCode.titoVoucherRedemptionRequested,
            });

            subscriptions.Add("Wat", new List<t_eventCode>()
            {
                t_eventCode.smartCardKsiMismatch,
                t_eventCode.watDisabled,
                t_eventCode.watEnabled,
                t_eventCode.watNotSupported,
                t_eventCode.watSupported,
                t_eventCode.watXferCommit,
                t_eventCode.watXferFromGameAmbiguous,
                t_eventCode.watXferFromGameEscrow,
                t_eventCode.watXferFromGameFail,
                t_eventCode.watXferFromGameSuccess,
                t_eventCode.watXferToGameAmbiguous,
                t_eventCode.watXferToGameEscrow,
                t_eventCode.watXferToGameFail,
                t_eventCode.watXferToGameSuccess
            });

            subscriptions.Add("Player", new List<t_eventCode>()
            {
                t_eventCode.plrAbandonedCard,
                t_eventCode.plrCardInserted,
                t_eventCode.plrCardInserted_Abandoned,
                t_eventCode.plrCardInserted_Audit,
                t_eventCode.plrCardInserted_Bad_Location,
                t_eventCode.plrCardInserted_Banned,
                t_eventCode.plrCardInserted_CashDrop,
                t_eventCode.plrCardInserted_CoinDrop,
                t_eventCode.plrCardInserted_CoinTest,
                t_eventCode.plrCardInserted_CombinedDrop,
                t_eventCode.plrCardInserted_Employee,
                t_eventCode.plrCardInserted_FirstVisit,
                t_eventCode.plrCardInserted_Invalid_Card,
                t_eventCode.plrCardInserted_NewMember,
                t_eventCode.plrCardInserted_Player,
                t_eventCode.plrCardInserted_SelfLimited,
                t_eventCode.plrCardInserted_Unregistered,
                t_eventCode.plrCardPINlocked,
                t_eventCode.plrCardRemoved,
                t_eventCode.plrHotPlayer,
                t_eventCode.plrPointLimitExceeded,
                t_eventCode.plrPointsToCredits,
                t_eventCode.plrPointsToCreditsRefund,
                t_eventCode.plrPointsToCreditsUseUnknown,
                t_eventCode.plrPointsToPayment,
                t_eventCode.plrPointsToXtraCredits,
                t_eventCode.plrSessionEnded,
                t_eventCode.plrSessionInterval,
                t_eventCode.plrSessionStarted,
                t_eventCode.plrTooManyPointsEarned_NoAward,
                t_eventCode.plrXCRefund,
                t_eventCode.plrXCUsedDuringSession,
                t_eventCode.plrXCUseUnknown,
                t_eventCode.plrXtraCreditPlayed
            });

            subscriptions.Add("OptionConfig", new List<t_eventCode>()
            {
                t_eventCode.cfgChangeComplete,
                t_eventCode.cfgChangeFailed,
                t_eventCode.cfgChangePending
            });

            subscriptions.Add("Download", new List<t_eventCode>()
            {
                t_eventCode.dldDownloadComplete,
                t_eventCode.dldDownloadFailed,
                t_eventCode.dldDownloadPending,
                t_eventCode.dldInstallFailed,
                t_eventCode.dldInstallSuccess
            });

            List<t_eventCode> bonusEvtList = new List<t_eventCode>()
            {
                t_eventCode.bnsCelebrationBonusFailed,
                t_eventCode.bnsCelebrationBonusPaid,
                t_eventCode.bnsCelebrationFailure,
                t_eventCode.bnsCelebrationSuccess,
                t_eventCode.bnsDisplayLimitExceeded,
                t_eventCode.bnsHopperPaidProgHit,
                t_eventCode.bnsProgHit,
                t_eventCode.bnsMJTBonusFailed,
                t_eventCode.bnsMJTBonusPaid,
                t_eventCode.bnsMysteryBonusFailed,
                t_eventCode.bnsMysteryBonusPaid,
                t_eventCode.bnsNotEligibleForCelebration,
                t_eventCode.bnsPaymentFailure,
                t_eventCode.bnsPaymentSuccess,
                t_eventCode.bnsPersonalProgressiveFailed,
                t_eventCode.bnsPersonalProgressivePaid,
                t_eventCode.bnsProgressiveDisabledByEGM,
                t_eventCode.bnsProgressiveFailed,
                t_eventCode.bnsProgressivePaid,
                t_eventCode.bnsScheduledReturnPlayAward,
                t_eventCode.bnsScheduledReturnPlayAwardFailed,
                t_eventCode.bnsSvcCelebration_no_winner,
                t_eventCode.bnsSvcPoolDeleted,
                t_eventCode.bnsSvcSeatRefusedPayment,
                t_eventCode.bnsVariablePromoCreditAward,
                t_eventCode.bnsVariablePromoCreditAwardFailed,
                t_eventCode.bnsWMLimitExceeded
            };

            subscriptions.Add("Gat", new List<t_eventCode>()
            {
                t_eventCode.gatApplicationSHAFailure,
                t_eventCode.gatDevicePassedSHA,
                t_eventCode.gatNoResponseToSHAChallenge,
                t_eventCode.gatROMSHAFailure,
                t_eventCode.gatVerificationComplete
            });

            subscriptions.Add("Eft", new List<t_eventCode>()
            {
                t_eventCode.aftDebitToEGM,
                t_eventCode.aftDebitToVoucher,
                t_eventCode.eftXferCommit
            });

            if (!EnableBonusMeters)
            {
                subscriptions.Add("Auth", new List<t_eventCode>() {
                    t_eventCode.authLogon,
                    t_eventCode.authLogoff
                });
                bonusEvtList.AddRange(new List<t_eventCode>() {
                    t_eventCode.bnsSvcHit,
                    t_eventCode.bnsSvcHitPaid,
                    t_eventCode.bnsSvcHitPaymentFailure,
                    t_eventCode.bnsSvcCfgChanged,
                    t_eventCode.bnsSvcCfgEGMAddedToBonus,
                    t_eventCode.bnsSvcCfgEGMRemovedFromBonus,
                    t_eventCode.bnsSvcCfgNewBonus,
                    t_eventCode.bnsSvcCfgPoolDeleted,
                    t_eventCode.bnsSvcCfgStatusChange
                });
            }

            subscriptions.Add("Bonus", bonusEvtList);

            return subscriptions;
        }

        public static void StopApp(this IServiceProvider services)
        {
            services.GetRequiredService<IMessageBus>().StopConsumer();
            services.GetRequiredService<IMessageBus>().Dispose();
        }

        public static readonly RpcProxyConfig _rpcProxyConfig = new RpcProxyConfig
        {
            NodeName = "ServiceSimulator",
            InstanceId = "ServiceSimulator",
            SiteId = "1",
            DeviceType = t_deviceType.FN_CMS_ID,
            Ttl = 10
        };
    }
}
