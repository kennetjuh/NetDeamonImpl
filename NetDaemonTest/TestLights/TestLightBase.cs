using NetDaemon.HassModel.Entities;

namespace NetDaemonTest.TestLights;

public record TestLightBase : LightEntity
{
    public new string? State;
    public double? Brightness;
    public double? ColorTemp;
    public double? MaxMireds;
    public double? MinMireds;

    public TestLightBase(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    public override EntityState<LightAttributes>? EntityState
    {
        get
        {
            var state = new EntityState<LightAttributes>(new EntityState() { State = State });
            return state;
        }
    }
}