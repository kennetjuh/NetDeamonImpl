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
        light = entities.Light.LightSlaapkamer;
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
        haContextMock.Setup(x => x.GetState(entities.Light.LightSlaapkamer.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.SlaapkamerNachtlampKen.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.SlaapkamerNachtlampGreet.EntityId)).Returns(new EntityState() { State = "off" });

        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(DeconzEventIdEnum.Single,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 1)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerBatteryLevel.EntityId, DeconzEventIdEnum.Single);

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
        haContextMock.Setup(x => x.GetState(entities.Light.LightSlaapkamer.EntityId)).Returns(new EntityState() { State = "on" });


        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 0)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerBatteryLevel.EntityId, DeconzEventIdEnum.Single);

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

        lightControlMock.Setup(x => x.ButtonDefault(id, It.Is<LightEntity>(x => x.EntityId == light.EntityId))).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 1)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerBatteryLevel.EntityId, id);

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
        haContextMock.Setup(x => x.GetState(entities.Light.LightSlaapkamer.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.SlaapkamerNachtlampKen.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.SlaapkamerNachtlampGreet.EntityId)).Returns(new EntityState() { State = "off" });

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 1)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Single);

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
        haContextMock.Setup(x => x.GetState(entities.Light.LightSlaapkamer.EntityId)).Returns(new EntityState() { State = "on" });

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 0)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Single);

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
        haContextMock.Setup(x => x.GetState(entities.Light.LightSlaapkamer.EntityId)).Returns(new EntityState() { State = "off" });
        haContextMock.Setup(x => x.GetState(entities.Light.SlaapkamerNachtlampKen.EntityId)).Returns(new EntityState() { State = "on" });

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 0)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Single);

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

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 5)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 1)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.LongPress);

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

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 0)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);

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

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 0)).Returns(null);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer2.EntityId), 1)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);

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

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 0)).Returns(null);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer2.EntityId), 1)).Returns(null);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer2.EntityId), 50, "red")).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonSlaapKamerBed_DoubleClick2TimesWithDelay_VerifyMocks()
    {
        // Arrange 
        SetupMocks();
        delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.FromMilliseconds(1));
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.LightSlaapkamer.EntityId), 0)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(null);
        lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 0)).Returns(null);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);
        Task.Delay(TimeSpan.FromMilliseconds(10)).Wait();
        Sut.ButtonPressed(entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId, DeconzEventIdEnum.Double);

        // Assert
        VerifyAllMocks();
    }
}
