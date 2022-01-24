using NetDaemon.Extensions.Scheduler;
using NetDaemon.HassModel.Entities;
using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class NotifyHandlerApp : MyNetDaemonBaseApp
{
    private readonly INotify notify;

    public NotifyHandlerApp(IHaContext haContext, INetDaemonScheduler scheduler, ILogger<DeconzEventHandlerApp> logger,
        INotify notify)
        : base(haContext, scheduler, logger)
    {
        this.notify = notify;

        _entities.MediaPlayer.Hal.StateChanges()
            .Where(x => x.New?.State == "off")
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(x => _entities.MediaPlayer.Hal.VolumeSet(0.7));

        _entities.MediaPlayer.Woonkamer.StateChanges()
            .Where(x => x.New?.State == "off")
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(x => _entities.MediaPlayer.Woonkamer.VolumeSet(0.8));

        _scheduler.RunDaily(TimeSpan.FromHours(18), () =>
        {
            notify.NotifyHouse("Attentie, Damon en Caitlyn jullie mogen je bed aan zetten.");
        });

        _entities.Person.Greet.StateChanges()
            .Subscribe(x => LocationChangedGreet(x));

        _entities.Person.Ken.StateChanges()
            .Subscribe(x => LocationChangedKen(x));

        _entities.Sensor.TempKeukenSetpoint.StateChanges()
            .Subscribe(x => notify.NotifyGsmKen("Thermostaat", $"setpoint {x.New?.State}"));
    }

    private void LocationChangedKen(StateChange<PersonEntity, EntityState<PersonAttributes>> x)
    {
        if (x.Old == null)
        {
            return;
        }

        if (x.Old.State == _entities.Zone.WerkKen.Attributes?.FriendlyName && DateTime.Now.Hour > 13)
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

        if (x.Old.State == _entities.Zone.WerkGreet.Attributes?.FriendlyName && DateTime.Now.Hour > 13)
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