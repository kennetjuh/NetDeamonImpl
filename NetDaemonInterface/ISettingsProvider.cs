namespace NetDaemonInterface;

public interface ISettingsProvider
{
    public int BrightnessSfeerlampSpeelkamerDay { get; }
    public int BrightnessSfeerlampSpeelkamerNight { get; }
    public int BrightnessSfeerlampWoonkamer1Day { get; }
    public int BrightnessSfeerlampWoonkamer1Night { get; }
    public int BrightnessSfeerlampWoonkamer2Day { get; }
    public int BrightnessSfeerlampWoonkamer2Night { get; }
    public int BrightnessSfeerlampKeukenDay { get; }
    public int BrightnessSfeerlampKeukenNight{ get; }
    public int BrightnessSfeerlampHalDay { get; }
    public int BrightnessSfeerlampHalNight{ get; }
    public int BrightnessSfeerlampBovenDay { get; }
    public int BrightnessSfeerlampBovenNight{ get; }
}
