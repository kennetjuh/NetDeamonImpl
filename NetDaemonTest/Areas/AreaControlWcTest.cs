using Moq;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlWcTest : AreaControlTestBase<AreaControlWc>
{
    readonly LightEntity singleLight;

    public AreaControlWcTest()
    {
        light = entities.Light.WcWclamp;
        singleLight = entities.Light.Wc1;
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
        lightControlMock.Setup(x => x.LuxBasedBrightness).Returns(luxBasedBrightnessMock.Object);
        luxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(100);
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

    [Fact]
    public void ButtonPressed_SingleLight_VerifyMocks()
    {
        // Arrange
        SetupMocks();

        haContextMock.Setup(x => x.GetState(light.EntityId)).Returns(new TestEntityStateLightAttributes() { State = "off" });
        lightControlMock.Setup(x => x.LuxBasedBrightness).Returns(luxBasedBrightnessMock.Object);
        luxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(1);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == singleLight.EntityId), 1)).Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }
}
