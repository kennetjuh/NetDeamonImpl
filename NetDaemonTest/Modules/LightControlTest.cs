using Microsoft.Extensions.Logging;
using Moq;
using NetDaemonImpl;
using NetDaemonImpl.Modules;
using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonTest.TestLights;
using System.Collections.Generic;
using Xunit;

namespace NetDaemonTest.Modules;

public class LightControlTest : ServiceProviderTestBase
{
    private readonly Mock<ILuxBasedBrightness> luxMock = new(MockBehavior.Strict);
    private readonly Mock<ILogger<LightControl>> loggerMock = new();

    private LightControl CreateSut()
    {
        return new LightControl(serviceProviderMock.Object, luxMock.Object, loggerMock.Object);
    }

    // ──────────────────────────────────────────────────
    // SetLight – null SupportedColorModes
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLight_NullSupportedModes_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightNull(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLight(light, 100);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLight – unsupported mode
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLight_UnsupportedMode_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightUnsupported(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLight(light, 100);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLight – color_temp mode
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLight_ColorTemp_NullBrightness_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.SetLight(light, null);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_ColorTemp_AllwaysWhite_NullBrightness_TurnsOnWithMaxKelvin()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddAllwaysWhiteLight(light);

        var result = sut.SetLight(light, null);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_ColorTemp_BrightnessPositive_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            Brightness = null,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.SetLight(light, 100);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_ColorTemp_BrightnessZero_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 200,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.SetLight(light, 0);

        Assert.False(result);
    }

    [Fact]
    public void SetLight_ColorTemp_MaxWhite_AtMax_SwitchesToWhite()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 255,
            ColorTemp = 2000,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddMaxWhiteLight(light);

        var result = sut.SetLight(light, 255);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_ColorTemp_MaxWhite_AtWhite_ResetsTo255()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 200,
            ColorTemp = 6500,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddMaxWhiteLight(light);

        var result = sut.SetLight(light, 100);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_ColorTemp_CustomColor_AtMax_SwitchesToRgb()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 255,
            ColorTemp = 2000,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddMaxCustomColorLight(light, new List<int> { 255, 0, 0 });

        var result = sut.SetLight(light, 255);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_ColorTemp_CustomColor_AtWhite_ResetsTo255()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 200,
            ColorTemp = 6500,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddMaxCustomColorLight(light, new List<int> { 255, 0, 0 });

        var result = sut.SetLight(light, 100);

        Assert.True(result);
    }

    // ──────────────────────────────────────────────────
    // SetLight – brightness mode
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLight_Brightness_NullBrightness_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLight(light, null);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_Brightness_BrightnessPositive_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLight(light, 100);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_Brightness_BrightnessZero_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test") { State = "on" };

        var result = sut.SetLight(light, 0);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLight – onoff mode
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLight_OnOff_BrightnessPositive_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightOnOff(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLight(light, 100);

        Assert.True(result);
    }

    [Fact]
    public void SetLight_OnOff_BrightnessZero_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightOnOff(haContextMock.Object, "light.test") { State = "on" };

        var result = sut.SetLight(light, 0);

