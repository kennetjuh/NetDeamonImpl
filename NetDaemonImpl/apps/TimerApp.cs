using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class TimerApp : MyNetDaemonBaseApp
{
    public TimerApp(IHaContext haContext, IScheduler scheduler, INotify notify, ILogger<CallBackHandlerApp> logger, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {        
        //_scheduler.ScheduleCron("0 8 * * *", () => // every day at 08:00
        //{
        //    _entities.Switch.SwitchZwembad.TurnOn();
        //});

        //_scheduler.ScheduleCron("0 23 * * *", () => // every day at 23:00
        //{
        //    _entities.Switch.SwitchZwembad.TurnOff();
        //});

        _scheduler.ScheduleCron("0 21 * * *", () => // every day at 21:00
        {
            if (Helper.GetHouseState(_entities) == HouseStateEnum.Holiday)
            {
                _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
                _entities.Vacuum.DreameP20294b09RobotCleaner.Start();
            }
        });

        _entities.Sensor.PowerTariff.StateChanges()
             .Subscribe(x =>
             {
                 HandlePowerState(notify, x.New?.State?.ToString());
             });
        HandlePowerState(notify, _entities.Sensor.PowerTariff.State?.ToString());
    }

    private void HandlePowerState(INotify notify, string? state)
    {
        if (state == null)
        {
            return;
        }
        if (state == "low")
        {
            notify.NotifyGsmKen("Lync lader ON", "", NotifyPriorityEnum.high);
            _entities.Switch.LyncLader.TurnOn();
        }
        if (state == "normal")
        {
            notify.NotifyGsmKen("Lync lader OFF", "", NotifyPriorityEnum.high);
            _entities.Switch.LyncLader.TurnOff();
        }
    }
}