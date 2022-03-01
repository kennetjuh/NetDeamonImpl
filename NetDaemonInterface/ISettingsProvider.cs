namespace NetDaemonInterface;

public interface ISettingsProvider
{
    public int BrightnessSfeerlampWoonkamerDay { get; }
    public int BrightnessSfeerlampWoonkamerNight { get; }
    public int BrightnessSfeerlampKeukenDay { get; }
    public int BrightnessSfeerlampKeukenNight { get; }
    public int BrightnessSfeerlampHalDay { get; }
    public int BrightnessSfeerlampHalNight { get; }
    public int BrightnessSfeerlampBovenDay { get; }
    public int BrightnessSfeerlampBovenNight { get; }
    bool BeddenAlarmKids { get; }
    bool JimmieAlarm { get; }
}
