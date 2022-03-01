using Microsoft.Extensions.Logging;
using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl;
using NetDaemonImpl.Extensions;
using NetDaemonImpl.Modules;
using NetDaemonInterface;
using NetDaemonTest.TestLights;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Modules
{
    public class DayNightTest : ServiceProviderTestBase
    {
        private readonly Mock<ISettingsProvider> settingsProvidermock = new(MockBehavior.Strict);
        private readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
        private readonly Mock<ITwinkle> twinkleMock = new(MockBehavior.Strict);
        private readonly Mock<ILuxBasedBrightness> luxBasedBrightnessMock = new(MockBehavior.Strict);
        private readonly Mock<INotify> notifyMock = new(MockBehavior.Strict);
        private readonly Mock<ILogger<DayNight>> loggerMock = new();

        internal override void VerifyAllMocks()
        {
            base.VerifyAllMocks();
            lightControlMock.VerifyAll();
            twinkleMock.VerifyAll();
            notifyMock.VerifyAll();
        }

        internal override void SetupMocks()
        {
            base.SetupMocks();
            lightControlMock.Reset();
            twinkleMock.Reset();
            notifyMock.Reset();
        }

        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            SetupMocks();

            // Act
            _ = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void DayNight_CheckDayNightLightsOn_SunAtNight()
        {
            // Arrange 
            SetupMocks();
            luxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(20);
            haContextMock.SetupSequence(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = DayNightEnum.Day.ToString() })
                .Returns(new EntityState() { State = DayNightEnum.Night.ToString() });
            var sut = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);
            var testsun = new TestSun(haContextMock.Object, "Sun") { Elevation = -1, Rising = false };
            sut.SetSunEntity(testsun);

            setupNightMocks(false, true);

            // Act
            sut.CheckDayNight();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void DayNight_CheckDayNightLightsOn_SunAtDay()
        {
            // Arrange 
            SetupMocks();
            luxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(30);
            haContextMock.SetupSequence(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = DayNightEnum.Night.ToString() })
                .Returns(new EntityState() { State = DayNightEnum.Day.ToString() });
            var sut = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);
            var testsun = new TestSun(haContextMock.Object, "Sun") { Elevation = -1, Rising = true };
            sut.SetSunEntity(testsun);

            setupDayMocks(false, true);

            // Act
            sut.CheckDayNight();

            // Assert
            VerifyAllMocks();
        }


        [Fact]
        public void DayNight_CheckDayNightLightsOff_SunAtNight()
        {
            // Arrange 
            SetupMocks();
            luxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(20);
            haContextMock.SetupSequence(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = DayNightEnum.Day.ToString() })
                .Returns(new EntityState() { State = DayNightEnum.Night.ToString() });
            var sut = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);
            var testsun = new TestSun(haContextMock.Object, "Sun") { Elevation = -1, Rising = false };
            sut.SetSunEntity(testsun);

            setupNightMocks(false, false);

            // Act
            sut.CheckDayNight();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void DayNight_CheckDayNightLightsOff_SunAtDay()
        {
            // Arrange 
            SetupMocks();
            luxBasedBrightnessMock.Setup(x => x.GetLux()).Returns(30);
            haContextMock.SetupSequence(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = DayNightEnum.Night.ToString() })
                .Returns(new EntityState() { State = DayNightEnum.Day.ToString() });
            var sut = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);
            var testsun = new TestSun(haContextMock.Object, "Sun") { Elevation = -1, Rising = true };
            sut.SetSunEntity(testsun);

            setupDayMocks(false, false);

            // Act
            sut.CheckDayNight();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void DayNight_ForceDayNightLightsOff_Night()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = DayNightEnum.Night.ToString() });
            var sut = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);

            setupNightMocks(true, false);

            // Act
            sut.ForceDayNight();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void DayNight_ForceDayNightLightsOff_Day()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = DayNightEnum.Day.ToString() });
            var sut = new DayNight(serviceProviderMock.Object, lightControlMock.Object, loggerMock.Object, settingsProvidermock.Object, luxBasedBrightnessMock.Object);

            setupDayMocks(true, false);

            // Act
            sut.ForceDayNight();

            // Assert
            VerifyAllMocks();
        }



        private void setupDayMocks(bool force, bool lightsOn)
        {
            if (!force)
            {
                haContextMock.Setup(x => x.CallService("input_datetime", "set_datetime", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputDatetime.Daynightlastdaytrigger.EntityId),
                    It.Is<MyInputDatetimeSetDatetimeParameters>(y => y.Time != null)));

                haContextMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputText.Daynight.EntityId),
                    It.Is<InputTextSetValueParameters>(y => y.Value == "Day")));
            }

            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.BuitenvoorGrondspots.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenopritWandlamp.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WandlampBuiten.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SfeerlampHalboven.EntityId), 55)).Returns(true);
            settingsProvidermock.Setup(x => x.BrightnessSfeerlampBovenDay).Returns(55);

            haContextMock.Setup(x => x.GetState(entities.Light.Wandlampen.EntityId)).Returns(new EntityState() { State = lightsOn ? "on" : "off" });
            haContextMock.Setup(x => x.GetState(entities.Light.LightHalSfeer.EntityId)).Returns(new EntityState() { State = lightsOn ? "on" : "off" });
            haContextMock.Setup(x => x.GetState(entities.Light.SfeerlampKeuken.EntityId)).Returns(new EntityState() { State = lightsOn ? "on" : "off" });
            if (lightsOn || force)
            {
                settingsProvidermock.Setup(x => x.BrightnessSfeerlampHalDay).Returns(52);
                settingsProvidermock.Setup(x => x.BrightnessSfeerlampKeukenDay).Returns(53);
                settingsProvidermock.Setup(x => x.BrightnessSfeerlampWoonkamerDay).Returns(56);
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Wandlampen.EntityId), Constants.brightnessWandDay)).Returns(true);
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightHalSfeer.EntityId), 52)).Returns(true);
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SfeerlampKeuken.EntityId), 53)).Returns(true);
            }
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.GrondlampZij.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterFonteinlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WandlampHut.EntityId), 0)).Returns(true);
        }


        private void setupNightMocks(bool force, bool lightsOn)
        {
            if (!force)
            {
                haContextMock.Setup(x => x.CallService("input_datetime", "set_datetime", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputDatetime.Daynightlastnighttrigger.EntityId),
                            It.Is<MyInputDatetimeSetDatetimeParameters>(y => y.Time != null)));

                haContextMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputText.Daynight.EntityId),
                    It.Is<InputTextSetValueParameters>(y => y.Value == "Night")));
            }

            haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.BuitenvoorGrondspots.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenopritWandlamp.EntityId), 50)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WandlampBuiten.EntityId), 50)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SfeerlampHalboven.EntityId), 15)).Returns(true);
            settingsProvidermock.Setup(x => x.BrightnessSfeerlampBovenNight).Returns(15);

            haContextMock.Setup(x => x.GetState(entities.Light.Wandlampen.EntityId)).Returns(new EntityState() { State = lightsOn ? "on" : "off" });
            haContextMock.Setup(x => x.GetState(entities.Light.LightHalSfeer.EntityId)).Returns(new EntityState() { State = lightsOn ? "on" : "off" });
            haContextMock.Setup(x => x.GetState(entities.Light.SfeerlampKeuken.EntityId)).Returns(new EntityState() { State = lightsOn ? "on" : "off" });
            if (lightsOn || force)
            {
                settingsProvidermock.Setup(x => x.BrightnessSfeerlampHalNight).Returns(12);
                settingsProvidermock.Setup(x => x.BrightnessSfeerlampKeukenNight).Returns(13);
                settingsProvidermock.Setup(x => x.BrightnessSfeerlampWoonkamerNight).Returns(16);
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Wandlampen.EntityId), Constants.brightnessWandNight)).Returns(true);
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightHalSfeer.EntityId), 12)).Returns(true);
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SfeerlampKeuken.EntityId), 13)).Returns(true);
            }
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.GrondlampZij.EntityId), Constants.brightnessBuitenZij)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterFonteinlamp.EntityId), Constants.brightnessFontein)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WandlampHut.EntityId), Constants.brightnessHutWand)).Returns(true);
        }
    }
}

