using Microsoft.Extensions.Options;
using Moq;
using NetDaemonImpl.Modules.Notify;
using NetDaemonInterface;
using NetDaemonInterface.Models;
using NetDaemonTest.Apps.Helpers;
using Xunit;

namespace NetDaemonTest.Modules;

public class NotifyActionsTest
{
    [Fact]
    public void NotifyActions_Constructor_CreatesFourActions()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var notifyMock = new Mock<INotify>(MockBehavior.Strict);
        var thinginoClientMock = new Mock<IThinginoClient>();

        // Act
        var optionsMock = new Mock<IOptions<ThinginoSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ThinginoSettings());
        var actions = new NotifyActions(haMock.Object, notifyMock.Object, thinginoClientMock.Object, optionsMock.Object);   

        // Assert
        Assert.Equal(5, actions.Actions.Count);
    }

    [Fact]
    public void NotifyActions_Constructor_ContainsThermostat17()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var notifyMock = new Mock<INotify>(MockBehavior.Strict);
        var thinginoClientMock = new Mock<IThinginoClient>();

        // Act
        var optionsMock = new Mock<IOptions<ThinginoSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ThinginoSettings());
        var actions = new NotifyActions(haMock.Object, notifyMock.Object, thinginoClientMock.Object, optionsMock.Object);

        // Assert
        Assert.True(actions.Actions.ContainsKey(NotifyActionEnum.Thermostat15));
        Assert.Equal("Set to 15", actions.Actions[NotifyActionEnum.Thermostat15].Text);
    }

    [Fact]
    public void NotifyActions_Constructor_ContainsThermostat20()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var notifyMock = new Mock<INotify>(MockBehavior.Strict);
        var thinginoClientMock = new Mock<IThinginoClient>();

        // Act
        var optionsMock = new Mock<IOptions<ThinginoSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ThinginoSettings());
        var actions = new NotifyActions(haMock.Object, notifyMock.Object, thinginoClientMock.Object, optionsMock.Object);

        // Assert
        Assert.True(actions.Actions.ContainsKey(NotifyActionEnum.Thermostat19));
        Assert.Equal("Set to 19", actions.Actions[NotifyActionEnum.Thermostat19].Text);
    }

    [Fact]
    public void NotifyActions_Constructor_ContainsUriActions()
    {
        // Arrange
        var haMock = new HaContextMock(Moq.MockBehavior.Strict);
        haMock.CallBase = true;
        var notifyMock = new Mock<INotify>(MockBehavior.Strict);
        var thinginoClientMock = new Mock<IThinginoClient>();

        // Act
        var optionsMock = new Mock<IOptions<ThinginoSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ThinginoSettings());
        var actions = new NotifyActions(haMock.Object, notifyMock.Object, thinginoClientMock.Object, optionsMock.Object);   

        // Assert
        Assert.True(actions.Actions.ContainsKey(NotifyActionEnum.UriThermostat));
        Assert.True(actions.Actions.ContainsKey(NotifyActionEnum.UriDeurbel));
    }
}
