namespace NetDaemonInterface;

public interface INotify
{
    void NotifyGsm(string title, string message);
    void NotifyGsmAlarm();
    void NotifyGsmGreet(string title, string message);
    void NotifyGsmGreetTTS(string message);
    void NotifyGsmKen(string title, string message);
    void NotifyGsmKenTTS(string message);
    void NotifyHouse(string message);
}
