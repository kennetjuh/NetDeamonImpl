using Moq;
using NetDaemonImpl.Modules;
using NetDaemonInterface;
using Xunit;

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
    }
}
