using NetDaemonInterface;
using System.Collections.Generic;

namespace NetDaemonImpl;

public class DeconzButtonMapping
{
    public List<Tuple<string, AreaControlEnum, string>> mapping;

    public DeconzButtonMapping(IEntities _entities)
    {
        mapping = new()
        {
            new("button_keuken_1", AreaControlEnum.Keuken, _entities.Sensor.ButtonKeuken1Battery.EntityId),
            new("button_keuken_2", AreaControlEnum.Keuken, _entities.Sensor.ButtonKeuken2Battery.EntityId),
            new("button_woonkamer", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonWoonkamerBattery.EntityId),
            new("button_slaapkamerbed", AreaControlEnum.Slaapkamer, _entities.Sensor.ButtonSlaapkamerbedBattery.EntityId),
            new("button_wc", AreaControlEnum.Wc, _entities.Sensor.ButtonWcBattery.EntityId),
            new("button_badkamer", AreaControlEnum.Badkamer, _entities.Sensor.ButtonBadkamerBattery.EntityId),
            new("button_speelkamer", AreaControlEnum.Speelkamer, _entities.Sensor.ButtonSpeelkamerBattery.EntityId),
            new("button_buitenachter", AreaControlEnum.BuitenAchter, _entities.Sensor.ButtonBuitenachterBattery.EntityId),
            new("button_slaapkamer", AreaControlEnum.Slaapkamer, _entities.Sensor.ButtonSlaapkamerBattery.EntityId),
            new("button_halboven_1", AreaControlEnum.HalBoven, _entities.Sensor.ButtonHalboven1Battery.EntityId),
            new("button_halboven_2", AreaControlEnum.HalBoven, _entities.Sensor.ButtonHalboven2Battery.EntityId),
            new("button_cabine", AreaControlEnum.Cabine, _entities.Sensor.ButtonCabineBattery.EntityId),
            new("button_hal", AreaControlEnum.Hal, _entities.Sensor.ButtonHalBattery.EntityId),
            new("button_kamerlamp", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonKamerlampBattery.EntityId),
            new("button_bureaulamp", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonBureaulampBattery.EntityId),
            new("button_slaapkamerkids", AreaControlEnum.SlaapkamerKids, _entities.Sensor.ButtonSlaapkamerkidsBattery.EntityId),
            new("button_booglamp", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonBooglampBattery.EntityId),
            new("button_washal", AreaControlEnum.Washal, _entities.Sensor.ButtonWashalBattery.EntityId),
            new("button_buitenachterlamp", AreaControlEnum.BuitenAchter, _entities.Sensor.ButtonBuitenachterlampBattery.EntityId),
            new("button_buitenachterzithoek", AreaControlEnum.BuitenAchter, _entities.Sensor.ButtonBuitenachterzithoekBattery.EntityId),
            new("button_traphal", AreaControlEnum.Traphal, _entities.Sensor.ButtonTraphalBattery.EntityId),
            new("button_hangstoel", AreaControlEnum.BuitenAchter, _entities.Sensor.Buttonhangstoel.EntityId),
        };
    }
}
