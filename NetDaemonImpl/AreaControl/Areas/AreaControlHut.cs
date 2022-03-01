using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlHut : AreaControl
{
    private readonly LightEntity light;
    private readonly LightEntity sier;

    public AreaControlHut(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.LightHut;
        sier = entities.Light.BuitenzijHutsier;
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        var isOn = lightControl.ButtonDefault(eventId, light);
        if (isOn)
        {
            sier.TurnOn();
        }
        else
        {
            sier.TurnOff();
        }
    }

}
