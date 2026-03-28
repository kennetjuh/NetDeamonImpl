using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class GarageTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
    }

    [Fact]
    public void Garage_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Garage>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Garage_SingleClick_SfeerOn_TurnsAllOff()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.Light.SfeerGarage.EntityId, new() { State = "on" });
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.SfeerGarage.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageLinks.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts1.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts2.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Garage>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Garage, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Garage_SingleClick_SfeerOff_TurnsSfeerOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.SfeerGarage.EntityId), 150)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageLinks.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts1.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts2.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Garage>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Garage, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Garage_DoubleClick_LinksOnly()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.SfeerGarage.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageLinks.EntityId), 1)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts1.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts2.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Garage>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Garage, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Garage_LongPress_AllWorkLights()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.SfeerGarage.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageLinks.EntityId), 1)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts1.EntityId), 1)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightGarageRechts2.EntityId), 1)).Returns(true);

        var app = Context.GetApp<Garage>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Garage, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
