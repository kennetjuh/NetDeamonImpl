using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Eetkamer : AreaBase
{
    private readonly LightEntity light;
    private const double minBrightness = 20;
    private const double maxBrightness = 200;

    public Eetkamer(IHaContext haContext, IScheduler scheduler, ILogger<Eetkamer> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        light = _entities.Light.Eetkamer;
        lightControl.AddMaxWhiteLight(light);

        SubscribeToHouseMotion();
        SubscribeToDeconzButton(Button.Eetkamer);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        DefaultMotionManualButton([light]);

        lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);

    }

    public override void MotionCleared(string entityId)
    {
        // Disabled because of kerst
        //DefaultMotionCleared([light]);
    }

    public override void MotionDetected(string entityId)
    {
        // Disabled because of kerst
        //DefaultMotionDetected([light], 0.2);
    }
}
