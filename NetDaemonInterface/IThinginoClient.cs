using System.Threading.Tasks;

namespace NetDaemonInterface
{
    public interface IThinginoClient
    {
        Task<string?> LoginAsync(string baseUrl);
        Task SetPrivacyModeAsync(string baseUrl, bool enabled);
    }
}
