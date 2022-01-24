using NetDaemon.Extensions.Scheduler;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class PersistanceHandlerApp : MyNetDaemonBaseApp
{
    public PersistanceHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger)
        : base(haContext, scheduler, logger)
    {
        // Creation is only once but i keep them here just so i know what i created

        //_services.Netdaemon.EntityCreate(entityId: "sensor.housestate", HouseModeEnum.Awake.ToString());
        //_services.Netdaemon.EntityCreate(entityId: "sensor.daynight", DayNightEnum.Day.ToString());
        //_services.Netdaemon.EntityCreate(entityId: "sensor.daynight_lastdaytrigger", DateTime.Now.ToString(Statics.dateTime_TimeFormat));
        //_services.Netdaemon.EntityCreate(entityId: "sensor.daynight_lastnighttrigger", DateTime.Now.ToString(Statics.dateTime_TimeFormat));    
        //_services.Netdaemon.EntityCreate(entityId: "switch.watchdog_buiten");    
        //_services.Netdaemon.EntityRemove(entityId: "switch.watchdog_wandlamp");    



        //_services.Netdaemon.EntityUpdate(entityId: "sensor.daynight_lastdaytrigger", "08:57:58");
        //_services.Netdaemon.EntityUpdate(entityId: "sensor.daynight_lastnighttrigger", "16:39:39");
    }
}