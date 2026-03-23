using NetDaemonInterface;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Veranda : AreaBase
{
    private readonly LightEntity light;

    public Veranda(IHaContext haContext, IScheduler scheduler, ILogger<Badkamer> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.LightVeranda;

        SubscribeToMotionSensor(_entities.BinarySensor.MotionVeranda);
    }

    public override void MotionCleared(string entityId)
    {
        StartAfterTask(delayProvider.MotionClear, () =>
        {
            lightControl.SetLight(light, 0);
        });
    }

    public override void MotionDetected(string entityId)
    {
        StopAfterTask();

        //Only trigger when night
        if (Helper.GetDayNightState(_entities) == DayNightEnum.Night)
        {
            lightControl.SetLight(light, 1);
        }
    }
}
