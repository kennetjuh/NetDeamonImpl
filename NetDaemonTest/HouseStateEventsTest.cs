using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NetDaemonImpl.IObservable;
using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest;

public class HouseStateEventsTest
{
    private readonly HaContextMock _haMock;
    private readonly Subject<ButtonEvent> _buttonSubject = new();
    private readonly HouseStateEvents _houseStateEvents;

    public HouseStateEventsTest()
    {
        _haMock = new HaContextMock(Moq.MockBehavior.Strict);
        _haMock.CallBase = true;

        var services = new ServiceCollection();
        services.AddTransient<IHaContext>(_ => _haMock.Object);
        var provider = services.BuildServiceProvider();

        var logger = new Mock<ILogger<HouseStateEvents>>();
        var luxMock = new Mock<ILuxBasedBrightness>(MockBehavior.Strict);
        var buttonEventsMock = new Mock<IButtonEvents>(MockBehavior.Strict);
        buttonEventsMock.Setup(x => x.Event).Returns(_buttonSubject);

        _houseStateEvents = new HouseStateEvents(provider, logger.Object, luxMock.Object, buttonEventsMock.Object);
    }

    [Fact]
    public void SingleClick_HouseInkom_PublishesAwake()
    {
        // Arrange
        HouseStateEvent? received = null;
        _houseStateEvents.Event.Subscribe(e => received = e);

        // Act
        _buttonSubject.OnNext(new ButtonEvent(Button.HouseInkom, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        Assert.NotNull(received);
        Assert.Equal(HouseStateEnum.Awake, received.State);
    }

    [Fact]
    public void DoubleClick_HouseInkom_PublishesAway()
    {
        // Arrange
        HouseStateEvent? received = null;
        _houseStateEvents.Event.Subscribe(e => received = e);

        // Act
        _buttonSubject.OnNext(new ButtonEvent(Button.HouseInkom, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        Assert.NotNull(received);
        Assert.Equal(HouseStateEnum.Away, received.State);
    }

    [Fact]
    public void LongPress_HouseInkom_PublishesSleeping()
    {
        // Arrange
        HouseStateEvent? received = null;
        _houseStateEvents.Event.Subscribe(e => received = e);

        // Act
        _buttonSubject.OnNext(new ButtonEvent(Button.HouseInkom, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        Assert.NotNull(received);
        Assert.Equal(HouseStateEnum.Sleeping, received.State);
    }

    [Fact]
    public void SingleClick_HouseVoordeur_PublishesAwake()
    {
        // Arrange
        HouseStateEvent? received = null;
        _houseStateEvents.Event.Subscribe(e => received = e);

        // Act
        _buttonSubject.OnNext(new ButtonEvent(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        Assert.NotNull(received);
        Assert.Equal(HouseStateEnum.Awake, received.State);
    }

    [Fact]
    public void OtherButton_IsIgnored()
    {
        // Arrange
        HouseStateEvent? received = null;
        _houseStateEvents.Event.Subscribe(e => received = e);

        // Act
        _buttonSubject.OnNext(new ButtonEvent(Button.Kip, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void ButtonEvent_IncludesOriginalButtonEvent()
    {
        // Arrange
        HouseStateEvent? received = null;
        _houseStateEvents.Event.Subscribe(e => received = e);
        var buttonEvent = new ButtonEvent(Button.HouseInkom, ButtonEventType.Single, DateTime.MinValue);

        // Act
        _buttonSubject.OnNext(buttonEvent);

        // Assert
        Assert.NotNull(received);
        Assert.Equal(buttonEvent, received.button);
    }
}
