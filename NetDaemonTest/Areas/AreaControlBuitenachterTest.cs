using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlBuitenachterTest : AreaControlTestBase<AreaControlBuitenAchter>
{
    public AreaControlBuitenachterTest()
    {
        light = entities.Light.BuitenachterLamp;
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();

        // Act
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void BuitenachterlampButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.BuitenachterLamp;

        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonBuitenachterlampBatteryLevel.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery_level")]
    [InlineData("sensor.button_buitenachter_battery_level")]
    public void ButtonSingleClick_LightIsOn_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        haContextMock.Setup(x => x.GetState(entities.Switch.SwitchInfinityMirror.EntityId)).Returns(new EntityState() { State = "on" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterSierverlichting.EntityId), 0)).Returns(null);
        haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.SwitchInfinityMirror.EntityId)), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery_level")]
    [InlineData("sensor.button_buitenachter_battery_level")]
    public void ButtonSingleClick_LightIsOff_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        haContextMock.Setup(x => x.GetState(entities.Switch.SwitchInfinityMirror.EntityId)).Returns(new EntityState() { State = "off" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterSierverlichting.EntityId), 1)).Returns(null);
        haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.SwitchInfinityMirror.EntityId)), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery_level")]
    [InlineData("sensor.button_buitenachter_battery_level")]
    public void ButtondoubleClick_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        haContextMock.Setup(x => x.CallService("switch", "toggle", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.BuitenachterFontein.EntityId)), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery_level")]
    [InlineData("sensor.button_buitenachter_battery_level")]
    public void ButtonlongPress_LightIsOn_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        haContextMock.Setup(x => x.GetState(entities.Light.BuitenachterLamp.EntityId)).Returns(new EntityState() { State = "on" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterLamp.EntityId), 0)).Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery_level")]
    [InlineData("sensor.button_buitenachter_battery_level")]
    public void ButtonlongPress_LightIsOff_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        haContextMock.Setup(x => x.GetState(entities.Light.BuitenachterLamp.EntityId)).Returns(new EntityState() { State = "off" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterLamp.EntityId), 255)).Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }
}