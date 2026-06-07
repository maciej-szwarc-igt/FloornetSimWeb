using IGT.FloorNet.EX.evt;
using IGT.FloorNet.MessageBus.Events;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.ProgressEvent;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels
{
    public class ResponseViewModel
    {

        private RelayCommand _clearCommand;
        private RelayCommand _copyLogToClipboardCommand;
        private readonly ILogger<ResponseViewModel> _logger;

        public ResponseViewModel(ILogger<ResponseViewModel> logger)
        {
            _logger = logger;
        }


        public RelayCommand ClearCommand
        {
            get
            {
                _clearCommand = new RelayCommand(
                    Clear,
                    param => true
                );

                return _clearCommand;
            }
        }
        public RelayCommand CopyLogToClipboardCommand
        {
            get
            {
                _copyLogToClipboardCommand = new RelayCommand(
                    CopyLogToClipboard,
                    param => true
                );

                return _copyLogToClipboardCommand;
            }
        }

        public ResponseModel Model { get; } = new ResponseModel();

        public ObservableCollection<ProgressStatus> ProgressList { get; set; } = new();

        private string AsJsonText(object resp)
        {
            var json = JsonConvert.SerializeObject(resp);
            return JToken.Parse(json).ToString(Formatting.Indented);
        }

        private string AsUnformattedJsonText(object resp)
        {
            var json = JsonConvert.SerializeObject(resp, Formatting.None);
            return JToken.Parse(json).ToString(Formatting.None);
        }

        private ResponseViewModel AppendText(string text)
        {   
            Model.Response = (Model.Response ?? string.Empty) + text + Environment.NewLine;

            if (Model.LimitResponseWindowSize && (Model.Response.Length > ResponseModel.MaxResponseStrSize))
            {
                int newStringStartIndex = text.Length + (Model.Response.Length - ResponseModel.MaxResponseStrSize);
                Model.Response = Model.Response.Substring(newStringStartIndex);
            }

            return this;
        }

        public void Log(string text)
        {
            _logger.LogInformation(text);
            if (Startup.EnableResponseView)
            {
                AppendText(text);
            }
        }

        public void LogRpc(string name, object request, object response, RpcCallContext rpcCallContext)
        {
            var logFileEntry = new StringBuilder()
                .AppendLine()
                .AppendLine($"[RPC] {name}");
            if (rpcCallContext != null)
            {
                logFileEntry
                    .AppendLine($" {DateTime.UtcNow}  Uid: {rpcCallContext.Uid}  DeviceType: {rpcCallContext.DeviceType}  MachineNum: {rpcCallContext.MachineNum}  SiteId: {rpcCallContext.SiteId}");
            }
            else
            {
                logFileEntry.AppendLine($" {DateTime.UtcNow}");
            }
            logFileEntry
                .AppendLine(Constants.Request)
                .AppendLine(AsUnformattedJsonText(request))
                .AppendLine(Constants.Response)
                .AppendLine(response == null ? Constants.InvalidResponse : AsUnformattedJsonText(response))
                .AppendLine(Constants.BlockLine);
            _logger.LogInformation(logFileEntry.ToString());

            if (Startup.EnableResponseView)
            {
                if (!Model.FilterByUID || (Model.FilterByUID && rpcCallContext?.Uid == Model.uId) || rpcCallContext == null)
                {
                    var builder = new StringBuilder()
                        .AppendLine($"[RPC] {name}");
                    if (rpcCallContext != null)
                    {
                        builder
                            .AppendLine($" {DateTime.UtcNow}  Uid: {rpcCallContext.Uid}  DeviceType: {rpcCallContext.DeviceType}  MachineNum: {rpcCallContext.MachineNum}  SiteId: {rpcCallContext.SiteId}");
                    }
                    else
                    {
                        builder.AppendLine($" {DateTime.UtcNow}");
                    }
                    builder
                        .AppendLine(Constants.Request)
                        .AppendLine(AsJsonText(request))
                        .AppendLine(Constants.Response)
                        .AppendLine(response == null ? Constants.InvalidResponse : AsJsonText(response))
                        .AppendLine(Constants.BlockLine);
                    AppendText(builder.ToString());
                }
            }

        }

        public void LogRpcResponse(string name, object request, object response, RpcCallContext rpcCallContext)
        {
            var prefix = response == null ? "[RPC!]" : "[RPC<]";
            var logFileEntry = new StringBuilder()
                .AppendLine()
                .AppendLine($"{prefix} {name}");
            if (rpcCallContext != null)
            {
                logFileEntry.AppendLine($" {DateTime.UtcNow}  Uid: {rpcCallContext.Uid}  DeviceType: {rpcCallContext.DeviceType}  MachineNum: {rpcCallContext.MachineNum}  SiteId: {rpcCallContext.SiteId}");
            }
            else
            {
                logFileEntry.AppendLine($" {DateTime.UtcNow}");
            }
            logFileEntry
                .AppendLine(Constants.RequestParams)
                .AppendLine(AsUnformattedJsonText(request))
                .AppendLine(Constants.Response)
                .AppendLine(response == null ? Constants.InvalidResponse : AsUnformattedJsonText(response))
                .AppendLine(Constants.BlockLine);
            _logger.LogInformation(logFileEntry.ToString());

            if (Startup.EnableResponseView)
            {
                if (!Model.FilterByUID || (Model.FilterByUID && rpcCallContext?.Uid == Model.uId) || rpcCallContext == null)
                {
                    var builder = new StringBuilder()
                    .AppendLine($"{prefix} {name}");
                    if (rpcCallContext != null)
                    {
                        builder.AppendLine($" {DateTime.UtcNow}  Uid: {rpcCallContext.Uid}  DeviceType: {rpcCallContext.DeviceType}  MachineNum: {rpcCallContext.MachineNum}  SiteId: {rpcCallContext.SiteId}");
                    }
                    else
                    {
                        builder.AppendLine($" {DateTime.UtcNow}");
                    }
                    builder
                    .AppendLine(Constants.RequestParams)
                    .AppendLine(AsJsonText(request))
                    .AppendLine(Constants.Response)
                    .AppendLine(response == null ? Constants.InvalidResponse : AsJsonText(response))
                    .AppendLine(Constants.BlockLine);
                    AppendText(builder.ToString());
                }
            }
        }

        public void LogAuditEvent(auditEvent busEvent, EventCallContext eventCallContext)
        {
            if (Startup.EnableAuditEvents && Startup.EnableResponseView)
            {
                LogEvent(busEvent, busEvent.code.ToString(), eventCallContext);
            }
        }

        public void LogEvent(t_busEvent busEvent, string info, EventCallContext eventCallContext)
        {
            var logFileEntry = new StringBuilder()
                .AppendLine()
                .AppendLine($"[EVT] {info}")
                .AppendLine($" {DateTime.UtcNow}  Uid: {eventCallContext.Uid}  DeviceType: {eventCallContext.DeviceType}  MachineNum: {eventCallContext.MachineNum}  SiteId: {eventCallContext.SiteId}")
                .AppendLine(AsUnformattedJsonText(busEvent))
                .AppendLine(Constants.BlockLine);
            _logger.LogInformation(logFileEntry.ToString());

            if (Startup.EnableResponseView)
            {
                if (!Model.FilterByUID || (Model.FilterByUID && eventCallContext?.Uid == Model.uId) || eventCallContext == null)
                {
                    var builder = new StringBuilder()
                    .AppendLine($"[EVT] {info}")
                    .AppendLine($" {DateTime.UtcNow}  Uid: {eventCallContext.Uid}  DeviceType: {eventCallContext.DeviceType}  MachineNum: {eventCallContext.MachineNum}  SiteId: {eventCallContext.SiteId}")
                    .AppendLine(AsJsonText(busEvent))
                    .AppendLine(Constants.BlockLine);
                    AppendText(builder.ToString());
                }
            }
        }

        public void LogOutBoundEvent(string name, object request)
        {
            var logFileEntry = new StringBuilder()
                .AppendLine()
                .AppendLine($"[EVT>] {name}")
                .AppendLine($" {DateTime.UtcNow}")
                .AppendLine(AsUnformattedJsonText(request))
                .AppendLine(Constants.BlockLine);
            _logger.LogInformation(logFileEntry.ToString());

            var builder = new StringBuilder()
                .AppendLine($"[EVT>] {name}")
                .AppendLine($" {DateTime.UtcNow}")
                .AppendLine(AsJsonText(request))
                .AppendLine(Constants.BlockLine);
            AppendText(builder.ToString());
        }

        public void LogProgress(t_busEvent busEvent, string info, EventCallContext eventCallContext)
        {
            Progress progress = (Progress)busEvent;
            if (progress != null)
            {
                var progressStatus = new ProgressStatus
                {
                    requestIdStr = progress.status.requestIdStr,
                    function = progress.status.function,
                    message = progress.status.message,
                    progress = progress.status.progress,
                    sequence = progress.status.sequence,
                    result = progress.status.result,
                    requestId = progress.status.requestId,
                    datetimme = DateTime.UtcNow.ToString(),
                    DeviceType = eventCallContext.DeviceType == t_deviceType.FN_EVENTS_ID ? "SERVICE" : "SMIB"
                };
                ProgressList.Add(progressStatus);
            }
        }

        public void LogHTTPRequest(HttpContext context, IDictionary<string, object> requestParams)
        {
            var clientContext = $"Id-{context.Connection.Id} Endpoint-{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}";
            var builder = new StringBuilder()
                .AppendLine(DateTime.UtcNow.ToString())
                .AppendLine($"{Constants.Request.ToUpperInvariant()} : {context.Request.Method}")
                .AppendLine($"Received request from client {clientContext}");

            if (requestParams != null)
            {
                builder.AppendLine(Constants.RequestParams.ToUpperInvariant());
                foreach (var param in requestParams)
                {
                    builder.AppendLine(AsJsonText(param));
                }
            }

            if (context.Request.Headers != null)
            {
                string authorization = context.Request.Headers["Authorization"];
                JwtSecurityToken token;
                var extractedToken = ExtractedJwtToken.ExtractJwtToken(authorization, out token);
                if (extractedToken != null)
                {
                    builder.AppendLine(Constants.JWTDecodedToken.ToUpperInvariant());
                    builder.AppendLine(AsJsonText(extractedToken));
                }
                else if (token != null)
                {
                    builder.AppendLine(Constants.JWTToken.ToUpperInvariant());
                    builder.AppendLine(token.ToString());
                }
                else
                {
                    builder.AppendLine(Constants.HttpHeaderRequest.ToUpperInvariant());
                    foreach (var param in context.Request.Headers.Values)
                    {
                        builder.AppendLine(AsJsonText(param));
                    }
                }
            }

            builder.AppendLine(Constants.BlockLine);
            AppendText(builder.ToString());
        }

        public void LogHTTPResponse(HttpContext context, ObjectResult result)
        {
            var builder = new StringBuilder()
                .AppendLine($"{Constants.HttpResponse.ToUpperInvariant()}")
                .AppendLine($"Status Code: {result.StatusCode}");

            if (result.Value != null)
            {
                builder.AppendLine(AsJsonText(result.Value));
            }

            builder.AppendLine(Constants.BlockLine);
            AppendText(builder.ToString());
        }

        public void Clear(object obj)
        {
            Model.Response = string.Empty;
        }

        public void CopyLogToClipboard(object obj)
        {
            // Clipboard not available in web context
        }


        public void DisplayMessage(string message)
        {

            Model.Response = message;
        }
    }
}



