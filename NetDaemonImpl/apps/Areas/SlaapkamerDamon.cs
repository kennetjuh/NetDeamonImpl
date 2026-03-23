using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class SlaapkamerDamon : AreaBase
{
    private readonly LightEntity light;

    private const double minBrightness = 10;
    private const double maxBrightness = 255;

    public SlaapkamerDamon(IHaContext haContext, IScheduler scheduler, ILogger<Badkamer> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.SlaapkamerDamon;

        SubscribeToDeconzButton(Button.SlaapkamerDamon);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);
    }
}
