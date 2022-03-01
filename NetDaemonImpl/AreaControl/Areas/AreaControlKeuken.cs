using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlKeuken : AreaControl
{
    private readonly LightEntity light;
    private readonly ITwinkle twinkle;

    public AreaControlKeuken(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl, ITwinkle twinkle) : base(entities, delayProvider, lightControl)
    {
        this.twinkle = twinkle;
        light = entities.Light.KeukenKeukenlamp;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        var isOff = light.IsOff();
        var isTwinkeActive = twinkle.IsActive();

        switch (eventId)
        {
            case DeconzEventIdEnum.Single:
                if (isTwinkeActive)
                {
                    lightControl.SetLight(light, 0);
                }
                else if (isOff)
                {
                    if (Helper.GetDayNightState(entities) == DayNightEnum.Night)
                    {
                        lightControl.SetLight(light, 1);
                    }
                    else
                    {
                        lightControl.SetLight(light, 10);
                    }
                }
                else
                {
                    twinkle.Start();
                }
                break;
            case DeconzEventIdEnum.Double:
                if (isTwinkeActive || isOff)
                {
                    if (Helper.GetDayNightState(entities) == DayNightEnum.Night)
                    {
                        lightControl.SetLight(light, 1);
                    }
                    else
                    {
                        lightControl.SetLight(light, 20);
                    }
                }
                else
                {
                    lightControl.ButtonDefault(eventId, light);
                }
                break;
            case DeconzEventIdEnum.LongPress:
                if (isTwinkeActive || isOff)
                {
                    lightControl.SetLight(light, 100);
                }
                else
                {
                    lightControl.ButtonDefault(eventId, light);
                }
                break;
            default:
                break;
        }
    }
}
