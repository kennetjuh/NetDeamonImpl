using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Threading.Tasks;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlSlaapkamerTest : AreaControlTestBase<AreaControlSlaapkamer>
{
    public AreaControlSlaapkamerTest()
    {
        light = entities.Light.Slaapkamer;
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));

        // Act
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamer_SingleAllOff_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.Slaapkamer.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.NachtlampKen.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.NachtlampGreet.EntityId)).Returns(new EntityState() { State = "off" });

        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 1)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerBattery.EntityId, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamer_SingleCurrentOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.Slaapkamer.EntityId)).Returns(new EntityState() { State = "on" });


        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 0)).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 0)).Returns(false);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerBattery.EntityId, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData(DeconzEventIdEnum.Double)]
    [InlineData(DeconzEventIdEnum.LongPress)]
    public void ButtonSlaapKamer_SingleAndDouble_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.ButtonDefault(id, It.Is<LightEntity>(x => x.EntityId == light.EntityId))).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 1)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerBattery.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_SingleAllOff_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.Slaapkamer.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.NachtlampKen.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.NachtlampGreet.EntityId)).Returns(new EntityState() { State = "off" });

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 1)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_KamerOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.Slaapkamer.EntityId)).Returns(new EntityState() { State = "on" });

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_SingleOnlyNightOn_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);
        haContextMock.Setup(x => x.GetState(entities.Light.Slaapkamer.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.NachtlampKen.EntityId)).Returns(new EntityState() { State = "on" });

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 0)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 0)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Single);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_Longpress_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 5)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 1)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.LongPress);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_DoubleClick1Time_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 0)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_DoubleClick2Times_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 0)).Returns(true);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 0)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer3.EntityId), 1)).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_DoubleClick3Times_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(100));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 0)).Returns(false);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 0)).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer3.EntityId), 1)).Returns(true);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer3.EntityId), 50, "red")).Returns(true);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public async Task ButtonSlaapKamerBed_DoubleClick2TimesWithDelay_VerifyMocksAsync()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(1));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(false);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.NachtlampGreet.EntityId), 0)).Returns(false);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBattery.EntityId, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }
}
