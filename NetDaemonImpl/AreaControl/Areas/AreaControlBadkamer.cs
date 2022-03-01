using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlBadkamer : AreaControl
{
    private readonly LightEntity light;

    private const double minBrightness = 10;
    private const double maxBrightness = 255;

    public AreaControlBadkamer(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.Badkamer;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness);
    }
}
