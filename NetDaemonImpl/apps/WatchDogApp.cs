using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

/// <summary>
/// Watchdog buiten is needed because the wifi outside is not stable so when the switch happens but some light is not available the light will not turn on/off
/// </summary>
[NetDaemonApp]
public class WatchDogApp : MyNetDaemonBaseApp
{
    private readonly ILightControl lightControl;
    private IDisposable? watchdogBuitenTask;

    public WatchDogApp(IHaContext haContext, IScheduler scheduler, ILogger<WatchDogApp> logger,
        ILightControl lightControl)
        : base(haContext, scheduler, logger)
    {
        this.lightControl = lightControl;

        _entities.Switch.WatchdogBuiten.StateChanges()
            .Where(x => x.Entity.IsOn())
            .Subscribe(x => StartWatchdogBuiten());
        _entities.Switch.WatchdogBuiten.StateChanges()
           .Where(x => x.Entity.IsOff())
           .Subscribe(x => watchdogBuitenTask?.Dispose());

        if (_entities.Switch.WatchdogBuiten.IsOn())
        {
            StartWatchdogBuiten();
        }
    }

    private void StartWatchdogBuiten()
    {
        watchdogBuitenTask = _scheduler.ScheduleCron("0/1 * * * *", () => WatchdogBuiten()); //every minute
        WatchdogBuiten();
    }

    private void WatchdogBuiten()
    {
        if (Helper.GetDayNightState(_entities) == DayNightEnum.Day)
        {
            lightControl.SetLight(_entities.Light.GrondlampZij, 0);
            lightControl.SetLight(_entities.Light.BuitenachterFonteinlamp, 0);
            lightControl.SetLight(_entities.Light.WandlampHut, 0);
            lightControl.SetLight(_entities.Light.LightHut, 0);
        }
        else
        {
            lightControl.SetLight(_entities.Light.GrondlampZij, Constants.brightnessBuitenZij);
            lightControl.SetLight(_entities.Light.BuitenachterFonteinlamp, Constants.brightnessFontein);
            lightControl.SetLight(_entities.Light.WandlampHut, Constants.brightnessHutWand);
            lightControl.SetLight(_entities.Light.LightHut, Constants.brightnessHut);
        }
    }
}