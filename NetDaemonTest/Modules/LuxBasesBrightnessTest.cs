using NetDaemon.HassModel.Entities;
using NetDaemonImpl.Modules;
using Xunit;

namespace NetDaemonTest.Modules
{
    public class LuxBasesBrightnessTest : ServiceProviderTestBase
    {
        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 

            // Act
            _ = new LuxBasedBrightness(serviceProviderMock.Object);

            // Assert
        }

        [Fact]
        public void GetLux_StateNull_VerifyReturn()
        {
            // Arrange 
            haContextMock.Setup(x => x.GetState(entities.Sensor.LightSensor.EntityId)).Returns(new EntityState() { State = null });

            // Act
            var sut = new LuxBasedBrightness(serviceProviderMock.Object);
            var lux = sut.GetLux();

            // Assert
            Assert.Equal(1, lux);
        }

        [Fact]
        public void GetLux_StateOk_VerifyReturn()
        {
            // Arrange 
            haContextMock.Setup(x => x.GetState(entities.Sensor.LightSensor.EntityId)).Returns(new EntityState() { State = "10" });

            // Act
            var sut = new LuxBasedBrightness(serviceProviderMock.Object);
            var lux = sut.GetLux();

            // Assert
            Assert.Equal(10, lux);
        }

        [Theory]
        [InlineData(1, 100, 0, 1)]
        [InlineData(1, 100, 30, 17.5)]
        [InlineData(1, 100, 80, 34)]
        [InlineData(1, 100, 150, 50.5)]
        [InlineData(1, 100, 250, 67)]
        [InlineData(1, 100, 750, 83.5)]
        [InlineData(1, 100, 1500, 100)]
        public void GetBrightness_VariableLuxAndMinAndMax_VerifyResult(int min, int max, int lux, double expected)
        {
            // Arrange 
            haContextMock.Setup(x => x.GetState(entities.Sensor.LightSensor.EntityId)).Returns(new EntityState() { State = lux.ToString() });

            // Act
            var sut = new LuxBasedBrightness(serviceProviderMock.Object);
            var actual = sut.GetBrightness(min, max);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetBrightness_LuxNull_VerifyResult()
        {
            // Arrange 
            haContextMock.Setup(x => x.GetState(entities.Sensor.LightSensor.EntityId)).Returns(new EntityState() { State = null });

            // Act
            var sut = new LuxBasedBrightness(serviceProviderMock.Object);
            var actual = sut.GetBrightness(1, 100);

            // Assert
            Assert.Equal(100, actual);
        }
    }
}
