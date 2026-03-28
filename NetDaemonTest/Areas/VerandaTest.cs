using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps.Areas;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Areas;

public class VerandaTest : TestBase
{
    [Fact]
    public void Veranda_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<Veranda>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Veranda_MotionDetected_Night_LightOn()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Night" });
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.LightVeranda.EntityId), 1)).Returns(true);

        var app = Context.GetApp<Veranda>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionVeranda, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Veranda_MotionDetected_Day_NoAction()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputText.Daynight.EntityId, new EntityState() { State = "Day" });

        var app = Context.GetApp<Veranda>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionVeranda, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Veranda_MotionCleared_StartsAfterTask()
    {
        // Arrange
        ResetAllMocks();
        DelayProviderMock.Setup(x => x.MotionClear).Returns(TimeSpan.FromMinutes(5));

        var app = Context.GetApp<Veranda>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.MotionVeranda, "off");

        // Assert
        VerifyAllMocks();
    }
}
