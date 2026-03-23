using NetDaemon.HassModel.Entities;
using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Keuken : AreaBase
{
    private const double minBrightness = 50;
    private const double maxBrightness = 255;
    private readonly LightEntity eiland;
    private readonly LightEntity keuken;

    public Keuken(IHaContext haContext, IScheduler scheduler, ILogger<Keuken> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        eiland = _entities.Light.LightKeukenEiland;
        keuken = _entities.Light.LightKeuken;

        SubscribeToHouseMotion();
        SubscribeToDeconzButton(Button.Keuken1);
        SubscribeToDeconzButton(Button.Keuken2);
        SubscribeToDeconzButton(Button.Keuken3);
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        DefaultMotionManualButton([keuken, eiland]);

        var brightness = lightControl.LuxBasedBrightness.GetBrightness(minBrightness, maxBrightness);
        switch (buttonEvent.Event)
		{
            case ButtonEventType.Single:
                if (eiland.IsOff() && keuken.IsOff())
                {
                    lightControl.SetLight(eiland, brightness);
                    lightControl.SetLight(keuken, brightness);
                }
                else
                {
                    if (eiland.IsOn() && keuken.IsOff())
                    {
                        lightControl.SetLight(eiland, 0);
                        lightControl.SetLight(keuken, 0);
                    }
                    else
                    {
                        lightControl.SetLight(eiland, brightness);
                        lightControl.SetLight(keuken, 0);
                    }
                }
                break;
            case ButtonEventType.Double:
                lightControl.ButtonDefault(buttonEvent.Event, keuken);
                lightControl.ButtonDefault(buttonEvent.Event, eiland);
                break;
            case ButtonEventType.LongPress:
                lightControl.ButtonDefault(buttonEvent.Event, keuken);
                lightControl.ButtonDefault(buttonEvent.Event, eiland);
                break;
            default:
                break;
        }
    }

    public override void MotionCleared(string entityId)
    {
        DefaultMotionCleared([keuken]);
    }

    public override void MotionDetected(string entityId)
    {
        DefaultMotionDetected([keuken, eiland], 20);
    }
}
