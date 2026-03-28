using NetDaemonImpl.Modules.Notify;
using NetDaemonInterface;
using Xunit;

namespace NetDaemonTest.Modules;

public class NotifyActionTest
{
    [Fact]
    public void NotifyAction_ActionConstructor_SetsProperties()
    {
        // Arrange
        var executed = false;

        // Act
        var action = new NotifyAction(NotifyActionEnum.Thermostat15, "Set to 15", () => executed = true);

        // Assert
        Assert.Equal(NotifyActionEnum.Thermostat15, action.Id);
        Assert.Equal("Set to 15", action.Text);
        Assert.NotNull(action.Action);
        Assert.Null(action.Uri);
        action.Action!();
        Assert.True(executed);
    }

    [Fact]
    public void NotifyAction_UriConstructor_SetsProperties()
    {
        // Act
        var action = new NotifyAction(NotifyActionEnum.UriDeurbel, "Open Camera", "https://example.com");

        // Assert
        Assert.Equal(NotifyActionEnum.UriDeurbel, action.Id);
        Assert.Equal("Open Camera", action.Text);
        Assert.Null(action.Action);
        Assert.Equal("https://example.com", action.Uri);
    }
}
