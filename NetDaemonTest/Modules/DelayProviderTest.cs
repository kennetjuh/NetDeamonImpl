using NetDaemonImpl.Modules;
using Xunit;

namespace NetDaemonTest.Modules
{
    public class DelayProviderTest
    {
        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 

            // Act
            _ = new DelayProvider();

            // Assert
        }

        [Fact]
        public void GetDelays_Verifydelay()
        {
            // Arrange 

            // Act
            var delayProvider = new DelayProvider();

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(1), delayProvider.MotionClear);
            Assert.Equal(TimeSpan.FromMinutes(10), delayProvider.MotionClearManual);
            Assert.Equal(TimeSpan.FromSeconds(5), delayProvider.ManualOffTimeout);
            Assert.Equal(TimeSpan.FromSeconds(1), delayProvider.MotionOnSequenceDelay);
            Assert.Equal(TimeSpan.FromSeconds(5), delayProvider.ModeCycleTimeout);
        }
    }
}
