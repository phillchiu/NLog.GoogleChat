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
    public sealed class GoogleChatTarget : TargetWithLayout
    {
        private IHttpClientFactory _httpClientFactory;

        [RequiredParameter]
        public string WebhookUrl { get; set; }

        public GoogleChatTarget()
        {
            
        }

        protected override void InitializeTarget()
        {
            if (string.IsNullOrWhiteSpace(WebhookUrl))
            {
                throw new ArgumentOutOfRangeException("WebhookUrl", "WebhookUrl cannot be empty.");
            }

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

            base.InitializeTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            try
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

                var client = _httpClientFactory.CreateClient("GoogleChatLogger");
                var response = client.PostAsync(WebhookUrl, content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"Failed to send to Google Chat. Status: {response.StatusCode}, Response: {response.Content.ReadAsStringAsync().Result}";
                    InternalLogger.Error(errorMessage);
                }
                else
                {
                    InternalLogger.Debug($"Status Code:{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, ex.Message);
                throw;
            }
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            //TooManyRequests(429) or >= 500 will retry
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => (int)msg.StatusCode == 429 || (int)msg.StatusCode >= 500)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
