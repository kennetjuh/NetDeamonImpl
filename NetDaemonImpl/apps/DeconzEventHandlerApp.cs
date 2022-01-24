using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetDaemonImpl.apps;

/// <summary>
/// This app handles all deconz events (buttons use events) and supply the events to Area's
/// </summary>
[NetDaemonApp]
public class DeconzEventHandlerApp : MyNetDaemonBaseApp
{
    private readonly Dictionary<string, Action<DeconzEventIdEnum>> mapping;

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

    public DeconzEventHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        IAreaCollection areaCollection)
        : base(haContext, scheduler, logger)
    {
        mapping = new()
        {
            { "button_keuken_1", (x) => areaCollection.Keuken.ButtonPressed(_entities.Sensor.ButtonKeuken1BatteryLevel.EntityId, x) },
            { "button_keuken_2", (x) => areaCollection.Keuken.ButtonPressed(_entities.Sensor.ButtonKeuken2BatteryLevel.EntityId, x) },
            { "button_woonkamer", (x) => areaCollection.Woonkamer.ButtonPressed(_entities.Sensor.ButtonWoonkamerBatteryLevel.EntityId, x) },
            { "button_slaapkamerbed", (x) => areaCollection.Slaapkamer.ButtonPressed(_entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, x) },
            { "button_wc", (x) => areaCollection.Wc.ButtonPressed(_entities.Sensor.ButtonWcBatteryLevel.EntityId, x) },
            { "button_badkamer", (x) => areaCollection.Badkamer.ButtonPressed(_entities.Sensor.ButtonBadkamerBatteryLevel.EntityId, x) },
            { "button_speelkamer", (x) => areaCollection.Speelkamer.ButtonPressed(_entities.Sensor.ButtonSpeelkamerBatteryLevel.EntityId, x) },
            { "button_buitenachter", (x) => areaCollection.BuitenAchter.ButtonPressed(_entities.Sensor.ButtonBuitenachterBatteryLevel.EntityId, x) },
            { "button_slaapkamer", (x) => areaCollection.Slaapkamer.ButtonPressed(_entities.Sensor.ButtonSlaapkamerBatteryLevel.EntityId, x) },
            { "button_halboven_1", (x) => areaCollection.HalBoven.ButtonPressed(_entities.Sensor.ButtonHalboven1BatteryLevel.EntityId, x) },
            { "button_halboven_2", (x) => areaCollection.HalBoven.ButtonPressed(_entities.Sensor.ButtonHalboven2BatteryLevel.EntityId, x) },
            { "button_cabine", (x) => areaCollection.Cabine.ButtonPressed(_entities.Sensor.ButtonCabineBatteryLevel.EntityId, x) },
            { "button_hal", (x) => areaCollection.Hal.ButtonPressed(_entities.Sensor.ButtonHalBatteryLevel.EntityId, x) },
            { "button_kamerlamp", (x) => areaCollection.Woonkamer.ButtonPressed(_entities.Sensor.ButtonKamerlampBatteryLevel.EntityId, x) },
            { "button_bureaulamp", (x) => areaCollection.Woonkamer.ButtonPressed(_entities.Sensor.ButtonBureaulampBatteryLevel.EntityId, x) },
            { "button_slaapkamerkids", (x) => areaCollection.SlaapkamerKids.ButtonPressed(_entities.Sensor.ButtonSlaapkamerkidsBatteryLevel.EntityId, x) },
            { "button_booglamp", (x) => areaCollection.Woonkamer.ButtonPressed(_entities.Sensor.ButtonBooglampBatteryLevel.EntityId, x) },
            { "button_washal", (x) => areaCollection.Washal.ButtonPressed(_entities.Sensor.ButtonWashalBatteryLevel.EntityId, x) },
            { "button_buitenachterlamp", (x) => areaCollection.BuitenAchter.ButtonPressed(_entities.Sensor.ButtonBuitenachterlampBatteryLevel.EntityId, x) },
            { "button_buitenachterzithoek", (x) => areaCollection.BuitenAchter.ButtonPressed(_entities.Sensor.ButtonBuitenachterzithoekBatteryLevel.EntityId, x) },
            { "button_traphal", (x) => areaCollection.Traphal.ButtonPressed(_entities.Sensor.ButtonTraphalBatteryLevel.EntityId, x) },
        };

        _haContext.Events
            .Where(x => x.EventType == "deconz_event")
            .Subscribe(x =>
              {
                  HandleDeconzEvent(x);
              });
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

            if (mapping.ContainsKey(deconzEventDataElement.Id))
            {
                mapping[deconzEventDataElement.Id](deconzEvent);
            }
            else
            {
                _logger.LogWarning($"Unmapped button pressed '{deconzEventDataElement.Id}'");
            }

        }
    }
}