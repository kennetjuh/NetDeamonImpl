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
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.Is<NotifyMobileAppA53Parameters>(y =>
                    y.Title == title &&
                    y.Message == message)));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKen(title, message, NotifyPriorityEnum.high);

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
            sut.NotifyGsmGreet(title, message, NotifyPriorityEnum.high);

            // Assert
            VerifyAllMocks();
        }


        [Theory]
        [InlineData("message")]
        public void NotifyGsmKenTTS_Notify_VerifyMocks(string message)
        {
            // Arrange 
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.Is<NotifyMobileAppA53Parameters>(y =>
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
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.Is<NotifyMobileAppA53Parameters>(y =>
                    y.Title == title &&
                    y.Message == message)));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsm(title, message, NotifyPriorityEnum.high);

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
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.Is<NotifyMobileAppA53Parameters>(y =>
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
            haContextMock.Setup(x => x.CallService("tts", "google_translate_say", null,
                It.Is<TtsGoogleTranslateSayParameters>(y => y.EntityId == entities.MediaPlayer.Speelkamer.EntityId && y.Message == message)));
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
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.Is<NotifyMobileAppA53Parameters>(y =>
                    y.Title == "" &&
                    y.Message == "clear_notification")));

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.Clear(NotifyTagEnum.OpenCloseTuindeur);

            // Assert
            VerifyAllMocks();
        }

        [Theory]
        [MemberData(nameof(NotifyTagEnumValues))]
        public void Notify_NotifyWithTags_VerifyMocks(NotifyTagEnum tag)
        {
            // Arrange 
            NotifyMobileAppA53Parameters parameters = new NotifyMobileAppA53Parameters();
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.IsAny<NotifyMobileAppA53Parameters>()))
                .Callback<string, string, ServiceTarget, object>((a, b, c, d) => parameters = (NotifyMobileAppA53Parameters)d);

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKen("", "", NotifyPriorityEnum.high, tag, null);

            // Assert
            AssertRecordNotifyData(parameters.Data as RecordNotifyData, "default", null, NotifyPriorityEnum.high, tag, null);
            VerifyAllMocks();
        }

        [Fact]
        public void Notify_NotifyHouseState_VerifyMocks()
        {
            // Arrange 
            NotifyMobileAppA53Parameters parameters = new NotifyMobileAppA53Parameters();
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.IsAny<NotifyMobileAppA53Parameters>()))
                .Callback<string, string, ServiceTarget, object>((a, b, c, d) => parameters = (NotifyMobileAppA53Parameters)d);

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyHouseStateGsmKen("", "", "image", NotifyPriorityEnum.high, null);

            // Assert
            AssertRecordNotifyData(parameters.Data as RecordNotifyData, "default", "image", NotifyPriorityEnum.high, NotifyTagEnum.HouseStateChanged, null);
            VerifyAllMocks();
        }

        [Theory]
        [MemberData(nameof(NotifyActionEnumValues))]
        public void Notify_NotifyWithAction_VerifyMocks(NotifyActionEnum action)
        {
            // Arrange 
            NotifyMobileAppA53Parameters parameters = new NotifyMobileAppA53Parameters();
            SetupMocks();
            haContextMock.Setup(x => x.CallService("notify", "mobile_app_a53", null,
                It.IsAny<NotifyMobileAppA53Parameters>()))
                .Callback<string, string, ServiceTarget, object>((a, b, c, d) => parameters = (NotifyMobileAppA53Parameters)d);

            var sut = new Notify(serviceProviderMock.Object);

            // Act
            sut.NotifyGsmKen("", "", NotifyPriorityEnum.high, null, new() { action });

            // Assert
            AssertRecordNotifyData(parameters.Data as RecordNotifyData, "default", null, NotifyPriorityEnum.high, null, action);
            VerifyAllMocks();
        }

        [Theory]
        [MemberData(nameof(NotifyActionEnumValues))]
        public void Notify_HandleNotificationEvent_VerifyMocks(NotifyActionEnum action)
        {
            // Arrange 
            NotifyMobileAppA53Parameters parameters = new NotifyMobileAppA53Parameters();
            SetupMocks();
            if (!action.ToString().StartsWith("Uri"))
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

        public static IEnumerable<object[]> NotifyActionEnumValues()
        {
            foreach (var value in Enum.GetValues(typeof(NotifyActionEnum)))
            {
                yield return new object[] { value };
            }
        }

        public static IEnumerable<object[]> NotifyTagEnumValues()
        {
            foreach (var value in Enum.GetValues(typeof(NotifyTagEnum)))
            {
                yield return new object[] { value };
            }
        }

        private void AssertRecordNotifyData(RecordNotifyData? data, string? channel, string? image, NotifyPriorityEnum priority, NotifyTagEnum? tag, NotifyActionEnum? action)
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
                Assert.Equal(image, data.image);
                Assert.Equal(priority.ToString(), data.importance);

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
