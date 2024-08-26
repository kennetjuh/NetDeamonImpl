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
    public void SingleClick_TwinkleOffLightOffDay_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = "Day" });
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "off" });
        twinkleMock.Setup(x => x.IsActive()).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 10)).Returns(true);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SingleClick_TwinkleOffLightOffNight_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = "Night" });
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "off" });
        twinkleMock.Setup(x => x.IsActive()).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 1)).Returns(true);

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
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 0)).Returns(true);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DoubleClick_TwinkleOnDay_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = "Day" });
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 20)).Returns(true);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DoubleClick_TwinkleOnNight_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, twinkleMock.Object);
        haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = "Night" });
        haContextMock.Setup(x => x.GetState(entities.Light.KeukenKeukenlamp.EntityId)).Returns(new EntityState() { State = "on" });
        twinkleMock.Setup(x => x.IsActive()).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 1)).Returns(true);

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
        lightControlMock.Setup(x => x.ButtonDefault(NetDaemonInterface.DeconzEventIdEnum.Double, It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId))).Returns(true);

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
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 100)).Returns(true);

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
        lightControlMock.Setup(x => x.ButtonDefault(NetDaemonInterface.DeconzEventIdEnum.LongPress, It.Is<LightEntity>(x => x.EntityId == entities.Light.KeukenKeukenlamp.EntityId))).Returns(true);

        // Act
        Sut.ButtonPressed("", NetDaemonInterface.DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }
}
