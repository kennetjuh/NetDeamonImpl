using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class SlaapkamerDamonTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
    }

    [Fact]
    public void SlaapkamerDamon_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<SlaapkamerDamon>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerDamon_SingleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerDamon.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerDamon>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerDamon, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerDamon_DoubleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerDamon.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerDamon>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerDamon, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerDamon_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerDamon.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerDamon>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerDamon, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
