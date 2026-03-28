using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class SlaapkamerKenTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
    }

    [Fact]
    public void SlaapkamerKen_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<SlaapkamerKen>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerKen_SingleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerKen.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerKen>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerKen, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerKen_DoubleClick_BedButton_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerKen.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerKen>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerKenBed, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerKen_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerKen.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerKen>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerKen, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
