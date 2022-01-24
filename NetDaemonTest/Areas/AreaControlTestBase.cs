using Moq;
using NetDaemonImpl.AreaControl;
using NetDaemonInterface;
using System.Collections.Generic;

namespace NetDaemonTest.Areas;

public abstract class AreaControlTestBase<T> where T : AreaControl
{
    internal LightEntity light;

    internal T? Sut { get; set; }
    internal readonly Mock<IHaContext> haContextMock = new(MockBehavior.Strict);

    internal readonly Mock<IAreaCollection> areaCollectionMock = new(MockBehavior.Strict);
    internal readonly Mock<IDelayProvider> delayProviderMock = new(MockBehavior.Strict);
    internal readonly Mock<IHouseState> houseStateMock = new(MockBehavior.Strict);
    internal readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
    internal readonly Mock<ILuxBasedBrightness> luxBasedBrightnessMock = new(MockBehavior.Strict);
    internal readonly Mock<INotify> notifyMock = new(MockBehavior.Strict);
    internal readonly Mock<ITwinkle> twinkleMock = new(MockBehavior.Strict);

    internal readonly IEntities entities;
    internal readonly IServices services;

    public AreaControlTestBase()
    {
        entities = new Entities(haContextMock.Object);
        services = new Services(haContextMock.Object);
        light = new LightEntity(haContextMock.Object, "Dummy");
        SetupMocks();
    }

    internal void SetupMocks()
    {
        haContextMock.Reset();
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
        areaCollectionMock.VerifyAll();
        delayProviderMock.VerifyAll();
        houseStateMock.VerifyAll();
        lightControlMock.VerifyAll();
        luxBasedBrightnessMock.VerifyAll();
        notifyMock.VerifyAll();
        twinkleMock.VerifyAll();
    }

    internal static IEnumerable<object[]> DeconzEventIdValues()
    {
        foreach (var number in Enum.GetValues(typeof(DeconzEventIdEnum)))
        {
            yield return new object[] { number };
        }
    }
}
