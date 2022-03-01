using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules.Notify;

public class Notify : INotify
{
    private readonly IHaContext HaContext;
    private readonly Entities Entities;
    private readonly Services Services;

    private readonly NotifyActions notifyActions;

    public Notify(IServiceProvider provider)
    {
        HaContext = DiHelper.GetHaContext(provider);
        Entities = new Entities(HaContext);
        Services = new Services(HaContext);
        notifyActions = new NotifyActions(HaContext, this);
    }

    public void NotifyHouse(string message)
    {
        Entities.MediaPlayer.Hal.VolumeSet(1);
        Entities.MediaPlayer.Woonkamer.VolumeSet(1);
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Hal.EntityId, Message = message });
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Woonkamer.EntityId, Message = message });
    }

    public void NotifyGsm(string title, string message, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        NotifyGsmKen(title, message);
        NotifyGsmGreet(title, message);
    }

    public void NotifyGsmKen(string title, string message, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(false, tag, actions);
        Services.Notify.MobileAppGsmKen(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmKenTTS(string message)
    {
        var title = message;
        message = "TTS";
        var data = ConstructData(true);
        Services.Notify.MobileAppGsmKen(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmGreet(string title, string message, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(false, tag, actions);
        Services.Notify.MobileAppGsmGreet(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmGreetTTS(string message)
    {
        var title = message;
        message = "TTS";
        var data = ConstructData(true);
        Services.Notify.MobileAppGsmGreet(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmAlarm()
    {
        var message = "The home alarm has been activated, i repeat, the home alarm has been activated!";
        NotifyGsmKenTTS(message);
        NotifyGsmGreetTTS(message);
    }

    public void HandleNotificationEvent(NotifyActionEnum action)
    {
        notifyActions.Actions[action]?.Action?.Invoke();      
    }

    private RecordNotifyData ConstructData(bool tts = false, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        //construct the data here

        RecordNotifyData data = new()
        {
            priority = "high",
            ttl = 0,
            tag = tag.HasValue ? tag.ToString() : null,
            color = "",
            sticky= "true"
        };

        if (tts)
        {
            data.channel = "alarm_stream_max";
        }

        if (actions != null)
        {
            data.actions = notifyActions.Actions.Where(x => actions.Contains(x.Key)).Select(x => new RecordNotifyAction(x.Value)).ToList();
        }

        return data;
    }

    public void Clear(NotifyTagEnum tag)
    {
        var data = ConstructData(false, tag, null);
        Services.Notify.MobileAppGsmKen(new() { Title = "", Message = "clear_notification", Data = data });
    }
}
