using Microsoft.Extensions.Logging;
using Moq;
using NetDaemon.HassModel.Entities;
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
    private readonly TestLightUnsupported testLightUnsupported;
    private readonly TestLightNull testLightNull;

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
        testLightColortemp = new TestLightColorTemp(haContextMock.Object, "Light.testLightColortemp");
        testLightOnOff = new TestLightOnOff(haContextMock.Object, "Light.testLightOnOff");
        testLightHs = new TestLightHs(haContextMock.Object, "Light.testLightHs");
        testLightBrightness = new TestLightBrightness(haContextMock.Object, "Light.testLightBrightness");
        testLightUnsupported = new TestLightUnsupported(haContextMock.Object, "Light.testLightUnsupported");
        testLightNull = new TestLightNull(haContextMock.Object, "Light.testLightNull");
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
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.IsAny<LightTurnOnParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Single, testLightColortemp);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultSingleClick_CurrentBrightness50_LightOff()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 50;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Single, testLightColortemp);

        // Assert
        VerifyAllMocks();
        Assert.False(retVal);
    }

    [Fact]
    public void ButtonDefaultDoubleClick_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == Constants.doubleClick_brightness)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Double, testLightColortemp);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultDoubleClick_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == 100 + Constants.doubleClick_increment)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.Double, testLightColortemp);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLongpress_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == Constants.longPress_brightness)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.LongPress, testLightColortemp);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLongpress_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == 100 - Constants.longPress_decrement)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";


        // Act
        var retVal = sut.ButtonDefault(DeconzEventIdEnum.LongPress, testLightColortemp);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedSingleClick_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == 100)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        luxBasedBrightnessMock.Setup(x => x.GetBrightness(10, 255)).Returns(100);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Single, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedSingleClick_CurrentBrightness50_LightOff()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 50;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Single, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        Assert.False(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedDoubleClick_CurrentOffl_LightOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == Constants.doubleClick_brightness)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Double, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedDoubleClick_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == 100 + Constants.doubleClick_increment)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.Double, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedLongpress_CurrentOff_LightOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == Constants.longPress_brightness)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.State = "off";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.LongPress, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void ButtonDefaultLuxBasedLongpress_CurrentBrightness50_LightIncrement()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == 100 - Constants.longPress_decrement)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 100;
        testLightColortemp.State = "on";

        // Act
        var retVal = sut.ButtonDefaultLuxBased(DeconzEventIdEnum.LongPress, testLightColortemp, 10, 255);

        // Assert
        VerifyAllMocks();
        Assert.True(retVal);
    }

    [Fact]
    public void SetLight_AllwaysWhite_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 0)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddAllwaysWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, null);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_NoMaxWhiteNull_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 5000)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, null);

        // Assert
        VerifyAllMocks();
        //VerifyServiceCallOnTestLight(testLightColortemp, "turn_on", null, 5000);
    }

    [Fact]
    public void SetLight_NoMaxWhite50_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 5000 && x.Brightness == 50)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 50);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_NoMaxWhite0_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.ColorTemp =
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 0);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_MaxWhiteNull_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 5000)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, null);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_MaxWhite50_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 5000)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 50);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_MaxWhite0_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        sut.AddMaxWhiteLight(testLightColortemp);
        testLightColortemp.MinMireds = 0;
        testLightColortemp.MaxMireds = 5000;

        // Act
        sut.SetLight(testLightColortemp, 0);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_MaxWhiteRedToWhite_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 0)));
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
    }

    [Fact]
    public void SetLight_MaxWhiteFromWhiteToRed_VerifyColorTempInServiceCall()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightColortemp.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorTemp != null && (long)x.ColorTemp == 5000)));
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
    }

    [Fact]
    public void SetLight_OnOffLightCurrentOff_lightTurnOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightOnOff.EntityId),
            It.IsAny<LightTurnOnParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 0;

        // Act
        sut.SetLight(testLightOnOff, 1);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_OnOffLightCurrentOn_lightTurnOff()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightOnOff.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightOnOff.Brightness = 100;

        // Act
        sut.SetLight(testLightOnOff, 0);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_BrightnessLightCurrentOff_lightTurnOn()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightBrightness.EntityId),
            It.IsAny<LightTurnOnParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightBrightness.Brightness = 0;

        // Act
        sut.SetLight(testLightBrightness);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_BrightnessLightCurrentOff_lightTurnOnWithBrightness()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightBrightness.EntityId),
            It.Is<LightTurnOnParameters>(x => x.Brightness == 50)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightColortemp.Brightness = 0;

        // Act
        sut.SetLight(testLightBrightness, 50);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_BrightnessLightCurrentOn_lightTurnOff()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightBrightness.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightBrightness.Brightness = 1;

        // Act
        sut.SetLight(testLightBrightness, 0);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLightColor_TurnOn_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightHs.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorName != null && (string)x.ColorName == "red")));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightHs.Brightness = 0;

        // Act
        sut.SetLight(testLightHs, null, "red");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLightColor_TurnOnWithBrightness_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_on",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightHs.EntityId),
            It.Is<LightTurnOnParameters>(x => x.ColorName != null && (string)x.ColorName == "red" && x.Brightness == 50)));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightHs.Brightness = 0;

        // Act
        sut.SetLight(testLightHs, 50, "red");

        // Assert
        VerifyAllMocks();
        //VerifyServiceCallOnTestLight(testLightHs, "turn_on", 50, null, "red");
    }

    [Fact]
    public void SetLightColor_TurnOff_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("Light", "turn_off",
            It.Is<ServiceTarget>(x => x.EntityIds!.Single() == testLightHs.EntityId),
            It.IsAny<LightTurnOffParameters>()));
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);
        testLightHs.Brightness = 0;

        // Act
        sut.SetLight(testLightHs, 0, "red");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLightColor_UnsupportedLight_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);

        // Act
        sut.SetLight(testLightUnsupported, null, "red");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_UnsupportedLight_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);

        // Act
        sut.SetLight(testLightUnsupported, null);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLight_NullLight_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);

        // Act
        sut.SetLight(testLightNull, null);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SetLightColor_NullLight_VerifyCalls()
    {
        // Arrange 
        SetupMocks();
        var sut = new LightControl(serviceProviderMock.Object, luxBasedBrightnessMock.Object, loggerMock.Object);

        // Act
        sut.SetLight(testLightNull, null, "red");

        // Assert
        VerifyAllMocks();
    }
}