using Microsoft.Extensions.Logging;
using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.Modules;
using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Modules
{
    public class HouseStateTest : ServiceProviderTestBase
    {
        private readonly Mock<IDayNight> dayNightMock = new(MockBehavior.Strict);
        private readonly Mock<ISettingsProvider> settingsProvidermock = new(MockBehavior.Strict);
        private readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
        private readonly Mock<ITwinkle> twinkleMock = new(MockBehavior.Strict);
        private readonly Mock<INotify> notifyMock = new(MockBehavior.Strict);
        private readonly Mock<ILogger<HouseState>> loggerMock = new();

        internal override void VerifyAllMocks()
        {
            base.VerifyAllMocks();
            lightControlMock.VerifyAll();
            twinkleMock.VerifyAll();
            notifyMock.VerifyAll();
            dayNightMock.VerifyAll();
        }

        internal override void SetupMocks()
        {
            base.SetupMocks();
            lightControlMock.Reset();
            twinkleMock.Reset();
            notifyMock.Reset();
            dayNightMock.Reset();
        }

        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            SetupMocks();

            // Act
            _ = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateAwake_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAwake();
            haContextMock.Setup(x => x.GetState(entities.InputText.Daynight.EntityId)).Returns(new EntityState() { State = "Day" });
            dayNightMock.Setup(x => x.ForceDayNight());

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Act
            houseState.HouseStateAwake();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_TvMode_VerifyMocks()
        {
            // Arrange 
            SetupMocks();

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterLamp.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchFontein.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SfeerlampKeuken.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterFonteinlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Booglamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Kamerlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Bureaulamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Wandlampen.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterHangstoel.EntityId), 0)).Returns(true);

            haContextMock.Setup(x => x.CallService("input_boolean", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputBoolean.WatchdogBuiten.EntityId), null));

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Act
            houseState.TvMode();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateAway_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAway(HouseStateEnum.Away);
            SetupMocksJimmie(false);
            dayNightMock.Setup(x => x.CheckDayNight());

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Act
            houseState.HouseStateAway(HouseStateEnum.Away);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateAway_NoJimmie_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAway(HouseStateEnum.Away);
            SetupMocksJimmie(true);
            dayNightMock.Setup(x => x.CheckDayNight());

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Act
            houseState.HouseStateAway(HouseStateEnum.Away);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateSleeping_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAway(HouseStateEnum.Sleeping);
            SetupMocksSleeping();
            SetupMocksJimmie(true);
            dayNightMock.Setup(x => x.CheckDayNight());

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Act
            houseState.HouseStateSleeping();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateHoliday_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAway(HouseStateEnum.Away);
            SetupMocksJimmie(true);
            haContextMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputText.Housestate.EntityId),
                It.Is<InputTextSetValueParameters>(y => y.Value == "Holiday")));
            dayNightMock.Setup(x => x.CheckDayNight());

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object, settingsProvidermock.Object, dayNightMock.Object);

            // Act
            houseState.HouseStateHoliday();

            // Assert
            VerifyAllMocks();
        }

        private void SetupMocksSleeping()
        {         
            haContextMock.Setup(x => x.CallService("climate", "set_temperature", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Climate.Keuken.EntityId),
                It.Is<ClimateSetTemperatureParameters>(y => y.Temperature == 19)));
            //lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.NachtlampGreet.EntityId), 1)).Returns(true);
            //lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
            haContextMock.Setup(x => x.CallService("vacuum", "set_fan_speed", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Vacuum.DreameP20294b09RobotCleaner.EntityId),
                It.Is<VacuumSetFanSpeedParameters>(y => y.FanSpeed == "Strong")));
            haContextMock.Setup(x => x.CallService("vacuum", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Vacuum.DreameP20294b09RobotCleaner.EntityId), null));
        }

        private void SetupMocksAwake()
        {
            haContextMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputText.Housestate.EntityId),
                It.Is<InputTextSetValueParameters>(y => y.Value == "Awake")));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Booglamp.EntityId), 125)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Kamerlamp.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Bureaulamp.EntityId), 0)).Returns(false);
            haContextMock.Setup(x => x.CallService("input_boolean", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputBoolean.WatchdogBuiten.EntityId), null));

            twinkleMock.Setup(x => x.Start());

            //Kerst
            //haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.Binnen1.EntityId), null));
            //haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.Binnen2.EntityId), null));

        }

        private void SetupMocksAway(HouseStateEnum state)
        {
            haContextMock.Setup(x => x.CallService("input_text", "set_value", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputText.Housestate.EntityId),
                It.Is<InputTextSetValueParameters>(y => y.Value == state.ToString())));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterLamp.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.BuitenachterGrondpomp.EntityId), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchFontein.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Cabine.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Cabineplafond.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("input_boolean", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputBoolean.Cabinethermostat.EntityId), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchSierCabine.EntityId), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.CabineHeater.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Booglamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Kamerlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Bureaulamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Hal.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Washal.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WcWclamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Badkamer.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SpeelkamerLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Wandlampen.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightHut.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterHangstoel.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Halboven.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Slaapkamer.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SfeerlampKeuken.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightHalSfeer.EntityId), 0)).Returns(true);
            if (state != HouseStateEnum.Sleeping)
            {
                lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SlaapkamerNachtlampen.EntityId), 0)).Returns(true);
            }
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Slaapkamerkids.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("input_boolean", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.InputBoolean.WatchdogBuiten.EntityId), null));


            // Kerst
            //haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.Binnen1.EntityId), null));
            //haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.Binnen2.EntityId), null));
        }

        private void SetupMocksJimmie(bool enabled)
        {
            settingsProvidermock.Setup(x => x.JimmieAlarm).Returns(enabled);
            if (enabled)
            {
                haContextMock.Setup(x => x.GetState(entities.DeviceTracker.GsmGreet.EntityId)).Returns(new EntityState() { State = "home" });
                haContextMock.Setup(x => x.GetState(entities.DeviceTracker.GsmKen.EntityId)).Returns(new EntityState() { State = "home" });
                notifyMock.Setup(x => x.NotifyGsmGreet("Jimmie", "Zit ie in de bench?", NotifyPriorityEnum.high, It.IsAny<NotifyTagEnum?>(), It.IsAny<List<NotifyActionEnum>?>()));
                notifyMock.Setup(x => x.NotifyGsmKen("Jimmie", "Zit ie in de bench?", NotifyPriorityEnum.high, It.IsAny<NotifyTagEnum?>(), It.IsAny<List<NotifyActionEnum>?>()));
            }
        }
    }
}
