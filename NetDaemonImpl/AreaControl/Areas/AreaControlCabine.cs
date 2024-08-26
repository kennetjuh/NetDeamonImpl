using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlCabine : AreaControl
{
    private readonly LightEntity spots;
    private readonly LightEntity plafond;
    private readonly SwitchEntity lightSier;
    private readonly ILuxBasedBrightness luxBasedBrightness;
    private const double minBrightness = 50;
    private const double maxBrightness = 150;

    public AreaControlCabine(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl, ILuxBasedBrightness luxBasedBrightness) : base(entities, delayProvider, lightControl)
    {
        spots = entities.Light.Cabine;
        plafond = entities.Light.Cabineplafond;
        lightSier = entities.Switch.SwitchSierCabine;
        this.luxBasedBrightness = luxBasedBrightness;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        if (ButtonSensor == entities.Sensor.ButtonCabineBattery.EntityId)
        {
            if (lightControl.ButtonDefaultLuxBased(eventId, spots, minBrightness, maxBrightness))
            {
                var brightness = luxBasedBrightness.GetBrightness(minBrightness, maxBrightness);
                lightControl.SetLight(plafond, brightness);
                lightSier.TurnOn();
            }
            else
            {
                lightControl.SetLight(plafond, 0);
                lightSier.TurnOff();
            }
        }
    }
}
