using Moq;
using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonTest.Apps;

public abstract class AppTestBase
{
    internal readonly Mock<IHaContext> haContextMock = new(MockBehavior.Strict);
    internal readonly Mock<INetDaemonScheduler> schedulerMock = new(MockBehavior.Strict);

    internal readonly Mock<IAreaCollection> areaCollectionMock = new(MockBehavior.Strict);
    internal readonly Mock<IDelayProvider> delayProviderMock = new(MockBehavior.Strict);
    internal readonly Mock<IHouseState> houseStateMock = new(MockBehavior.Strict);
    internal readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
    internal readonly Mock<ILuxBasedBrightness> luxBasedBrightnessMock = new(MockBehavior.Strict);
    internal readonly Mock<INotify> notifyMock = new(MockBehavior.Strict);
    internal readonly Mock<ITwinkle> twinkleMock = new(MockBehavior.Strict);

    internal readonly IEntities entities;
    internal readonly IServices services;

    public AppTestBase()
    {
        entities = new Entities(haContextMock.Object);
        services = new Services(haContextMock.Object);
        SetupMocks();
    }

    internal void SetupMocks()
    {
        haContextMock.Reset();
        schedulerMock.Reset();
        areaCollectionMock.Reset();
        delayProviderMock.Reset();
        houseStateMock.Reset();
        lightControlMock.Reset();
        luxBasedBrightnessMock.Reset();
        notifyMock.Reset();
        twinkleMock.Reset();
    }

    internal void VerifyAllMocks()
    {
        haContextMock.VerifyAll();
        schedulerMock.VerifyAll();
        areaCollectionMock.VerifyAll();
        delayProviderMock.VerifyAll();
        houseStateMock.VerifyAll();
        lightControlMock.VerifyAll();
        luxBasedBrightnessMock.VerifyAll();
        notifyMock.VerifyAll();
        twinkleMock.VerifyAll();
    }
}
