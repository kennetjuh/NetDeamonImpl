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
        Entities.MediaPlayer.Keuken.VolumeSet(1);
        Entities.MediaPlayer.Woonkamer.VolumeSet(1);
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Keuken.EntityId, Message = message });
        Services.Tts.GoogleTranslateSay(new() { EntityId = Entities.MediaPlayer.Woonkamer.EntityId, Message = message });
    }

    public void NotifyGsm(string title, string message, NotifyPriorityEnum prority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        NotifyGsmKen(title, message, prority, tag, actions);
    }

    public void NotifyGsmKen(string title, string message, NotifyPriorityEnum prority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(null, false, null, prority, tag, actions);
        Services.Notify.MobileAppA53(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmKenTTS(string message)
    {
        var title = message;
        var data = ConstructData(null, true, message);
        message = "TTS";
        Services.Notify.MobileAppA53(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyHouseStateGsmKen(string title, string message, string image, NotifyPriorityEnum priority, List<NotifyActionEnum>? actions = null)
    {
        var data = ConstructData(image, false, null, priority, NotifyTagEnum.HouseStateChanged, actions);
        Services.Notify.MobileAppA53(new() { Title = title, Message = message, Data = data });
    }

    public void NotifyGsmAlarm()
    {
        var message = "Huis alarm is geactiveerd, ik herhaal het huis alarm is geactiveerd";
        NotifyGsmKen("Alarm triggered", "Alarm triggered", NotifyPriorityEnum.high);
        NotifyGsmKenTTS(message);
    }

    public void HandleNotificationEvent(NotifyActionEnum action)
    {
        notifyActions.Actions[action]?.Action?.Invoke();
    }

    private RecordNotifyData ConstructData(string? image = null, bool tts = false, string? ttsText = null, NotifyPriorityEnum priority = NotifyPriorityEnum.high, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null)
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
            data.tts_text = ttsText;
        }

        if (actions != null)
        {
            data.actions = notifyActions.Actions.Where(x => actions.Contains(x.Key)).Select(x => new RecordNotifyAction(x.Value)).ToList();
        }

        return data;
    }

    public void Clear(NotifyTagEnum tag)
    {
        var data = ConstructData(null, false, null, NotifyPriorityEnum.high, tag, null);
        Services.Notify.MobileAppA53(new() { Title = "", Message = "clear_notification", Data = data });
    }
}
