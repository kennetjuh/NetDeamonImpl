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
    public class SettingsProviderTest : ServiceProviderTestBase
    { 
        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            SetupMocks();

            // Act
            _ = new SettingsProvider(serviceProviderMock.Object);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void SettingsProvider_BrightnessSfeerlampSpeelkamerDay_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.GetState(entities.InputNumber.Brightnesssfeerlampspeelkamerday.EntityId)).Returns(new EntityState() { State = "10" });

            var settingsProvider = new SettingsProvider(serviceProviderMock.Object);

            // Act
            var result = settingsProvider.BrightnessSfeerlampSpeelkamerDay;

            // Assert
            Assert.Equal(10, result);
            VerifyAllMocks();
        }

        [Fact]
        public void SettingsProvider_BrightnessSfeerlampSpeelkamerNight_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.GetState(entities.InputNumber.Brightnesssfeerlampspeelkamernight.EntityId)).Returns(new EntityState() { State = "10" });

            var settingsProvider = new SettingsProvider(serviceProviderMock.Object);

            // Act
            var result = settingsProvider.BrightnessSfeerlampSpeelkamerNight;

            // Assert
            Assert.Equal(10, result);
            VerifyAllMocks();
        }

        [Fact]
        public void SettingsProvider_BrightnessSfeerlampHalDay_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.GetState(entities.InputNumber.Brightnesssfeerlamphalday.EntityId)).Returns(new EntityState() { State = "10" });

            var settingsProvider = new SettingsProvider(serviceProviderMock.Object);

            // Act
            var result = settingsProvider.BrightnessSfeerlampHalDay;

            // Assert
            Assert.Equal(10, result);
            VerifyAllMocks();
        }

        [Fact]
        public void SettingsProvider_BrightnessSfeerlampHalNight_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.GetState(entities.InputNumber.Brightnesssfeerlamphalnight.EntityId)).Returns(new EntityState() { State = "10" });

            var settingsProvider = new SettingsProvider(serviceProviderMock.Object);

            // Act
            var result = settingsProvider.BrightnessSfeerlampHalNight;

            // Assert
            Assert.Equal(10, result);
            VerifyAllMocks();
        }       
    }
}
