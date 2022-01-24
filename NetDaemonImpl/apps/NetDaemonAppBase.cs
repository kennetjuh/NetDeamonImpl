using NetDaemon.Extensions.Scheduler;

namespace NetDaemonImpl.apps;

/// <summary>
/// A base for the NetDeamon apps
/// It provides access to all essentials
/// </summary>
public class MyNetDaemonBaseApp
{
    internal readonly IHaContext _haContext;
    internal readonly INetDaemonScheduler _scheduler;
    internal readonly ILogger<DeconzEventHandlerApp> _logger;
    internal readonly IEntities _entities;
    internal readonly IServices _services;

    public MyNetDaemonBaseApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger)
    {
        _haContext = haContext;
        _scheduler = scheduler;
        _logger = logger;

        _entities = new Entities(haContext);
        _services = new Services(haContext);
    }
}
