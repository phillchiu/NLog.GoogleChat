using Microsoft.Extensions.DependencyInjection;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NLog.GoogleChat
{
    [Target("GoogleChat")]
    public sealed class GoogleChatTarget : AsyncTaskTarget
    {
        private readonly IHttpClientFactory _httpClientFactory;
        [RequiredParameter]
        public string WebhookUrl { get; set; }

        public GoogleChatTarget()
        {
            InternalLogger.Debug("=======initialize constructor start=======");
            var services = new ServiceCollection();
            services.AddHttpClient("GoogleChatLogger", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy());

            _httpClientFactory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

            InternalLogger.Debug("=======initialize constructor end=======");
        }

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            var message = Layout.Render(logEvent);
            var payload = new
            {
                text = $"{message}"
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            try
            {
                var client = _httpClientFactory.CreateClient("GoogleChatLogger");
                var response = await client.PostAsync(WebhookUrl, content, cancellationToken);
                InternalLogger.Debug($"response status : {response.StatusCode}");
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, ex.Message);
                throw;
            }
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
