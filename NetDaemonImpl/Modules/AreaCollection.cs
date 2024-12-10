using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;
using System.Collections.Generic;

namespace NetDaemonImpl.Modules;

public class AreaCollection : IAreaCollection
{
    private readonly Dictionary<AreaControlEnum, IAreaControl> collection = new();

    public AreaCollection(IServiceProvider serviceProvider, IDelayProvider delayProvider, ILightControl lightControl, ILuxBasedBrightness luxBasedBrightness, IHouseState houseState, ITwinkle twinkle)
    {
        var haContext = DiHelper.GetHaContext(serviceProvider);
        var entities = new Entities(haContext);

        collection.Add(AreaControlEnum.Washal, new AreaControlWashal(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Hal, new AreaControlHal(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Badkamer, new AreaControlBadkamer(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Traphal, new AreaControlTraphal(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Speelkamer, new AreaControlSpeelkamer(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Wc, new AreaControlWc(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Voordeur, new AreaControlVoordeur(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.SlaapkamerKids, new AreaControlSlaapkamerKids(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Cabine, new AreaControlCabine(entities, delayProvider, lightControl, luxBasedBrightness));
        collection.Add(AreaControlEnum.HalBoven, new AreaControlHalBoven(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.Woonkamer, new AreaControlWoonkamer(entities, delayProvider, lightControl, houseState));
        collection.Add(AreaControlEnum.Keuken, new AreaControlKeuken(entities, delayProvider, lightControl, twinkle));
        collection.Add(AreaControlEnum.Slaapkamer, new AreaControlSlaapkamer(entities, delayProvider, lightControl));
        collection.Add(AreaControlEnum.BuitenAchter, new AreaControlBuitenAchter(entities, delayProvider, lightControl));
    }

    public IAreaControl GetArea(AreaControlEnum area)
    {
        if (!collection.ContainsKey(area))
        {
            throw new ArgumentException("unknown area");
        }
        return collection[area];
    }
}
