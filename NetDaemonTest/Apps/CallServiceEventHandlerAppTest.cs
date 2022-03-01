using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using System.Text.Json;
using Xunit;

namespace NetDaemonTest.Apps;

public class CallServiceEventHandlerAppTest : TestBase
{
    private const string keukenLightEvent = @"
{
    ""event_type"": ""call_service"",
    ""data"": {
        ""domain"": ""light"",
        ""service"": ""turn_on"",
        ""service_data"": {
                    ""color_temp"": 370,
                    ""entity_id"": [""light.keuken_keukenlamp""] }   
    },
    ""origin"": ""LOCAL"",
    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
    ""context"": {
        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
        ""parent_id"": null,
        ""user_id"": null
    }
}";


    [Fact]
    public void CallBackHandlerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<CallServiceEventHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CallBackHandlerApp_TriggerEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        TwinkleMock.Setup(x => x.Stop());

        // Act
        var app = Context.GetApp<CallServiceEventHandlerApp>();
        var keukenEvent = JsonSerializer.Deserialize<Event>(keukenLightEvent)!;
        HaMock.TriggerEvent(keukenEvent);


        // Assert
        VerifyAllMocks();
    }
}