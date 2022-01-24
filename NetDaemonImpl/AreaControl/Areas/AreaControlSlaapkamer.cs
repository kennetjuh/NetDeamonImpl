using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlSlaapkamer : AreaControl
{
    private readonly LightEntity lightKamer;
    private readonly LightEntity lightKamerSingle;
    private readonly LightEntity lightKen;
    private readonly LightEntity lightGreet;

    private const double minBrightness = 10;
    private const double maxBrightness = 255;
    private readonly ModeCycler modeCycler;

    public AreaControlSlaapkamer(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        lightKamer = entities.Light.LightSlaapkamer;
        lightKamerSingle = entities.Light.LightSlaapkamer2;
        lightKen = entities.Light.SlaapkamerNachtlampKen;
        lightGreet = entities.Light.SlaapkamerNachtlampGreet;
        modeCycler = new ModeCycler(delayProvider.ModeCycleTimeout,
            () => 
            {
                lightControl.SetLight(lightKamer, 0);
                lightControl.SetLight(lightKen, 1);
                lightControl.SetLight(lightGreet, 0);
            },
            () =>
            {
                lightControl.SetLight(lightKen, 0);
                lightControl.SetLight(lightKamerSingle, 1);
            },
            () =>
            {
                lightControl.SetLight(lightKamerSingle, 50, "red");
            });
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        if (ButtonSensor == entities.Sensor.ButtonSlaapkamerBatteryLevel.EntityId)
        {
            switch (eventId)
            {
                case DeconzEventIdEnum.Single:
                    if (lightKamer.IsOff() && lightKen.IsOff() && lightGreet.IsOff())
                    {
                        lightControl.ButtonDefaultLuxBased(eventId, lightKamer, minBrightness, maxBrightness);
                        lightControl.SetLight(lightKen, 1);
                        lightControl.SetLight(lightGreet, 1);
                    }
                    else
                    {
                        lightControl.SetLight(lightKamer, 0);
                        lightControl.SetLight(lightKen, 0);
                        lightControl.SetLight(lightGreet, 0);
                    }
                    break;
                case DeconzEventIdEnum.Double:
                case DeconzEventIdEnum.LongPress:
                    {
                        lightControl.ButtonDefault(eventId, lightKamer);
                        lightControl.SetLight(lightKen, 1);
                        lightControl.SetLight(lightGreet, 1);
                    }
                    break;
                default:
                    break;
            }
        }
        if (ButtonSensor == entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId)
        {
            switch (eventId)
            {
                case DeconzEventIdEnum.Single:
                    if (lightKamer.IsOn())
                    {
                        lightControl.SetLight(lightKamer, 0);
                    }
                    else
                    {
                        if (lightKen.IsOff() && lightGreet.IsOff())
                        {
                            lightControl.SetLight(lightKen, 1);
                            lightControl.SetLight(lightGreet, 1);
                        }
                        else
                        {
                            lightControl.SetLight(lightKen, 0);
                            lightControl.SetLight(lightGreet, 0);
                        }
                    }
                    break;
                case DeconzEventIdEnum.Double:
                    modeCycler.Cycle();
                    break;
                case DeconzEventIdEnum.LongPress:
                    lightControl.SetLight(lightKamer, 5);
                    lightControl.SetLight(lightKen, 1);
                    lightControl.SetLight(lightGreet, 1);
                    break;
                default:
                    break;
            }
        }

    }
}
