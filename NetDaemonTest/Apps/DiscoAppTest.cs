using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Apps;

public class DiscoAppTest : TestBase
{
    [Fact]
    public void DiscoApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act - no entities registered so _lights is empty
        var app = Context.GetApp<DiscoApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DiscoApp_DiscoModeOn_NoLights_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        var app = Context.GetApp<DiscoApp>();

        // Act - toggle disco mode on; no lights available, so StartDisco is a no-op
        HaMock.TriggerStateChange(Entities.InputBoolean.Discomode, "on");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DiscoApp_DiscoModeOff_NoLights_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        var app = Context.GetApp<DiscoApp>();

        // Act - toggle disco mode off; StopDisco with no lights is a no-op
        HaMock.TriggerStateChange(Entities.InputBoolean.Discomode, "off");

        // Assert
        VerifyAllMocks();
    }
}
