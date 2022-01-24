using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class CallBackHandlerApp : MyNetDaemonBaseApp
{
    private readonly IHouseState houseState;

    public CallBackHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        ITwinkle twinkle, IHouseState houseState)
        : base(haContext, scheduler, logger)
    {
        this.houseState = houseState;

        _haContext.RegisterServiceCallBack<ChangeHouseStateData>("ChangeHouseState", (x) => ChangeHouseState(x));
        _haContext.RegisterServiceCallBack<EmptyStateData>("Twinkle", (x) => twinkle.Start());
    }

    record ChangeHouseStateData(string state);
    record EmptyStateData();

    private void ChangeHouseState(ChangeHouseStateData data)
    {
        switch (data.state)
        {
            case "Awake":
                houseState.HouseStateAwake();
                break;
            case "Away":
                houseState.HouseStateAway();
                break;
            case "Sleeping":
                houseState.HouseStateSleeping();
                break;
            default:
                break;
        }
    }
}