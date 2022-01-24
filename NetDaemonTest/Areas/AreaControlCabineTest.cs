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
        light = entities.Light.LightCabine;
        CabineSfeer = entities.Switch.SwitchSierCabine;
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
    public void ButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(null);
        haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(CabineSfeer.EntityId)), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", id);

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
        haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(CabineSfeer.EntityId)), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);

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
        haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(CabineSfeer.EntityId)), null));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }
}
