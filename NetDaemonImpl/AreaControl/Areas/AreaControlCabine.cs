using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlCabine : AreaControl
{
    private readonly LightEntity light;
    private readonly SwitchEntity lightSier;

    private const double minBrightness = 50;
    private const double maxBrightness = 255;

    public AreaControlCabine(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.LightCabine;
        lightSier = entities.Switch.SwitchSierCabine;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        var isOn = lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness);
        if (isOn)
        {
            lightSier.TurnOn();
        }
        else
        {
            lightSier.TurnOff();
        }
    }
}
