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
            new("button_keuken_1", AreaControlEnum.Keuken, _entities.Sensor.ButtonKeuken1BatteryLevel.EntityId),
            new("button_keuken_2", AreaControlEnum.Keuken, _entities.Sensor.ButtonKeuken2BatteryLevel.EntityId),
            new("button_woonkamer", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonWoonkamerBatteryLevel.EntityId),
            new("button_slaapkamerbed", AreaControlEnum.Slaapkamer, _entities.Sensor.ButtonSlaapkamerbedBatteryLevel.EntityId),
            new("button_wc", AreaControlEnum.Wc, _entities.Sensor.ButtonWcBatteryLevel.EntityId),
            new("button_badkamer", AreaControlEnum.Badkamer, _entities.Sensor.ButtonBadkamerBatteryLevel.EntityId),
            new("button_speelkamer", AreaControlEnum.Speelkamer, _entities.Sensor.ButtonSpeelkamerBatteryLevel.EntityId),
            new("button_buitenachter", AreaControlEnum.BuitenAchter, _entities.Sensor.ButtonBuitenachterBatteryLevel.EntityId),
            new("button_slaapkamer", AreaControlEnum.Slaapkamer, _entities.Sensor.ButtonSlaapkamerBatteryLevel.EntityId),
            new("button_halboven_1", AreaControlEnum.HalBoven, _entities.Sensor.ButtonHalboven1BatteryLevel.EntityId),
            new("button_halboven_2", AreaControlEnum.HalBoven, _entities.Sensor.ButtonHalboven2BatteryLevel.EntityId),
            new("button_cabine", AreaControlEnum.Cabine, _entities.Sensor.ButtonCabineBatteryLevel.EntityId),
            new("button_hal", AreaControlEnum.Hal, _entities.Sensor.ButtonHalBatteryLevel.EntityId),
            new("button_kamerlamp", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonKamerlampBatteryLevel.EntityId),
            new("button_bureaulamp", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonBureaulampBatteryLevel.EntityId),
            new("button_slaapkamerkids", AreaControlEnum.SlaapkamerKids, _entities.Sensor.ButtonSlaapkamerkidsBatteryLevel.EntityId),
            new("button_booglamp", AreaControlEnum.Woonkamer, _entities.Sensor.ButtonBooglampBatteryLevel.EntityId),
            new("button_washal", AreaControlEnum.Washal, _entities.Sensor.ButtonWashalBatteryLevel.EntityId),
            new("button_buitenachterlamp", AreaControlEnum.BuitenAchter, _entities.Sensor.ButtonBuitenachterlampBatteryLevel.EntityId),
            new("button_buitenachterzithoek", AreaControlEnum.BuitenAchter, _entities.Sensor.ButtonBuitenachterzithoekBatteryLevel.EntityId),
            new("button_traphal", AreaControlEnum.Traphal, _entities.Sensor.ButtonTraphalBatteryLevel.EntityId),
        };
    }
}