        Assert.False(result);
    }

    [Fact]
    public void SetLight_OnOff_NullBrightness_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightOnOff(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLight(light, null);

        Assert.True(result);
    }

    // ──────────────────────────────────────────────────
    // ButtonDefault
    // ──────────────────────────────────────────────────

    [Fact]
    public void ButtonDefault_Single_LightOn_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 100,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Single, light);

        Assert.False(result);
    }

    [Fact]
    public void ButtonDefault_Single_LightOff_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Single, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_Double_LightOnWithBrightness_IncreaseBrightness()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 100,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Double, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_Double_LightOff_TurnsOnAtHalfBrightness()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Double, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_LongPress_LightOnWithBrightness_DecreaseBrightness()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 200,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.LongPress, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_LongPress_LightOff_TurnsOnAtFullBrightness()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.LongPress, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_Unknown_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Unknown, light);

        Assert.False(result);
    }

    [Fact]
    public void ButtonDefault_Double_LightOn_NoBrightness_TurnsOnAtHalfBrightness()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = null,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Double, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_LongPress_LightOn_NoBrightness_TurnsOnAtFullBrightness()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = null,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.LongPress, light);

        Assert.True(result);
    }

    // ──────────────────────────────────────────────────
    // ButtonDefaultLuxBased
    // ──────────────────────────────────────────────────

    [Fact]
    public void ButtonDefaultLuxBased_Single_LightOn_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 100,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefaultLuxBased(ButtonEventType.Single, light, 1, 100);

        Assert.False(result);
    }

    [Fact]
    public void ButtonDefaultLuxBased_Single_LightOff_TurnsOnWithLuxBrightness()
    {
        luxMock.Setup(x => x.GetBrightness(1, 100)).Returns(50);

        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefaultLuxBased(ButtonEventType.Single, light, 1, 100);

        Assert.True(result);
        luxMock.Verify(x => x.GetBrightness(1, 100), Times.Once);
    }

    [Fact]
    public void ButtonDefaultLuxBased_Double_SameAsButtonDefault()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 100,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefaultLuxBased(ButtonEventType.Double, light, 1, 100);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefaultLuxBased_LongPress_SameAsButtonDefault()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 200,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefaultLuxBased(ButtonEventType.LongPress, light, 1, 100);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefaultLuxBased_Unknown_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefaultLuxBased(ButtonEventType.Unknown, light, 1, 100);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLightKelvin
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLightKelvin_BrightnessPositive_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.SetLightKelvin(light, 100, 4000);

        Assert.True(result);
    }

    [Fact]
    public void SetLightKelvin_BrightnessZero_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.SetLightKelvin(light, 0, 4000);

        Assert.False(result);
    }

    [Fact]
    public void SetLightKelvin_NoColorTempMode_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLightKelvin(light, 100, 4000);

        Assert.False(result);
    }

    [Fact]
    public void SetLightKelvin_NullSupportedModes_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightNull(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLightKelvin(light, 100, 4000);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLightColorName
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLightColorName_BrightnessPositive_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightHs(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLightColorName(light, 100, "red");

        Assert.True(result);
    }

    [Fact]
    public void SetLightColorName_BrightnessZero_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightHs(haContextMock.Object, "light.test") { State = "on" };

        var result = sut.SetLightColorName(light, 0, "red");

        Assert.False(result);
    }

    [Fact]
    public void SetLightColorName_NoHsMode_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLightColorName(light, 100, "red");

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLightRgb
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLightRgb_BrightnessPositive_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightHs(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLightRgb(light, 100, new List<int> { 255, 0, 0 });

        Assert.True(result);
    }

    [Fact]
    public void SetLightRgb_BrightnessZero_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightHs(haContextMock.Object, "light.test") { State = "on" };

        var result = sut.SetLightRgb(light, 0, new List<int> { 255, 0, 0 });

        Assert.False(result);
    }

    [Fact]
    public void SetLightRgb_NoHsMode_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test") { State = "off" };

        var result = sut.SetLightRgb(light, 100, new List<int> { 255, 0, 0 });

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // SetLightXY
    // ──────────────────────────────────────────────────

    [Fact]
    public void SetLightXY_NoXYMode_ReturnsFalse()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "off",
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.SetLightXY(light, 100, new List<double> { 0.3, 0.3 });

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // AddMaxWhiteLight / AddAllwaysWhiteLight / AddMaxCustomColorLight
    // ──────────────────────────────────────────────────

    [Fact]
    public void AddMaxWhiteLight_MaxWhite_BrightnessZero_DoesNotSwitchToWhite()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 255,
            ColorTemp = 6500,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddMaxWhiteLight(light);

        var result = sut.SetLight(light, 0);

        Assert.False(result);
    }

    [Fact]
    public void AddMaxCustomColorLight_CustomColor_BrightnessZero_DoesNotSwitchToColor()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 255,
            ColorTemp = 6500,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };
        sut.AddMaxCustomColorLight(light, new List<int> { 255, 0, 0 });

        var result = sut.SetLight(light, 0);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────
    // ButtonDefault with brightness-only lights
    // ──────────────────────────────────────────────────

    [Fact]
    public void ButtonDefault_Single_BrightnessLight_On_TurnsOff()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 100
        };

        var result = sut.ButtonDefault(ButtonEventType.Single, light);

        Assert.False(result);
    }

    [Fact]
    public void ButtonDefault_Single_BrightnessLight_Off_TurnsOn()
    {
        var sut = CreateSut();
        var light = new TestLightBrightness(haContextMock.Object, "light.test")
        {
            State = "off"
        };

        var result = sut.ButtonDefault(ButtonEventType.Single, light);

        Assert.True(result);
    }

    // ──────────────────────────────────────────────────
    // Edge cases: double click clamping
    // ──────────────────────────────────────────────────

    [Fact]
    public void ButtonDefault_Double_HighBrightness_ClampsTo255()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 230,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.Double, light);

        Assert.True(result);
    }

    [Fact]
    public void ButtonDefault_LongPress_LowBrightness_ClampsTo1()
    {
        var sut = CreateSut();
        var light = new TestLightColorTemp(haContextMock.Object, "light.test")
        {
            State = "on",
            Brightness = 10,
            MinColorTempKelvin = 2000,
            MaxColorTempKelvin = 6500
        };

        var result = sut.ButtonDefault(ButtonEventType.LongPress, light);

        Assert.True(result);
    }

    // ──────────────────────────────────────────────────
    // LuxBasedBrightness property
    // ──────────────────────────────────────────────────

    [Fact]
    public void LuxBasedBrightness_ReturnsInjectedInstance()
    {
        var sut = CreateSut();

        Assert.Equal(luxMock.Object, sut.LuxBasedBrightness);
    }
}
