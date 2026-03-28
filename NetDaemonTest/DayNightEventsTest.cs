using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.IObservable;
using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest;

public class DayNightEventsTest
{
    private readonly HaContextMock _haMock;
    private readonly Entities _entities;
    private readonly Mock<ILuxBasedBrightness> _luxMock;
    private readonly Subject<HouseStateEvent> _houseStateSubject = new();
    private readonly DayNightEvents _dayNightEvents;

    private static readonly HouseStateEvent DummyHouseStateEvent =
        new(HouseStateEnum.Awake, new ButtonEvent(Button.HouseInkom, ButtonEventType.Single, DateTime.MinValue));

    public DayNightEventsTest()
    {
        _haMock = new HaContextMock(Moq.MockBehavior.Strict);
        _haMock.CallBase = true;
        _haMock.Setup(x => x.CallService(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ServiceTarget?>(), It.IsAny<object?>())).CallBase();
        _entities = new Entities(_haMock.Object);

        var services = new ServiceCollection();
        services.AddTransient<IHaContext>(_ => _haMock.Object);
        var provider = services.BuildServiceProvider();

        var logger = new Mock<ILogger<DayNightEvents>>();
        _luxMock = new Mock<ILuxBasedBrightness>(MockBehavior.Strict);
        _luxMock.Setup(x => x.GetLux()).Returns(50);

        var houseStateMock = new Mock<IHouseStateEvents>(MockBehavior.Strict);
        houseStateMock.Setup(x => x.Event).Returns(_houseStateSubject);

        _dayNightEvents = new DayNightEvents(provider, logger.Object, _luxMock.Object, houseStateMock.Object);
    }

    private void SetSunState(double elevation, bool rising)
    {
        var attrs = new { elevation, rising }.AsJsonElement();
        var state = new EntityState() { State = "below_horizon", AttributesJson = attrs };
        _haMock.Object._entityStates["sun.sun"] = state;
    }

    [Fact]
    public void Constructor_NoTransitionOnStartup()
    {
        // Assert - no exception and no transition during construction
        DayNightEvent? received = null;
        _dayNightEvents.DayNightEvent.Subscribe(e => received = e);
        Assert.Null(received);
    }

    [Fact]
    public void DayToNight_ConditionsMet_PublishesNight()
    {
        // Arrange
        DayNightEvent? received = null;
        _dayNightEvents.DayNightEvent.Subscribe(e => received = e);

        SetSunState(elevation: -1.0, rising: false);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Day");
        _luxMock.Setup(x => x.GetLux()).Returns(10);

        // Act - trigger CheckDayNight via house state event
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.NotNull(received);
        Assert.Equal(DayNightEnum.Night, received.State);
    }

    [Fact]
    public void DayToNight_LastNightChangedEvent_Fires()
    {
        // Arrange
        object? received = null;
        _dayNightEvents.LastNightChangedEvent.Subscribe(e => received = e);

        SetSunState(elevation: -1.0, rising: false);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Day");
        _luxMock.Setup(x => x.GetLux()).Returns(10);

        // Act
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.NotNull(received);
    }

    [Fact]
    public void NightToDay_ConditionsMet_PublishesDay()
    {
        // Arrange
        DayNightEvent? received = null;
        _dayNightEvents.DayNightEvent.Subscribe(e => received = e);

        SetSunState(elevation: -3.0, rising: true);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Night");
        _luxMock.Setup(x => x.GetLux()).Returns(30);

        // Act - trigger CheckDayNight via house state event
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.NotNull(received);
        Assert.Equal(DayNightEnum.Day, received.State);
    }

    [Fact]
    public void NightToDay_LastDayChangedEvent_Fires()
    {
        // Arrange
        object? received = null;
        _dayNightEvents.LastDayChangedEvent.Subscribe(e => received = e);

        SetSunState(elevation: -3.0, rising: true);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Night");
        _luxMock.Setup(x => x.GetLux()).Returns(30);

        // Act
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.NotNull(received);
    }

    [Fact]
    public void DayToNight_LuxTooHigh_NoTransition()
    {
        // Arrange
        DayNightEvent? received = null;
        _dayNightEvents.DayNightEvent.Subscribe(e => received = e);

        SetSunState(elevation: -1.0, rising: false);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Day");
        _luxMock.Setup(x => x.GetLux()).Returns(50);

        // Act - lux 50 > 30, should not transition
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void NightToDay_LuxTooLow_NoTransition()
    {
        // Arrange
        DayNightEvent? received = null;
        _dayNightEvents.DayNightEvent.Subscribe(e => received = e);

        SetSunState(elevation: -3.0, rising: true);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Night");
        _luxMock.Setup(x => x.GetLux()).Returns(10);

        // Act - lux 10 <= 20, should not transition
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void DayToNight_ElevationPositive_NoTransition()
    {
        // Arrange
        DayNightEvent? received = null;
        _dayNightEvents.DayNightEvent.Subscribe(e => received = e);

        SetSunState(elevation: 5.0, rising: false);
        _haMock.TriggerStateChange(_entities.InputText.Daynight, "Day");
        _luxMock.Setup(x => x.GetLux()).Returns(10);

        // Act - elevation 5 >= 0, should not transition
        _houseStateSubject.OnNext(DummyHouseStateEvent);

        // Assert
        Assert.Null(received);
    }
}
