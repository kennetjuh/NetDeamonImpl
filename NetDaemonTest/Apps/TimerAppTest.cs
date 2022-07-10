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

    [Fact]
    public void TimerApp_ZwembadAan_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(7).AddMinutes(59).ToUniversalTime().Ticks);
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.SwitchZwembad.EntityId), null));


        // Act
        var app = Context.GetApp<TimerApp>();
        Scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_ZwembadOff_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(21).AddMinutes(59).ToUniversalTime().Ticks);
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.SwitchZwembad.EntityId), null));


        // Act
        var app = Context.GetApp<TimerApp>();
        Scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

        // Assert
        VerifyAllMocks();
    }
}