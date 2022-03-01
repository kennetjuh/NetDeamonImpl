using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Areas;

public class AreaControlBuitenachterTest : AreaControlTestBase<AreaControlBuitenAchter>
{
    public AreaControlBuitenachterTest()
    {
        light = entities.Light.BuitenachterLamp;
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
    public void BuitenaAchterButtonPressed_VerifyMocks(DeconzEventIdEnum id)
    {
        buttonCheck(id, "sensor.button_buitenachterzithoek_battery");
        buttonCheck(id, "sensor.button_buitenachter_battery");
        buttonCheck(id, "sensor.button_buitenachterlamp_battery");
    }

    private void buttonCheck(DeconzEventIdEnum id, string button)
    {
        // Arrange
        light = entities.Light.BuitenachterLamp;

        SetupMocks();
        lightControlMock.Setup(x => x.ButtonDefaultLuxBased(
                id,
                It.Is<LightEntity>(x => x.EntityId == light.EntityId),
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns(true);

        Sut = new(entities, delayProviderMock.Object, lightControlMock.Object);

        // Act
        Sut.ButtonPressed(button, id);

        // Assert
        VerifyAllMocks();
    }    
}