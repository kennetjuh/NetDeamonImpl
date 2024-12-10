using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlBuitenAchter : AreaControl
{
    private readonly LightEntity lightBuiten;

    public AreaControlBuitenAchter(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        lightBuiten = entities.Light.BuitenachterLamp;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
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
