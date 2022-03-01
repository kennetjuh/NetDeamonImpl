using System.Collections.Generic;

namespace NetDaemonInterface;

public interface INotify
{
    void Clear(NotifyTagEnum tag);
    void NotifyGsm(string title, string message, NotifyPriorityEnum priority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null);
    void NotifyGsmAlarm();
    void NotifyGsmGreet(string title, string message, NotifyPriorityEnum priority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null);
    void NotifyGsmGreetTTS(string message);
    void NotifyGsmKen(string title, string message, NotifyPriorityEnum priority, NotifyTagEnum? tag = null, List<NotifyActionEnum>? actions = null);
    void NotifyHouseStateGsmKen(string title, string message, string imagePath, NotifyPriorityEnum priority, List<NotifyActionEnum>? actions = null);
    void NotifyGsmKenTTS(string message);
    void NotifyHouse(string message);
    void HandleNotificationEvent(NotifyActionEnum action);
}
