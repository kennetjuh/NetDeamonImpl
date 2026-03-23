using NetDaemonInterface;
using NetDaemonInterface.Models;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace NetDaemonImpl.Modules
{
    public class FrigateClient : IFrigateClient
    {
        private readonly FrigateSettings _settings;        

        public FrigateClient(IOptions<FrigateSettings> options)
        {
            _settings = options?.Value ?? new FrigateSettings();
        }

        public async Task MarkReviewedAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            var url = _settings.Api?.BaseUrl?.TrimEnd('/') ?? _settings.Local?.BaseUrl?.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(url)) return;
            var full = $"{url}/api/reviews/viewed";
            var body = new { ids = new[] { id } };
            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            await httpClient.PostAsync(full, content);
        }

        public async Task MarkReviewedWithLoginAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            var loginBase = _settings.LocalLogin?.BaseUrl?.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(loginBase)) return;

            var loginUrl = loginBase + "/api/login";
            var loginBody = new { user = _settings?.LocalLogin?.Username, password = _settings?.LocalLogin?.Password };
            var loginJson = JsonSerializer.Serialize(loginBody);
            using var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

            using var insecureHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var insecureClient = new HttpClient(insecureHandler);

            var loginReq = new HttpRequestMessage(HttpMethod.Post, loginUrl) { Content = loginContent };
            var loginResp = await insecureClient.SendAsync(loginReq).ConfigureAwait(false);
            if (!loginResp.IsSuccessStatusCode) return;

            string? cookieHeader = null;
            if (loginResp.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                cookieHeader = string.Join("; ", cookies.Select(c => c.Split(';', 2)[0]));
            }

            var reviewUrl = loginBase + "/api/reviews/viewed";
            var reviewBody = new { ids = new[] { id } };
            var reviewJson = JsonSerializer.Serialize(reviewBody);
            using var reviewContent = new StringContent(reviewJson, Encoding.UTF8, "application/json");
            var reviewReq = new HttpRequestMessage(HttpMethod.Post, reviewUrl) { Content = reviewContent };
            if (!string.IsNullOrWhiteSpace(cookieHeader))
                reviewReq.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

            await insecureClient.SendAsync(reviewReq).ConfigureAwait(false);
        }
    }
}
