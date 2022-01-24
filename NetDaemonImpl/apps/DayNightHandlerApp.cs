using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class DayNightHandlerApp : MyNetDaemonBaseApp
{
    IDisposable? lastDayTask;
    IDisposable? lastNightTask;
    private readonly ILightControl lightControl;
    private readonly ILuxBasedBrightness luxBasedBrightness;

    public DayNightHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        ILightControl lightControl, ILuxBasedBrightness luxBasedBrightness)
        : base(haContext, scheduler, logger)
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
            .Subscribe(x => {
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
        //_entities.Switch.BuitenvoorKerst.TurnOff();
    }

    private void SetLastNightTrigger()
    {
        lastNightTask?.Dispose();
        lastNightTask = _scheduler.RunDaily(Helper.StringToDateTime(_entities.Sensor.DaynightLastnighttrigger.State).TimeOfDay.Subtract(TimeSpan.FromHours(1)), () => LastNightTrigger());
    }

    private void LastNightTrigger()
    {
        //_entities.Switch.BuitenvoorKerst.TurnOn();
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
        lightControl.SetLight(_entities.Light.BuitenvoorWandlamp, 50);
        lightControl.SetLight(_entities.Light.BuitenvoorGrondspots, 255);

        lightControl.SetLight(_entities.Light.WoonkamerSfeer1, 1);
        lightControl.SetLight(_entities.Light.WoonkamerSfeer2, 1);
        lightControl.SetLight(_entities.Light.KeukenSfeer, 1);
        lightControl.SetLight(_entities.Light.HalSfeer, 1);
        lightControl.SetLight(_entities.Light.HalbovenSfeer, 1);

        if (_entities.Light.LightWoonWand.IsOn())
        {
            lightControl.SetLight(_entities.Light.LightWoonWand, 70);
        }
    }

    private void Day()
    {
        _entities.Sensor.DaynightLastdaytrigger.SetState(_services, DateTime.Now.ToString(Constants.dateTime_TimeFormat));
        _entities.Sensor.Daynight.SetState(_services, DayNightEnum.Day.ToString());

        lightControl.SetLight(_entities.Light.BuitenopritWandlamp, 0);
        lightControl.SetLight(_entities.Light.BuitenvoorWandlamp, 0);
        lightControl.SetLight(_entities.Light.BuitenvoorGrondspots, 0);

        lightControl.SetLight(_entities.Light.WoonkamerSfeer1, 50);
        lightControl.SetLight(_entities.Light.WoonkamerSfeer2, 50);
        lightControl.SetLight(_entities.Light.KeukenSfeer, 50);
        lightControl.SetLight(_entities.Light.HalSfeer, 50);
        lightControl.SetLight(_entities.Light.HalbovenSfeer, 50);

        if (_entities.Light.LightWoonWand.IsOn())
        {
            lightControl.SetLight(_entities.Light.LightWoonWand, 125);
        }
    }
}