using NetDaemonImpl.Modules.Notify;
using NetDaemonInterface;
using Xunit;

namespace NetDaemonTest.Modules;

public class NotifyRecordTest
{
    [Fact]
    public void RecordNotifyAction_UriPrefix_SetsActionToURI()
    {
        // Arrange
        var notifyAction = new NotifyAction(NotifyActionEnum.UriDeurbel, "Open Camera", "https://example.com");

        // Act
        var record = new RecordNotifyAction(notifyAction);

        // Assert
        Assert.Equal("URI", record.action);
        Assert.Equal("Open Camera", record.title);
        Assert.Equal("https://example.com", record.uri);
    }

    [Fact]
    public void RecordNotifyAction_NonUriPrefix_SetsActionToIdString()
    {
        // Arrange
        var notifyAction = new NotifyAction(NotifyActionEnum.Thermostat15, "Set to 15", () => { });

        // Act
        var record = new RecordNotifyAction(notifyAction);

        // Assert
        Assert.Equal("Thermostat15", record.action);
        Assert.Equal("Set to 15", record.title);
        Assert.Null(record.uri);
    }

    [Fact]
    public void RecordNotifyAction_UriThermostat_SetsActionToURI()
    {
        // Arrange
        var notifyAction = new NotifyAction(NotifyActionEnum.UriThermostat, "More", "entityId:climate.test");

        // Act
        var record = new RecordNotifyAction(notifyAction);

        // Assert
        Assert.Equal("URI", record.action);
        Assert.Equal("More", record.title);
        Assert.Equal("entityId:climate.test", record.uri);
    }

}