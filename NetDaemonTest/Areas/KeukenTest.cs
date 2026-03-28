using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class KeukenTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();
    private readonly Subject<HouseStateEvent> houseStateSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
        HouseStateEventsMock.Setup(x => x.Event).Returns(houseStateSubject);
    }

    [Fact]
    public void Keuken_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Keuken>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_SingleClick_BothOff_TurnsBothOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.Light.LightKeukenEiland.EntityId, new() { State = "off" });
        HaMock.TriggerStateChange(Entities.Light.LightKeuken.EntityId, new() { State = "off" });
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LuxBasedBrightnessMock.Setup(x => x.GetBrightness(50, 255)).Returns(100);
        LightControlMock.Setup(x => x.LuxBasedBrightness).Returns(LuxBasedBrightnessMock.Object);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId), 100)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId), 100)).Returns(true);

        var app = Context.GetApp<Keuken>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Keuken1, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_SingleClick_EilandOnKeukenOff_TurnsAllOff()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.Light.LightKeukenEiland.EntityId, new() { State = "on" });
        HaMock.TriggerStateChange(Entities.Light.LightKeuken.EntityId, new() { State = "off" });
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LuxBasedBrightnessMock.Setup(x => x.GetBrightness(50, 255)).Returns(100);
        LightControlMock.Setup(x => x.LuxBasedBrightness).Returns(LuxBasedBrightnessMock.Object);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Keuken>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Keuken1, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_SingleClick_BothOn_EilandOnKeukenOff()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.Light.LightKeukenEiland.EntityId, new() { State = "on" });
        HaMock.TriggerStateChange(Entities.Light.LightKeuken.EntityId, new() { State = "on" });
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LuxBasedBrightnessMock.Setup(x => x.GetBrightness(50, 255)).Returns(100);
        LightControlMock.Setup(x => x.LuxBasedBrightness).Returns(LuxBasedBrightnessMock.Object);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId), 100)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Keuken>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Keuken1, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_DoubleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LuxBasedBrightnessMock.Setup(x => x.GetBrightness(50, 255)).Returns(100);
        LightControlMock.Setup(x => x.LuxBasedBrightness).Returns(LuxBasedBrightnessMock.Object);
        LightControlMock.Setup(x => x.ButtonDefault(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId))).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefault(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId))).Returns(true);

        var app = Context.GetApp<Keuken>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Keuken2, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LuxBasedBrightnessMock.Setup(x => x.GetBrightness(50, 255)).Returns(100);
        LightControlMock.Setup(x => x.LuxBasedBrightness).Returns(LuxBasedBrightnessMock.Object);
        LightControlMock.Setup(x => x.ButtonDefault(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId))).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefault(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId))).Returns(true);

        var app = Context.GetApp<Keuken>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Keuken3, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_MotionDetected_AwakeNightIdle_LightsOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate.EntityId, new EntityState() { State = "Awake" });
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Night" });
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId), 20)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId), 20)).Returns(true);

        var app = Context.GetApp<Keuken>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionKeuken, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Keuken_MotionCleared_MotionMode_StartsAfterTask()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate.EntityId, new EntityState() { State = "Awake" });
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Night" });
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeuken.EntityId), 20)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightKeukenEiland.EntityId), 20)).Returns(true);
        DelayProviderMock.Setup(x => x.MotionClear).Returns(TimeSpan.FromMinutes(5));

        var app = Context.GetApp<Keuken>();
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionKeuken, "on");

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionKeuken, "off");

        // Assert
        VerifyAllMocks();
    }
}
