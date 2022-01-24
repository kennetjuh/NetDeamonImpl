using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlWoonkamer : AreaControl
{
    private readonly LightEntity boog;
    private readonly LightEntity bureau;
    private readonly LightEntity kamer;
    private readonly IHouseState houseState;
    private const double boogMinBrightness = 50;
    private const double boogmaxBrightness = 150;

    public AreaControlWoonkamer(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl, IHouseState houseState) : base(entities, delayProvider, lightControl)
    {
        this.houseState = houseState;
        boog = entities.Light.WoonkamerBoog;
        bureau = entities.Light.WoonkamerBureau;
        kamer = entities.Light.WoonkamerKamer;
        lightControl.AddMaxWhiteLight(bureau);
        lightControl.AddMaxWhiteLight(kamer);
    }

    public override void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId)
    {
        if (ButtonSensor == entities.Sensor.ButtonBooglampBatteryLevel.EntityId)
        {
            lightControl.ButtonDefaultLuxBased(eventId, boog, boogMinBrightness, boogmaxBrightness);
        }
        if (ButtonSensor == entities.Sensor.ButtonBureaulampBatteryLevel.EntityId)
        {
            lightControl.ButtonDefault(eventId, bureau);
        }
        if (ButtonSensor == entities.Sensor.ButtonKamerlampBatteryLevel.EntityId)
        {
            lightControl.ButtonDefault(eventId, kamer);
        }
        if (ButtonSensor == entities.Sensor.ButtonWoonkamerBatteryLevel.EntityId)
        {
            switch (eventId)
            {
                case DeconzEventIdEnum.Single:
                    houseState.HouseStateAwake();
                    break;
                case DeconzEventIdEnum.Double:
                    houseState.HouseStateAway();
                    break;
                case DeconzEventIdEnum.LongPress:
                    houseState.HouseStateSleeping();
                    break;
                default:
                    break;
            }
        }
    }
}
