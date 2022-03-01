using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Threading.Tasks;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlVoordeurTest : AreaControlTestBase<AreaControlVoordeur>
{
    public AreaControlVoordeurTest()
    {
        light = entities.Light.WandlampBuiten;
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
    public void MotionDetected_LightOff_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.WandlampBuiten.EntityId)).Returns(new EntityState() { State = "off" });

        // Act
        Sut.MotionDetected("");

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
    }

    [Fact]
    public void MotionDetected_LightOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.WandlampBuiten.EntityId)).Returns(new EntityState() { State = "on" });
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WandlampBuiten.EntityId), Constants.brightnessWandVoorMotion)).Returns(true);

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
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WandlampBuiten.EntityId), Constants.brightnessWandVoor)).Returns(true);

        // Act
        Sut.MotionCleared("");
        await Task.Delay(TimeSpan.FromMilliseconds(200));

        // Assert
        VerifyAllMocks();
        Assert.Equal(AreaModeEnum.Idle, Sut.GetPrivate<AreaModeEnum>("mode"));
    }
}
