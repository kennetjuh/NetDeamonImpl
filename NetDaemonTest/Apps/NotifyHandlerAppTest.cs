using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Text.Json;
using Xunit;

namespace NetDaemonTest.Apps;

public class NotifyHandlerAppTest : TestBase
{
    private string GetNotificationEvent(string action)
    {
        return @"{
    ""event_type"": ""mobile_app_notification_action"",
    ""data"" : {
        ""action"": ""<action>""
    },
    ""origin"": ""LOCAL"",
    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
    ""context"": {
        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
        ""parent_id"": null,
        ""user_id"": null
    }
}".Replace("<action>", action);
    }

    [Fact]
    public void NotifyHandlerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<NotifyHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyHandlerApp_HandleEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        NotifyMock.Setup(x => x.HandleNotificationEvent(NotifyActionEnum.OpenCloseVoordeurOmroepen));

        // Act
        var app = Context.GetApp<NotifyHandlerApp>();
        var notificationEvent = JsonSerializer.Deserialize<Event>(GetNotificationEvent(NotifyActionEnum.OpenCloseVoordeurOmroepen.ToString()))!;
        HaMock.TriggerEvent(notificationEvent);

        // Assert
        VerifyAllMocks();
    }
}