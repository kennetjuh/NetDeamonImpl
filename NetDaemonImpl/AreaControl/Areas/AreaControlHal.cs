using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlHal : AreaControl
{
    private readonly LightEntity light;

    private const double minBrightness = 50;
    private const double maxBrightness = 255;

    public AreaControlHal(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.LightHal;
    }

    public override void ButtonPressed(string entityId, DeconzEventIdEnum eventId)
    {
        lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness);
    }
}
