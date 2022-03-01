using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Moq;
using NetDaemonInterface;

namespace NetDaemonTest.Apps.Helpers;

public class TestBase
{
    public TestContext Context = new();
    public Entities Entities => Context.GetRequiredService<Entities>();
    public HaContextMock HaMock => Context.GetRequiredService<HaContextMock>();
    public TestScheduler Scheduler => Context.GetRequiredService<TestScheduler>();

    public Mock<IAreaCollection> AreaCollectionMock => Context.GetRequiredService<Mock<IAreaCollection>>();
    public Mock<IDelayProvider> DelayProviderMock => Context.GetRequiredService<Mock<IDelayProvider>>();
    public Mock<IHouseState> HouseStateMock => Context.GetRequiredService<Mock<IHouseState>>();
    public Mock<ILightControl> LightControlMock => Context.GetRequiredService<Mock<ILightControl>>();
    public Mock<ILuxBasedBrightness> LuxBasedBrightnessMock => Context.GetRequiredService<Mock<ILuxBasedBrightness>>();
    public Mock<INotify> NotifyMock => Context.GetRequiredService<Mock<INotify>>();
    public Mock<ITwinkle> TwinkleMock => Context.GetRequiredService<Mock<ITwinkle>>();
    public Mock<IHouseNotificationImageCreator> HouseNotificationImageCreatorMock => Context.GetRequiredService<Mock<IHouseNotificationImageCreator>>();
    public Mock<ISettingsProvider> SettingsProviderMock => Context.GetRequiredService<Mock<ISettingsProvider>>();
    public Mock<IDayNight> DayNightMock => Context.GetRequiredService<Mock<IDayNight>>();

    internal virtual void VerifyAllMocks()
    {
        HaMock.VerifyAll();
        AreaCollectionMock.VerifyAll();
        DelayProviderMock.VerifyAll();
        HouseStateMock.VerifyAll();
        LightControlMock.VerifyAll();
        LuxBasedBrightnessMock.VerifyAll();
        NotifyMock.VerifyAll();
        HouseNotificationImageCreatorMock.VerifyAll();
        SettingsProviderMock.VerifyAll();
        DayNightMock.VerifyAll();
    }

    internal virtual void ResetAllMocks()
    {
        HaMock.Reset();
        AreaCollectionMock.Reset();
        DelayProviderMock.Reset();
        HouseStateMock.Reset();
        LightControlMock.Reset();
        LuxBasedBrightnessMock.Reset();
        NotifyMock.Reset();
        HouseNotificationImageCreatorMock.Reset();
        SettingsProviderMock.Reset();
        DayNightMock.Reset();
    }
}