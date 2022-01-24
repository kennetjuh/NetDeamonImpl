using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlKeukenTest : AreaControlTestBase<AreaControlKeuken>
{
    public AreaControlKeukenTest()
    {
        light = entities.Light.KeukenKeukenlamp;
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();

        // Act
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SingleClick_TwinkleOffLightOff_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "off" });
        twinkleMock.Setup(x => x.IsActive()).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 10)).Returns(null);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SingleClick_TwinkleOffLightOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(false);
        twinkleMock.Setup(x => x.Start());

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SingleClick_TwinkleOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "off" });
        twinkleMock.Setup(x => x.IsActive()).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 0)).Returns(null);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DoubleClick_TwinkleOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 50)).Returns(null);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DoubleClick_TwinkleOff_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(false);
        lightControlMock.Setup(x => x.ButtonDefault(NetDaemonInterface.DeconzEventIdEnum.Double, It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId))).Returns(null);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void LongPress_TwinkleOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 100)).Returns(null);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void LongPress_TwinkleOff_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(false);
        lightControlMock.Setup(x => x.ButtonDefault(NetDaemonInterface.DeconzEventIdEnum.LongPress, It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId))).Returns(null);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }
}
