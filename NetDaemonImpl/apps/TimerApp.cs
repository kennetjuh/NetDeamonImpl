using NetDaemon.Extensions.Scheduler;
using NetDaemonInterface;
using System.Text.Json.Serialization;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class TimerApp : MyNetDaemonBaseApp
{
    private readonly ILightControl lightControl;

    public TimerApp(IHaContext haContext, IScheduler scheduler, INotify notify, ILogger<CallBackHandlerApp> logger, ISettingsProvider settingsProvider, ILightControl lightControl)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        _haContext.Events
            .Where(x => x.EventType == "timer.finished")
            .Subscribe(x =>
            {
                if (x.DataElement.HasValue)
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<TimerFinishedDataElement>(x.DataElement.Value);
                    if (data?.entity_id == _entities.Timer.Sleeptimerbedden.EntityId)
                    {
                        _entities.Switch.BedGreet.TurnOff();
                        _entities.Switch.BedKen.TurnOff();
                    }
                    if (data?.entity_id == _entities.Timer.Sleeptimerbeddenkids.EntityId)
                    {
                        _entities.Switch.BedCaitlyn.TurnOff();
                        _entities.Switch.BedDamon.TurnOff();
                    }
                    if (data?.entity_id == _entities.Timer.Sleeptimerkids.EntityId)
                    {
                        _entities.Light.Slaapkamerkids.TurnOff();
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

        _entities.InputNumber.Sleeptimerkids.StateChanges()
            .Subscribe(x =>
            {
                var newState = x.New!.State!;
                if (newState == 0)
                {
                    _entities.Light.Slaapkamerkids.TurnOff();
                    _entities.Timer.Sleeptimerkids.Cancel();
                }
                else
                {
                    _entities.Light.Slaapkamerkids.TurnOff();
                    lightControl.SetLight(_entities.Light.LightSlaapkamerkids2, 20, "red");
                    _entities.Timer.Sleeptimerkids.Start((x.New!.State! * 60).ToString());
                }
            });


        //_scheduler.ScheduleCron("0 8 * * *", () => // every day at 08:00
        //{
        //   // _entities.Switch.SwitchZwembad.TurnOn();
        //});

        //_scheduler.ScheduleCron("0 20 * * *", () => // every day at 20:00
        //{
        //   // _entities.Switch.SwitchZwembad.TurnOff();
        //});

        _scheduler.ScheduleCron("0 21 * * *", () => // every day at 21:00
        {
            if (Helper.GetHouseState(_entities) == HouseStateEnum.Holiday)
            {
                _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
                _entities.Vacuum.DreameP20294b09RobotCleaner.Start();
            }
        });

        _entities.Sensor.PowerTariff.StateChanges()
             .Subscribe(x =>
             {
                 HandlePowerState(notify, x.New?.State?.ToString());
             });
        HandlePowerState(notify, _entities.Sensor.PowerTariff.State?.ToString());
        this.lightControl = lightControl;
    }

    private void HandlePowerState(INotify notify, string? state)
    {
        if (state == null)
        {
            return;
        }
        if (state == "low")
        {
            _entities.Switch.LyncLader.TurnOn();
        }
        if (state == "normal")
        {
            _entities.Switch.LyncLader.TurnOff();
        }
    }

    private record TimerFinishedDataElement
    {
        [JsonPropertyName("entity_id")]
        public string? entity_id { get; init; }
    }
}