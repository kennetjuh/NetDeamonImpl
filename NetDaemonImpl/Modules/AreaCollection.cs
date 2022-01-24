using NetDaemonImpl.AreaControl.Areas;
using NetDaemonInterface;

namespace NetDaemonImpl.Modules;

public class AreaCollection : IAreaCollection
{
    public IAreaControl Washal { get; }
    public IAreaControl Hal { get; }
    public IAreaControl Badkamer { get; }
    public IAreaControl Traphal { get; }
    public IAreaControl Speelkamer { get; }
    public IAreaControl Wc { get; }
    public IAreaControl Voordeur { get; }
    public IAreaControl SlaapkamerKids { get; }
    public IAreaControl Cabine { get; }
    public IAreaControl HalBoven { get; }
    public IAreaControl Woonkamer { get; }
    public IAreaControl Keuken { get; }
    public IAreaControl Slaapkamer { get; }
    public IAreaControl BuitenAchter { get; }

    public AreaCollection(IServiceProvider serviceProvider, IDelayProvider delayProvider, ILightControl lightControl, IHouseState houseState, ITwinkle twinkle)
    {
        var haContext = DiHelper.GetHaContext(serviceProvider);
        var entities = new Entities(haContext);

        Washal = new AreaControlWashal(entities, delayProvider, lightControl);
        Hal = new AreaControlHal(entities, delayProvider, lightControl);
        Badkamer = new AreaControlBadkamer(entities, delayProvider, lightControl);
        Traphal = new AreaControlTraphal(entities, delayProvider, lightControl);
        Speelkamer = new AreaControlSpeelkamer(entities, delayProvider, lightControl);
        Wc = new AreaControlWc(entities, delayProvider, lightControl);
        Voordeur = new AreaControlVoordeur(entities, delayProvider, lightControl);
        SlaapkamerKids = new AreaControlSlaapkamerKids(entities, delayProvider, lightControl);
        Cabine = new AreaControlCabine(entities, delayProvider, lightControl);
        HalBoven = new AreaControlHalBoven(entities, delayProvider, lightControl);
        Woonkamer = new AreaControlWoonkamer(entities, delayProvider, lightControl, houseState);
        Keuken = new AreaControlKeuken(entities, delayProvider, lightControl, twinkle);
        Slaapkamer = new AreaControlSlaapkamer(entities, delayProvider, lightControl);
        BuitenAchter = new AreaControlBuitenAchter(entities, delayProvider, lightControl);
    }
}
