using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using Xunit;
using System.Linq;

namespace NetDaemonTest.Apps;

public class DayNightHandlerAppTest : TestBase
{
    [Fact]
    public void DayNightHandlerApp_Constructor_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        LuxBasedBrightnessMock.Setup(x=>x.GetLux()).Returns(1);
        HaMock.TriggerStateChange(Entities.Sensor.DaynightLastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.Sensor.DaynightLastnighttrigger, "18:00:00");

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DayNightHandlerApp_ConstructorDay_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        LuxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(50);
        HaMock.TriggerStateChange(Entities.Sensor.DaynightLastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.Sensor.DaynightLastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Night");
        HaMock.TriggerStateChange(Entities.Sun.Sun, "", new SunAttributes
        {
            Elevation = 5,
            Rising=true
        });
        HaMock.Setup(x => x.CallService("netdaemon", "entity_update", null, 
            It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == Entities.Sensor.DaynightLastdaytrigger.EntityId)));
        HaMock.Setup(x => x.CallService("netdaemon", "entity_update", null, 
            It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == Entities.Sensor.Daynight.EntityId &&
                                                        y.State!.ToString()=="Day")));

        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BuitenvoorGrondspots.EntityId), null));

        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.BuitenopritWandlamp.EntityId),0)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.WandlampBuiten.EntityId),0)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.SfeerlampKamer1.EntityId),50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.SfeerlampKamer2.EntityId),50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.SfeerlampKeuken.EntityId),50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.SfeerlampHal.EntityId),50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.SfeerlampBoven.EntityId),50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.LightSpeelkamerSfeer.EntityId),50)).Returns(null);
        HaMock.TriggerStateChange(Entities.Light.LightWoonWand, "on");
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.LightWoonWand.EntityId), Constants.brightnessWandDay)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.BuitenzijHutsier.EntityId), 0)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y=>y.EntityId == Entities.Light.LightHut.EntityId), 0)).Returns(null);

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DayNightHandlerApp_ConstructorNight_VerifyEvents()
    {
        // Arrange
        ResetAllMocks();
        LuxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(10);
        HaMock.TriggerStateChange(Entities.Sensor.DaynightLastdaytrigger, "08:00:00");
        HaMock.TriggerStateChange(Entities.Sensor.DaynightLastnighttrigger, "18:00:00");
        HaMock.TriggerStateChange(Entities.Sensor.Daynight, "Day");
        HaMock.TriggerStateChange(Entities.Sun.Sun, "", new SunAttributes
        {
            Elevation = -1,
            Rising = false
        });
        HaMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
            It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == Entities.Sensor.DaynightLastnighttrigger.EntityId)));
        HaMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
            It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == Entities.Sensor.Daynight.EntityId &&
                                                        y.State!.ToString() == "Night")));

        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BuitenvoorGrondspots.EntityId), null));


        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.BuitenopritWandlamp.EntityId), 50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.WandlampBuiten.EntityId), 50)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKamer1.EntityId), 1)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKamer2.EntityId), 1)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampKeuken.EntityId), 1)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampHal.EntityId), 1)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.SfeerlampBoven.EntityId), 1)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.LightSpeelkamerSfeer.EntityId), 1)).Returns(null);
        HaMock.TriggerStateChange(Entities.Light.LightWoonWand, "on");
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.LightWoonWand.EntityId), Constants.brightnessWandNight)).Returns(null);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.LightHut.EntityId), Constants.brightnessHut)).Returns(null);

        // Act
        var app = Context.GetApp<DayNightHandlerApp>();

        // Assert
        VerifyAllMocks();
    }
}