using NetDaemon.HassModel.Entities;
using NetDaemonInterface;
using System.Collections.Generic;
using NetDaemon.Extensions.Scheduler;
using System.Globalization;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class NotifyApp : MyNetDaemonBaseApp
{
    private readonly INotify notify;

    public NotifyApp(IHaContext haContext, IScheduler scheduler, ILogger<NotifyApp> logger,
        INotify notify)
        : base(haContext, scheduler, logger)
    {
        this.notify = notify;

        _scheduler.ScheduleCron("45 7 * * 1", () => //At 07:45 on Monday.
        {
            var cal = new CultureInfo("nl-NL").Calendar;
            int week = cal.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            if (week % 2 == 1) //only on uneven weeks
            {
                notify.NotifyGsmGreet("", "Vergeet je laptop niet");
                notify.NotifyHouse("Attentie, Greet vergeet je laptop niet");
            }
        });
        
        //_scheduler.ScheduleCron("0 18 * * *", () => // every day at 18:00
        //{
        //    notify.NotifyHouse("Attentie, Damon en Caitlyn jullie mogen je bed aan zetten.");
        //});

        _entities.Sensor.PowerTariff.StateChanges()
            .Subscribe(x => notify.NotifyGsmKen("", $"Energy tarif: {x.New?.State}", NotifyTagEnum.PowerTarifChanged));

        _entities.Person.Greet.StateChanges()
            .Subscribe(x => LocationChangedGreet(x));

        _entities.Person.Ken.StateChanges()
            .Subscribe(x => LocationChangedKen(x));

        _entities.Sensor.TempKeukenSetpoint.StateChanges().Subscribe(x =>
        {
            if (x.New != null && x.New.State != null)
            {
                notify.NotifyGsmKen("", $"Thermostat changed to {x.New.State}", NotifyTagEnum.ThermostatChanged, new() { NotifyActionEnum.Thermostat15, NotifyActionEnum.Thermostat21, NotifyActionEnum.UriThermostat });
            }
        });

        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseVoordeur, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseVoordeur, new() { NotifyActionEnum.OpenCloseVoordeurOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseGarage, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseGarage, new() { NotifyActionEnum.OpenCloseGarageOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseAchterdeur, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseAchterdeur, new() { NotifyActionEnum.OpenCloseAchterdeurOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseTuindeur, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseTuindeur, new() { NotifyActionEnum.OpenCloseTuindeurOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseAchterdeurgarage, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseAchterdeurgarage, new() { NotifyActionEnum.OpenCloseAchterdeurgarageOmroepen });
    }

    private void CreateNotificationForOpenClose(BinarySensorEntity entity, IScheduler scheduler, TimeSpan throttle, NotifyTagEnum tag, List<NotifyActionEnum>? actions = null)
    {        
        string message = $"{entity.Attributes?.FriendlyName} is open for {throttle.TotalMinutes} minutes";
        entity.StateChanges()
            .Throttle(throttle, scheduler)
            .Where(x => x.New?.State == "on")
            .Subscribe(x => notify.NotifyGsmKen("", message, tag, actions));

        entity.StateChanges()
            .Where(x => x.New?.State == "off")
            .Subscribe(x => notify.Clear(tag));
    }

    private void LocationChangedKen(StateChange<PersonEntity, EntityState<PersonAttributes>> x)
    {
        if (x.Old == null)
        {
            return;
        }

        if (x.Old.State == _entities.Zone.WerkKen.Attributes?.FriendlyName && _scheduler.Now.Hour > 13)
        {
            notify.NotifyGsmGreet("Ken lokatie", "Ken is vertrokken vanuit werk");
            notify.NotifyHouse("Attentie, Ken is vertrokken vanuit werk");
            return;
        }
    }

    private void LocationChangedGreet(StateChange<PersonEntity, EntityState<PersonAttributes>> x)
    {
        if (x.Old == null)
        {
            return;
        }

        if (x.Old.State == _entities.Zone.WerkGreet.Attributes?.FriendlyName && _scheduler.Now.Hour > 13)
        {
            notify.NotifyGsmKen("Greet lokatie", "Greet is vertrokken vanuit werk");
            notify.NotifyHouse("Attentie, Great is vertrokken vanuit werk");
            return;
        }
        if (x.Old.State == _entities.Zone.IjzerenMan.Attributes?.FriendlyName)
        {
            notify.NotifyGsmKen("Greet lokatie", "Greet is vertrokken vanuit de ijzeren man");
            notify.NotifyHouse("Attentie, Great is vertrokken vanuit de ijzeren man");
            return;
        }
    }
}