using NetDaemon.Extensions.Scheduler;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class TimerApp : MyNetDaemonBaseApp
{
    public TimerApp(IHaContext haContext, IScheduler scheduler, ILogger<CallBackHandlerApp> logger)
        : base(haContext, scheduler, logger)
    {
        _scheduler.ScheduleCron("0 8 * * *", () => // every day at 08:00
        {
            _entities.Switch.SwitchZwembad.TurnOn();
        });

        _scheduler.ScheduleCron("0 23 * * *", () => // every day at 23:00
        {
            _entities.Switch.SwitchZwembad.TurnOff();
        });
    }
}