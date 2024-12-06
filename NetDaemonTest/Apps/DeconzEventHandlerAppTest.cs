using Moq;
using NetDaemonImpl;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace NetDaemonTest.Apps;

public class DeconzEventHandlerAppTest : TestBase
{
    private string GetDeconzEvent(string id, DeconzEventIdEnum eventId)
    {
        return @"
                {
                    ""event_type"": ""deconz_event"",
                    ""data"": {
                        ""id"": ""<id>"",
                        ""event"": <event>,
                        ""service_data"": {}     
                    },
                    ""origin"": ""LOCAL"",
                    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
                    ""context"": {
                        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
                        ""parent_id"": null,
                        ""user_id"": null
                    }
                }".Replace("<id>", id).Replace("<event>", ((int)eventId).ToString());
    }

    private string GetNullEvent() => @"
    {
        ""event_type"": ""deconz_event"",
        ""data"": null,
        ""origin"": ""LOCAL"",
        ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
        ""context"": {
            ""id"": ""49f79a459d1ac8b425848224da4c9149"",
            ""parent_id"": null,
            ""user_id"": null
        }
    }";

    private string GetDeconz1003Event()
    {
        return @"
                {
                    ""event_type"": ""deconz_event"",
                    ""data"": {
                        ""id"": ""1003"",
                        ""event"": 1003,
                        ""service_data"": {}     
                    },
                    ""origin"": ""LOCAL"",
                    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
                    ""context"": {
                        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
                        ""parent_id"": null,
                        ""user_id"": null
                    }
                }";
    }

    private string GetDeconzNoAreaEvent()
    {
        return @"
                {
                    ""event_type"": ""deconz_event"",
                    ""data"": {
                        ""id"": ""1003"",
                        ""event"": 1001,
                        ""service_data"": {}     
                    },
                    ""origin"": ""LOCAL"",
                    ""time_fired"": ""2021-11-12T15:35:49.493480+00:00"",
                    ""context"": {
                        ""id"": ""49f79a459d1ac8b425848224da4c9149"",
                        ""parent_id"": null,
                        ""user_id"": null
                    }
                }";
    }
    private readonly Mock<IAreaControl> areaMock = new Mock<IAreaControl>(MockBehavior.Strict);

    internal override void ResetAllMocks()
    {
        base.ResetAllMocks();
        areaMock.Reset();
    }

    internal override void VerifyAllMocks()
    {
        base.VerifyAllMocks();
        areaMock.VerifyAll();
    }

    [Fact]
    public void DeconzEventHandlerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<DeconzEventHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void DeconzEventHandlerApp_TriggerEvent_VerifyCalls(string name, AreaControlEnum area, string entity, DeconzEventIdEnum id)
    {
        // Arrange
        ResetAllMocks();
        AreaCollectionMock.Setup(x => x.GetArea(area)).Returns(areaMock.Object);
        areaMock.Setup(x => x.ButtonPressed(entity, id));

        // Act
        var app = Context.GetApp<DeconzEventHandlerApp>();
        var deconzEvent = JsonSerializer.Deserialize<Event>(GetDeconzEvent(name, id))!;
        HaMock.TriggerEvent(deconzEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DeconzEventHandlerApp_TriggerNullEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<DeconzEventHandlerApp>();
        var deconzEvent = JsonSerializer.Deserialize<Event>(GetNullEvent())!;
        HaMock.TriggerEvent(deconzEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DeconzEventHandlerApp_Trigger1003Event_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<DeconzEventHandlerApp>();
        var deconzEvent = JsonSerializer.Deserialize<Event>(GetDeconz1003Event())!;
        HaMock.TriggerEvent(deconzEvent);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void DeconzEventHandlerApp_TriggerNoAreaEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<DeconzEventHandlerApp>();
        var deconzEvent = JsonSerializer.Deserialize<Event>(GetDeconzNoAreaEvent())!;
        HaMock.TriggerEvent(deconzEvent);

        // Assert
        VerifyAllMocks();
    }


    public class TestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new();

        public TestDataGenerator()
        {
            var mapping = new DeconzButtonMapping(new Entities(null!));

            foreach (var map in mapping.mapping)
            {
                foreach (var eventId in Enum.GetValues(typeof(DeconzEventIdEnum)))
                {
                    _data.Add(new object[] { map.Item1, map.Item2, map.Item3, eventId });
                }
            }
        }


        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}