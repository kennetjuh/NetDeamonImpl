using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface;

namespace NetDaemonTest.Areas;

public abstract class AreaControlTestBase<T>  : ServiceProviderTestBase
    where T : AreaBase
{
    internal LightEntity light;

    internal T? Sut { get; set; }
    
    internal readonly Mock<IDelayProvider> delayProviderMock = new(MockBehavior.Strict);
    internal readonly Mock<ILightControl> lightControlMock = new(MockBehavior.Strict);
    internal readonly Mock<ILuxBasedBrightness> luxBasedBrightnessMock = new(MockBehavior.Strict);
    internal readonly Mock<INotify> notifyMock = new(MockBehavior.Strict);
    
    internal readonly IServices services;

    public AreaControlTestBase()
    {        
        services = new Services(haContextMock.Object);
        light = new LightEntity(haContextMock.Object, "Dummy");
        SetupMocks();
    }

    internal override void SetupMocks()
    {
        base.SetupMocks();
        haContextMock.Reset();
        delayProviderMock.Reset();
        lightControlMock.Reset();
        luxBasedBrightnessMock.Reset();
        notifyMock.Reset();
    }

    internal override void VerifyAllMocks()
    {
        base.VerifyAllMocks();
        haContextMock.VerifyAll();
        delayProviderMock.VerifyAll();
        lightControlMock.VerifyAll();
        luxBasedBrightnessMock.VerifyAll();
        notifyMock.VerifyAll();
    }
}
