using NetDaemonImpl;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest;

public class HelperTest
{
    [Fact]
    public void StringToDateTime_Null_ReturnsMinValue()
    {
        // Act
        var result = Helper.StringToDateTime(null);

        // Assert
        Assert.Equal(DateTime.MinValue, result);
    }

    [Fact]
    public void StringToDateTime_ValidTime_ReturnsParsedDateTime()
    {
        // Act
        var result = Helper.StringToDateTime("14:30:00");

        // Assert
        Assert.Equal(14, result.Hour);
        Assert.Equal(30, result.Minute);
        Assert.Equal(0, result.Second);
    }

    [Fact]
    public void GetDayNightState_Day_ReturnsDay()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Daynight, DayNightEnum.Day.ToString());

        // Act
        var result = Helper.GetDayNightState(entities);

        // Assert
        Assert.Equal(DayNightEnum.Day, result);
    }

    [Fact]
    public void GetDayNightState_Night_ReturnsNight()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Daynight, DayNightEnum.Night.ToString());

        // Act
        var result = Helper.GetDayNightState(entities);

        // Assert
        Assert.Equal(DayNightEnum.Night, result);
    }

    [Fact]
    public void GetDayNightState_InvalidState_ReturnsDay()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Daynight, "invalid");

        // Act
        var result = Helper.GetDayNightState(entities);

        // Assert
        Assert.Equal(DayNightEnum.Day, result);
    }

    [Fact]
    public void GetHouseState_Awake_ReturnsAwake()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Housestate, HouseStateEnum.Awake.ToString());

        // Act
        var result = Helper.GetHouseState(entities);

        // Assert
        Assert.Equal(HouseStateEnum.Awake, result);
    }

    [Fact]
    public void GetHouseState_Away_ReturnsAway()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Housestate, HouseStateEnum.Away.ToString());

        // Act
        var result = Helper.GetHouseState(entities);

        // Assert
        Assert.Equal(HouseStateEnum.Away, result);
    }

    [Fact]
    public void GetHouseState_Sleeping_ReturnsSleeping()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Housestate, HouseStateEnum.Sleeping.ToString());

        // Act
        var result = Helper.GetHouseState(entities);

        // Assert
        Assert.Equal(HouseStateEnum.Sleeping, result);
    }

    [Fact]
    public void GetHouseState_InvalidState_ReturnsAwake()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputText.Housestate, "invalid");

        // Act
        var result = Helper.GetHouseState(entities);

        // Assert
        Assert.Equal(HouseStateEnum.Awake, result);
    }

    [Fact]
    public void GetAlarmState_On_ReturnsArmed()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputBoolean.Alarm, "on");

        // Act
        var result = Helper.GetAlarmState(entities);

        // Assert
        Assert.Equal(AlarmEnum.Armed, result);
    }

    [Fact]
    public void GetAlarmState_Off_ReturnsDisarmed()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputBoolean.Alarm, "off");

        // Act
        var result = Helper.GetAlarmState(entities);

        // Assert
        Assert.Equal(AlarmEnum.Disarmed, result);
    }
}
