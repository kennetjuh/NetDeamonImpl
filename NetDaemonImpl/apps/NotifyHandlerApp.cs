using NetDaemonInterface;
using System.Text.Json.Serialization;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class NotifyHandlerApp : MyNetDaemonBaseApp
{
    private readonly INotify notify;

    private record notificationEventDataElement
    {
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("message")]
        public string? Message { get; init; }

        [JsonPropertyName("action")]
        public string? Action { get; init; }

        [JsonPropertyName("device_id")]
        public string? DeviceId { get; init; }
    }

    public NotifyHandlerApp(IHaContext haContext, IScheduler scheduler, ILogger<NotifyHandlerApp> logger, INotify notify, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        this.notify = notify;
        _haContext.Events.Where(x => x.EventType == "mobile_app_notification_action")
        .Subscribe(x =>
        {
            HandleNotificationEvent(x);
        });
    }

    private void HandleNotificationEvent(Event x)
    {
        //Event {
        //  DataElement = {
        //      "title": "Test",
        //      "message": "Test",
        //      "action_1_title": "Action 1",
        //      "action_1_key": "Action1",
        //      "action": "Action1",
        //      "device_id": "22a04b947225e0c2"
        //      },
        //  EventType = mobile_app_notification_action,
        //  Origin = REMOTE, TimeFired = 17-2-2022 15:21:49 }}

        if (x.DataElement.HasValue)
        {
            var notificationEventDataElement = System.Text.Json.JsonSerializer.Deserialize<notificationEventDataElement>(x.DataElement.Value);

            if (notificationEventDataElement == null || notificationEventDataElement.Action == null)
            {
                _logger.LogWarning($"Unable to parse notification event: {x.DataElement.Value}");
                return;
            }

            _logger.LogInformation($"{notificationEventDataElement.Action}");

            if (Enum.TryParse<NotifyActionEnum>(notificationEventDataElement.Action, out var result))
            {
                notify.HandleNotificationEvent(result);
            }
        }
    }
}