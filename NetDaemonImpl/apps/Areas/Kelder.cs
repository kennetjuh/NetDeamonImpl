using NetDaemonInterface;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Kelder : AreaBase
{
    private readonly SwitchEntity plug;    

    public Kelder(IHaContext haContext, IScheduler scheduler, ILogger<Kelder> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {        
        plug = _entities.Switch.PlugKelder;
        
        SubscribeToOpenCloseSensor(_entities.BinarySensor.OpencloseKelderOpening);
    }

    public override void OpenCloseChanged(string entityId, bool isOpen)
    {
        if (isOpen)
        {
            StartAfterTask(TimeSpan.FromMilliseconds(500),() =>
            {
                plug.TurnOn();
            });            
        }
        else
        {
            StopAfterTask();
            plug.TurnOff();
        }
    }

}
