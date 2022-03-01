using Moq;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetDaemonTest.Apps;

public class MotionEventHandlerAppTest : TestBase
{
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
    public void MotionEventHandlerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<MotionEventHandlerApp>();

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void MotionEventHandlerApp_Motion_VerifyCalls(NetDaemon.HassModel.Entities.Entity entity, AreaControlEnum area)
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(entity, "off");
        AreaCollectionMock.Setup(x => x.GetArea(area)).Returns(areaMock.Object);
        areaMock.Setup(x => x.MotionDetected(entity.EntityId));
        areaMock.Setup(x => x.MotionCleared(entity.EntityId));

        // Act
        var app = Context.GetApp<MotionEventHandlerApp>();
        HaMock.TriggerStateChange(entity, "on");
        HaMock.TriggerStateChange(entity, "off");

        // Assert
        VerifyAllMocks();
    }


    public class TestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new();

        public TestDataGenerator()
        {
            var entities = new Entities(null!);

            _data.Add(new object[] { entities.BinarySensor.MotionWashal, AreaControlEnum.Washal });
            _data.Add(new object[] { entities.BinarySensor.MotionTraphal1, AreaControlEnum.Traphal });
            _data.Add(new object[] { entities.BinarySensor.MotionTraphal2, AreaControlEnum.Traphal });
            _data.Add(new object[] { entities.BinarySensor.MotionBuitenvoor, AreaControlEnum.Voordeur });

        }

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}