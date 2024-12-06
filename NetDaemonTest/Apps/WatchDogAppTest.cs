using Moq;
using NetDaemonImpl;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Apps;

public class WatchDogAppTest : TestBase
{
    [Fact]
    public void WatchDogApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<WatchDogApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WatchDogApp_Startup_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.WatchdogBuiten, "on");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");
        DayNightMock.Setup(x => x.WatchdogBuiten());

        // Act
        var app = Context.GetApp<WatchDogApp>();

        // Assert
        VerifyAllMocks();
        DayNightMock.Verify(x => x.WatchdogBuiten(), Times.Exactly(1));
    }    

    [Fact]
    public void WatchDogApp_TurnOn_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.WatchdogBuiten, "off");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");
        DayNightMock.Setup(x => x.WatchdogBuiten());

        // Act
        var app = Context.GetApp<WatchDogApp>();
        HaMock.TriggerStateChange(Entities.InputBoolean.WatchdogBuiten, "on");

        // Assert
        VerifyAllMocks();
        DayNightMock.Verify(x => x.WatchdogBuiten(), Times.Exactly(1));
    }

    [Fact]
    public void WatchDogApp_TurnOnDaySwapNight_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.WatchdogBuiten, "off");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");
        DayNightMock.Setup(x => x.WatchdogBuiten());

        // Act
        var app = Context.GetApp<WatchDogApp>();
        HaMock.TriggerStateChange(Entities.InputBoolean.WatchdogBuiten, "on");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Night");
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(5).Ticks);

        // Assert
        VerifyAllMocks();
        DayNightMock.Verify(x => x.WatchdogBuiten(), Times.Exactly(6));
    }
}