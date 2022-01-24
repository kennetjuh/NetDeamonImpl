using NetDaemon.HassModel.Entities;
using System.Collections.Generic;

namespace NetDaemonTest.TestLights;

public record TestLightBase : LightEntity
{
    public new string? State;
    public double? Brightness;
    public double? ColorTemp;
    public double? MaxMireds;
    public double? MinMireds;
    public List<Tuple<string, object>> ServiceCalls = new();

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
    public override void CallService(string service, object? data = null)
    {
        if (data == null)
        {
            throw new Exception("Invalid data recieved");
        }
        ServiceCalls.Add(new(service, data));
    }
}