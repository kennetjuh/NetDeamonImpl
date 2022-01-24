using Moq;
using Xunit;
using NetDaemonImpl.Modules;
using NetDaemon.HassModel.Entities;
using System.Linq;

namespace NetDaemonTest.Modules
{
    public class NotifyTest : ServiceProviderTestBase
    {
        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 
            SetupMocks();

            // Act
            _ = new Notify(serviceProviderMock.Object);

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [InlineData("title", "message")]
        public void NotifyGsmKen_Notify_VerifyMocks(string title, string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.Is<NotifyMobileAppGsmKenParameters>(y =>
                    y.Title == title &&
                    y.Message == message)));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKen(title, message);

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [InlineData("title", "message")]
        public void NotifyGsmGreet_Notify_VerifyMocks(string title, string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_greet", null,
                It.Is<NotifyMobileAppGsmGreetParameters>(y =>
                    y.Title == title &&
                    y.Message == message)));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmGreet(title, message);

            // Assert
            VerifyAllMocks();
        }


        [Theory]
        [InlineData("message")]
        public void NotifyGsmKenTTS_Notify_VerifyMocks(string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.Is<NotifyMobileAppGsmKenParameters>(y =>
                    y.Title == message &&
                    y.Message == "TTS")));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKenTTS(message);

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [InlineData("message")]
        public void NotifyGsmGreetTTS_Notify_VerifyMocks(string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_greet", null,
                It.Is<NotifyMobileAppGsmGreetParameters>(y =>
                    y.Title == message &&
                    y.Message == "TTS")));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmGreetTTS(message);

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [InlineData("title", "message")]
        public void NotifyGsm_Notify_VerifyMocks(string title, string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_greet", null,
                It.Is<NotifyMobileAppGsmGreetParameters>(y =>
                    y.Title == title &&
                    y.Message == message)));
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.Is<NotifyMobileAppGsmKenParameters>(y =>
                    y.Title == title &&
                    y.Message == message)));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsm(title, message);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void NotifyGsmAlarm_Notify_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_greet", null,
                It.Is<NotifyMobileAppGsmGreetParameters>(y =>
                    y.Title != null &&
                    y.Title.Contains("alarm") &&
                    y.Message == "TTS")));
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.Is<NotifyMobileAppGsmKenParameters>(y =>
                    y.Title != null &&
                    y.Title.Contains("alarm") &&
                    y.Message == "TTS")));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmAlarm();

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [InlineData("message")]
        public void NotifyHouse_Notify_VerifyMocks(string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("media_player", "volume_set",
                It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.MediaPlayer.Hal.EntityId)),
                It.Is<MediaPlayerVolumeSetParameters>(y => y.VolumeLevel == 1)));
            haContextMock.Setup(x => x.CallService("media_player", "volume_set",
                It.Is<ServiceTarget>(x => x.EntityIds != null && x.EntityIds.Contains(entities.MediaPlayer.Woonkamer.EntityId)),
                It.Is<MediaPlayerVolumeSetParameters>(y => y.VolumeLevel == 1)));
            haContextMock.Setup(x => x.CallService("tts", "google_translate_say", null,
                It.Is<TtsGoogleTranslateSayParameters>(y => y.EntityId == entities.MediaPlayer.Hal.EntityId && y.Message == message)));
            haContextMock.Setup(x => x.CallService("tts", "google_translate_say", null,
                It.Is<TtsGoogleTranslateSayParameters>(y => y.EntityId == entities.MediaPlayer.Woonkamer.EntityId && y.Message == message)));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyHouse(message);

            // Assert
            VerifyAllMocks();
        }
    }
}
