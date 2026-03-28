using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class RommelkamerTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
    }

    [Fact]
    public void Rommelkamer_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Rommelkamer>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Rommelkamer_SingleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.Rommelkamer.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<Rommelkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Rommelkamer, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Rommelkamer_DoubleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.Rommelkamer.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<Rommelkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Rommelkamer, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Rommelkamer_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.Rommelkamer.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<Rommelkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Rommelkamer, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
