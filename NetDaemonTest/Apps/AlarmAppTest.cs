using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Apps;

public class AlarmAppTest : TestBase
{
    private void SetupAlarmPulse()
    {
        HaMock.Setup(x => x.CallService("select", "select_option",
            It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.Select.SireneVolume.EntityId),
            It.IsAny<SelectSelectOptionParameters>()));
        HaMock.Setup(x => x.CallService("siren", "turn_on",
            It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.Siren.Sirene.EntityId),
            It.IsAny<SirenTurnOnParameters>()));
        HaMock.Setup(x => x.CallService("siren", "turn_off",
            It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.Siren.Sirene.EntityId), null));
    }

    private void SetupAlarmStart()
    {
        NotifyMock.Setup(x => x.NotifyHouse("Alarm geactiveerd!"));
        NotifyMock.Setup(x => x.NotifyGsmAlarm());
        // SelectOption("high") and TurnOn are already covered by SetupAlarmPulse
    }

    [Fact]
    public void AlarmApp_Constructor_DefaultDisarmed_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        // Default state: Alarm off → Disarmed → AlarmStop + 2x AlarmPulse
        SetupAlarmPulse();

        // Act
        var app = Context.GetApp<AlarmApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_Constructor_Armed_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.Alarm.EntityId, new EntityState() { State = "on" });
        // Armed → 1x AlarmPulse
        SetupAlarmPulse();

        // Act
        var app = Context.GetApp<AlarmApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_AlarmToggleToArmed_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        // Constructor runs Disarmed path
        SetupAlarmPulse();

        var app = Context.GetApp<AlarmApp>();

        // Act - toggle alarm to on (Armed → AlarmPulse)
        HaMock.TriggerStateChange(Entities.InputBoolean.Alarm, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_AlarmToggleToDisarmed_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.Alarm.EntityId, new EntityState() { State = "on" });
        // Constructor runs Armed path
        SetupAlarmPulse();

        var app = Context.GetApp<AlarmApp>();

        // Act - toggle alarm to off (Disarmed → AlarmStop + 2x AlarmPulse)
        HaMock.TriggerStateChange(Entities.InputBoolean.Alarm, "off");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_KelderOpen_Armed_AlarmStart()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.Alarm.EntityId, new EntityState() { State = "on" });
        SetupAlarmPulse();
        SetupAlarmStart();

        var app = Context.GetApp<AlarmApp>();

        // Act - kelder opens while armed
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseKelderOpening, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_KelderOpen_Disarmed_NoAlarmStart()
    {
        // Arrange
        ResetAllMocks();
        // Default: Disarmed
        SetupAlarmPulse();

        var app = Context.GetApp<AlarmApp>();

        // Act - kelder opens while disarmed → AlarmStart returns early
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseKelderOpening, "on");

        // Assert - no NotifyHouse/NotifyGsmAlarm expected
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_KelderClose_AlarmStop()
    {
        // Arrange
        ResetAllMocks();
        SetupAlarmPulse();

        var app = Context.GetApp<AlarmApp>();

        // Act - kelder closes → AlarmStop (TurnOff already set up)
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseKelderOpening, "off");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_InkomOpen_Armed_AlarmStart()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.Alarm.EntityId, new EntityState() { State = "on" });
        SetupAlarmPulse();
        SetupAlarmStart();

        var app = Context.GetApp<AlarmApp>();

        // Act - inkom opens while armed
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseInkomOpening, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void AlarmApp_InkomOpen_Disarmed_NoAlarmStart()
    {
        // Arrange
        ResetAllMocks();
        SetupAlarmPulse();

        var app = Context.GetApp<AlarmApp>();

        // Act - inkom opens while disarmed → no alarm
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseInkomOpening, "on");

        // Assert
        VerifyAllMocks();
    }
}
