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
}".Replace("<id>", id).Replace("<event>",((int)eventId).ToString());
    }

    private Mock<IAreaControl> areaMock = new Mock<IAreaControl>(MockBehavior.Strict);

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
                   _data.Add( new object[] { map.Item1, map.Item2, map.Item3, eventId });
                }
            }
        }
     

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}