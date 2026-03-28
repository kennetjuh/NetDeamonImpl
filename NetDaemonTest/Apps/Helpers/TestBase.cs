using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Moq;
using NetDaemonInterface;
using NetDaemonInterface.Observable;

namespace NetDaemonTest.Apps.Helpers;

public class TestBase
{
    public TestContext Context = new();
    public Entities Entities => Context.GetRequiredService<Entities>();
    public HaContextMock HaMock => Context.GetRequiredService<HaContextMock>();
    public TestScheduler Scheduler => Context.GetRequiredService<TestScheduler>();
    public Mock<IDelayProvider> DelayProviderMock => Context.GetRequiredService<Mock<IDelayProvider>>();
    public Mock<ILightControl> LightControlMock => Context.GetRequiredService<Mock<ILightControl>>();
    public Mock<ILuxBasedBrightness> LuxBasedBrightnessMock => Context.GetRequiredService<Mock<ILuxBasedBrightness>>();
    public Mock<INotify> NotifyMock => Context.GetRequiredService<Mock<INotify>>();
    public Mock<IHouseNotificationImageCreator> HouseNotificationImageCreatorMock => Context.GetRequiredService<Mock<IHouseNotificationImageCreator>>();
    public Mock<ISettingsProvider> SettingsProviderMock => Context.GetRequiredService<Mock<ISettingsProvider>>();
    public Mock<IDayNightEvents> DayNightEventsMock => Context.GetRequiredService<Mock<IDayNightEvents>>();
    public Mock<IButtonEvents> DeconzButtonEventsMock => Context.GetRequiredService<Mock<IButtonEvents>>();
    public Mock<IHouseStateEvents> HouseStateEventsMock => Context.GetRequiredService<Mock<IHouseStateEvents>>();
    public Mock<ITriggerManager> TriggerManagerMock => Context.GetRequiredService<Mock<ITriggerManager>>();
    public Mock<IFrigateClient> FrigateClientMock => Context.GetRequiredService<Mock<IFrigateClient>>();
    public Mock<IThinginoClient> ThinginoClientMock => Context.GetRequiredService<Mock<IThinginoClient>>();

    internal virtual void VerifyAllMocks()
    {
        HaMock.VerifyAll();
        DelayProviderMock.VerifyAll();
        LightControlMock.VerifyAll();
        LuxBasedBrightnessMock.VerifyAll();
        NotifyMock.VerifyAll();
        HouseNotificationImageCreatorMock.VerifyAll();
        SettingsProviderMock.VerifyAll();
        DayNightEventsMock.VerifyAll();
        DeconzButtonEventsMock.VerifyAll();
        HouseStateEventsMock.VerifyAll();
        FrigateClientMock.VerifyAll();
        ThinginoClientMock.VerifyAll();
    }

    internal virtual void ResetAllMocks()
    {
        HaMock.Reset();
        DelayProviderMock.Reset();
        LightControlMock.Reset();
        LuxBasedBrightnessMock.Reset();
        NotifyMock.Reset();
        HouseNotificationImageCreatorMock.Reset();
        SettingsProviderMock.Reset();
        DayNightEventsMock.Reset();
        DeconzButtonEventsMock.Reset();
        HouseStateEventsMock.Reset();
        FrigateClientMock.Reset();
        ThinginoClientMock.Reset();
    }
}