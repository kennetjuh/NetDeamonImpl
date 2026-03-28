using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Apps;

public class HouseAppTest : TestBase
{
    private readonly Subject<DayNightEvent> dayNightSubject = new();
    private readonly Subject<object> lastNightSubject = new();
    private readonly Subject<object> lastDaySubject = new();
    private readonly Subject<HouseStateEvent> houseStateSubject = new();

    private void SetupObservables()
    {
        DayNightEventsMock.Setup(x => x.DayNightEvent).Returns(dayNightSubject);
        DayNightEventsMock.Setup(x => x.LastNightChangedEvent).Returns(lastNightSubject);
        DayNightEventsMock.Setup(x => x.LastDayChangedEvent).Returns(lastDaySubject);
        HouseStateEventsMock.Setup(x => x.Event).Returns(houseStateSubject);
        // RegisterServiceCallBack internally calls CallService("netdaemon", "register_service", ...)
        HaMock.Setup(x => x.CallService("netdaemon", "register_service", null, It.IsAny<object>()));
    }

    private void SetupAwayAction()
    {
        LightControlMock.Setup(x => x.SetLight(It.IsAny<LightEntity>(), 0)).Returns(false);
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.IsAny<ServiceTarget>(), null));
    }

    private void SetupSetHouseState()
    {
        HaMock.Setup(x => x.CallService("input_text", "set_value",
            It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.InputText.Housestate.EntityId),
            It.IsAny<InputTextSetValueParameters>()));
    }

    [Fact]
    public void HouseApp_Constructor_DefaultDay_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        // Default DayNight state = Day → SetLight(WandlampVoordeur, 0)
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        // Act
        var app = Context.GetApp<HouseApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_Constructor_Night_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Night" });
        // Night state → SetLight(WandlampVoordeur, 1)
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 1)).Returns(false);

        // Act
        var app = Context.GetApp<HouseApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_DayNightEvent_Day_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        var app = Context.GetApp<HouseApp>();

        // Also set up for the event trigger (brightness 1 for Night, then 0 for Day)
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        // Act
        dayNightSubject.OnNext(new DayNightEvent(DayNightEnum.Day));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_DayNightEvent_Night_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        // Constructor Day → SetLight(WandlampVoordeur, 0)
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        var app = Context.GetApp<HouseApp>();

        // Set up for Night event → SetLight(WandlampVoordeur, 1)
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 1)).Returns(false);

        // Act
        dayNightSubject.OnNext(new DayNightEvent(DayNightEnum.Night));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_HouseState_Away_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        var app = Context.GetApp<HouseApp>();

        SetupSetHouseState();
        SetupAwayAction();

        // Act
        houseStateSubject.OnNext(new HouseStateEvent(HouseStateEnum.Away,
            new ButtonEvent(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_HouseState_Awake_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        var app = Context.GetApp<HouseApp>();

        SetupSetHouseState();
        // Awake calls AwayAction (SetLight 0) + Awake-specific lights + TurnOn/TurnOff switches
        LightControlMock.Setup(x => x.SetLight(It.IsAny<LightEntity>(), It.IsAny<double?>())).Returns(false);
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.IsAny<ServiceTarget>(), null));
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.IsAny<ServiceTarget>(), null));

        // Act
        houseStateSubject.OnNext(new HouseStateEvent(HouseStateEnum.Awake,
            new ButtonEvent(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_HouseState_Sleeping_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        var app = Context.GetApp<HouseApp>();

        SetupSetHouseState();
        SetupAwayAction();
        // Sleeping-specific: SetLight(SlaapkamerKen, 1)
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerKen.EntityId), 1)).Returns(false);

        // Act
        houseStateSubject.OnNext(new HouseStateEvent(HouseStateEnum.Sleeping,
            new ButtonEvent(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void HouseApp_HouseState_Holiday_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupObservables();
        LightControlMock.Setup(x => x.SetLight(
            It.Is<LightEntity>(l => l.EntityId == Entities.Light.WandlampVoordeur.EntityId), 0)).Returns(false);

        var app = Context.GetApp<HouseApp>();

        SetupSetHouseState();
        SetupAwayAction();

        // Act
        houseStateSubject.OnNext(new HouseStateEvent(HouseStateEnum.Holiday,
            new ButtonEvent(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));

        // Assert
        VerifyAllMocks();
    }
}
