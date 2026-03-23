using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Garage : AreaBase
{
    private readonly LightEntity lightSfeer;
    private readonly LightEntity lightLinks;    
    private readonly LightEntity lightRechts1;    
    private readonly LightEntity lightRechts2;   

    public Garage(IHaContext haContext, IScheduler scheduler, ILogger<Garage> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        lightSfeer = _entities.Light.SfeerGarage;
        lightLinks = _entities.Light.LightGarageLinks;
        lightRechts1 = _entities.Light.LightGarageRechts1;
        lightRechts2 = _entities.Light.LightGarageRechts2;

        SubscribeToDeconzButton(Button.Garage);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        switch (buttonEvent.Event)
        {
            case ButtonEventType.Single:
                if (lightSfeer.IsOn())
                {
                    lightControl.SetLight(lightSfeer, 0);
                    lightControl.SetLight(lightLinks, 0);
                    lightControl.SetLight(lightRechts1, 0);
                    lightControl.SetLight(lightRechts2, 0);
                }
                else
                {
                    lightControl.SetLight(lightSfeer, 150);
                    lightControl.SetLight(lightLinks, 0);
                    lightControl.SetLight(lightRechts1, 0);
                    lightControl.SetLight(lightRechts2, 0);
                }
                break;
            case ButtonEventType.Double:
                lightControl.SetLight(lightSfeer, 0);
                lightControl.SetLight(lightLinks, 1);
                lightControl.SetLight(lightRechts1, 0);
                lightControl.SetLight(lightRechts2, 0);
                break;
            case ButtonEventType.LongPress:
                lightControl.SetLight(lightSfeer, 0);
                lightControl.SetLight(lightLinks, 1);
                lightControl.SetLight(lightRechts1, 1);
                lightControl.SetLight(lightRechts2, 1);
                break;
        }        
    }
}
