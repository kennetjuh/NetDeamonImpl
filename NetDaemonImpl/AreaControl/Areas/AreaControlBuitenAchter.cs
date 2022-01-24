using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlBuitenAchter : AreaControl
{
    private readonly LightEntity lightBuiten;
    private readonly LightEntity lightSier;
    private readonly SwitchEntity switchInfinity;
    private readonly SwitchEntity switchfontein;

    private const double minBrightness = 10;
    private const double maxBrightness = 255;

    public AreaControlBuitenAchter(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        lightBuiten = entities.Light.BuitenachterLamp;
        lightSier = entities.Light.BuitenachterSierverlichting;
        switchInfinity = entities.Switch.SwitchInfinityMirror;
        switchfontein = entities.Switch.BuitenachterFontein;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        if (ButtonSensor == entities.Sensor.ButtonBuitenachterlampBatteryLevel.EntityId)
        {
            lightControl.ButtonDefaultLuxBased(eventId, lightBuiten, minBrightness, maxBrightness);
        }
        if (ButtonSensor == entities.Sensor.ButtonBuitenachterBatteryLevel.EntityId ||
            ButtonSensor == entities.Sensor.ButtonBuitenachterzithoekBatteryLevel.EntityId)
        {
            switch (eventId)
            {
                case DeconzEventIdEnum.Single:
                    if (switchInfinity.IsOn())
                    {
                        lightControl.SetLight(lightSier, 0);
                        switchInfinity.TurnOff();
                    }
                    else
                    {
                        lightControl.SetLight(lightSier, 1);
                        switchInfinity.TurnOn();
                    }
                    break;
                case DeconzEventIdEnum.Double:
                    switchfontein.Toggle();
                    break;
                case DeconzEventIdEnum.LongPress:
                    if (lightBuiten.IsOff())
                    {
                        lightControl.SetLight(lightBuiten, 255);
                    }
                    else
                    {
                        lightControl.SetLight(lightBuiten, 0);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
