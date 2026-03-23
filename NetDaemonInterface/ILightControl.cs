using HomeAssistantGenerated;
using NetDaemonInterface.Enums;
using System.Collections.Generic;

namespace NetDaemonInterface;

public interface ILightControl
{
    /// <summary>
    /// Provide access to the luxBasedBrightness
    /// </summary>
    public ILuxBasedBrightness LuxBasedBrightness { get; }

    /// <summary>
    /// Add a light to the list of lights which are set to 'cold' when max brightness is reached.
    /// </summary>
    /// <param name="light">The light to add</param>
    void AddMaxWhiteLight(LightEntity light);

    void AddMaxCustomColorLight(LightEntity light, List<int> rgb);

    /// <summary>
    /// Add a light to the list of lights which are allways set to 'cold'
    /// </summary>
    /// <param name="light">The light to add</param>
    void AddAllwaysWhiteLight(LightEntity light);

    /// <summary>
    /// Default button behaviour
    /// Single click
    ///     Turn off light when on, Turn on when off (no brightness supplied so on to last state)
    /// Double click
    ///     Increase brightness when light is on, Turn on light to half brightness when off
    /// Long click
    ///     Decrease brightness when light is on, Turn on light to full brightness when off
    /// </summary>
    /// <param name="deconzEventId">The event id from the deconz event (single,double,etc)</param>
    /// <param name="light">The light to control</param>
    /// <returns>true if the light is on, false otherwise</returns>
    bool ButtonDefault(ButtonEventType deconzEventId, LightEntity light);

    /// <summary>
    /// Default button behaviour but single click turn on uses the lux based brightness
    /// Single click
    ///     Turn off light when on, Turn on using lux based brightness when off
    /// Double click
    ///     Increase brightness when light is on, Turn on light to half brightness when off
    /// Long click
    ///     Decrease brightness when light is on, Turn on light to full brightness when off
    /// </summary>
    /// <param name="deconzEventId">The event id from the deconz event (single,double,etc)</param>
    /// <param name="light">The light to control</param>
    /// <param name="minBrightness">The minimum brightness</param>
    /// <param name="maxBrightness">The maximum brightness</param>
    /// <returns>true if the light is on, false otherwise</returns>
    bool ButtonDefaultLuxBased(ButtonEventType deconzEventId, LightEntity light, double minBrightness, double maxBrightness);

    /// <summary>
    /// A routine used to turn lights on, off set the brightness and colortemp
    /// 
    /// By default all lights are turned on using the warmest color available
    /// When the current light is in the MaxWhiteLight list it will turn to white light when max light is requested again
    /// If a light is on white and a decrease is requested it will stay on full but the color goes to warm again
    /// 
    /// Brightness is null means the light will turn on without providing brightness
    /// Brightness is 0 means the light will turn off
    /// other brightness will turn on providing the brightness
    /// </summary>
    /// <param name="light">The light to change</param>
    /// <param name="brightness">The brightness</param>
    /// <returns>true if the light is on, false otherwise</returns>
    public bool SetLight(LightEntity light, double? brightness = null);

    public bool SetLightColorName(LightEntity light, double brightness, string colorName);
    
    public bool SetLightRgb(LightEntity light, double brightness, List<int> rgb);

    public bool SetLightKelvin(LightEntity light, double brightness, int colorTemp);

    public bool SetLightXY(LightEntity light, double brightness, List<double> xyColor);
}
