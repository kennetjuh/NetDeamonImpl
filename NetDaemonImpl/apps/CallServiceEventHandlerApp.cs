using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class CallServiceEventHandlerApp : MyNetDaemonBaseApp
{
    public CallServiceEventHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        ITwinkle twinkle)
        : base(haContext, scheduler, logger)
    {
        //{
        //"event_type": "call_service",
        //"data": {
        //        "domain": "light",
        //        "service": "turn_on",
        //        "service_data": {
        //            "color_temp": 370,
        //            "entity_id": ["light.woonkamer_kamer"]
        //            }
        //        }
        //}
        _haContext.Events.Where(x => x.EventType == "call_service").Subscribe(x =>
          {
              try
              {
                  var data = x.DataElement;
                  var domain = data?.GetProperty("domain").GetString();
                  var service = data?.GetProperty("service").GetString();
                  var serviceData = data?.GetProperty("service_data");
                  var entity = serviceData?.GetProperty("entity_id").ToString();

                  if (entity == null || serviceData == null || service == null || domain == null || data == null)
                  {
                      // invalid event, skip
                      return;
                  }

                  if (domain == "light" && (service == "turn_on" || service == "turn_off"))
                  {
                      if (entity.Contains(_entities.Light.KeukenKeukenlamp.EntityId))
                      {
                          twinkle.Stop();
                      }
                  }
              }
              catch
              {
                  //ignore exceptions
              }
          });
    }
}