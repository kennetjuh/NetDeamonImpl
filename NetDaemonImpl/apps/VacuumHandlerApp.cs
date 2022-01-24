using NetDaemon.Extensions.Scheduler;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class VacuumHandlerApp : MyNetDaemonBaseApp
{
    public VacuumHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger)
        : base(haContext, scheduler, logger)
    {
        _entities.Vacuum.DreameP20294b09RobotCleaner.StateChanges()
            .Where(x => x.New?.State == "docked")
            .Throttle(TimeSpan.FromMinutes(5))
            .Subscribe(x => _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Basic"));    
        
        if(_entities.Vacuum.DreameP20294b09RobotCleaner.State == "docked")
        {
            _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Basic");
        }
    }
}