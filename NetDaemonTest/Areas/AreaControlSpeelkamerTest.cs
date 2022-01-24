using Moq;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlSpeelkamerTest : AreaControlTestBase<AreaControlSpeelkamer>
{
    public AreaControlSpeelkamerTest()
    {
        light = entities.Light.SpeelkamerLamp;
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == light.EntityId)));

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
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == light.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", id);

        // Assert
        VerifyAllMocks();
    }

}
