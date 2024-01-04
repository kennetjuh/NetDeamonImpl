using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;
using System.Linq;

namespace NetDaemonTest.Apps;

public class TimerAppTest : TestBase
{  
    [Fact]
    public void TimerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<TimerApp>();

        // Assert
        VerifyAllMocks();
    }

    //[Fact]
    //public void TimerApp_ZwembadAan_VerifyCalls()
    //{
    //    // Arrange
    //    ResetAllMocks();
    //    Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(7).AddMinutes(59).ToUniversalTime().Ticks);
    //    HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.SwitchZwembad.EntityId), null));


    //    // Act
    //    var app = Context.GetApp<TimerApp>();
    //    Scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

    //    // Assert
    //    VerifyAllMocks();
    //}

    //[Fact]
    //public void TimerApp_ZwembadOff_VerifyCalls()
    //{
    //    // Arrange
    //    ResetAllMocks();

    //    Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(19).AddMinutes(59).ToUniversalTime().Ticks);
    //    HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.SwitchZwembad.EntityId), null));


    //    // Act
    //    var app = Context.GetApp<TimerApp>();
    //    Scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

    //    // Assert
    //    VerifyAllMocks();
    //}

    [Fact]
    public void TimerApp_VacuumHoliday_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.Housestate.EntityId, new EntityState() { State = "Holiday" });
        HaMock.Setup(x => x.CallService("vacuum", "set_fan_speed", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Vacuum.DreameP20294b09RobotCleaner.EntityId), It.IsAny<object>()));
        HaMock.Setup(x => x.CallService("vacuum", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Vacuum.DreameP20294b09RobotCleaner.EntityId), null));
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(20).AddMinutes(59).ToUniversalTime().Ticks);

        // Act
        var app = Context.GetApp<TimerApp>();
        Scheduler.AdvanceBy(TimeSpan.FromHours(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_VacuumNoHoliday_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.Housestate.EntityId, new EntityState() { State = "Awake" });
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(20).AddMinutes(59).ToUniversalTime().Ticks);

        // Act
        var app = Context.GetApp<TimerApp>();
        Scheduler.AdvanceBy(TimeSpan.FromHours(1).Ticks);

        // Assert
        VerifyAllMocks();
    }
}