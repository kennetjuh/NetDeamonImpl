namespace NetDaemonInterface.Models
{
    public class FrigateSettings
    {
        public FrigateLocalLogin LocalLogin { get; set; } = new FrigateLocalLogin();
        public FrigateLocal Local { get; set; } = new FrigateLocal();
        public FrigateApi Api { get; set; } = new FrigateApi();
    }

    public class FrigateLocalLogin
    {
        public string? BaseUrl { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class FrigateLocal
    {
        public string? BaseUrl { get; set; }
    }

    public class FrigateApi
    {
        public string? BaseUrl { get; set; }
    }
}
