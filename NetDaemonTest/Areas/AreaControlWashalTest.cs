using Moq;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Threading.Tasks;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlWashalTest : AreaControlTestBase<AreaControlWashal>
{
    public AreaControlWashalTest()
    {
        light = entities.Light.Washal;
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

    [Fact]
    public void ButtonPressed_SingleClickLightGoesOn_VerifyMocks()
    {
        // Arrange
        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);

        // Assert
        Assert.Equal(AreaModeEnum.Manual, Sut.GetPrivate<AreaModeEnum>("mode"));
        VerifyAllMocks();
    }

    [Fact]
    public async Task ButtonPressed_SingleClickLightGoesOff_VerifyMocks()
    {
        // Arrange
        SetupMocks();
        delayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMilliseconds(50));
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(false);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);

        // Assert
        Assert.Equal(AreaModeEnum.Manual, Sut.GetPrivate<AreaModeEnum>("mode"));
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
        VerifyAllMocks();
    }

    [Fact]
    public void MotionDetected_ModeManual_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        Sut.SetPrivate("mode", AreaModeEnum.Manual);

        // Act
        Sut.MotionDetected("");

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Manual, Sut.GetPrivate<AreaModeEnum>("mode"));
    }

    [Fact]
    public void MotionDetected_ModeIdle_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        lightControlMock.Setup(x => x.LuxBasedBrightness).Returns(luxBasedBrightnessMock.Object);
        luxBasedBrightnessMock.Setup(x => x.GetBrightness(10, 255)).Returns(100);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Washal.EntityId), 100)).Returns(true);

        // Act
        Sut.MotionDetected("");

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Motion, Sut.GetPrivate<AreaModeEnum>("mode"));
    }

    [Fact]
    public void MotionCleared_ModeIdle_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        Sut.SetPrivate("mode", AreaModeEnum.Idle);

        // Act
        Sut.MotionCleared("");

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
    }

    [Fact]
    public async Task MotionCleared_ModeMotion_VerifyMocksAsync()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        Sut.SetPrivate("mode", AreaModeEnum.Motion);
        delayProviderMock.Setup(x => x.MotionClear).Returns(TimeSpan.FromMilliseconds(1));
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Washal.EntityId), 0)).Returns(true);

        // Act
        Sut.MotionCleared("");
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
    }
}