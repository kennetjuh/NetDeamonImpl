using NetDaemonInterface;

namespace NetDaemonImpl.Modules.Notify;

public class NotifyAction
{
    public NotifyActionEnum Id { get; init; }
    public string Text { get; init; }
    public string? Uri { get; set; }
    public Action? Action { get; init; }

    public NotifyAction(NotifyActionEnum id, string text, Action action)
    {
        Id = id;
        Text = text;
        Action = action;
        Uri = null;
    }

    public NotifyAction(NotifyActionEnum id, string text, string uri)
    {
        Id = id;
        Text = text;
        Action = null;
        Uri = uri;
    }
}
