using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Threading.Tasks;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlTraphalTest : AreaControlTestBase<AreaControlTraphal>
{
    internal LightEntity lightWand;

    public AreaControlTraphalTest()
    {
        lightWand = entities.Light.TraphalWand;
        light = entities.Light.Traphal;
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
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == lightWand.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);
        haContextMock.Setup(x => x.GetState(lightWand.EntityId)).Returns(new EntityState() { State = "off" });

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act & Assert
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);
        Assert.Equal(AreaModeEnum.Manual, Sut.GetPrivate<AreaModeEnum>("mode"));
        VerifyAllMocks();
    }

    [Fact]
    public async Task ButtonPressed_SingleClickLightGoesOff_VerifyMocksAsync()
    {
        // Arrange
        SetupMocks();
        delayProviderMock.Setup(x => x.ManualOffTimeout).Returns(TimeSpan.FromMilliseconds(10));
        lightControlMock.Setup(x => x.SetLight(light, 0)).Returns(false);
        lightControlMock.Setup(x => x.SetLight(lightWand, 0)).Returns(false);

        haContextMock.Setup(x => x.GetState(lightWand.EntityId)).Returns(new EntityState() { State = "on" });


        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act & Assert
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
        Sut.ButtonPressed("", DeconzEventIdEnum.Single);
        Assert.Equal(AreaModeEnum.Manual, Sut.GetPrivate<AreaModeEnum>("mode"));
        await Task.Delay(TimeSpan.FromMilliseconds(300)); // Sleep is to make sure all of the Tasks are completed
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

    [Theory]
    [InlineData("binary_sensor.motion_traphal_1")]
    [InlineData("binary_sensor.motion_traphal_2")]
    public async Task MotionDetected_ModeIdleSensor1_VerifyMocksAsync(string EntityId)
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        delayProviderMock.Setup(x => x.MotionOnSequenceDelay).Returns(TimeSpan.Zero);
        lightControlMock.Setup(x => x.LuxBasedBrightness).Returns(luxBasedBrightnessMock.Object);
        luxBasedBrightnessMock.Setup(x => x.GetBrightness(50, 255)).Returns(100);
        luxBasedBrightnessMock.Setup(x => x.GetBrightness(1, 150)).Returns(50);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Traphal1.EntityId), 100)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Traphal2.EntityId), 100)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Traphal3.EntityId), 100)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Traphal.EntityId), 50)).Returns(true);

        // Act
        Sut.MotionDetected(EntityId);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

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
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.TraphalWand.EntityId), 0)).Returns(false);

        // Act
        Sut.MotionCleared("");
        await Task.Delay(TimeSpan.FromMilliseconds(200));

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
    }
}