using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlBuitenAchter : AreaControl
{
    private readonly LightEntity lightBuiten;
    private readonly LightEntity lightSier;
    private readonly LightEntity lightHangstoel;
    private readonly SwitchEntity switchInfinity;
    private readonly SwitchEntity switchfontein;

    //private const double minBrightness = 10;
    //private const double maxBrightness = 255;

    public AreaControlBuitenAchter(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        lightBuiten = entities.Light.BuitenachterLamp;
        lightSier = entities.Light.BuitenachterSierverlichting;
        switchInfinity = entities.Switch.SwitchInfinityMirror;
        switchfontein = entities.Switch.SwitchFontein;
        lightHangstoel = entities.Light.BuitenachterHangstoel;

        lightControl.AddAllwaysWhiteLight(lightHangstoel);
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        //if (ButtonSensor == entities.Sensor.ButtonBuitenachterlampBattery.EntityId)
        //{
        //    lightControl.ButtonDefaultLuxBased(eventId, lightBuiten, minBrightness, maxBrightness);
        //}
        if(ButtonSensor == entities.Sensor.Buttonhangstoel.EntityId)
        {
            lightControl.ButtonDefaultLuxBased(eventId, lightHangstoel, 100, 255);
        }

        if (ButtonSensor == entities.Sensor.ButtonBuitenachterBattery.EntityId ||
            ButtonSensor == entities.Sensor.ButtonBuitenachterzithoekBattery.EntityId ||
            ButtonSensor == entities.Sensor.ButtonBuitenachterlampBattery.EntityId)
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
                        lightControl.SetLight(entities.Light.Buitenachter3, 1);
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
