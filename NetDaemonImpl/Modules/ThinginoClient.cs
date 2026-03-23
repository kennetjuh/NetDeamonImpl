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
    public class ThinginoClient : IThinginoClient
    {
        private readonly ThinginoSettings _settings;

        public ThinginoClient(IOptions<ThinginoSettings> options)
        {
            _settings = options?.Value ?? new ThinginoSettings();
        }

        public async Task<string?> LoginAsync(string baseUrl)
        {
            var loginUrl = $"{baseUrl}/x/login.cgi";
            var loginBody = new { username = _settings.Username, password = _settings.Password };
            var loginJson = JsonSerializer.Serialize(loginBody);
            using var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            var loginReq = new HttpRequestMessage(HttpMethod.Post, loginUrl) { Content = loginContent };
            var loginResp = await client.SendAsync(loginReq).ConfigureAwait(false);
            if (!loginResp.IsSuccessStatusCode) return null;
            if (loginResp.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                return string.Join("; ", cookies.Select(c => c.Split(';', 2)[0]));
            }
            return null;
        }

        public async Task SetPrivacyModeAsync(string baseUrl, bool enabled)
        {
            var cookie = await LoginAsync(baseUrl).ConfigureAwait(false);
            var reviewUrl = $"{baseUrl}/x/json-prudynt.cgi";
            var reviewBody = new { privacy = new { enabled } };
            var reviewJson = JsonSerializer.Serialize(reviewBody);
            using var reviewContent = new StringContent(reviewJson, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            var reviewReq = new HttpRequestMessage(HttpMethod.Post, reviewUrl) { Content = reviewContent };
            if (!string.IsNullOrWhiteSpace(cookie)) reviewReq.Headers.TryAddWithoutValidation("Cookie", cookie);
            await client.SendAsync(reviewReq).ConfigureAwait(false);
        }
    }
}
