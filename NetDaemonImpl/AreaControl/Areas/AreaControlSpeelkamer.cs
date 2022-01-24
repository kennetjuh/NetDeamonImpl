using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlSpeelkamer : AreaControl
{
    private readonly LightEntity light;

    private const double minBrightness = 150;
    private const double maxBrightness = 255;

    public AreaControlSpeelkamer(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.SpeelkamerLamp;
        lightControl.AddMaxWhiteLight(light);
    }

    public override void ButtonPressed(string entityId, DeconzEventIdEnum eventId)
    {
        lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness);
    }
}
