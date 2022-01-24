using Xunit;
using NetDaemonImpl.Modules;
using Moq;
using NetDaemonInterface;
using NetDaemon.HassModel.Entities;
using System.Linq;

namespace NetDaemonTest.Modules
{
    public class HouseStateTest : ServiceProviderTestBase
    {
        private readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
        private readonly Mock<ITwinkle> twinkleMock = new(MockBehavior.Strict);
        private readonly Mock<INotify> notifyMock = new(MockBehavior.Strict);
           

        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            SetupMocks();

            // Act
            _ = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object);

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

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object);

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

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object);

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

            var houseState = new HouseState(serviceProviderMock.Object, lightControlMock.Object, twinkleMock.Object, notifyMock.Object);

            // Act
            houseState.HouseStateSleeping();

            // Assert
            VerifyAllMocks();
        }

        private void SetupMocksSleeping()
        {
            haContextMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
                It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId == entities.Sensor.Housestate.EntityId && y.State == "Sleeping")));
            haContextMock.Setup(x => x.CallService("climate", "set_temperature", It.Is<ServiceTarget>(x => x.EntityIds != null &&
                x.EntityIds.Contains(entities.Climate.Keuken.EntityId)), It.Is<ClimateSetTemperatureParameters>(y => y.Temperature == 15)));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SlaapkamerNachtlampGreet.EntityId), 1)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SlaapkamerNachtlampKen.EntityId), 1)).Returns(true);
            haContextMock.Setup(x => x.CallService("vacuum", "set_fan_speed", It.Is<ServiceTarget>(x => x.EntityIds != null &&
                x.EntityIds.Contains(entities.Vacuum.DreameP20294b09RobotCleaner.EntityId)), It.Is<VacuumSetFanSpeedParameters>(y => y.FanSpeed == "Strong")));
            haContextMock.Setup(x => x.CallService("vacuum", "start", It.Is<ServiceTarget>(x => x.EntityIds != null &&
                x.EntityIds.Contains(entities.Vacuum.DreameP20294b09RobotCleaner.EntityId)), null));
            notifyMock.Setup(x => x.NotifyGsm("Pillen", "Vergeet je avondpillen niet :)"));

        }

        private void SetupMocksAwake()
        {
            haContextMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
                            It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId == entities.Sensor.Housestate.EntityId && y.State == "Awake")));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerBoog.EntityId), 125)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKamer.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerBureau.EntityId), 0)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.TabletScreen.EntityId), null)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKisten.EntityId), null)).Returns(false);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightWoonWand.EntityId), 125)).Returns(false);
            //haContextMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.BinnenKerst.EntityId)), null));
            twinkleMock.Setup(x => x.Start());
        }

        private void SetupMocksAway()
        {
            haContextMock.Setup(x => x.CallService("netdaemon", "entity_update", null,
                It.Is<NetdaemonEntityUpdateParameters>(y => y.EntityId == entities.Sensor.Housestate.EntityId && y.State == "Away")));
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BuitenachterSierverlichting.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.SwitchInfinityMirror.EntityId)), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.BuitenachterFontein.EntityId)), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.BuitenachterGrondpomp.EntityId)), null));
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.SwitchVliegenlamp.EntityId)), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightCabine.EntityId), 0)).Returns(true);
            haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.SwitchSierCabine.EntityId)), null));

            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerBoog.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKamer.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerBureau.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.TabletScreen.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKisten.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightHal.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenKeukenlamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WashalWashal.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WcWclamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.BadkamerLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SpeelkamerLamp.EntityId), 0)).Returns(true);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.LightWoonWand.EntityId), 0)).Returns(true);


            //haContextMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.Switch.BinnenKerst.EntityId)), null));
        }
    }
}
