using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps.Areas;
using NetDaemonTest.Apps.Helpers;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Areas;

public class KelderTest : TestBase
{
    [Fact]
    public void Kelder_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<Kelder>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Kelder_OpenCloseSensorOpen_NoImmediateError()
    {
        // Arrange
        ResetAllMocks();
        // TurnOn is executed inside StartAfterTask with a 500ms delay, so it won't fire synchronously

        var app = Context.GetApp<Kelder>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseKelderOpening, "on");

        // Assert - no error during event processing
        VerifyAllMocks();
    }

    [Fact]
    public void Kelder_OpenCloseSensorClose_TurnsPlugOff()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.Switch.PlugKelder.EntityId), null));

        var app = Context.GetApp<Kelder>();

        // Act
        HaMock.TriggerStateChange(Entities.BinarySensor.OpencloseKelderOpening, "off");

        // Assert
        VerifyAllMocks();
    }
}
