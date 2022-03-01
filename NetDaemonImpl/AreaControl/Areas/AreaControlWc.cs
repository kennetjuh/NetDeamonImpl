using NetDaemon.HassModel.Entities;
using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlWc : AreaControl
{
    private readonly LightEntity light;
    private readonly LightEntity singleLight;

    private const double minBrightness = 1;
    private const double maxBrightness = 100;

    public AreaControlWc(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.WcWclamp;
        singleLight = entities.Light.Wc1;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        var lux = lightControl.LuxBasedBrightness.GetLux();

        // When a single click is performed, the light is off and it's dark only turn on a single light
        if (eventId == DeconzEventIdEnum.Single && lux <= 1 && light.IsOff())
        {
            lightControl.SetLight(singleLight, minBrightness);
        }
        else
        {
            lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness);
        }
    }
}
