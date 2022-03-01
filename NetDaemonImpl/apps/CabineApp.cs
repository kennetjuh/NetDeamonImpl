using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class CabineApp : MyNetDaemonBaseApp
{
    readonly SwitchEntity heater;

    public CabineApp(IHaContext haContext, INotify notify, IScheduler scheduler, ILogger<CallBackHandlerApp> logger, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        heater = _entities.Switch.CabineHeater;

        _entities.InputNumber.Cabinetemptarget.StateChanges().Subscribe(x =>
        {
            Thermostat(notify);
        });

        _entities.InputBoolean.Cabinethermostat.StateChanges().Subscribe(x =>
        {
            var thermostatEnabled = _entities.InputBoolean.Cabinethermostat.State == "on";
            if (thermostatEnabled)
            {
                Thermostat(notify);
            }
            else
            {
                heater.TurnOff();
            }
        });

        _entities.Sensor.MultiCabineTemp.StateChanges().Subscribe(x =>
        {
            Thermostat(notify);
        });
    }

    private void Thermostat(INotify notify)
    {
        var thermostatEnabled = _entities.InputBoolean.Cabinethermostat.State == "on";
        if (!thermostatEnabled)
        {
            return;
        }

        var actualTemp = _entities.Sensor.MultiCabineTemp.State;
        var targetTemp = _entities.InputNumber.Cabinetemptarget.State;

        if (actualTemp == null || targetTemp == null)
        {
            notify.NotifyGsmKen("CabineThermostat", "Actual or Target is NULL", NotifyPriorityEnum.high);
            return;
        }

        if (actualTemp < targetTemp)
        {
            heater.TurnOn();
        }
        else
        {
            heater.TurnOff();
        }
    }
}