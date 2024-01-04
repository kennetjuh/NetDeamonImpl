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
    public void WatchDogApp_StartupOnDay_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();        
        HaMock.TriggerStateChange(Entities.Switch.WatchdogBuiten, "on");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Day");
        SetupDayMocks();

        // Act
        var app = Context.GetApp<WatchDogApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WatchDogApp_StartupOnNight_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Switch.WatchdogBuiten, "on");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Night");
        SetupNightMocks();

        // Act
        var app = Context.GetApp<WatchDogApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WatchDogApp_TurnOn_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Switch.WatchdogBuiten, "off");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Day");
        SetupDayMocks();

        // Act
        var app = Context.GetApp<WatchDogApp>();
        HaMock.TriggerStateChange(Entities.Switch.WatchdogBuiten, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void WatchDogApp_TurnOnDaySwapNight_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Switch.WatchdogBuiten, "off");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Day");
        SetupDayMocks();
        SetupNightMocks();

        // Act
        var app = Context.GetApp<WatchDogApp>();
        HaMock.TriggerStateChange(Entities.Switch.WatchdogBuiten, "on");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Night");
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(5).Ticks);

        // Assert
        VerifyAllMocks();
    }

    private void SetupDayMocks()
    {
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.GrondlampZij.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.BuitenachterFonteinlamp.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.WandlampHut.EntityId), 0)).Returns(false);
    }

    private void SetupNightMocks()
    {
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.GrondlampZij.EntityId), Constants.brightnessBuitenZij)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.BuitenachterFonteinlamp.EntityId), Constants.brightnessFontein)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.WandlampHut.EntityId), Constants.brightnessHutWand)).Returns(true);
    }
}