using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlBuitenAchter : AreaControl
{
    private readonly LightEntity lightBuiten;
    private readonly LightEntity lightHangstoel;

    public AreaControlBuitenAchter(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        lightBuiten = entities.Light.BuitenachterLamp;
        lightHangstoel = entities.Light.BuitenachterHangstoel;

        lightControl.AddAllwaysWhiteLight(lightHangstoel);
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    { 
        if (ButtonSensor == entities.Sensor.Buttonhangstoel.EntityId)
        {
            lightControl.ButtonDefaultLuxBased(eventId, lightHangstoel, 100, 255);
        }

        if (ButtonSensor == entities.Sensor.ButtonBuitenachterBattery.EntityId ||
            ButtonSensor == entities.Sensor.ButtonBuitenachterzithoekBattery.EntityId ||
            ButtonSensor == entities.Sensor.ButtonBuitenachterlampBattery.EntityId)
        {
            {
                lightControl.ButtonDefaultLuxBased(eventId, lightBuiten, 1, 255);
            }
        }

    }
}
