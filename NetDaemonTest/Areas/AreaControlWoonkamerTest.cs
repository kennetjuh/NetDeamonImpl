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
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Booglamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Kamerlamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Bureaulamp.EntityId)));

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
        light = entities.Light.Booglamp;
        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Booglamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Kamerlamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Bureaulamp.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonBooglampBattery.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void KamerButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.Kamerlamp;

        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Booglamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Kamerlamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Bureaulamp.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefault(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonKamerlampBattery.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void BureauButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange
        light = entities.Light.Bureaulamp;

        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Booglamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Kamerlamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Bureaulamp.EntityId)));
        lightControlMock.Setup(x => x.ButtonDefault(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId)))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonBureaulampBattery.EntityId, id);

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [MemberData(nameof(DeconzEventIdValues))]
    public void WoonkamerButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        // Arrange        

        SetupMocks();
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Booglamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Kamerlamp.EntityId)));
        lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(x => x.EntityId == entities.Light.Bureaulamp.EntityId)));
        switch (id)
        {
            case DeconzEventIdEnum.Single:
                houseStateMock.Setup(x => x.HouseStateAwake());
                break;
            case DeconzEventIdEnum.Double:
                houseStateMock.Setup(x => x.HouseStateAway(HouseStateEnum.Away));
                break;
            case DeconzEventIdEnum.LongPress:
                houseStateMock.Setup(x => x.HouseStateSleeping());
                break;
        }

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object);

        // Act
        Sut.ButtonPressed(entities.Sensor.ButtonWoonkamerBattery.EntityId, id);

        // Assert
        VerifyAllMocks();
    }
}
