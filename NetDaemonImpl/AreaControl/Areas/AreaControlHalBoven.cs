using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlHalBoven : AreaControl
{
    private readonly LightEntity light;

    private const double minBrightness = 50;
    private const double maxBrightness = 255;

    public AreaControlHalBoven(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.LightHalboven;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness);
    }
}
