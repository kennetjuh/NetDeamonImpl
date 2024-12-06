using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class DayNightHandlerApp : MyNetDaemonBaseApp
{
    IDisposable? lastDayTask;
    IDisposable? lastNightTask;
    private readonly IDayNight dayNight;

    public DayNightHandlerApp(IHaContext haContext, IScheduler scheduler, ILogger<DayNightHandlerApp> logger,
          ISettingsProvider settingsProvider, IDayNight dayNight)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        this.dayNight = dayNight;

        _entities.InputText.Housestate.StateAllChanges()
            .Subscribe(x => dayNight.CheckDayNight());

        _entities.Sun.Sun.StateAllChanges()
            .Where(x => x.Old?.Attributes?.Elevation != x.New?.Attributes?.Elevation)
            .Subscribe(x => dayNight.CheckDayNight());

        _entities.Sensor.LightSensor.StateChanges()
            .Subscribe(x => dayNight.CheckDayNight());

        dayNight.CheckDayNight();

        _entities.InputDatetime.Daynightlastnighttrigger.StateChanges()
            .Subscribe(x =>
            {
                SetLastNightTrigger();
                SetLastDayTrigger();
            });

        SetLastDayTrigger();
        SetLastNightTrigger();
    }

    private void SetLastDayTrigger()
    {
        lastDayTask?.Dispose();
        lastDayTask = _scheduler.RunDaily(Helper.StringToDateTime(_entities.InputDatetime.Daynightlastdaytrigger.State).TimeOfDay.Add(TimeSpan.FromHours(1)), () => LastDayTrigger());
    }

    private void LastDayTrigger()
    {
        // Halloween
        //_entities.Switch.Binnen1.TurnOff();
    }

    private void SetLastNightTrigger()
    {
        lastNightTask?.Dispose();
        lastNightTask = _scheduler.RunDaily(Helper.StringToDateTime(_entities.InputDatetime.Daynightlastnighttrigger.State).TimeOfDay.Subtract(TimeSpan.FromHours(1)), () => LastNightTrigger());
    }

    private void LastNightTrigger()
    {
        // Halloween
        //_entities.Switch.Binnen1.TurnOn();
    }    
}