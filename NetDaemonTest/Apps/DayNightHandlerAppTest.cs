using Moq;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Apps;

public class DayNightHandlerAppTest : TestBase
{
    [Fact]
    public void DayNightHandlerApp_Constructor_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");
        DayNightMock.Setup(x => x.CheckDayNight());

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DayNightHandlerApp_Sunchanges_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");
        DayNightMock.Setup(x => x.CheckDayNight());

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();
        HaMock.TriggerStateChange(Entities.Sun.Sun, "", new SunAttributes
        {
            Elevation = 5,
            Rising = true
        });

        // Assert
        VerifyAllMocks();
        DayNightMock.Verify(x => x.CheckDayNight(),Times.Exactly(2));
    }

    [Fact]
    public void DayNightHandlerApp_LightChanges_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");
        DayNightMock.Setup(x => x.CheckDayNight());

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();
        HaMock.TriggerStateChange(Entities.Sensor.LightSensor, "", "10");

        // Assert
        VerifyAllMocks();
        DayNightMock.Verify(x => x.CheckDayNight(), Times.Exactly(2));
    }    
}