using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules;

public class LightControl : ILightControl
{
    private readonly List<LightEntity> lightEntitiesMaxWhite = new();
    private readonly List<LightEntity> lightEntitiesAllwaysWhite = new();
    public ILuxBasedBrightness LuxBasedBrightness { get; private set; }
    private ILogger<LightControl> Logger { get; }
    private readonly Entities Entities;
    private readonly Services Services;

    public LightControl(IServiceProvider provider, ILuxBasedBrightness luxBasedBrightness, ILogger<LightControl> logger)
    {
        Logger = logger;
        LuxBasedBrightness = luxBasedBrightness;
        var haContext = DiHelper.GetHaContext(provider);
        Entities = new Entities(haContext);
        Services = new Services(haContext);
    }

    /// <inheritdoc/> 
    public void AddMaxWhiteLight(LightEntity light)
    {
        lightEntitiesMaxWhite.Add(light);
    }

    public void AddAllwaysWhiteLight(LightEntity light)
    {
        lightEntitiesAllwaysWhite.Add(light);
    }

    /// <inheritdoc/> 
    public bool ButtonDefaultLuxBased(DeconzEventIdEnum deconzEventId, LightEntity light, double minBrightness, double maxBrightness)
    {
        return deconzEventId switch
        {
            DeconzEventIdEnum.Single => ButtonSingleClickLuxBased(light, minBrightness, maxBrightness),
            DeconzEventIdEnum.Double => ButtonDoubleClick(light),
            DeconzEventIdEnum.LongPress => ButtonLongPress(light),
            _ => false,
        };
    }

    /// <inheritdoc/>   
    public bool ButtonDefault(DeconzEventIdEnum deconzEventId, LightEntity light)
    {
        return deconzEventId switch
        {
            DeconzEventIdEnum.Single => ButtonSingleClick(light),
            DeconzEventIdEnum.Double => ButtonDoubleClick(light),
            DeconzEventIdEnum.LongPress => ButtonLongPress(light),
            _ => false,
        };
    }

    internal bool ButtonSingleClick(LightEntity light)
    {
        return light.IsOn()
            ? SetLight(light, 0)
            : SetLight(light, null);
    }

    internal bool ButtonSingleClickLuxBased(LightEntity light, double minBrightness, double maxBrightness)
    {
        return light.IsOn()
            ? SetLight(light, 0)
            : SetLight(light, LuxBasedBrightness.GetBrightness(minBrightness, maxBrightness));
    }

    internal bool ButtonDoubleClick(LightEntity light)
    {
        var currentBrightness = light.Attributes?.Brightness;

        return light.IsOn() && currentBrightness.HasValue
            ? SetLight(light, Math.Min(currentBrightness.Value + Constants.doubleClick_increment, 255))
            : SetLight(light, Constants.doubleClick_brightness);
    }

    internal bool ButtonLongPress(LightEntity light)
    {
        var currentBrightness = light.Attributes?.Brightness;

        return light.IsOn() && currentBrightness.HasValue
            ? SetLight(light, Math.Max(currentBrightness.Value - Constants.longPress_decrement, 1))
            : SetLight(light, Constants.longPress_brightness);
    }

    /// <inheritdoc/>   
    public bool SetLight(LightEntity light, double? brightness = null)
    {
        var supportedModes = light.Attributes?.SupportedColorModes;
        var colorTemp = lightEntitiesAllwaysWhite.Any(x => x.EntityId == light.EntityId) ? light.Attributes?.MinMireds : light.Attributes?.MaxMireds;
        var currentBrightness = light.Attributes?.Brightness;
        var currentColorTemp = light.Attributes?.ColorTemp;

        if (supportedModes == null)
        {
            Logger.LogWarning($"Entity {light.EntityId} has no supported modes");
            return false;
        }

        if (supportedModes.Contains("color_temp"))
        {
            if (!brightness.HasValue)
            {
                light.TurnOn(colorTemp: Convert.ToInt64(colorTemp));
                return true;
            }
            else
            {
                if (lightEntitiesMaxWhite.Any(x => x.EntityId == light.EntityId))
                {
                    // set color to white when max is reached and asked again or when long pressed from off
                    if ((currentBrightness == 255 || currentBrightness == null) && brightness == 255)
                    {
                        colorTemp = light.Attributes?.MinMireds;
                    }
                    //set brightness back to 255 when color is white to go back to red color on max brightness
                    else if (currentColorTemp == light.Attributes?.MinMireds && brightness != 0)
                    {
                        brightness = 255;
                    }
                }
                if (brightness > 0)
                {
                    light.TurnOn(brightness: (long)brightness, colorTemp: Convert.ToInt64(colorTemp));
                    return true;
                }
                else
                {
                    light.TurnOff();
                    return false;
                }
            }
        }
        else if (supportedModes.Contains("brightness"))
        {
            if (!brightness.HasValue)
            {
                light.TurnOn();
                return true;
            }
            else if (brightness == 0)
            {
                light.TurnOff();
                return false;
            }
            else
            {
                light.TurnOn(brightness: (long)brightness);
                return true;
            }
        }
        else if (supportedModes.Contains("onoff"))
        {
            if (brightness == 0)
            {
                light.TurnOff();
                return false;
            }
            else
            {
                light.TurnOn();
                return true;
            }
        }
        else
        {
            Logger.LogWarning($"Entity {light.EntityId} has no known supported modes");
            return false;
        }
    }

    public bool SetLight(LightEntity light, double? brightness, string colorName)
    {
        var supportedModes = light.Attributes?.SupportedColorModes;

        if (supportedModes == null || !supportedModes.Contains("hs"))
        {
            Logger.LogWarning($"Entity {light.EntityId} has no 'hs' mode");
            return false;
        }

        if (!brightness.HasValue)
        {
            light.TurnOn(colorName: colorName);
            return true;
        }
        else
        {
            if (brightness > 0)
            {
                light.TurnOn(brightness: (long)brightness, colorName: colorName);
                return true;
            }
            else
            {
                light.TurnOff();
                return false;
            }
        }
    }
}
