using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Inkom : AreaBase
{
    private readonly LightEntity light;

    private const double minBrightness = 20;
    private const double maxBrightness = 255;

    public Inkom(IHaContext haContext, IScheduler scheduler, ILogger<Inkom> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.Inkom;
        lightControl.AddMaxWhiteLight(light);

        SubscribeToHouseMotion();
        SubscribeToDeconzButton(Button.Inkom1);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        DefaultMotionManualButton([light]);

        lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);
    }

    public override void MotionCleared(string entityId)
    {
        DefaultMotionCleared([light]);
    }

    public override void MotionDetected(string entityId)
    {
        DefaultMotionDetected([light], 0.2);
    }
}
