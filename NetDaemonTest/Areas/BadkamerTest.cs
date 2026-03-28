using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class BadkamerTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
    }

    [Fact]
    public void Badkamer_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Badkamer>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Badkamer_SingleClick_LightsOn_TurnsOff()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.Light.LightBadkamerNis.EntityId, new() { State = "on" });
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightBadkamerNis.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightBadkamer.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Badkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Badkamer, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Badkamer_SingleClick_LightsOff_TurnsOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightBadkamerNis.EntityId), 30)).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightBadkamer.EntityId), 50, 255)).Returns(true);

        var app = Context.GetApp<Badkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Badkamer, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Badkamer_DoubleClick_TurnsOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightBadkamerNis.EntityId), 30)).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightBadkamer.EntityId), 50, 255)).Returns(true);

        var app = Context.GetApp<Badkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Badkamer, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
