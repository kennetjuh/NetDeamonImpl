using Moq;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlWoonkamerTest : AreaControlTestBase<AreaControlWoonkamer>
{
    public AreaControlWoonkamerTest()
    {

    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerKamer.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerBureau.EntityId)));

        // Act
        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void BoogButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.WoonkamerBoog;
        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerKamer.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerBureau.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonBooglampBatteryLevel.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void KamerButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.WoonkamerKamer;

        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerKamer.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerBureau.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefault(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonKamerlampBatteryLevel.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void BureauButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.WoonkamerBureau;

        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerKamer.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerBureau.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefault(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(null);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonBureaulampBatteryLevel.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void WoonkamerButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange        

        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerKamer.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.WoonkamerBureau.EntityId)));
        switch (id)
        {
            case DeconzEventIdEnum.Single:
                houseStateMock.Setup(x => x.HouseStateAwake());
                break;
            case DeconzEventIdEnum.Double:
                houseStateMock.Setup(x => x.HouseStateAway());
                break;
            case DeconzEventIdEnum.LongPress:
                houseStateMock.Setup(x => x.HouseStateSleeping());
                break;
        }

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonWoonkamerBatteryLevel.EntityId, id);

        // Assert
        VerifyAllMocks();
    }
}
