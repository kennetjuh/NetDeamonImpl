namespace NetDaemonTest.TestLights;

public record TestLightOnOff : TestLightBase
{
    public TestLightOnOff(IHaContext haContext, string entityId) : base(haContext, entityId)
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
                SupportedColorModes = ""
            };
            return attributes;
        }
    }
}