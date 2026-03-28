using Microsoft.Extensions.Options;
using NetDaemonInterface;
using NetDaemonInterface.Models;
using Renci.SshNet;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonImpl.Modules
{
    public class ThinginoClient : IThinginoClient, IDisposable
    {
        private readonly ThinginoSettings _settings;
        private readonly ILogger<ThinginoClient> _logger;
        private CancellationTokenSource? _deurbelCts;
        private readonly ConcurrentDictionary<string, SshClient> _connections = new();
        private readonly object _connectionLock = new();
        private bool _disposed;

        public ThinginoClient(IOptions<ThinginoSettings> options, ILogger<ThinginoClient> logger)
        {
            _settings = options?.Value ?? new ThinginoSettings();
            _logger = logger;
        }

        public void Deurbel(string host)
        {
            _deurbelCts?.Cancel();
            var cts = new CancellationTokenSource();
            _deurbelCts = cts;

            Task.Run(async () =>
            {
                try
                {
                    EnsureRingExists(host);
                    SetLight(host, true);

                    // Ring is ~4 sec; keep replaying for up to 30 seconds or until cancelled
                    var timeout = TimeSpan.FromSeconds(30);
                    var interval = TimeSpan.FromSeconds(4);
                    var elapsed = TimeSpan.Zero;

                    while (elapsed < timeout)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        PlayRingSound(host);
                        await Task.Delay(interval, cts.Token);
                        elapsed += interval;
                    }

                    SetLight(host, false);
                }
                catch
                {
                    SetLight(host, false);
                }                
            });
        }

        public void SetPrivacyMode(string host, bool enabled)
        {
            var enabledValue = enabled ? "true" : "false";
            RunCommand(host, $"prudyntctl json '{{\"privacy\":{{\"enabled\":{enabledValue}}}}}'");
        }

        public void SetLight(string host, bool enabled)
        {
            var value = enabled ? "1" : "0";
            RunCommand(host, $"light white {value}");
        }

        public void Tell(string host, string message)
        {
            RunCommand(host, $"tell '{message}'");
        }

        private void EnsureRingExists(string host)
        {
            RunCommand(host, "[ -f /tmp/ring.opus ] || curl --output-dir /tmp --output ring.opus \"http://192.168.1.3:8123/local/ring.opus\"");
        }

        private void PlayRingSound(string host)
        {
            RunCommand(host, "play /tmp/ring.opus");
        }

        public void StopDeurbel(string host)
        {
            _deurbelCts?.Cancel();
            SetLight(host, false);
        }

        private void RunCommand(string host, string command)
        {
            try
            {
                var client = GetOrCreateConnection(host);
                var result = client.RunCommand(command);
                if (result.ExitStatus != 0)
                {
                    _logger.LogWarning("SSH command '{Command}' on {Host} failed with exit code {ExitCode}: {Error}", command, host, result.ExitStatus, result.Error);
                }
            }
            catch (Exception ex)
            {
                // Connection may have gone stale; remove so next call reconnects
                RemoveConnection(host);
                _logger.LogError(ex, "SSH command '{Command}' on {Host} threw an exception", command, host);
            }
        }

        public void Connect(string host)
        {
            GetOrCreateConnection(host);
        }

        private SshClient GetOrCreateConnection(string host)
        {
            lock (_connectionLock)
            {
                if (_connections.TryGetValue(host, out var existing) && existing.IsConnected)
                {
                    return existing;
                }

                existing?.Dispose();
                var client = new SshClient(host, _settings.Username ?? "root", _settings.Password ?? string.Empty);
                client.KeepAliveInterval = TimeSpan.FromSeconds(30);
                client.Connect();
                _connections[host] = client;
                _logger.LogInformation("SSH connection established to {Host}", host);
                return client;
            }
        }

        private void RemoveConnection(string host)
        {
            lock (_connectionLock)
            {
                if (_connections.TryRemove(host, out var client))
                {
                    try { client.Dispose(); } catch { /* best effort */ }
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _deurbelCts?.Cancel();
            _deurbelCts?.Dispose();

            foreach (var kvp in _connections)
            {
                try { kvp.Value.Dispose(); } catch { /* best effort */ }
            }
            _connections.Clear();
        }
    }
}
