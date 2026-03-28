using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NetDaemonImpl.IObservable;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Text.Json;
using Xunit;

namespace NetDaemonTest;

public class ButtonEventsTest
{
    private readonly HaContextMock _haMock;
    private readonly ButtonEvents _buttonEvents;

    public ButtonEventsTest()
    {
        _haMock = new HaContextMock(Moq.MockBehavior.Strict);
        _haMock.CallBase = true;

        var services = new ServiceCollection();
        services.AddTransient<IHaContext>(_ => _haMock.Object);
        var provider = services.BuildServiceProvider();

        var logger = new Mock<ILogger<ButtonEvents>>();
        _buttonEvents = new ButtonEvents(provider, logger.Object);
    }

    [Fact]
    public void DeconzEvent_SingleClick_PublishesButtonEvent()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { id = "becbdc29ed9e8e39d220feed7e215e7a", unique_id = "test", @event = 1002, device_id = "test" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });

        // Assert
        Assert.NotNull(received);
        Assert.Equal(Button.Kip, received.Button);
        Assert.Equal(ButtonEventType.Single, received.Event);
    }

    [Fact]
    public void DeconzEvent_DoubleClick_PublishesButtonEvent()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { id = "becbdc29ed9e8e39d220feed7e215e7a", unique_id = "test", @event = 1004, device_id = "test" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });

        // Assert
        Assert.NotNull(received);
        Assert.Equal(Button.Kip, received.Button);
        Assert.Equal(ButtonEventType.Double, received.Event);
    }

    [Fact]
    public void DeconzEvent_LongPress_PublishesButtonEvent()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { id = "becbdc29ed9e8e39d220feed7e215e7a", unique_id = "test", @event = 1001, device_id = "test" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });

        // Assert
        Assert.NotNull(received);
        Assert.Equal(Button.Kip, received.Button);
        Assert.Equal(ButtonEventType.LongPress, received.Event);
    }

    [Fact]
    public void DeconzEvent_ReleaseEvent_IsFiltered()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { id = "becbdc29ed9e8e39d220feed7e215e7a", unique_id = "test", @event = 1003, device_id = "test" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void DeconzEvent_UnknownButtonId_DoesNotPublish()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { id = "unknown_button_id", unique_id = "test", @event = 1002, device_id = "test" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void DeconzEvent_NullDataElement_DoesNotPublish()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = null });

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void ZhaEvent_SingleClick_PublishesButtonEvent()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { command = "single", device_id = "5a6c7fad830336920fe922bf816f2256" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "zha_event", DataElement = json });

        // Assert
        Assert.NotNull(received);
        Assert.Equal(Button.Bank, received.Button);
        Assert.Equal(ButtonEventType.Single, received.Event);
    }

    [Fact]
    public void ZhaEvent_DoubleClick_PublishesButtonEvent()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { command = "double", device_id = "5a6c7fad830336920fe922bf816f2256" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "zha_event", DataElement = json });

        // Assert
        Assert.NotNull(received);
        Assert.Equal(Button.Bank, received.Button);
        Assert.Equal(ButtonEventType.Double, received.Event);
    }

    [Fact]
    public void ZhaEvent_Hold_PublishesLongPress()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { command = "hold", device_id = "5a6c7fad830336920fe922bf816f2256" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "zha_event", DataElement = json });

        // Assert
        Assert.NotNull(received);
        Assert.Equal(Button.Bank, received.Button);
        Assert.Equal(ButtonEventType.LongPress, received.Event);
    }

    [Fact]
    public void ZhaEvent_UnknownCommand_DoesNotPublish()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { command = "unknown_cmd", device_id = "5a6c7fad830336920fe922bf816f2256" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "zha_event", DataElement = json });

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void ZhaEvent_UnknownDeviceId_DoesNotPublish()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { command = "single", device_id = "unknown_device" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "zha_event", DataElement = json });

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void ZhaEvent_NullDataElement_DoesNotPublish()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        // Act
        _haMock.TriggerEvent(new Event { EventType = "zha_event", DataElement = null });

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void DeconzEvent_PreviousEventTime_IsTracked()
    {
        // Arrange
        ButtonEvent? first = null;
        ButtonEvent? second = null;
        var count = 0;
        _buttonEvents.Event.Subscribe(e =>
        {
            count++;
            if (count == 1) first = e;
            if (count == 2) second = e;
        });

        var json = JsonSerializer.SerializeToElement(new { id = "becbdc29ed9e8e39d220feed7e215e7a", unique_id = "test", @event = 1002, device_id = "test" });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });
        _haMock.TriggerEvent(new Event { EventType = "deconz_event", DataElement = json });

        // Assert
        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Equal(DateTime.MinValue, first.previousEvent);
        Assert.NotEqual(DateTime.MinValue, second.previousEvent);
    }

    [Fact]
    public void OtherEventType_IsIgnored()
    {
        // Arrange
        ButtonEvent? received = null;
        _buttonEvents.Event.Subscribe(e => received = e);

        var json = JsonSerializer.SerializeToElement(new { id = "becbdc29ed9e8e39d220feed7e215e7a", @event = 1002 });

        // Act
        _haMock.TriggerEvent(new Event { EventType = "other_event", DataElement = json });

        // Assert
        Assert.Null(received);
    }
}
