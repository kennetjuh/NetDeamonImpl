using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class DiscoApp : MyNetDaemonBaseApp
{
    private const int MinIntervalMs = 2500;
    private const int MaxIntervalMs = 5000;

    private readonly List<Timer> _timers = [];
    private readonly Dictionary<string, LightSnapshot> _snapshots = [];
    private readonly List<LightEntity> _lights;

    private volatile bool _running;

    public DiscoApp(IHaContext haContext, IScheduler scheduler, INotify notify, ILogger<TimerApp> logger, ISettingsProvider settingsProvider, ILightControl lightControl)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        _lights = haContext.GetAllEntities()
            .Where(x => x.EntityId.StartsWith("light") && x.Registration?.Device?.Name != "Home Assistant Connect ZBT-2")
            .Select(x => new LightEntity(x))
            .Where(x => x.Attributes?.SupportedColorModes?.Contains("hs") == true || x.Attributes?.SupportedColorModes?.Contains("xy") == true || x.Attributes?.SupportedColorModes?.Contains("color_temp") == true)
            .ToList();

        //_lights =
        //[
        //    _entities.Light.LightKeuken,
        //    _entities.Light.LightKeuken1,
        //];

        _entities.InputBoolean.Discomode.StateChanges()
            .Subscribe(x =>
            {
                if (x?.New?.State == "on")
                {
                    StartDisco(lightControl);
                }
                else
                {
                    StopDisco();
                }
            });
    }

    public void StartDisco(ILightControl lightControl)
    {
        if (_running) return;

        _running = true;
        _snapshots.Clear();

        foreach (var light in _lights)
        {
            _snapshots[light.EntityId] = LightSnapshot.Capture(light);

            var timer = new Timer(GetNextIntervalMs())
            {
                AutoReset = false,
            };

            timer.Elapsed += (_, _) =>
            {
                if (!_running) return;

                try
                {
                    if (light.Attributes?.SupportedColorModes?.Contains("hs") == true)
                    {
                        var rgb = new List<int>
                        {
                            Random.Shared.Next(0, 256),
                            Random.Shared.Next(0, 256),
                            Random.Shared.Next(0, 256)
                        };
                        lightControl.SetLightRgb(light, 255, rgb);
                    }
                    else if (light.Attributes?.SupportedColorModes?.Contains("xy") == true)
                    {
                        var xyColor = new List<double>
                        {
                            Random.Shared.NextDouble(),
                            Random.Shared.NextDouble()
                        };
                        lightControl.SetLightXY(light, 255, xyColor);
                    }
                    else
                    {
                        var temp = Random.Shared.Next((int)light.Attributes?.MinColorTempKelvin, (int)light.Attributes?.MaxColorTempKelvin);
                        lightControl.SetLightKelvin(light, 255, temp);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Exception occured during disco mode");
                }
                finally
                {
                    try
                    {
                        if (_running)
                        {
                            timer.Interval = GetNextIntervalMs();
                            timer.Start();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            };

            _timers.Add(timer);
            timer.Start();
        }
    }

    public void StopDisco()
    {
        if (!_running) return;

        _running = false;

        foreach (var timer in _timers)
        {
            timer.Dispose();
        }
        _timers.Clear();

        foreach (var light in _lights)
        {
            if (!_snapshots.TryGetValue(light.EntityId, out var snapshot)) continue;
            snapshot.Restore(light);
        }

        _snapshots.Clear();
    }

    private static int GetNextIntervalMs() => Random.Shared.Next(MinIntervalMs, MaxIntervalMs + 1);

    private sealed record LightSnapshot(bool IsOn, string colorMode, long? Brightness, long? ColorTempKelvin, List<int>? RgbColor, List<int>? XyColor)
    {
        public static LightSnapshot Capture(LightEntity light)
        {
            var brightness = light.Attributes?.Brightness is { } b ? Convert.ToInt64(b) : (long?)null;
            var colorTempKelvin = light.Attributes?.ColorTempKelvin is { } ct ? Convert.ToInt64(ct) : (long?)null;
            var rgbColor = light.Attributes?.RgbColor?.Select(x => Convert.ToInt32(x)).ToList();
            var xyColor = light.Attributes?.XyColor?.Select(x => Convert.ToInt32(x)).ToList();
            var colorMode = light.Attributes?.ColorMode ?? "unknown";

            return new LightSnapshot(light.IsOn(), colorMode, brightness, colorTempKelvin, rgbColor, xyColor);
        }

        public void Restore(LightEntity light)
        {

            if (colorMode == "color_temp")
            {
                light.TurnOn(brightness: Brightness, colorTempKelvin: ColorTempKelvin);
            }
            else if (colorMode == "hs")
            {
                light.TurnOn(brightness: Brightness, rgbColor: RgbColor);
            }
            else if (colorMode == "xy")
            {
                light.TurnOn(brightness: Brightness, xyColor: XyColor);
            }
            else
            {
                light.TurnOn();
            }

            if (!IsOn)
            {
                light.TurnOff();
                return;
            }
        }
    }
}