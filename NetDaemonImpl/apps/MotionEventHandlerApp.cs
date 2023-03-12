using NetDaemonInterface;

namespace NetDaemonImpl.apps;

/// <summary>
/// This app handles Motion state changes and supplies the events to Area's
/// </summary>
[NetDaemonApp]
public class MotionEventHandlerApp : MyNetDaemonBaseApp
{
    public MotionEventHandlerApp(IHaContext haContext, IScheduler scheduler, ILogger<MotionEventHandlerApp> logger,
        IAreaCollection areaCollection, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        _entities.BinarySensor.MotionWashal.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Washal).MotionDetected(_entities.BinarySensor.MotionWashal.EntityId));

        _entities.BinarySensor.MotionWashal.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Washal).MotionCleared(_entities.BinarySensor.MotionWashal.EntityId));

        _entities.BinarySensor.MotionTraphal1.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Traphal).MotionDetected(_entities.BinarySensor.MotionTraphal1.EntityId));

        _entities.BinarySensor.MotionTraphal1.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Traphal).MotionCleared(_entities.BinarySensor.MotionTraphal1.EntityId));

        _entities.BinarySensor.MotionTraphal2.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Traphal).MotionDetected(_entities.BinarySensor.MotionTraphal2.EntityId));

        _entities.BinarySensor.MotionTraphal2.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Traphal).MotionCleared(_entities.BinarySensor.MotionTraphal2.EntityId));

        _entities.BinarySensor.MotionBuitenvoor.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Voordeur).MotionDetected(_entities.BinarySensor.MotionBuitenvoor.EntityId));

        _entities.BinarySensor.MotionBuitenvoor.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.GetArea(AreaControlEnum.Voordeur).MotionCleared(_entities.BinarySensor.MotionBuitenvoor.EntityId));
    }
}