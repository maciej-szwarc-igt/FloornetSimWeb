using IGT.FloorNet.Tools.ServiceSimulator;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using NLog.Extensions.Logging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure NLog
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddNLog(builder.Configuration);

// Configure Kestrel port (appsettings.json "RESTServerPort" is the single source of truth).
// This is a developer-facing local simulator that listens HTTP-only on loopback; there is no
// public HTTPS endpoint to terminate TLS on. The tech-stack v26.01 "TLS 1.2 minimum" requirement
// applies to the RabbitMQ/MessageBus transport, which is negotiated by IGT.FloorNet.MessageBus.
var port = builder.Configuration.GetValue<int>("RESTServerPort", 5004);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// Register ASP.NET controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.IncludeFields = true;
    });
builder.Services.AddOpenApi();

// Register all FloorNet services (MessageBus, RPC Providers, Event Handlers, ViewModels)
builder.Services.ConfigureApp();

var app = builder.Build();

// Start the FloorNet MessageBus (RPC providers, event subscriptions, proxies)
app.Services.StartApp();

// Middleware pipeline
app.MapOpenApi();
app.MapScalarApiReference();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

Console.WriteLine($"FloorNet Service Simulator Web running on http://localhost:{port}");
app.Run();
