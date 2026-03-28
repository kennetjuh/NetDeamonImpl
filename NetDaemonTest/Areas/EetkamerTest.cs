using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class EetkamerTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();
    private readonly Subject<HouseStateEvent> houseStateSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
        HouseStateEventsMock.Setup(x => x.Event).Returns(houseStateSubject);
        LightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.Eetkamer.EntityId)));
    }

    [Fact]
    public void Eetkamer_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Eetkamer>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Eetkamer_ButtonPressed_Single_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.Eetkamer.EntityId), 20, 200)).Returns(true);

        var app = Context.GetApp<Eetkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Eetkamer, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Eetkamer_ButtonPressed_Double_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.Eetkamer.EntityId), 20, 200)).Returns(true);

        var app = Context.GetApp<Eetkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Eetkamer, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Eetkamer_MotionDetected_NoAction()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        var app = Context.GetApp<Eetkamer>();

        // Act - motion detected is disabled (commented out in source)
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionInkom, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Eetkamer_MotionCleared_NoAction()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        var app = Context.GetApp<Eetkamer>();

        // Act - motion cleared is disabled (commented out in source)
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionInkom, "off");

        // Assert
        VerifyAllMocks();
    }
}
