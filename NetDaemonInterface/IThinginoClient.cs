using System;

namespace NetDaemonInterface
{
    public interface IThinginoClient : IDisposable
    {
        void Connect(string host);
        void SetPrivacyMode(string host, bool enabled);
        void SetLight(string host, bool enabled);
        void Tell(string host, string message);        
        void Deurbel(string host);
        void StopDeurbel(string host);
    }
}
