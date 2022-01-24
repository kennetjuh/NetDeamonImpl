using Xunit;
using NetDaemonImpl.Modules;
using Moq;
using NetDaemonInterface;
using NetDaemonImpl.AreaControl.Areas;

namespace NetDaemonTest.Modules
{
    public class AreaCollectionTest : ServiceProviderTestBase
    {
        private readonly Mock<IDelayProvider> delayProviderMock = new(MockBehavior.Strict);
        private readonly Mock<IHouseState> houseStateMock = new(MockBehavior.Strict);
        private readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
        private readonly Mock<ITwinkle> twinkleMock = new(MockBehavior.Strict);

        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            SetupMocks();
            lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SpeelkamerLamp.EntityId)));
            lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerBureau.EntityId)));
            lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKamer.EntityId)));
            delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.Zero);

            // Act
            _ = new AreaCollection(serviceProviderMock.Object, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object, twinkleMock.Object);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void GetAreas_VerifyType()
        {
            // Arrange 
            SetupMocks();
            lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.SpeelkamerLamp.EntityId)));
            lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerBureau.EntityId)));
            lightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.WoonkamerKamer.EntityId)));
            delayProviderMock.Setup(x => x.ModeCycleTimeout).Returns(TimeSpan.Zero);

            // Act
            var areas = new AreaCollection(serviceProviderMock.Object, delayProviderMock.Object, lightControlMock.Object, houseStateMock.Object, twinkleMock.Object);

            // Assert
            Assert.Equal(typeof(AreaControlWashal), areas.Washal.GetType());
            Assert.Equal(typeof(AreaControlHal), areas.Hal.GetType());
            Assert.Equal(typeof(AreaControlBadkamer), areas.Badkamer.GetType());
            Assert.Equal(typeof(AreaControlTraphal), areas.Traphal.GetType());
            Assert.Equal(typeof(AreaControlSpeelkamer), areas.Speelkamer.GetType());
            Assert.Equal(typeof(AreaControlWc), areas.Wc.GetType());
            Assert.Equal(typeof(AreaControlVoordeur), areas.Voordeur.GetType());
            Assert.Equal(typeof(AreaControlSlaapkamerKids), areas.SlaapkamerKids.GetType());
            Assert.Equal(typeof(AreaControlCabine), areas.Cabine.GetType());
            Assert.Equal(typeof(AreaControlHalBoven), areas.HalBoven.GetType());
            Assert.Equal(typeof(AreaControlWoonkamer), areas.Woonkamer.GetType());
            Assert.Equal(typeof(AreaControlKeuken), areas.Keuken.GetType());
            Assert.Equal(typeof(AreaControlSlaapkamer), areas.Slaapkamer.GetType());
            Assert.Equal(typeof(AreaControlBuitenAchter), areas.BuitenAchter.GetType());           
            VerifyAllMocks();
        }

    }
}
