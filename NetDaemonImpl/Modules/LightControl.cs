using NetDaemonInterface;
using NetDaemonInterface.Enums;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules;

public class LightControl : ILightControl
{
    private readonly Dictionary<LightEntity, List<int>> lightEntitiesCustomMaxColor = new();
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

    public void AddMaxCustomColorLight(LightEntity light, List<int> rgb)
    {
        lightEntitiesCustomMaxColor.Add(light, rgb);
    }

    /// <inheritdoc/> 
    public bool ButtonDefaultLuxBased(ButtonEventType deconzEventId, LightEntity light, double minBrightness, double maxBrightness)
    {
        return deconzEventId switch
        {
            ButtonEventType.Single => ButtonSingleClickLuxBased(light, minBrightness, maxBrightness),
            ButtonEventType.Double => ButtonDoubleClick(light),
            ButtonEventType.LongPress => ButtonLongPress(light),
            _ => false,
        };
    }

    /// <inheritdoc/>   
    public bool ButtonDefault(ButtonEventType deconzEventId, LightEntity light)
    {
        return deconzEventId switch
        {
            ButtonEventType.Single => ButtonSingleClick(light),
            ButtonEventType.Double => ButtonDoubleClick(light),
            ButtonEventType.LongPress => ButtonLongPress(light),
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
        var colorTempKelvin = lightEntitiesAllwaysWhite.Any(x => x.EntityId == light.EntityId) ? light.Attributes?.MaxColorTempKelvin : light.Attributes?.MinColorTempKelvin;
        var currentBrightness = light.Attributes?.Brightness;
        var currentColorTemp = light.Attributes?.ColorTempKelvin;

        if (supportedModes == null)
        {
            Logger.LogWarning($"Entity {light.EntityId} has no supported modes");
            return false;
        }

        if (supportedModes.Contains("color_temp"))
        {
            if (!brightness.HasValue)
            {
                light.TurnOn(colorTempKelvin: Convert.ToInt64(colorTempKelvin));
                return true;
            }
            else
            {
                if (lightEntitiesCustomMaxColor.Any(x => x.Key.EntityId == light.EntityId))
                {
                    // set color to custom color when max is reached and asked again or when long pressed from off
                    if ((currentBrightness == 255) && brightness == 255)
                    {
                        light.TurnOn(brightness: (long)brightness, rgbColor: lightEntitiesCustomMaxColor[light]);
                        return true;
                    }
                    //set brightness back to 255 when color is white to go back to red color on max brightness
                    else if (currentColorTemp == light.Attributes?.MaxColorTempKelvin && brightness != 0)
                    {
                        brightness = 255;
                    }
                }
                if (lightEntitiesMaxWhite.Any(x => x.EntityId == light.EntityId))
                {
                    // set color to white when max is reached and asked again or when long pressed from off
                    if ((currentBrightness == 255) && brightness == 255)
                    {
                        colorTempKelvin = light.Attributes?.MaxColorTempKelvin;
                    }
                    //set brightness back to 255 when color is white to go back to red color on max brightness
                    else if (currentColorTemp == light.Attributes?.MaxColorTempKelvin && brightness != 0)
                    {
                        brightness = 255;
                    }
                }
                if (brightness > 0)
                {
                    light.TurnOn(brightness: (long)brightness, colorTempKelvin: Convert.ToInt64(colorTempKelvin));
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

    public bool SetLightKelvin(LightEntity light, double brightness, int colorTempKelvin)
    {
        var supportedModes = light.Attributes?.SupportedColorModes;

        if (supportedModes == null || !supportedModes.Contains("color_temp"))
        {
            Logger.LogWarning($"Entity {light.EntityId} has no 'color_temp' mode");
            return false;
        }

        if (brightness > 0)
        {
            light.TurnOn(brightness: (long)brightness, colorTempKelvin: Convert.ToInt64(colorTempKelvin));
            return true;
        }
        else
        {
            light.TurnOff();
            return false;
        }
    }

    public bool SetLightXY(LightEntity light, double brightness, List<double> xyColor)
    {
        var supportedModes = light.Attributes?.SupportedColorModes;

        if (supportedModes == null || !supportedModes.Contains("xy"))
        {
            Logger.LogWarning($"Entity {light.EntityId} has no 'xy' mode");
            return false;
        }

        if (brightness > 0)
        {
            light.TurnOn(brightness: (long)brightness, xyColor: xyColor);
            return true;
        }
        else
        {
            light.TurnOff();
            return false;
        }
    }

    public bool SetLightColorName(LightEntity light, double brightness, string colorName)
    {
        var supportedModes = light.Attributes?.SupportedColorModes;

        if (supportedModes == null || !supportedModes.Contains("hs"))
        {
            Logger.LogWarning($"Entity {light.EntityId} has no 'hs' mode");
            return false;
        }

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

    public bool SetLightRgb(LightEntity light, double brightness, List<int> rgb)
    {
        var supportedModes = light.Attributes?.SupportedColorModes;

        if (supportedModes == null || !supportedModes.Contains("hs"))
        {
            Logger.LogWarning($"Entity {light.EntityId} has no 'hs' mode");
            return false;
        }

        if (brightness > 0)
        {
            light.TurnOn(brightness: (long)brightness, rgbColor: rgb);
            return true;
        }
        else
        {
            light.TurnOff();
            return false;
        }
    }
}
