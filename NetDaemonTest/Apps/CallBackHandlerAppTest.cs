using Moq;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Text.Json;
using Xunit;

namespace NetDaemonTest.Apps;

public class CallBackHandlerAppTest : TestBase
{
    private const string callTwinkleEvent = @"
{
    ""event_type"": ""call_service"",
    ""data"": {
        ""domain"": ""netdaemon"",
        ""service"": ""twinkle"",
        ""service_data"": {}     
    },
    ""origin"": ""LOCAL"",
    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
    ""context"": {
        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
        ""parent_id"": null,
        ""user_id"": null
    }
}";

    private string GetCallChangeHouseStateEvent(string state)
    {
        return @"
{
    ""event_type"": ""call_service"",
    ""data"": {
        ""domain"": ""netdaemon"",
        ""service"": ""ChangeHouseState"",
        ""service_data"": {""state"": ""<state>""}     
    },
    ""origin"": ""LOCAL"",
    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
    ""context"": {
        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
        ""parent_id"": null,
        ""user_id"": null
    }
}".Replace("<state>", state);
    }


    [Fact]
    public void CallBackHandlerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object?>()));

        // Act
        var app = Context.GetApp<CallBackHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CallBackHandlerApp_Twinkle_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object?>()));
        TwinkleMock.Setup(x => x.Start());

        // Act
        var app = Context.GetApp<CallBackHandlerApp>();
        var twinkleEvent = JsonSerializer.Deserialize<Event>(callTwinkleEvent)!;
        HaMock.TriggerEvent(twinkleEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CallBackHandlerApp_ChangeHouseStateAwake_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object?>()));
        HouseStateMock.Setup(x => x.HouseStateAwake());

        // Act
        var app = Context.GetApp<CallBackHandlerApp>();
        var changeHouseStateEvent = JsonSerializer.Deserialize<Event>(GetCallChangeHouseStateEvent("Awake"))!;
        HaMock.TriggerEvent(changeHouseStateEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CallBackHandlerApp_ChangeHouseStateAway_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object?>()));
        HouseStateMock.Setup(x => x.HouseStateAway(HouseStateEnum.Away));

        // Act
        var app = Context.GetApp<CallBackHandlerApp>();
        var changeHouseStateEvent = JsonSerializer.Deserialize<Event>(GetCallChangeHouseStateEvent("Away"))!;
        HaMock.TriggerEvent(changeHouseStateEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CallBackHandlerApp_ChangeHouseStateSleeping_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object?>()));
        HouseStateMock.Setup(x => x.HouseStateSleeping());

        // Act
        var app = Context.GetApp<CallBackHandlerApp>();
        var changeHouseStateEvent = JsonSerializer.Deserialize<Event>(GetCallChangeHouseStateEvent("Sleeping"))!;
        HaMock.TriggerEvent(changeHouseStateEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CallBackHandlerApp_ChangeHouseStateHoliday_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object?>()));
        HouseStateMock.Setup(x => x.HouseStateHoliday());

        // Act
        var app = Context.GetApp<CallBackHandlerApp>();
        var changeHouseStateEvent = JsonSerializer.Deserialize<Event>(GetCallChangeHouseStateEvent("Holiday"))!;
        HaMock.TriggerEvent(changeHouseStateEvent);

        // Assert
        VerifyAllMocks();
    }
}