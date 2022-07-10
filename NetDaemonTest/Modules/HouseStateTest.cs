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
            _ = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateAwake_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAwake();
            haContextMock.Setup(x => x.GetState(entities.Sensor.Daynight.EntityId)).Returns(new EntityState() { State = "Day" });

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object);

            // Act
            houseState.HouseStateAwake();

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void HouseState_HouseStateAway_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAway();

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object);

            // Act
            houseState.HouseStateAway();

            // Assert
            VerifyAllMocks();
        }


        [Fact]
        public void HouseState_HouseStateSleeping_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            SetupMocksAway();
            SetupMocksSleeping();

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object, loggerMock.Object);

            // Act
            houseState.HouseStateSleeping();

            // Assert
            VerifyAllMocks();
        }

        private void SetupMocksSleeping()
        {
            haContextMock.Setup(x => x.GetState(entities.DeviceTracker.GsmGreet.EntityId)).Returns(new EntityState() { State = "home"});
            haContextMock.Setup(x => x.GetState(entities.DeviceTracker.GsmKen.EntityId)).Returns(new EntityState() { State = "Away" });
            haContextMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
                It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == entities.Sensor.Housestate.EntityId && y.State!.ToString() == "Sleeping")));
            haContextMock.Setup(x => x.CallService("climate", "set_temperature", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Climate.Keuken.EntityId),
                It.Is<ClimateSetTemperatureParameters>(y => y.Temperature == 15)));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.NachtlampGreet.EntityId), 1)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.NachtlampKen.EntityId), 1)).Returns(true);
            haContextMock.Setup(x => x.CallService("vacuum", "set_fan_speed", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Vacuum.DreameP20294b09RobotCleaner.EntityId),
                It.Is<VacuumSetFanSpeedParameters>(y => y.FanSpeed == "Strong")));
            haContextMock.Setup(x => x.CallService("vacuum", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Vacuum.DreameP20294b09RobotCleaner.EntityId), null));
            notifyMock.Setup(x => x.NotifyGsmGreet("Pillen", "Vergeet je avondpillen niet :)", It.IsAny<NotifyTagEnum?>(), It.IsAny<List<NotifyActionEnum>?>()));
        }

        private void SetupMocksAwake()
        {
            haContextMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
                            It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == entities.Sensor.Housestate.EntityId && y.State!.ToString() == "Awake")));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Booglamp.EntityId), 125)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Kamerlamp.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Bureaulamp.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BdfM107Screen.EntityId), null)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKisten.EntityId), null)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerValentijn.EntityId), null)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightWoonWand.EntityId), 50)).Returns(false);
            twinkleMock.Setup(x => x.Start());
        }

        private void SetupMocksAway()
        {
            haContextMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
                It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId!.ToString() == entities.Sensor.Housestate.EntityId && y.State!.ToString() == "Away")));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterSierverlichting.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchInfinityMirror.EntityId), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchFontein.EntityId), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.BuitenachterGrondpomp.EntityId), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchVliegenlamp.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightCabine.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.Switch.SwitchSierCabine.EntityId), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Booglamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Kamerlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Bureaulamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BdfM107Screen.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKisten.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerValentijn.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightHal.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.Washal.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WcWclamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BadkamerLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SpeelkamerLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightWoonWand.EntityId), 0)).Returns(true);
        }
    }
}
