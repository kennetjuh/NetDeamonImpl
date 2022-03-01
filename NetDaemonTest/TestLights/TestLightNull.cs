namespace NetDaemonTest.TestLights;

public record TestLightNull : TestLightBase
{
    public TestLightNull(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    public override LightAttributes? Attributes
    {
        get
        {
            return null;
        }
    }
}