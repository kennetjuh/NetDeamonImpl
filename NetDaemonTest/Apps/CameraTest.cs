using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetDaemonTest.Apps;

public class CameraTest : TestBase
{
    internal override void ResetAllMocks()
    {
        base.ResetAllMocks();
        ThinginoClientMock.Setup(x => x.Connect(It.IsAny<string>()));
        ThinginoClientMock.Setup(x => x.StopDeurbel(It.IsAny<string>()));
    }
    [Fact]
    public void Camera_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<Camera>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Camera_DeurbelOn_NotifiesAndActivatesSequence()
    {
        // Arrange
        ResetAllMocks();
        ThinginoClientMock.Setup(x => x.Deurbel(It.IsAny<string>()));
        FrigateClientMock.Setup(x => x.SaveLatestImageAsync("deurbel.png")).Returns(Task.CompletedTask);
        NotifyMock.Setup(x => x.NotifyGsmKenDeurbel("/local/deurbel.png"));
        HaMock.Setup(x => x.CallService("light", "turn_off",
            It.Is<ServiceTarget>(s => s.EntityIds!.Single() == Entities.Light.Deurbel.EntityId),
            It.IsAny<object>()));

        var app = Context.GetApp<Camera>();

        // Act
        HaMock.TriggerStateChange(Entities.Light.Deurbel, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Camera_DeurbelOnTwiceWithinOneSecond_NotifiesOnlyOnce()
    {
        // Arrange
        ResetAllMocks();
        ThinginoClientMock.Setup(x => x.Deurbel(It.IsAny<string>()));
        FrigateClientMock.Setup(x => x.SaveLatestImageAsync("deurbel.png")).Returns(Task.CompletedTask);
        NotifyMock.Setup(x => x.NotifyGsmKenDeurbel("/local/deurbel.png"));
        HaMock.Setup(x => x.CallService("light", "turn_off",
            It.Is<ServiceTarget>(s => s.EntityIds!.Single() == Entities.Light.Deurbel.EntityId),
            It.IsAny<object>()));

        var app = Context.GetApp<Camera>();        

        // Act - first trigger
        HaMock.TriggerStateChange(Entities.Light.Deurbel, "on");
        Thread.Sleep(500);
        // Act - second trigger within 1 second should be debounced (TurnOff still fires, but notify/thingino/frigate don't)
        HaMock.TriggerStateChange(Entities.Light.Deurbel, "on");

        // Assert - notify/thingino/frigate called only once
        ThinginoClientMock.Verify(x => x.Deurbel(It.IsAny<string>()), Times.Once);
        FrigateClientMock.Verify(x => x.SaveLatestImageAsync("deurbel.png"), Times.Once);
        NotifyMock.Verify(x => x.NotifyGsmKenDeurbel("/local/deurbel.png"), Times.Once);
    }

    [Fact]
    public void Camera_DeurbelOff_NoNotification()
    {
        // Arrange
        ResetAllMocks();
        var app = Context.GetApp<Camera>();

        // Act
        HaMock.TriggerStateChange(Entities.Light.Deurbel, "off");

        // Assert - no frigate/thingino/notify calls expected
        VerifyAllMocks();
    }

    [Fact]
    public async Task Camera_FrigateReview_HouseAwake_MarksReviewed()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate, HouseStateEnum.Awake.ToString());

        var reviewId = "review-123";
        var detectionId = "detection-456";

        FrigateClientMock.Setup(x => x.MarkReviewedAsync(reviewId)).Returns(Task.CompletedTask);
        FrigateClientMock.Setup(x => x.MarkReviewedWithLoginAsync(reviewId)).Returns(Task.CompletedTask);
        FrigateClientMock.Setup(x => x.MarkReviewedAsync(detectionId)).Returns(Task.CompletedTask);
        FrigateClientMock.Setup(x => x.MarkReviewedWithLoginAsync(detectionId)).Returns(Task.CompletedTask);

        var triggerSubject = new System.Reactive.Subjects.Subject<FrigateTriggerMessage?>();
        TriggerManagerMock
            .Setup(x => x.RegisterTrigger(It.IsAny<object>()))
            .Returns(triggerSubject.Select(m =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(m);
                return System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
            }));

        var app = Context.GetApp<Camera>();

        // Act
        triggerSubject.OnNext(new FrigateTriggerMessage(
            null, null, null, null, null, null, null, null,
            new FrigateReviewPayload(null, null,
                new FrigateReviewEntry(reviewId, null, null, null, null, null,
                    new FrigateReviewData(
                        new List<string> { detectionId },
                        null, null, null, null, null, null, null)))));

        await Task.Delay(100);

        // Assert
        FrigateClientMock.Verify(x => x.MarkReviewedAsync(reviewId), Times.Once);
        FrigateClientMock.Verify(x => x.MarkReviewedWithLoginAsync(reviewId), Times.Once);
        FrigateClientMock.Verify(x => x.MarkReviewedAsync(detectionId), Times.Once);
        FrigateClientMock.Verify(x => x.MarkReviewedWithLoginAsync(detectionId), Times.Once);
    }

    [Fact]
    public async Task Camera_FrigateReview_HouseNotAwake_DoesNotMarkReviewed()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate, HouseStateEnum.Away.ToString());

        var triggerSubject = new System.Reactive.Subjects.Subject<FrigateTriggerMessage?>();
        TriggerManagerMock
            .Setup(x => x.RegisterTrigger(It.IsAny<object>()))
            .Returns(triggerSubject.Select(m =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(m);
                return System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
            }));

        var app = Context.GetApp<Camera>();

        // Act
        triggerSubject.OnNext(new FrigateTriggerMessage(
            null, null, null, null, null, null, null, null,
            new FrigateReviewPayload(null, null,
                new FrigateReviewEntry("review-123", null, null, null, null, null,
                    new FrigateReviewData(
                        new List<string> { "detection-456" },
                        null, null, null, null, null, null, null)))));

        await Task.Delay(100);

        // Assert - no MarkReviewed calls should have been made
        FrigateClientMock.Verify(x => x.MarkReviewedAsync(It.IsAny<string>()), Times.Never);
        FrigateClientMock.Verify(x => x.MarkReviewedWithLoginAsync(It.IsAny<string>()), Times.Never);
    }
}
