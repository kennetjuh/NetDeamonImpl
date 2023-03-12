using System.Collections.Generic;

namespace NetDaemonImpl.Modules.Notify
{
    public record RecordNotifyAction
    {
        public RecordNotifyAction(NotifyAction notifyAction)
        {
            var idString = notifyAction.Id.ToString();
            if (idString.StartsWith("Uri"))
            {
                action = "URI";
            }
            else
            {
                action = notifyAction.Id.ToString();
            }
            title = notifyAction.Text;
            uri = notifyAction.Uri;
        }

        public string? action { get; set; }
        public string? title { get; set; }
        public string? uri { get; set; }
    }
    public record RecordNotifyData
    {
        public string? tag { get; set; }
        public int? ttl { get; set; }
        public string? priority { get; set; }
        public string? importance { get; set; }
        public string? channel { get; set; }
        public string? color { get; set; }
        public string? sticky { get; set; }
        public string? image { get; set; }
        public List<RecordNotifyAction>? actions { get; set; }
    }
}
