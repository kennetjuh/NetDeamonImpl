using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlHutTest : AreaControlTestBase<AreaControlHut>
{
    readonly LightEntity sfeer;
    public AreaControlHutTest()
    {
        light = entities.Light.LightHut;
        sfeer = entities.Light.BuitenzijHutsier;
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
        lightControlMock.Setup(x => x.ButtonDefault(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(false);
        haContextMock.Setup(x => x.CallService("light", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == sfeer.EntityId), It.IsAny<LightTurnOffParameters>()));

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
        lightControlMock.Setup(x => x.ButtonDefault(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(true);
        haContextMock.Setup(x => x.CallService("light", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == sfeer.EntityId), It.IsAny<LightTurnOnParameters>()));

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
        lightControlMock.Setup(x => x.ButtonDefault(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(false);
        haContextMock.Setup(x => x.CallService("light", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == sfeer.EntityId), It.IsAny<LightTurnOffParameters>()));

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }
}
