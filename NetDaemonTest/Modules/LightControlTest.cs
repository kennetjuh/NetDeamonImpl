using Microsoft.Extensions.Logging;
using Moq;
using NetDaemonImpl;
using NetDaemonImpl.Modules;
using NetDaemonInterface;
using NetDaemonTest.TestLights;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Modules;

public class LightControlTest : ServiceProviderTestBase
{
    private readonly Mock<ILuxBasedBrightness> luxBasedBrightnessMock = new(MockBehavior.Strict);
    private readonly Mock<ILogger<LightControl>> loggerMock = new();

    private readonly TestLightColorTemp testLightColortemp;
    private readonly TestLightOnOff testLightOnOff;
    private readonly TestLightHs testLightHs;
    private readonly TestLightBrightness testLightBrightness;

    internal override void VerifyAllMocks()
    {
        base.VerifyAllMocks();
        luxBasedBrightnessMock.VerifyAll();
    }

    internal override void SetupMocks()
    {
        base.SetupMocks();
        luxBasedBrightnessMock.Reset();
    }

    public LightControlTest()
    {        
        testLightColortemp = new TestLightColorTemp(haContextMock.Object, "TestLight");
        testLightOnOff = new TestLightOnOff(haContextMock.Object, "TestLight");
        testLightHs = new TestLightHs(haContextMock.Object, "TestLight");
        testLightBrightness = new TestLightBrightness(haContextMock.Object, "TestLight");
    }

    private void VerifyServiceCallOnTestLight(TestLightBase light, string service, double? brightness, double? colorTemp = null, string? colorName = null)
    {
        Assert.Single(light.ServiceCalls);
        var serviceCall = light.ServiceCalls.Single();
        Assert.Equal(service, serviceCall.Item1);
        if (service == "turn_on")
        {
            Assert.Equal(brightness, (long?)(serviceCall.Item2?.GetType().GetProperty("Brightness")?.GetValue(serviceCall.Item2, null)));
            if (colorTemp != null)
            {
                Assert.Equal(colorTemp, (long?)(serviceCall.Item2?.GetType().GetProperty("ColorTemp")?.GetValue(serviceCall.Item2, null)));
            }
            if (colorName != null)
            {
                Assert.Equal(colorName, (string?)(serviceCall.Item2?.GetType().GetProperty("ColorName")?.GetValue(serviceCall.Item2, null)));
            }
        }
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();

        // Act
        _ = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void ButtonDefaultSingleClick_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Single, testLightColortemp);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", null);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultSingleClick_CurrentBrightness50_LightOff()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 50;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Single, testLightColortemp);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_off", null);
        Assert.False(retVal);
    }

    [Fact]
    public void ButtonDefaultDoubleClick_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Double, testLightColortemp);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", Constants.doubleClick_brightness);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultDoubleClick_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Double, testLightColortemp);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 100 + Constants.doubleClick_increment);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLongpress_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.LongPress, testLightColortemp);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", Constants.longPress_brightness);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLongpress_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";


        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.LongPress, testLightColortemp);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 100 - Constants.longPress_decrement);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedSingleClick_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        luxBasedBrightnessMock.Setup(x => x.GetBrightness(10, 255)).Returns(100);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Single, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 100);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedSingleClick_CurrentBrightness50_LightOff()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 50;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Single, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_off", null);
        Assert.False(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedDoubleClick_CurrentOffl_LightOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Double, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", Constants.doubleClick_brightness);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedDoubleClick_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Double, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 100 + Constants.doubleClick_increment);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedLongpress_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.LongPress, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", Constants.longPress_brightness);
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedLongpress_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.LongPress, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 100 - Constants.longPress_decrement);
        Assert.True(retVal);
    }

    [Fact]
    public void SetLight_NoMaxWhiteNull_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, null);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", null, 5000);
    }

    [Fact]
    public void SetLight_NoMaxWhite50_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 50);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 50, 5000);
    }

    [Fact]
    public void SetLight_NoMaxWhite0_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.ColorTemp =
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 0);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_off", null);
    }

    [Fact]
    public void SetLight_MaxWhiteNull_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, null);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", null, 5000);
    }

    [Fact]
    public void SetLight_MaxWhite50_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 50);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 50, 5000);
    }

    [Fact]
    public void SetLight_MaxWhite0_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 0);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_off", null);
    }

    [Fact]
    public void SetLight_MaxWhiteRedToWhite_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.Brightness = 255;
        testLightColortemp.ColorTemp = 5000;
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 255);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 255, 0);
    }

    [Fact]
    public void SetLight_MaxWhiteFromWhiteToRed_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.Brightness = 255;
        testLightColortemp.ColorTemp = 0;
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 200);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", 255, 5000);
    }

    [Fact]
    public void SetLight_OnOffLightCurrentOff_lightTurnOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 0;

        // Act
        sut.SetLight(testLightOnOff, 1);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightOnOff, "turn_on", null, null);
    }

    [Fact]
    public void SetLight_OnOffLightCurrentOn_lightTurnOff()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;

        // Act
        sut.SetLight(testLightOnOff, 0);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightOnOff, "turn_off", null, null);
    }

    [Fact]
    public void SetLight_BrightnessLightCurrentOff_lightTurnOn()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 0;

        // Act
        sut.SetLight(testLightBrightness);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightBrightness, "turn_on", null, null);
    }

    [Fact]
    public void SetLight_BrightnessLightCurrentOff_lightTurnOnWithBrightness()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 0;

        // Act
        sut.SetLight(testLightBrightness, 50);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightBrightness, "turn_on", 50, null);
    }

    [Fact]
    public void SetLight_BrightnessLightCurrentOn_lightTurnOff()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 1;

        // Act
        sut.SetLight(testLightBrightness, 0);

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightBrightness, "turn_off", 0, null);
    }

    [Fact]
    public void SetLightColor_TurnOn_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightHs.Brightness = 0;

        // Act
        sut.SetLight(testLightHs, null, "red");

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightHs, "turn_on", null, null, "red");
    }

    [Fact]
    public void SetLightColor_TurnOnWithBrightness_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightHs.Brightness = 0;

        // Act
        sut.SetLight(testLightHs, 50, "red");

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightHs, "turn_on", 50, null, "red");
    }

    [Fact]
    public void SetLightColor_TurnOff_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightHs.Brightness = 0;

        // Act
        sut.SetLight(testLightHs, 0, "red");

        // Assert
        VerifyAllMocks();
        VerifyServiceCallOnTestLight(testLightHs, "turn_off", null, null);
    }
}