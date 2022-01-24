using NetDaemon.HassModel.Entities;

namespace NetDaemonTest;

public record TestEntityState : EntityState
{
    public double? Brightness;
    public double? ColorTemp;
    public double? MaxMireds;
    public double? MinMireds;

    public override LightAttributes? Attributes
    {
        get
        {
            var attributes = new LightAttributes()
            {
                Brightness = Brightness,
                ColorTemp = ColorTemp,
                MaxMireds = MaxMireds,
                MinMireds = MinMireds
            };
            return attributes;
        }
    }
}