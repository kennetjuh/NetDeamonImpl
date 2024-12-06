using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlCabineTest : AreaControlTestBase<AreaControlCabine>
{
    readonly SwitchEntity CabineSfeer;
    public AreaControlCabineTest()
    {
        light = entities.Light.Cabine;
        CabineSfeer = entities.Switch.SwitchSierCabine;
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();

        // Act
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, luxBasedBrightnessMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void ButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Cabineplafond.EntityId), 0)).Returns(true);
        haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == CabineSfeer.EntityId), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, luxBasedBrightnessMock.Object);

        // Act
        Sut.ButtonPressed("sensor.button_cabine_battery", id);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonPressed_LightOn_SfeerAlsoOn()
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);
        luxBasedBrightnessMock.Setup(x => x.GetBrightness(It.IsAny<double>(), It.IsAny<double>())).Returns(50);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Cabineplafond.EntityId), 50)).Returns(true);
        haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == CabineSfeer.EntityId), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, luxBasedBrightnessMock.Object);

        // Act
        Sut.ButtonPressed("sensor.button_cabine_battery", DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonPressed_LightOff_SfeerAlsOff()
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Cabineplafond.EntityId), 0)).Returns(true);
        haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == CabineSfeer.EntityId), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, luxBasedBrightnessMock.Object);

        // Act
        Sut.ButtonPressed("sensor.button_cabine_battery", DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }
}
