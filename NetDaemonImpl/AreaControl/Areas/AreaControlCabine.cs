using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlCabine : AreaControl
{
    private readonly LightEntity light;
    private readonly SwitchEntity lightSier;

    private const double minBrightness = 50;
    private const double maxBrightness = 150;

    public AreaControlCabine(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.Cabine;
        lightSier = entities.Switch.SwitchSierCabine;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        if (ButtonSensor == entities.Sensor.ButtonCabineBattery.EntityId)
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
        if(ButtonSensor == entities.Sensor.ButtonCabine1Battery.EntityId)
        {
            if(eventId == DeconzEventIdEnum.Single)
            {
                if(light.IsOff() && lightSier.IsOff())
                {        
                    lightControl.SetLight(light, lightControl.LuxBasedBrightness.GetBrightness(minBrightness, maxBrightness));
                    lightSier.TurnOn();
                }
                else if(light.IsOff() && lightSier.IsOn())
                {
                    lightControl.SetLight(light, lightControl.LuxBasedBrightness.GetBrightness(minBrightness, maxBrightness));
                }
                else
                {
                    light.TurnOff();
                }
            }
            else if (eventId == DeconzEventIdEnum.Double)
            {
                lightSier.TurnOn();
                lightControl.SetLight(light, 255, "darkorange");

            }
            else if (eventId == DeconzEventIdEnum.LongPress)
            {
                lightSier.TurnOn();
                lightControl.SetLight(light, 255, "blue");
            }
        }
    }
}
