using NetDaemonInterface;
using System.Linq;
using System.Text.Json.Serialization;

namespace NetDaemonImpl.apps;

/// <summary>
/// This app handles all deconz events (buttons use events) and supply the events to Area's
/// </summary>
[NetDaemonApp]
public class DeconzEventHandlerApp : MyNetDaemonBaseApp
{
    private readonly DeconzButtonMapping mapping;

    private IAreaCollection AreaCollection { get; }

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

    public DeconzEventHandlerApp(IHaContext haContext, IScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        IAreaCollection areaCollection, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        mapping = new(_entities);
        _haContext.Events
            .Where(x => x.EventType == "deconz_event")
            .Subscribe(x =>
              {
                  HandleDeconzEvent(x);
              });
        AreaCollection = areaCollection;
    }

    private void HandleDeconzEvent(Event x)
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
                _logger.LogWarning($"Unable to parse deconz event: {x.DataElement.Value}");
                return;
            }

            if (deconzEventDataElement.Event == 1003)
            {
                // I have no interest in the release events so i block them here
                return;
            }
            var deconzEvent = (DeconzEventIdEnum)deconzEventDataElement.Event;


            _logger.LogInformation($"{deconzEventDataElement.Id} {deconzEvent}");

            var map = mapping.mapping.SingleOrDefault(x => x.Item1 == deconzEventDataElement.Id);

            if (map != null)
            {
                AreaCollection.GetArea(map.Item2).ButtonPressed(map.Item3, deconzEvent);
            }
            else
            {
                _logger.LogWarning($"Unmapped button pressed '{deconzEventDataElement.Id}'");
            }
        }
    }
}