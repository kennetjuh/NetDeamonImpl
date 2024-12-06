using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Apps;

public class TestAppTest : TestBase
{
    [Fact]
    public void TestAppTest_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<TestApp>();

        // Assert
        VerifyAllMocks();
    }
}