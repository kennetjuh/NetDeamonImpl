using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Badkamer : AreaBase
{
    private readonly LightEntity light;
    private readonly LightEntity nis;

    private const double minBrightness = 50;
    private const double maxBrightness = 255;

    public Badkamer(IHaContext haContext, IScheduler scheduler, ILogger<Badkamer> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.LightBadkamer;
        nis = _entities.Light.LightBadkamerNis;

        SubscribeToDeconzButton(Button.Badkamer);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        if (buttonEvent.Event == ButtonEventType.Single && (nis.IsOn() || light.IsOn()))
        {
            lightControl.SetLight(nis, 0);
            lightControl.SetLight(light, 0);
        }
        else
        {
            lightControl.SetLight(nis, 30);
            lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);
        }
    }
}
