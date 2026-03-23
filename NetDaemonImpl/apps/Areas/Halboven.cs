using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Halboven : AreaBase
{
    private readonly LightEntity light;
    private readonly LightEntity light1;

    private const double minBrightness = 1;
    private const double maxBrightness = 255;

    public Halboven(IHaContext haContext, IScheduler scheduler, ILogger<Halboven> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.HalBoven;
        light1 = _entities.Light.HalBovenZij;        

        SubscribeToDeconzButton(Button.HalBoven1);
        SubscribeToDeconzButton(Button.HalBoven2);
        SubscribeToDeconzButton(Button.HalBoven3);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);
        lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light1, minBrightness, maxBrightness);
    }
}
