using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.Modules.Notify;
using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
                It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.MediaPlayer.Hal.EntityId),
                It.Is<MediaPlayerVolumeSetParameters>(y => y.VolumeLevel == 1)));
            haContextMock.Setup(x => x.CallService("media_player", "volume_set",
                It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == entities.MediaPlayer.Woonkamer.EntityId),
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

        [Fact]
        public void NotifyClear_Notify_VerifyMocks()
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.Is<NotifyMobileAppGsmKenParameters>(y =>
                    y.Title == "" &&
                    y.Message == "clear_notification")));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.Clear(NotifyTagEnum.ThermostatChanged);

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [MemberData(nameof(NotifyTagEnumValues))]
        public void Notify_NotifyWithTags_VerifyMocks(NotifyTagEnum tag)
        {
            // Arrange 
            NotifyMobileAppGsmKenParameters parameters= new NotifyMobileAppGsmKenParameters();
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.IsAny<NotifyMobileAppGsmKenParameters>()))
                .Callback<string,string,ServiceTarget,object>((a, b, c, d) => parameters = (NotifyMobileAppGsmKenParameters)d);

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKen("","",tag, null);

            // Assert
            AssertRecordNotifyData(parameters.Data as RecordNotifyData, null, tag, null);
            VerifyAllMocks();
        }

        [Theory]
        [MemberData(nameof(NotifyActionEnumValues))]
        public void Notify_NotifyWithAction_VerifyMocks(NotifyActionEnum action)
        {
            // Arrange 
            NotifyMobileAppGsmKenParameters parameters = new NotifyMobileAppGsmKenParameters();
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_gsm_ken", null,
                It.IsAny<NotifyMobileAppGsmKenParameters>()))
                .Callback<string, string, ServiceTarget, object>((a, b, c, d) => parameters = (NotifyMobileAppGsmKenParameters)d);

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKen("", "", null, new() { action });

            // Assert
            AssertRecordNotifyData(parameters.Data as RecordNotifyData, null, null, action);
            VerifyAllMocks();
        }

        [Theory]
        [MemberData(nameof(NotifyActionEnumValues))]
        public void Notify_HandleNotificationEvent_VerifyMocks(NotifyActionEnum action)
        {
            // Arrange 
            NotifyMobileAppGsmKenParameters parameters = new NotifyMobileAppGsmKenParameters();
            SetupMocks();    
            if(!action.ToString().StartsWith("Uri"))
            {
                //just a general setup, each action except Uri's will call a service
                haContextMock.Setup(x => x.CallService(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ServiceTarget>(), It.IsAny<object?>()));
            }

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.HandleNotificationEvent(action);

            // Assert
            VerifyAllMocks();
        }

        internal static IEnumerable<object[]> NotifyActionEnumValues()
        {
            foreach (var value in Enum.GetValues(typeof(NotifyActionEnum)))
            {
                yield return new object[] { value };
            }
        }

        internal static IEnumerable<object[]> NotifyTagEnumValues()
        {
            foreach (var value in Enum.GetValues(typeof(NotifyTagEnum)))
            {
                yield return new object[] { value };
            }
        }

        private void AssertRecordNotifyData(RecordNotifyData? data, string? channel, NotifyTagEnum? tag, NotifyActionEnum? action)
        {
            Assert.NotNull(data);
            if (data != null)
            {
                Assert.Equal("", data.color);
                Assert.Equal("high", data.priority);
                Assert.Equal("true", data.sticky);
                Assert.Equal(0, data.ttl);

                Assert.Equal(tag?.ToString(), data.tag);
                Assert.Equal(channel, data.channel);

                if (action != null)
                {
                    var actionString = action.ToString()!;
                    if (actionString.StartsWith("Uri"))
                    {
                        actionString = "URI";
                    }
                    Assert.Equal(actionString, data.actions!.Single().action);
                }
                else
                {
                    Assert.Null(data.actions);
                }
            }
        }
    }
}
