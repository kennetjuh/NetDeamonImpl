using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class DayNightHandlerApp : MyNetDaemonBaseApp
{
    IDisposable? lastDayTask;
    IDisposable? lastNightTask;
    private readonly ILightControl lightControl;
    private readonly ILuxBasedBrightness luxBasedBrightness;

    public DayNightHandlerApp(IHaContext haContext, IScheduler scheduler, ILogger<DayNightHandlerApp> logger,
        ILightControl lightControl, ILuxBasedBrightness luxBasedBrightness, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        this.lightControl = lightControl;
        this.luxBasedBrightness = luxBasedBrightness;

        _entities.Sun.Sun.StateAllChanges()
            .Where(x => x.Old?.Attributes?.Elevation != x.New?.Attributes?.Elevation)
            .Subscribe(x => CheckDayNight());

        _entities.Sensor.LightSensor.StateChanges()
            .Subscribe(x => CheckDayNight());

        CheckDayNight();

        _entities.Sensor.DaynightLastnighttrigger.StateChanges()
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
        lastDayTask = _scheduler.RunDaily(Helper.StringToDateTime(_entities.Sensor.DaynightLastdaytrigger.State).TimeOfDay.Add(TimeSpan.FromHours(1)), () => LastDayTrigger());
    }

    private void LastDayTrigger()
    {
        // Halloween
        //_entities.Switch.Binnen1.TurnOff();
    }

    private void SetLastNightTrigger()
    {
        lastNightTask?.Dispose();
        lastNightTask = _scheduler.RunDaily(Helper.StringToDateTime(_entities.Sensor.DaynightLastnighttrigger.State).TimeOfDay.Subtract(TimeSpan.FromHours(1)), () => LastNightTrigger());
    }

    private void LastNightTrigger()
    {
        // Halloween
        //_entities.Switch.Binnen1.TurnOn();
    }

    private void CheckDayNight()
    {
        var elevation = _entities.Sun.Sun.Attributes?.Elevation;
        var isRising = _entities.Sun.Sun.Attributes?.Rising;
        var lux = luxBasedBrightness.GetLux();
        var current = Helper.GetDayNightState(_entities);

        if (current == DayNightEnum.Day && isRising == false && elevation < 0 && lux < 30)
        {
            Night();
        }
        if (current == DayNightEnum.Night && isRising == true && elevation > -5 && lux > 20)
        {
            Day();
        }
    }

    private void Night()
    {
        _entities.Sensor.DaynightLastnighttrigger.SetState(_services, DateTime.Now.ToString(Constants.dateTime_TimeFormat));
        _entities.Sensor.Daynight.SetState(_services, DayNightEnum.Night.ToString());

        lightControl.SetLight(_entities.Light.BuitenopritWandlamp, 50);
        lightControl.SetLight(_entities.Light.WandlampBuiten, 50);
        _entities.Switch.BuitenvoorGrondspots.TurnOn();

        lightControl.SetLight(_entities.Light.SfeerlampKamer1, 1);
        lightControl.SetLight(_entities.Light.SfeerlampKamer2, 1);
        lightControl.SetLight(_entities.Light.SfeerlampKeuken, 1);
        lightControl.SetLight(_entities.Light.SfeerlampHal, _settingsProvider.BrightnessSfeerlampHalNight);
        lightControl.SetLight(_entities.Light.SfeerlampBoven, 1);
        lightControl.SetLight(_entities.Light.LightSpeelkamerSfeer, _settingsProvider.BrightnessSfeerlampSpeelkamerNight);

        if (_entities.Light.Wandlampen.IsOn())
        {
            lightControl.SetLight(_entities.Light.Wandlampen, Constants.brightnessWandNight);
        }

        lightControl.SetLight(_entities.Light.LightHut, Constants.brightnessHut);
    }

    private void Day()
    {
        _entities.Sensor.DaynightLastdaytrigger.SetState(_services, DateTime.Now.ToString(Constants.dateTime_TimeFormat));
        _entities.Sensor.Daynight.SetState(_services, DayNightEnum.Day.ToString());

        lightControl.SetLight(_entities.Light.BuitenopritWandlamp, 0);
        lightControl.SetLight(_entities.Light.WandlampBuiten, 0);
        _entities.Switch.BuitenvoorGrondspots.TurnOff();

        lightControl.SetLight(_entities.Light.SfeerlampKamer1, 50);
        lightControl.SetLight(_entities.Light.SfeerlampKamer2, 50);
        lightControl.SetLight(_entities.Light.SfeerlampKeuken, 50);
        lightControl.SetLight(_entities.Light.SfeerlampHal, _settingsProvider.BrightnessSfeerlampHalDay);
        lightControl.SetLight(_entities.Light.SfeerlampBoven, 50);
        lightControl.SetLight(_entities.Light.LightSpeelkamerSfeer, _settingsProvider.BrightnessSfeerlampSpeelkamerDay);

        if (_entities.Light.Wandlampen.IsOn())
        {
            lightControl.SetLight(_entities.Light.Wandlampen, Constants.brightnessWandDay);
        }

        lightControl.SetLight(_entities.Light.LightHut, 0);
        lightControl.SetLight(_entities.Light.BuitenzijHutsier, 0);
    }
}