using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules
{
    public class LuxBasedBrightness : ILuxBasedBrightness
    {
        private readonly NumericSensorEntity luxSensor;
        private readonly IEntities Entities;
        public LuxBasedBrightness(IServiceProvider provider)
        {
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
            luxSensor = Entities.Sensor.LightSensor;
        }

        private readonly List<Tuple<double, double>> luxRanges = new()
        {
            new(0, 20),
            new(20, 50),
            new(50, 100),
            new(100, 200),
            new(200, 500),
            new(500, 1000)
        };

        public double GetBrightness(double minBrightness, double maxBrightness)
        {
            if (luxSensor.State == null)
            {
                return maxBrightness;
            }

            var currentLux = GetLux();
            var levelRange = maxBrightness - minBrightness;
            var luxRange = GetLuxRange(currentLux);
            double brightness;

            if (luxRange == 0)
            {
                brightness = minBrightness;
            }
            else if (luxRange == luxRanges.Count)
            {
                brightness = maxBrightness;
            }
            else
            {
                brightness = Math.Round(minBrightness + luxRange * (levelRange / luxRanges.Count), 1);
            }

            return brightness;
        }

        public double GetLux()
        {
            if (luxSensor.State == null)
            {
                return 1;
            }
            return luxSensor.State.Value;
        }

        private int GetLuxRange(double lux)
        {
            foreach (var luxRange in luxRanges.Select((item, index) => (item, index)))
            {
                if (lux >= luxRange.item.Item1 && lux < luxRange.item.Item2)
                {
                    return luxRange.index;
                }
            }
            return luxRanges.Count;
        }
    }
}
