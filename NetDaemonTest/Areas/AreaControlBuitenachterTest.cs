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
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));

        // Act
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery")]
    [InlineData("sensor.button_buitenachter_battery")]
    [InlineData("sensor.button_buitenachterlamp_battery")]
    public void ButtonSingleClick_LightIsOn_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));
        haContextMock.Setup(x => x.GetState(entities.Switch.SwitchInfinityMirror.EntityId)).Returns(new EntityState() { State = "on" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterSierverlichting.EntityId), 0)).Returns(false);
        haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchInfinityMirror.EntityId), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery")]
    [InlineData("sensor.button_buitenachter_battery")]
    public void ButtonSingleClick_LightIsOff_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));
        haContextMock.Setup(x => x.GetState(entities.Switch.SwitchInfinityMirror.EntityId)).Returns(new EntityState() { State = "off" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterSierverlichting.EntityId), 1)).Returns(true);
        haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchInfinityMirror.EntityId), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery")]
    [InlineData("sensor.button_buitenachter_battery")]
    public void ButtondoubleClick_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));
        haContextMock.Setup(x => x.CallService("switch", "toggle", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchFontein.EntityId), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery")]
    [InlineData("sensor.button_buitenachter_battery")]
    public void ButtonlongPress_LightIsOn_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));
        haContextMock.Setup(x => x.GetState(entities.Light.BuitenachterLamp.EntityId)).Returns(new EntityState() { State = "on" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterLamp.EntityId), 0)).Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData("sensor.button_buitenachterzithoek_battery")]
    [InlineData("sensor.button_buitenachter_battery")]
    public void ButtonlongPress_LightIsOff_VerifyMocks(string button)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));
        haContextMock.Setup(x => x.GetState(entities.Light.BuitenachterLamp.EntityId)).Returns(new EntityState() { State = "off" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Buitenachter3.EntityId), 1)).Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }


    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void HangStoelButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.BuitenachterHangstoel;

        SetupMocks();
        lightControlMock.Setup(x => x.AddAllwaysWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.BuitenachterHangstoel.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.Buttonhangstoel.EntityId, id);

        // Assert
        VerifyAllMocks();
    }
}