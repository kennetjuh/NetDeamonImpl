using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class SlaapkamerCaitlyn : AreaBase
{
    private readonly LightEntity light;

    private const double minBrightness = 50;
    private const double maxBrightness = 255;

    public SlaapkamerCaitlyn(IHaContext haContext, IScheduler scheduler, ILogger<SlaapkamerCaitlyn> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.SlaapkamerCaitlyn;
        lightControl.AddMaxCustomColorLight(light, new() {190,0,255 });

        SubscribeToDeconzButton(Button.SlaapkamerCaitlyn1);
        SubscribeToDeconzButton(Button.SlaapkamerCaitlyn2);
        SubscribeToDeconzButton(Button.SlaapkamerCaitlyn3);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);
    }
}
