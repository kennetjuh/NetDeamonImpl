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
        //Entities.MediaPlayer.Speelkamer.VolumeSet(1);
        //Entities.MediaPlayer.Woonkamer.VolumeSet(1);
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Speelkamer.EntityId, Message = message });
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Woonkamer.EntityId, Message = message });
    }

    public void NotifyGsm(string title, string message, NotifyPriorityEnum prority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        NotifyGsmKen(title, message, prority, tag, actions);
        NotifyGsmGreet(title, message, prority, tag, actions);
    }

    public void NotifyGsmKen(string title, string message, NotifyPriorityEnum prority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(null, false, prority, tag, actions);
        Services.Notify.MobileAppA53(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmKenTTS(string message)
    {
        var title = message;
        message = "TTS";
        var data = ConstructData(null, true);
        Services.Notify.MobileAppA53(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyHouseStateGsmKen(string title, string message, string image, NotifyPriorityEnum priority, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(image, false, priority, NotifyTagEnum.HouseStateChanged, actions);
        Services.Notify.MobileAppA53(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmGreet(string title, string message, NotifyPriorityEnum prority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(null, false, prority, tag, actions);
        Services.Notify.MobileAppGsmGreet(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmGreetTTS(string message)
    {
        var title = message;
        message = "TTS";
        var data = ConstructData(null, true);
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

    private RecordNotifyData ConstructData(string? image = null, bool tts = false, NotifyPriorityEnum priority = NotifyPriorityEnum.high, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        //construct the data here

        RecordNotifyData data = new()
        {
            priority = "high",
            ttl = 0,
            tag = tag.HasValue ? tag.ToString() : null,
            color = "",
            sticky = "true",
            channel = priority == NotifyPriorityEnum.high ? "default" : "silent",
            importance = priority.ToString()
        };

        if (!string.IsNullOrEmpty(image))
        {
            data.image = image;
        }

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
        var data = ConstructData(null, false, NotifyPriorityEnum.high, tag, null);
        Services.Notify.MobileAppA53(new() { Title = "", Message = "clear_notification", Data = data });
    }
}
