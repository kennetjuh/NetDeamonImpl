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
        private readonly FrigateSettings _frigateSettings;

        public Camera(ITriggerManager triggerManager, IHaContext haContext, IScheduler scheduler, ILogger<Camera> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IDayNightEvents dayNightEvents, IHouseStateEvents houseStateEvents, IOptions<FrigateSettings> frigateOptions)
            : base(haContext, scheduler, logger, settingsProvider)
        {
            _frigateSettings = frigateOptions?.Value ?? new FrigateSettings();
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


            frigate.Subscribe(async m =>
            {
                try
                {

                    var id = m?.PayloadJson?.After?.Id;
                    await MarkFrigateReviewedAsync(id, _frigateSettings.Local);
                    // use configured credentials for local Frigate
                    await MarkFrigateReviewedWithLocalLoginAsync(id, _frigateSettings.LocalLogin);

                    foreach (var detection in m?.PayloadJson?.After?.Data?.Detections ?? Enumerable.Empty<string>())
                    {
                        await MarkFrigateReviewedAsync(detection, _frigateSettings.Local);
                        await MarkFrigateReviewedWithLocalLoginAsync(detection, _frigateSettings.LocalLogin);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing frigate review message");
                }
            });

        }
        public async Task MarkFrigateReviewedAsync(string? id, FrigateLocal frigateLocal)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            if (Helper.GetHouseState(_entities) != HouseStateEnum.Awake) return; // Only mark as reviewed if we're not awake, otherwise we might miss important notifications

            try
            {                
                var url = $"{frigateLocal.BaseUrl}/api/reviews/viewed";
                var body = new { ids = new[] { id } };
                var json = JsonSerializer.Serialize(body);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var resp = await _httpClient.PostAsync(url, content);
                var respBody = await resp.Content.ReadAsStringAsync();
     
                if (!resp.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Frigate 5000 mark-reviewed failed for {Id}, status {Status}", id, resp.StatusCode);
                }
                else
                {
                    //_logger?.LogInformation("Frigate 5000 marked reviewed: {Id}", id);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error marking frigate 5000 review as reviewed");
            }
        }

        public async Task MarkFrigateReviewedWithLocalLoginAsync(string? id, FrigateLocalLogin frigateLocalLogin)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            if (string.IsNullOrWhiteSpace(frigateLocalLogin.Username) || string.IsNullOrWhiteSpace(frigateLocalLogin.Password))
            {
                _logger?.LogWarning("Local login credentials missing");
                return;
            }
            if (Helper.GetHouseState(_entities) != HouseStateEnum.Awake) return;

            try
            {
                var loginUrl = frigateLocalLogin.BaseUrl + "/api/login";

                // Frigate login expects 'user' per some endpoints/examples
                var loginBody = new { user = frigateLocalLogin.Username, password = frigateLocalLogin.Password };
                var loginJson = JsonSerializer.Serialize(loginBody);
                using var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

                using var loginReq = new HttpRequestMessage(HttpMethod.Post, loginUrl) { Content = loginContent };
                loginReq.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Use an HttpClient that ignores invalid SSL certs for the local Frigate instance
                using var insecureHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var insecureClient = new HttpClient(insecureHandler);

                var loginResp = await insecureClient.SendAsync(loginReq).ConfigureAwait(false);
                var loginRespContentDbg = await loginResp.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!loginResp.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Frigate 8971 login failed, status {Status} body: {Body}", loginResp.StatusCode, loginRespContentDbg);
                    return;
                }

                string? cookieHeader = null;

                var loginRespContent = await loginResp.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (loginResp.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    cookieHeader = string.Join("; ", cookies.Select(c => c.Split(';', 2)[0]));
                }

                var reviewUrl = frigateLocalLogin.BaseUrl + "/api/reviews/viewed";
                var reviewBody = new { ids = new[] { id } };
                var reviewJson = JsonSerializer.Serialize(reviewBody);
                
                using var reviewContent = new StringContent(reviewJson, Encoding.UTF8, "application/json");
                using var reviewReq = new HttpRequestMessage(HttpMethod.Post, reviewUrl) { Content = reviewContent };
                reviewReq.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(cookieHeader))
                {
                    reviewReq.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
                }

                var reviewResp = await insecureClient.SendAsync(reviewReq).ConfigureAwait(false);
                var reviewRespBody = await reviewResp.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!reviewResp.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Frigate 8971 mark-reviewed failed for {Id}, status {Status}", id, reviewResp.StatusCode);
                }
                else
                {
                    //_logger?.LogInformation("Frigate 8971 marked reviewed: {Id}", id);
                }
            }
            catch (System.Exception ex)
            {
                _logger?.LogError(ex, "Error marking frigate 8971 review as reviewed");
            }
        }

    }
}
