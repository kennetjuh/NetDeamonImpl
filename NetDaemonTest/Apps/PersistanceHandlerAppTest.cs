using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Apps;

public class PersistanceHandlerAppTest : TestBase
{  
    [Fact]
    public void PersistanceHandlerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<PersistanceHandlerApp>();

        // Assert
        VerifyAllMocks();
    }    
}