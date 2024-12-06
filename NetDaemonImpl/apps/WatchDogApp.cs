using NetDaemon.Extensions.Scheduler;
using NetDaemon.HassModel.Entities;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

/// <summary>
/// Watchdog buiten is needed because the wifi outside is not stable so when the switch happens but some light is not available the light will not turn on/off
/// </summary>
[NetDaemonApp]
public class WatchDogApp : MyNetDaemonBaseApp
{
    private readonly ILightControl lightControl;
    private readonly IDayNight dayNight;
    private IDisposable? watchdogBuitenTask;

    public WatchDogApp(IHaContext haContext, IScheduler scheduler, ILogger<WatchDogApp> logger,
        ILightControl lightControl, ISettingsProvider settingsProvider, IDayNight dayNight)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        this.lightControl = lightControl;
        this.dayNight = dayNight;
        _entities.InputBoolean.WatchdogBuiten.StateChanges()
            .Where(x => x.Entity.IsOn())
            .Subscribe(x => StartWatchdogBuiten());
        _entities.InputBoolean.WatchdogBuiten.StateChanges()
           .Where(x => x.Entity.IsOff())
           .Subscribe(x => watchdogBuitenTask?.Dispose());

        if (_entities.InputBoolean.WatchdogBuiten.IsOn())
        {
            StartWatchdogBuiten();
        }
    }

    private void StartWatchdogBuiten()
    {
        watchdogBuitenTask = _scheduler.ScheduleCron("0/1 * * * *", () => dayNight.WatchdogBuiten()); //every minute
        dayNight.WatchdogBuiten();
    }

   
}