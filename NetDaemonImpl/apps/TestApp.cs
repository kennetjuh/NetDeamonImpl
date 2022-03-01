using NetDaemonInterface;

namespace NetDaemonImpl.apps;

//[NetDaemonApp]
//[Focus]
public class TestApp : MyNetDaemonBaseApp
{
    public TestApp(IHaContext haContext, IScheduler scheduler, ILogger<TestApp> logger,
        IAreaCollection areaCollection, IDelayProvider delayProvider, IHouseState houseState, ILightControl lightControl,
        ILuxBasedBrightness luxBasedBrightness, INotify notify, ITwinkle twinkle)
        : base(haContext, scheduler, logger)
    {
    }
}