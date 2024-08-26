using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;
using System.Linq;
using NetDaemonImpl.Extensions;

namespace NetDaemonTest.Apps;

public class DayNightHandlerAppTest : TestBase
{
    [Fact]
    public void DayNightHandlerApp_Constructor_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        LuxBasedBrightnessMock.Setup(x=>x.GetLux()).Returns(1);
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DayNightHandlerApp_HousestateChanges_VerifyEvents()
    {
        // Arrange        
        LuxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(50);
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Night");
        setupDayMocks();

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();
        HaMock.TriggerStateChange(Entities.Sun.Sun, "", new SunAttributes
        {
            Elevation = 5,
            Rising = true
        });
        
        HaMock.TriggerStateChange(Entities.InputText.Housestate, "away");
        
        // Assert
        VerifyAllMocks();
    }


    [Fact]
    public void DayNightHandlerApp_ConstructorDay_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        LuxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(50);
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Night");
        HaMock.TriggerStateChange(Entities.Sun.Sun, "", new SunAttributes
        {
            Elevation = 5,
            Rising = true
        });
        setupDayMocks();

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    private void setupDayMocks()
    {
        HaMock.Setup(x => x.CallService("input_datetime", "set_datetime", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.InputDatetime.Daynightlastdaytrigger.EntityId),
            It.Is<MyInputDatetimeSetDatetimeParameters>(y=>y.Time!=null)));

        HaMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.InputText.Daynight.EntityId),
            It.Is<InputTextSetValueParameters>(y=>y.Value=="Day")));

        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BuitenvoorGrondspots.EntityId), null));

        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.BuitenopritWandlamp.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.WandlampBuiten.EntityId), 0)).Returns(false);
        HaMock.TriggerStateChange(Entities.Light.Wandlampen, "on");
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.Wandlampen.EntityId), Constants.brightnessWandDay)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.BuitenzijHutsier.EntityId), 0)).Returns(false);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampSpeelkamerDay).Returns(51);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampHalDay).Returns(52);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampKeukenDay).Returns(53);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampWoonkamer1Day).Returns(54);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampBovenDay).Returns(55);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.LightSpeelkamerSfeer.EntityId), 51)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampHal.EntityId), 52)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKeuken.EntityId), 53)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKamer1.EntityId), 54)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampBoven.EntityId), 55)).Returns(true);
    }

    [Fact]
    public void DayNightHandlerApp_ConstructorNight_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        LuxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(10);
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.InputDatetime.Daynightlastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");
        HaMock.TriggerStateChange(Entities.Sun.Sun, "", new SunAttributes
        {
            Elevation = -1,
            Rising = false
        });     

        HaMock.Setup(x => x.CallService("input_datetime", "set_datetime", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.InputDatetime.Daynightlastnighttrigger.EntityId),
            It.Is<MyInputDatetimeSetDatetimeParameters>(y => y.Time != null)));

        HaMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.InputText.Daynight.EntityId),
            It.Is<InputTextSetValueParameters>(y => y.Value == "Night")));

        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BuitenvoorGrondspots.EntityId), null));


        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.BuitenopritWandlamp.EntityId), 50)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.WandlampBuiten.EntityId), 50)).Returns(true);
        HaMock.TriggerStateChange(Entities.Light.Wandlampen, "on");
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.Wandlampen.EntityId), Constants.brightnessWandNight)).Returns(true);        
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampSpeelkamerNight).Returns(11);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampHalNight).Returns(12);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampKeukenNight).Returns(13);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampWoonkamer1Night).Returns(14);
        SettingsProviderMock.Setup(x => x.BrightnessSfeerlampBovenNight).Returns(15);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.LightSpeelkamerSfeer.EntityId), 11)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampHal.EntityId), 12)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKeuken.EntityId), 13)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKamer1.EntityId), 14)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampBoven.EntityId), 15)).Returns(true);

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }
}