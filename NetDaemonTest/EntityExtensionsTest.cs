using NetDaemonImpl.Extensions;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest;

public class EntityExtensionsTest
{
    [Fact]
    public void LightEntity_IsOn_StateOn_ReturnsTrue()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.Light.LightKip, "on");

        // Act & Assert
        Assert.True(entities.Light.LightKip.IsOn());
    }

    [Fact]
    public void LightEntity_IsOn_StateOff_ReturnsFalse()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.Light.LightKip, "off");

        // Act & Assert
        Assert.False(entities.Light.LightKip.IsOn());
    }

    [Fact]
    public void LightEntity_IsOff_StateOff_ReturnsTrue()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.Light.LightKip, "off");

        // Act & Assert
        Assert.True(entities.Light.LightKip.IsOff());
    }

    [Fact]
    public void LightEntity_IsOff_StateOn_ReturnsFalse()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.Light.LightKip, "on");

        // Act & Assert
        Assert.False(entities.Light.LightKip.IsOff());
    }

    [Fact]
    public void SwitchEntity_IsOn_StateOn_ReturnsTrue()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.Switch.PlugGarage, "on");

        // Act & Assert
        Assert.True(entities.Switch.PlugGarage.IsOn());
    }

    [Fact]
    public void SwitchEntity_IsOff_StateOff_ReturnsTrue()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.Switch.PlugGarage, "off");

        // Act & Assert
        Assert.True(entities.Switch.PlugGarage.IsOff());
    }

    [Fact]
    public void InputBooleanEntity_IsOn_StateOn_ReturnsTrue()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputBoolean.Alarm, "on");

        // Act & Assert
        Assert.True(entities.InputBoolean.Alarm.IsOn());
    }

    [Fact]
    public void InputBooleanEntity_IsOff_StateOff_ReturnsTrue()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);
        haMock.TriggerStateChange(entities.InputBoolean.Alarm, "off");

        // Act & Assert
        Assert.True(entities.InputBoolean.Alarm.IsOff());
    }

    [Fact]
    public void InputBooleanEntity_IsOn_StateNull_ReturnsFalse()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var entities = new Entities(haMock.Object);

        // Act & Assert (state is null by default)
        Assert.False(entities.InputBoolean.Alarm.IsOn());
    }
}
