using System.Collections.Generic;

namespace NetDaemonTest.TestLights;

public record TestLightColorTemp : TestLightBase
{
    public TestLightColorTemp(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    public override LightAttributes? Attributes
    {
        get
        {
            var attributes = new LightAttributes()
            {
                Brightness = Brightness,
                ColorTempKelvin = ColorTemp,
                MaxColorTempKelvin = MaxColorTempKelvin,
                MinColorTempKelvin = MinColorTempKelvin,
                SupportedColorModes = new List<string> { "color_temp" },
            };
            return attributes;
        }
    }
}