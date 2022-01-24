
namespace NetDaemonInterface;

/// <summary>
/// Interface used for providing inputs/events to area's
/// </summary>
public interface IAreaControl
{
    /// <summary>
    /// Motion is detected
    /// </summary>
    /// <param name="MotionSensor">The sensor which detected motion</param>
    public void MotionDetected(string entityId);

    /// <summary>
    /// Motion is cleared
    /// </summary>
    /// <param name="MotionSensor">The sensor which cleared motion</param>
    public void MotionCleared(string entityId);

    /// <summary>
    /// A button is pressed
    /// </summary>
    /// <param name="ButtonSensor">A sensor on the button which was pressed</param>
    /// <param name="eventId">The deconz button event Id (single, double, etc)</param>
    public void ButtonPressed(string entityId, DeconzEventIdEnum eventId);
}
