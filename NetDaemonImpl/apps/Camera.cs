using NetDaemonInterface;
using NetDaemonInterface.Observable;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using NetDaemonInterface.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonImpl.apps
{
    public record FrigateTriggerMessage(
        [property: JsonPropertyName("id")] string? Id,
        [property: JsonPropertyName("idx")] string? Idx,
        [property: JsonPropertyName("alias")] string? Alias,
        [property: JsonPropertyName("platform")] string? Platform,
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("payload")] string? Payload,
        [property: JsonPropertyName("qos")] int? Qos,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("payload_json")] FrigateReviewPayload? PayloadJson
    );

    public record FrigateReviewPayload(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("before")] FrigateReviewEntry? Before,
        [property: JsonPropertyName("after")] FrigateReviewEntry? After
    );

    public record FrigateReviewEntry(
        [property: JsonPropertyName("id")] string? Id,
        [property: JsonPropertyName("camera")] string? Camera,
        [property: JsonPropertyName("start_time")] double? StartTime,
        [property: JsonPropertyName("end_time")] double? EndTime,
        [property: JsonPropertyName("severity")] string? Severity,
        [property: JsonPropertyName("thumb_path")] string? ThumbPath,
        [property: JsonPropertyName("data")] FrigateReviewData? Data
    );

    public record FrigateReviewData(
        [property: JsonPropertyName("detections")] List<string>? Detections,
        [property: JsonPropertyName("objects")] List<string>? Objects,
        [property: JsonPropertyName("verified_objects")] List<string>? VerifiedObjects,
        [property: JsonPropertyName("sub_labels")] List<string>? SubLabels,
        [property: JsonPropertyName("zones")] List<string>? Zones,
        [property: JsonPropertyName("audio")] List<string>? Audio,
        [property: JsonPropertyName("thumb_time")] double? ThumbTime,
        [property: JsonPropertyName("metadata")] JsonElement? Metadata
    );

    [NetDaemonApp]
    public class Camera : MyNetDaemonBaseApp
    {
        private static readonly HttpClient _httpClient = new();
        private readonly ITriggerManager triggerManager;
        private readonly IEnumerable<SwitchEntity> cameraAlertSwitches;
        private readonly IEnumerable<SwitchEntity> cameraDetectSwitches;        

        public Camera(ITriggerManager triggerManager, IHaContext haContext, IScheduler scheduler, ILogger<Camera> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IDayNightEvents dayNightEvents, IHouseStateEvents houseStateEvents,
        IFrigateClient frigateClient, IThinginoClient thinginoClient)
            : base(haContext, scheduler, logger, settingsProvider)
        {            

            this.triggerManager = triggerManager;
            //cameraAlertSwitches = _haContext.GetAllEntities().Where(e => e.EntityId.EndsWith("_review_alerts") && e.Registration?.Platform == "frigate").Select(x => new SwitchEntity(x));
            //cameraDetectSwitches = _haContext.GetAllEntities().Where(e => e.EntityId.EndsWith("_review_detections") && e.Registration?.Platform == "frigate").Select(x => new SwitchEntity(x));
            //houseStateEvents.Event.Subscribe(HandleHouseState);
            //HandleHouseState(new HouseStateEvent(Helper.GetHouseState(_entities), new(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));

            var frigate = triggerManager.RegisterTrigger<FrigateTriggerMessage>(
                new
                {
                    platform = "mqtt",
                    topic = $"frigate/reviews"
                });


            frigate.Subscribe(async x =>
            {
                try
                {
                    var id = x?.PayloadJson?.After?.Id;
                    await frigateClient.MarkReviewedAsync(id);
                    await frigateClient.MarkReviewedWithLoginAsync(id);

                    foreach (var detection in x?.PayloadJson?.After?.Data?.Detections ?? Enumerable.Empty<string>())
                    {
                        await frigateClient.MarkReviewedAsync(detection);
                        await frigateClient.MarkReviewedWithLoginAsync(detection);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing frigate review message");
                }
            });
        }
    }
}
