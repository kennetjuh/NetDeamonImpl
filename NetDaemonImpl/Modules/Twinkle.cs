using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonImpl.Modules
{
    public class Twinkle : ITwinkle
    {
        private readonly TimeSpan sleepTime;
        private const int nrOfToggleLamps = 1;

        private readonly List<LightEntity> lights;
        private Task? twinkleTask;
        private CancellationTokenSource cts;
        private readonly Random random;

        private readonly IEntities Entities;
        private readonly ILightControl lightControl;

        public Twinkle(IServiceProvider provider, ILightControl lightControl, IDelayProvider delayProvider, int? seed = null)
        {
            this.lightControl = lightControl;
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
            sleepTime = delayProvider.TwinkleDelay;

            lights = new()
            {
                Entities.Light.KeukenM1,
                Entities.Light.KeukenM2,
                Entities.Light.KeukenM3,
                Entities.Light.KeukenM4,
                Entities.Light.KeukenM5,
                Entities.Light.KeukenS1,
                Entities.Light.KeukenS2,
                Entities.Light.KeukenS3,
            };
            cts = new();

            if(seed == null)
            {
                seed = (int)DateTime.Now.Ticks;
            }
            random = new(seed.Value);
        }

        public bool IsActive()
        {
            return twinkleTask != null;
        }

        public void Start()
        {
            if (IsActive())
            {
                Stop();
            }
            cts = new CancellationTokenSource();
            twinkleTask = Task.Run(() => TwinkleImpl(), cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();
            twinkleTask = null;
        }

        public void TwinkleImpl()
        {
            try
            {
                var onLamps = new List<LightEntity>();

                // Loop lights in order to force startup brightness (use per light in order to not trigger the stop on the group entity)
                foreach (var light in lights)
                {
                    lightControl.SetLight(light, 1);
                }

                while (!cts.IsCancellationRequested)
                {
                    //recheck number of onlamps
                    var nrOfOnLamps = Helper.GetDayNightState(Entities) == DayNightEnum.Day ? 3 : 1;

                    //Add lights
                    while (onLamps.Count < nrOfOnLamps + nrOfToggleLamps)
                    {
                        var rand = random.Next(0, lights.Count);
                        if (onLamps.All(x => x.EntityId != lights[rand].EntityId))
                        {
                            onLamps.Add(lights[rand]);
                        }
                    }

                    //Remove lights                    
                    while (onLamps.Count > nrOfOnLamps)
                    {
                        onLamps.RemoveAt(0);
                    }

                    foreach (var light in lights)
                    {
                        if (onLamps.Any(x => x.EntityId == light.EntityId))
                        {
                            lightControl.SetLight(light, 1);
                        }
                        else
                        {
                            lightControl.SetLight(light, 0);
                        }
                    }
                    Task.Delay(sleepTime, cts.Token).Wait();
                }

            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }
    }
}
