using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

//[NetDaemonApp]
//[Focus]
public class TestApp : MyNetDaemonBaseApp
{
    public TestApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        IAreaCollection areaCollection, IDelayProvider delayProvider, IHouseState houseState, ILightControl lightControl,
        ILuxBasedBrightness luxBasedBrightness, INotify notify, ITwinkle twinkle)
        : base(haContext, scheduler, logger)
    {
        lightControl.SetLight(_entities.Light.KeukenKeukenlamp, 0);
    } 
}