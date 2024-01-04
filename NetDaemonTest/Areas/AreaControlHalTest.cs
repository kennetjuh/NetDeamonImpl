using Moq;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlHalTest : AreaControlTestBase<AreaControlHal>
{
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
        light = entities.Light.Hal;
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", id);

        // Assert
        VerifyAllMocks();
    }

    //Todo Add motion testcases

}
