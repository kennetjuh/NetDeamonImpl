using Microsoft.Extensions.Logging;
using Moq;
using NetDaemonImpl.apps;
using System.Collections.Generic;
using Xunit;

namespace NetDaemonTest.Apps;

public class CallBackHandlerAppTest : AppTestBase
{
    internal readonly Mock<ILogger<DeconzEventHandlerApp>> loggerMock = new(MockBehavior.Strict);

    private object ItIsAnonymousObject(object value)
    {
        return It.Is<object>(o => o.GetHashCode() == value.GetHashCode());
    }

    [Fact]
    public void Contructor_NoExceptions()
    {
        // Arrange 
        SetupMocks();
        haContextMock.Setup(x => x.CallService("netdaemon", "register_service", null, ItIsAnonymousObject(new {service = "ChangeHouseState" })));
        haContextMock.Setup(x => x.CallService("netdaemon", "register_service", null, ItIsAnonymousObject(new {service = "Twinkle" })));
        haContextMock.Setup(x=>x.Events).Returns(new List<Event>().ToObservable());

        // Act
        _ = new CallBackHandlerApp(haContextMock.Object, schedulerMock.Object, loggerMock.Object, twinkleMock.Object, houseStateMock.Object);

        // Assert
        VerifyAllMocks();
    }    
}