using Xunit;
using NetDaemonImpl.Modules;
using NetDaemon.HassModel.Entities;
using Moq;
using NetDaemonInterface;
using System.Threading.Tasks;

namespace NetDaemonTest.Modules
{
    public class TwinkleTest : ServiceProviderTestBase
    {
        private readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
        private readonly Mock<IDelayProvider> delayProviderMock = new(MockBehavior.Strict);

        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(10));

            // Act
            _ = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object);

            // Assert
        }

        [Fact]
        public void IsActive_TwinkleNotActive_VerifyResult()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(10));
            var sut = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object);

            // Act
            var result = sut.IsActive();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsActive_TwinkleActive_VerifyResult()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(10));
            var sut = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object);

            // Act
            sut.Start();
            var result = sut.IsActive();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Start_DoubleStart_VerifyResult()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(10));
            var sut = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object);

            // Act
            sut.Start();
            sut.Start();
            var result = sut.IsActive();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Stop_StartFirst_VerifyResult()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(10));
            var sut = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object);

            // Act
            sut.Start();
            sut.Stop();
            var result = sut.IsActive();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwinkleImpl_1CycleDay_VerifyResult()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(1000));
            var sut = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object, 1);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM1.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM2.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM3.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM4.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM5.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS1.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS2.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS3.EntityId), 1)).Returns(null);
            haContextMock.Setup(x => x.GetState(entities.Sensor.Daynight.EntityId)).Returns(new EntityState() { State = "Day" });
            //lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM1.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM2.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM3.EntityId), 0)).Returns(null);
            //lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM4.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM5.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS1.EntityId), 0)).Returns(null);
            //lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS2.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS3.EntityId), 0)).Returns(null);


            // Act
            sut.Start();
            Task.Delay(TimeSpan.FromMilliseconds(200)).Wait();
            sut.Stop();

            // Assert
            VerifyAllMocks();
            lightControlMock.VerifyAll();
            delayProviderMock.VerifyAll();
        }

        [Fact]
        public void TwinkleImpl_1CycleNight_VerifyResult()
        {
            // Arrange 
            delayProviderMock.Setup(x => x.TwinkleDelay).Returns(TimeSpan.FromMilliseconds(1000));
            var sut = new Twinkle(serviceProviderMock.Object, lightControlMock.Object, delayProviderMock.Object, 1);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM1.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM2.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM3.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM4.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM5.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS1.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS2.EntityId), 1)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS3.EntityId), 1)).Returns(null);
            haContextMock.Setup(x => x.GetState(entities.Sensor.Daynight.EntityId)).Returns(new EntityState() { State = "Night" });
            //lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM1.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM2.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM3.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM4.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenM5.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS1.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS2.EntityId), 0)).Returns(null);
            lightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == entities.Light.KeukenS3.EntityId), 0)).Returns(null);


            // Act
            sut.Start();
            Task.Delay(TimeSpan.FromMilliseconds(200)).Wait();
            sut.Stop();

            // Assert
            VerifyAllMocks();
            lightControlMock.VerifyAll();
            delayProviderMock.VerifyAll();
        }
    }
}
