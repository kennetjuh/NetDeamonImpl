using NetDaemonInterface;

namespace NetDaemonImpl.apps;

/// <summary>
/// A base for the NetDeamon apps
/// It provides access to all essentials
/// </summary>
public class MyNetDaemonBaseApp
{
    internal readonly IHaContext _haContext;
    internal readonly IScheduler _scheduler;
    internal readonly ILogger _logger;
    internal readonly ISettingsProvider _settingsProvider;
    internal readonly IEntities _entities;
    internal readonly IServices _services;

    public MyNetDaemonBaseApp(IHaContext haContext, IScheduler scheduler, ILogger logger, ISettingsProvider settingsProvider)
    {
        _haContext = haContext;
        _scheduler = scheduler;
        _logger = logger;
        _settingsProvider = settingsProvider;
        _entities = new Entities(haContext);
        _services = new Services(haContext);
    }
}
