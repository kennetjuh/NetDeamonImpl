using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules;

public class LightControl : ILightControl
{
    private readonly List<LightEntity> lightEntitiesMaxWhite = new();
    public ILuxBasedBrightness luxBasedBrightness { get; private set; }

    public LightControl(ILuxBasedBrightness luxBasedBrightness)
    {
        this.luxBasedBrightness = luxBasedBrightness;
    }

    /// <inheritdoc/> 
    public void AddMaxWhiteLight(LightEntity light)
    {
        lightEntitiesMaxWhite.Add(light);
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
            : SetLight(light, luxBasedBrightness.GetBrightness(minBrightness, maxBrightness));
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
        var supportedModes = light.Attributes?.SupportedColorModes?.ToString();
        var colorTemp = light.Attributes?.MaxMireds;
        var currentBrightness = light.Attributes?.Brightness;
        var currentColorTemp = light.Attributes?.ColorTemp;

        if (supportedModes != null && supportedModes.Contains("color_temp"))
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
        else
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
    }

    public bool SetLight(LightEntity light, double? brightness, string colorName)
    {
        var supportedModes = light.Attributes?.SupportedColorModes?.ToString();


        if (supportedModes == null || !supportedModes.Contains("hs"))
        {
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
