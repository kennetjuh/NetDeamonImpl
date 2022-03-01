using System.Collections.Generic;

namespace NetDaemonTest.TestLights;

public record TestLightUnsupported : TestLightBase
{
    public TestLightUnsupported(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    public override LightAttributes? Attributes
    {
        get
        {
            var attributes = new LightAttributes()
            {
                SupportedColorModes = new List<string> { "Unsupported" }
            };
            return attributes;
        }
    }
}