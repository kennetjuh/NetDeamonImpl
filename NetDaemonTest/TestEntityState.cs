using NetDaemon.HassModel.Entities;

namespace NetDaemonTest;

public record TestEntityStateLightAttributes : EntityState
{
    public double? Brightness;
    public double? ColorTemp;
    public double? MaxMireds;
    public double? MinMireds;
}