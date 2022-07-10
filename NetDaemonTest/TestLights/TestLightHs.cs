using System.Collections.Generic;

namespace NetDaemonTest.TestLights;

public record TestLightHs : TestLightBase
{
    public TestLightHs(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    public override LightAttributes? Attributes
    {
        get
        {
            var attributes = new LightAttributes()
            {
                Brightness = Brightness,
                ColorTemp = ColorTemp,
                MaxMireds = MaxMireds,
                MinMireds = MinMireds,
                SupportedColorModes = new List<string> { "hs" }
            };
            return attributes;
        }
    }
}