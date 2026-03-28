using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;

namespace NetDaemonImpl.IObservable
{
    public class ButtonEvents : IButtonEvents
    {
        private readonly Dictionary<Button, DateTime> previousEvents = [];

        private DateTime GetAndUpdatePreviousEvent(Button button)
        {
            var prevTime = DateTime.MinValue;
            if (previousEvents.TryGetValue(button, out DateTime value))
            {
                prevTime = value;
            }
            previousEvents[button] = DateTime.Now;
            return prevTime;
        }

        private record DeconzEventDataElement
        {
            [JsonPropertyName("id")]
            public string? Id { get; init; }

            [JsonPropertyName("unique_id")]
            public string? UniqueId { get; init; }

            [JsonPropertyName("event")]
            public int? Event { get; init; }

            [JsonPropertyName("device_id")]
            public string? DeviceId { get; init; }
        }

        private record ZhaEventDataElement
        {
            [JsonPropertyName("command")]
            public string? Command { get; init; }

            [JsonPropertyName("device_id")]
            public string? DeviceId { get; init; }
        }

        private readonly ILogger<ButtonEvents> logger;

        private readonly Subject<ButtonEvent> _subject = new();
        public IObservable<ButtonEvent> Event => _subject.AsObservable();

        public ButtonEvents(IServiceProvider provider, ILogger<ButtonEvents> logger)
        {
            this.logger = logger;
            var haContext = DiHelper.GetHaContext(provider);
            haContext.Events
            .Where(x => x.EventType == "deconz_event")
            .Subscribe(HandleDeconzEvent);

            haContext.Events
            .Where(x => x.EventType == "zha_event")
            .Subscribe(HandleZhaEvent);
        }

        private void HandleDeconzEvent(Event x)
        {
            try
            {
                //  Event {
                //      DataElement = {"id": "button_kamerlamp", "unique_id": "00:15:8d:00:06:36:db:8f", "event": 1002, "device_id": "baf3c4bd350ff84c35a2ab80edeef5d9"},
                //      EventType = deconz_event,
                //      Origin = LOCAL,
                //      TimeFired = 3-12-2021 16:38:30 }
                if (x.DataElement.HasValue)
                {
                    var deconzEventDataElement = System.Text.Json.JsonSerializer.Deserialize<DeconzEventDataElement>(x.DataElement.Value);

                    if (deconzEventDataElement == null || deconzEventDataElement.Event == null || deconzEventDataElement.Id == null)
                    {
                        logger.LogWarning($"Unable to parse deconz event: {x.DataElement.Value}");
                        return;
                    }

                    if (deconzEventDataElement.Event == 1003)
                    {
                        // I have no interest in the release events so i block them here
                        return;
                    }
                    var deconzEvent = (ButtonEventType)deconzEventDataElement.Event;
                    var id = deconzEventDataElement.Id.ToButtonId();

                    if (id == Button.Unknown)
                    {
                        logger.LogWarning($"Unknown Deconz button id: {deconzEventDataElement.Id}");
                        return;
                    }

                    logger.LogInformation($"Deconz event received: {id} {deconzEvent}");
                    _subject.OnNext(new ButtonEvent(id, deconzEvent, GetAndUpdatePreviousEvent(id)));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling deconz event");
            }
        }
        private void HandleZhaEvent(Event x)
        {
            try
            {
                /* event_type: zha_event
                    data:
                      device_ieee: 00:15:8d:00:06:26:90:72
                      device_id: 5a6c7fad830336920fe922bf816f2256
                      unique_id: 00:15:8d:00:06:26:90:72:1:0x0012
                      endpoint_id: 1
                      cluster_id: 18
                      command: double
                      args:
                        value: 2
                      params: {}
                    origin: LOCAL
                    time_fired: "2026-01-11T12:37:32.092076+00:00"
                    context:
                      id: 01KEPGZJSWSWH1096Y34MRT324
                      parent_id: null
                      user_id: null
                 */
                if (x.DataElement.HasValue)
                {
                    var ZhaEventDataElement = System.Text.Json.JsonSerializer.Deserialize<ZhaEventDataElement>(x.DataElement.Value);

                    if (ZhaEventDataElement == null || ZhaEventDataElement.Command == null || ZhaEventDataElement.DeviceId == null)
                    {
                        logger.LogWarning($"Unable to parse Zha event: {x.DataElement.Value}");
                        return;
                    }

                    var buttonEvent = ZhaEventDataElement.Command.ToButtonEventType();
                    var id = ZhaEventDataElement.DeviceId.ToButtonId();

                    if (buttonEvent == ButtonEventType.Unknown)
                    {
                        logger.LogWarning("Unknown Zha button event: {Command} from device: {DeviceId}, full data: {Data}", ZhaEventDataElement.Command, ZhaEventDataElement.DeviceId, x.DataElement.Value);
                        return;
                    }

                    if (id == Button.Unknown)
                    {
                        logger.LogWarning($"Unknown Zha button id: {ZhaEventDataElement.DeviceId}");
                        return;
                    }

                    logger.LogInformation($"Zha event received: {id} {buttonEvent}");
                    _subject.OnNext(new ButtonEvent(id, buttonEvent, GetAndUpdatePreviousEvent(id)));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling Zha event");
            }
        }

    }
}
