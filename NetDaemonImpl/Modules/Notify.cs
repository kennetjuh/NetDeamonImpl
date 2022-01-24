using NetDaemonInterface;

namespace NetDaemonImpl.Modules;

public class Notify : INotify
{
    private readonly object dataNormal = new { ttl = 0, priority = "high" };
    private readonly object dataTTS = new { ttl = 0, priority = "high", channel = "alarm_stream_max" };
    private readonly Entities Entities;
    private readonly Services Services;

    public Notify(IServiceProvider provider)
    {
        var haContext = DiHelper.GetHaContext(provider);
        Entities = new Entities(haContext);
        Services = new Services(haContext);
    }

    public void NotifyGsm(string title, string message)
    {
        NotifyGsmKen(title, message);
        NotifyGsmGreet(title, message);
    }

    public void NotifyGsmKen(string title, string message)
    {
        Services.Notify.MobileAppGsmKen(new() { Title = title, Message = message, Data = dataNormal });
    }

    public void NotifyGsmKenTTS(string message)
    {
        var title = message;
        message = "TTS";
        Services.Notify.MobileAppGsmKen(new() { Title = title, Message = message, Data = dataTTS });
    }

    public void NotifyGsmGreet(string title, string message)
    {
        Services.Notify.MobileAppGsmGreet(new() { Title = title, Message = message, Data = dataNormal });
    }

    public void NotifyGsmGreetTTS(string message)
    {
        var title = message;
        message = "TTS";
        Services.Notify.MobileAppGsmGreet(new() { Title = title, Message = message, Data = dataTTS });
    }

    public void NotifyGsmAlarm()
    {
        var message = "The home alarm has been activated, i repeat, the home alarm has been activated!";
        NotifyGsmKenTTS(message);
        NotifyGsmGreetTTS(message);
    }

    public void NotifyHouse(string message)
    {
        Entities.MediaPlayer.Hal.VolumeSet(1);
        Entities.MediaPlayer.Woonkamer.VolumeSet(1);
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Hal.EntityId, Message = message });
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Woonkamer.EntityId, Message = message });
    }
}
