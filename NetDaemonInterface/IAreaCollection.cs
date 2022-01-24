namespace NetDaemonInterface;

public interface IAreaCollection
{
    IAreaControl Badkamer { get; }
    IAreaControl BuitenAchter { get; }
    IAreaControl Cabine { get; }
    IAreaControl Hal { get; }
    IAreaControl HalBoven { get; }
    IAreaControl Keuken { get; }
    IAreaControl Slaapkamer { get; }
    IAreaControl SlaapkamerKids { get; }
    IAreaControl Speelkamer { get; }
    IAreaControl Traphal { get; }
    IAreaControl Voordeur { get; }
    IAreaControl Washal { get; }
    IAreaControl Wc { get; }
    IAreaControl Woonkamer { get; }
}
