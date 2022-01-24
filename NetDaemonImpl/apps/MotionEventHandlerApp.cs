using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

/// <summary>
/// This app handles Motion state changes and supplies the events to Area's
/// </summary>
[NetDaemonApp]
public class MotionEventHandlerApp : MyNetDaemonBaseApp
{
    public MotionEventHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        IAreaCollection areaCollection)
        : base(haContext, scheduler, logger)
    {
        _entities.BinarySensor.MotionWashal.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.Washal.MotionDetected(_entities.BinarySensor.MotionWashal.EntityId));

        _entities.BinarySensor.MotionWashal.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.Washal.MotionCleared(_entities.BinarySensor.MotionWashal.EntityId));

        _entities.BinarySensor.MotionTraphal1.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.Traphal.MotionDetected(_entities.BinarySensor.MotionTraphal1.EntityId));

        _entities.BinarySensor.MotionTraphal1.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.Traphal.MotionCleared(_entities.BinarySensor.MotionTraphal1.EntityId));

        _entities.BinarySensor.MotionTraphal2.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.Traphal.MotionDetected(_entities.BinarySensor.MotionTraphal2.EntityId));

        _entities.BinarySensor.MotionTraphal2.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.Traphal.MotionCleared(_entities.BinarySensor.MotionTraphal2.EntityId));

        _entities.BinarySensor.MotionBuitenvoor.StateChanges()
            .Where(x => x.New?.State == "on")
            .Subscribe(x => areaCollection.Voordeur.MotionDetected(_entities.BinarySensor.MotionBuitenvoor.EntityId));

        _entities.BinarySensor.MotionBuitenvoor.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => areaCollection.Voordeur.MotionCleared(_entities.BinarySensor.MotionBuitenvoor.EntityId));
    }
}