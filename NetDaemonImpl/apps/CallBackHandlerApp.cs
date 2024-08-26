using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class CallBackHandlerApp : MyNetDaemonBaseApp
{
    private readonly IHouseState houseState;

    public CallBackHandlerApp(IHaContext haContext, IScheduler scheduler, ILogger<CallBackHandlerApp> logger,
        ITwinkle twinkle, IHouseState houseState, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        this.houseState = houseState;

        _haContext.RegisterServiceCallBack<ChangeHouseStateData>("ChangeHouseState", (x) => ChangeHouseState(x));
        _haContext.RegisterServiceCallBack<EmptyStateData>("Twinkle", (x) => twinkle.Start());
        _haContext.RegisterServiceCallBack<EmptyStateData>("TvMode", (x) => houseState.TvMode());
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
                houseState.HouseStateAway(HouseStateEnum.Away);
                break;
            case "Sleeping":
                houseState.HouseStateSleeping();
                break;
            case "Holiday":
                houseState.HouseStateHoliday();
                break;
            default:
                break;
        }
    }
}