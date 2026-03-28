using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class WasHalTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();
    private readonly Subject<HouseStateEvent> houseStateSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
        HouseStateEventsMock.Setup(x => x.Event).Returns(houseStateSubject);
        LightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.Washal.EntityId)));
    }

    [Fact]
    public void WasHal_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<WasHal>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WasHal_ButtonPressed_Single_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        DelayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMinutes(5));
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.Washal.EntityId), 10, 255)).Returns(true);

        var app = Context.GetApp<WasHal>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Washal, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WasHal_MotionDetected_AwakeNightIdle_LightOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate.EntityId, new EntityState() { State = "Awake" });
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Night" });
        LightControlMock.Setup(x => x.SetLightRgb(It.Is<LightEntity>(l => l.EntityId == Entities.Light.Washal.EntityId), 0.2 * 255, It.IsAny<System.Collections.Generic.List<int>>())).Returns(true);

        var app = Context.GetApp<WasHal>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionInkom, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WasHal_MotionCleared_MotionMode_StartsAfterTask()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate.EntityId, new EntityState() { State = "Awake" });
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Night" });
        LightControlMock.Setup(x => x.SetLightRgb(It.Is<LightEntity>(l => l.EntityId == Entities.Light.Washal.EntityId), 0.2 * 255, It.IsAny<System.Collections.Generic.List<int>>())).Returns(true);
        DelayProviderMock.Setup(x => x.MotionClear).Returns(TimeSpan.FromMinutes(5));

        var app = Context.GetApp<WasHal>();
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionInkom, "on");

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionInkom, "off");

        // Assert
        VerifyAllMocks();
    }
}
