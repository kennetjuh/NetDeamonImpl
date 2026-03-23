using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps.Areas
{
    [NetDaemonApp]    
    public class KippenHok: AreaBase
    {
        private const double minBrightness = 20;
        private const double maxBrightness = 255;
        private readonly LightEntity light;

        public KippenHok(IHaContext haContext, IScheduler scheduler, ILogger<KippenHok> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
            : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
        {
            light = _entities.Light.LightKip;
            SubscribeToDeconzButton(Button.Kip);
        }

        public override void ButtonPressed(ButtonEvent buttonEvent)
        {
            lightControl.ButtonDefaultLuxBased(buttonEvent.Event, light, minBrightness, maxBrightness);
        }
    }
}
