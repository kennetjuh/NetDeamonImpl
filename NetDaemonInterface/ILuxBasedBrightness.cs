namespace NetDaemonInterface;

public interface ILuxBasedBrightness
{
    /// <summary>
    /// Returns a brightness based on the current lux value.
    /// The lower the measured light the lower the returned brightness
    /// </summary>
    /// <param name="minBrightness">The minimum brightness</param>
    /// <param name="maxBrightness">The maximum brightness</param>
    /// <returns></returns>
    public double GetBrightness(double minBrightness, double maxBrightness);

    /// <summary>
    /// Get the current LuxLevel
    /// </summary>
    /// <returns>The current lux level</returns>
    public double GetLux();
}
