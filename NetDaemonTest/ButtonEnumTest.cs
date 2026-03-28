using NetDaemonInterface.Enums;
using Xunit;

namespace NetDaemonTest;

public class ButtonEnumTest
{
    [Fact]
    public void ToButtonId_KnownId_ReturnsButton()
    {
        Assert.Equal(Button.Kip, "becbdc29ed9e8e39d220feed7e215e7a".ToButtonId());
    }

    [Fact]
    public void ToButtonId_UnknownId_ReturnsUnknown()
    {
        Assert.Equal(Button.Unknown, "unknown_id".ToButtonId());
    }

    [Fact]
    public void ToButtonId_Null_ReturnsUnknown()
    {
        Assert.Equal(Button.Unknown, ((string?)null).ToButtonId());
    }

    [Fact]
    public void ToRaw_KnownButton_ReturnsId()
    {
        Assert.Equal("becbdc29ed9e8e39d220feed7e215e7a", Button.Kip.ToRaw());
    }

    [Fact]
    public void ToRaw_Unknown_ReturnsUnknownString()
    {
        Assert.Equal("unknown", Button.Unknown.ToRaw());
    }

    [Fact]
    public void IsKnown_KnownButtonId_ReturnsTrue()
    {
        Assert.True(ButtonIdExtensions.IsKnown("becbdc29ed9e8e39d220feed7e215e7a"));
    }

    [Fact]
    public void IsKnown_UnknownButtonId_ReturnsFalse()
    {
        Assert.False(ButtonIdExtensions.IsKnown("unknown_id"));
    }

    [Fact]
    public void IsKnown_NullButtonId_ReturnsFalse()
    {
        Assert.False(ButtonIdExtensions.IsKnown(null));
    }

    [Fact]
    public void IsKnown_KnownEventType_ReturnsTrue()
    {
        Assert.True(ButtonEventTypeExtensions.IsKnown("single"));
    }

    [Fact]
    public void IsKnown_UnknownEventType_ReturnsFalse()
    {
        Assert.False(ButtonEventTypeExtensions.IsKnown("unknown_cmd"));
    }

    [Fact]
    public void ToButtonEventType_Single_ReturnsSingle()
    {
        Assert.Equal(ButtonEventType.Single, "single".ToButtonEventType());
    }

    [Fact]
    public void ToButtonEventType_Double_ReturnsDouble()
    {
        Assert.Equal(ButtonEventType.Double, "double".ToButtonEventType());
    }

    [Fact]
    public void ToButtonEventType_Hold_ReturnsLongPress()
    {
        Assert.Equal(ButtonEventType.LongPress, "hold".ToButtonEventType());
    }

    [Fact]
    public void ToButtonEventType_Unknown_ReturnsUnknown()
    {
        Assert.Equal(ButtonEventType.Unknown, "unknown_cmd".ToButtonEventType());
    }

    [Fact]
    public void ToButtonEventType_Null_ReturnsUnknown()
    {
        Assert.Equal(ButtonEventType.Unknown, ((string?)null).ToButtonEventType());
    }

    [Fact]
    public void ButtonEventType_ToRaw_Single_ReturnsSingle()
    {
        Assert.Equal("single", ButtonEventType.Single.ToRaw());
    }

    [Fact]
    public void ButtonEventType_ToRaw_Unknown_ReturnsUnknownString()
    {
        Assert.Equal("unknown", ButtonEventType.Unknown.ToRaw());
    }
}
