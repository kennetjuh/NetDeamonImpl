using NetDaemon.HassModel.Entities;

namespace NetDaemonTest.TestLights;

public record TestSun : SunEntity
{
    public new string? State;
    public double? Elevation;
    public bool? Rising;

    public TestSun(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    public override EntityState<SunAttributes>? EntityState
    {
        get
        {
            var state = new EntityState<SunAttributes>(new EntityState() { State = State });
            return state;
        }
    }
    public override SunAttributes? Attributes
    {
        get
        {
            var attributes = new SunAttributes()
            {
                Elevation = Elevation,
                Rising = Rising,
            };
            return attributes;
        }
    }
}