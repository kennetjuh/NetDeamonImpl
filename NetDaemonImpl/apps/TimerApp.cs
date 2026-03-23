using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class TimerApp : MyNetDaemonBaseApp
{
    //private readonly ILightControl lightControl;

    public TimerApp(IHaContext haContext, IScheduler scheduler, INotify notify, ILogger<TimerApp> logger, ISettingsProvider settingsProvider, ILightControl lightControl)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        /*
             * service: zha.set_zigbee_cluster_attribute
                data:
                  ieee: '00:15:8d...'
                  endpoint_id: 1
                  cluster_id: 1280
                  cluster_type: in
                  attribute: 2
                  value: 0
            */

        _entities.MediaPlayer.Speakers.StateChanges()
                .WhenStateIsFor(x => x?.State == "idle", TimeSpan.FromSeconds(10), scheduler)
            .Subscribe(x =>
             {
                 if (x.New?.State == "idle")
                 {
                     _entities.MediaPlayer.Speakers.VolumeSet(0.7);
                 }
             });

        var motionSensors = new Dictionary<BinarySensorEntity, string>
        {
            { _entities.BinarySensor.MotionInkom, "00:15:8D:00:06:59:D5:E9" },
            { _entities.BinarySensor.MotionKeuken, "00:15:8D:00:06:5A:27:10" },
            { _entities.BinarySensor.MotionEetkamer, "00:15:8D:00:01:DA:49:DE" },
            { _entities.BinarySensor.MotionVeranda, "00:15:8D:00:01:AB:FD:C6" }
        };

        foreach (var sensor in motionSensors)
        {
            sensor.Key.StateChanges()
                .WhenStateIsFor(x => x?.State == "on", TimeSpan.FromSeconds(2), scheduler)
            .Subscribe(x =>
            {
                _services.Zha.SetZigbeeClusterAttribute(new ZhaSetZigbeeClusterAttributeParameters()
                {
                    Ieee = sensor.Value,
                    EndpointId = 1,
                    ClusterId = 1280,
                    ClusterType = "in",
                    Attribute = 2,
                    Value = "0"
                });
            });
        }

        _haContext.Events
            .Where(x => x.EventType == "timer.finished")
            .Subscribe(x =>
            {
                if (x.DataElement.HasValue)
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<TimerFinishedDataElement>(x.DataElement.Value);
                    if (data?.entity_id == _entities.Timer.Sleeptimerbedden.EntityId)
                    {
                        _entities.Switch.BedKen.TurnOff();
                    }
                    if (data?.entity_id == _entities.Timer.Sleeptimerbeddenkids.EntityId)
                    {
                        _entities.Switch.BedCaitlyn.TurnOff();
                        _entities.Switch.BedDamon.TurnOff();
                    }
                }
            });

        _entities.InputNumber.Sleeptimerbeddenminutes.StateChanges()
            .Subscribe(x =>
            {
                var newState = x.New!.State!;
                if (newState == 0)
                {
                    _entities.Timer.Sleeptimerbedden.Cancel();
                }
                else
                {
                    _entities.Timer.Sleeptimerbedden.Start((x.New!.State! * 60).ToString());
                }
            });

        _entities.InputNumber.Sleeptimerbeddenminuteskids.StateChanges()
           .Subscribe(x =>
           {
               var newState = x.New!.State!;
               if (newState == 0)
               {
                   _entities.Timer.Sleeptimerbeddenkids.Cancel();
               }
               else
               {
                   _entities.Timer.Sleeptimerbeddenkids.Start((x.New!.State! * 60).ToString());
               }
           });

        //_entities.InputNumber.Sleeptimerkids.StateChanges()
        //    .Subscribe(x =>
        //    {
        //        var newState = x.New!.State!;
        //        if (newState == 0)
        //        {
        //            _entities.Light.Inkom.TurnOff();
        //            _entities.Timer.Sleeptimerkids.Cancel();
        //        }
        //        else
        //        {
        //            _entities.Light.Inkom.TurnOff();
        //            lightControl.SetLight(_entities.Light.LightInkom2, 20, "red");
        //            _entities.Timer.Sleeptimerkids.Start((x.New!.State! * 60).ToString());
        //        }
        //    });


        _scheduler.ScheduleCron("0 22 */2 * *", () => // every 2 days at 22:00
        {
            _entities.Switch.PlugGarage.TurnOn();
        });

        _scheduler.ScheduleCron("0 23 */2 * *", () => // every 2 days at 23:00
        {
            _entities.Switch.PlugGarage.TurnOff();
        });

        _scheduler.ScheduleCron("0 21 * * *", () => // every day at 21:00
        {
            //if (Helper.GetHouseState(_entities) == HouseStateEnum.Holiday)
            //{
            //    _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
            //    _entities.Vacuum.DreameP20294b09RobotCleaner.Start();
            //}
        });
    }

    private record TimerFinishedDataElement
    {
        [JsonPropertyName("entity_id")]
        public string? entity_id { get; init; }
    }
}