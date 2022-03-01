using NetDaemon.HassModel.Entities;
using NetDaemonInterface;
using System.Collections.Generic;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class NotifyApp : MyNetDaemonBaseApp
{
    private readonly INotify notify;
    private readonly List<NotifyActionEnum> thermostatActions;
    private IDisposable? beddenAlarmKidsSchedule;

    public NotifyApp(IHaContext haContext, IScheduler scheduler, ILogger<NotifyApp> logger,
        INotify notify, IHouseNotificationImageCreator houseNotificationImageCreator, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        this.notify = notify;

        thermostatActions = new List<NotifyActionEnum> { NotifyActionEnum.Thermostat17, NotifyActionEnum.Thermostat20, NotifyActionEnum.UriThermostat };

        houseNotificationImageCreator.AddFormattedText(5, 10, 10, "Ken: {0}", () => _entities.Person.Ken.State?.ToString());
        houseNotificationImageCreator.AddFormattedText(5, 20, 10, "Greet: {0}", () => _entities.Person.Greet.State?.ToString());
        houseNotificationImageCreator.AddConditionalImage(150, 50, 50, 50, Resource.Home, null);
        houseNotificationImageCreator.AddConditionalImage(125, 30, 20, 20, Resource.EnergyLow, () => _entities.Sensor.PowerTariff.State?.ToString() == "low");
        houseNotificationImageCreator.AddConditionalImage(125, 30, 20, 20, Resource.EnergyHigh, () => _entities.Sensor.PowerTariff.State?.ToString() == "normal");
        houseNotificationImageCreator.AddConditionalImage(150, 30, 20, 20, Resource.Moon, () => Helper.GetDayNightState(_entities) == DayNightEnum.Night);
        houseNotificationImageCreator.AddConditionalImage(150, 30, 20, 20, Resource.Sun, () => Helper.GetDayNightState(_entities) == DayNightEnum.Day);
        houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Holiday, () => Helper.GetHouseState(_entities) == HouseStateEnum.Holiday);
        houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Awake, () => Helper.GetHouseState(_entities) == HouseStateEnum.Awake);
        houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Away, () => Helper.GetHouseState(_entities) == HouseStateEnum.Away);
        houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Sleep, () => Helper.GetHouseState(_entities) == HouseStateEnum.Sleeping);
        houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Tv, () => Helper.GetHouseState(_entities) == HouseStateEnum.Tv);

        houseNotificationImageCreator.AddFormattedText(110, 90, 15, "{0}", () => _entities.Sensor.TempKeukenSetpoint.State?.ToString());
        houseNotificationImageCreator.AddConditionalImage(125, 70, 25, 25, Resource.Thermostat, null);

        houseNotificationImageCreator.CreateImage();
        notify.NotifyHouseStateGsmKen("House State", "HA startup", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.high, thermostatActions);

        _entities.InputDatetime.Beddenalarmkids.StateChanges()
            .Subscribe(x =>
            {
                if (x.New?.State != null)
                {
                    SetBeddenAlarmKidsSchedule(x.New.State);
                }
            });

        if (_entities.InputDatetime.Beddenalarmkids.State != null)
        {
            SetBeddenAlarmKidsSchedule(_entities.InputDatetime.Beddenalarmkids.State);
        }

        _entities.InputText.Housestate.StateChanges()
            .Subscribe(x =>
            {
                houseNotificationImageCreator.CreateImage();
                notify.NotifyHouseStateGsmKen("House State", $"House state: {x.New?.State}", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.low, thermostatActions);
            });
        _entities.InputText.Daynight.StateChanges()
            .Subscribe(x =>
            {
                houseNotificationImageCreator.CreateImage();
                notify.NotifyHouseStateGsmKen("House State", $"Day/Night state: {x.New?.State}", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.low, thermostatActions);
            });

        _entities.Sensor.PowerTariff.StateChanges()
            .Subscribe(x =>
            {
                houseNotificationImageCreator.CreateImage();
                notify.NotifyHouseStateGsmKen("House State", $"Energy tarif: {x.New?.State}", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.low, thermostatActions);
            });

        _entities.Person.Greet.StateChanges()
            .Subscribe(x =>
            {
                houseNotificationImageCreator.CreateImage();
                notify.NotifyHouseStateGsmKen("House State", $"Greet: {x.New?.State}", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.low, thermostatActions);
                LocationChangedGreet(x);
            });

        _entities.Person.Ken.StateChanges()
            .Subscribe(x =>
            {
                houseNotificationImageCreator.CreateImage();
                notify.NotifyHouseStateGsmKen("House State", $"Ken: {x.New?.State}", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.low, thermostatActions);
                LocationChangedKen(x);
            });

        _entities.Sensor.TempKeukenSetpoint.StateChanges().Subscribe(x =>
        {
            if (x.New != null && x.New.State != null)
            {
                houseNotificationImageCreator.CreateImage();
                notify.NotifyHouseStateGsmKen("House State", $"Thermostat: {x.New.State}", houseNotificationImageCreator.GetImagePath(), NotifyPriorityEnum.high, thermostatActions);
            }
        });

        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseVoordeur, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseVoordeur, new() { NotifyActionEnum.OpenCloseVoordeurOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseGarage, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseGarage, new() { NotifyActionEnum.OpenCloseGarageOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseAchterdeur, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseAchterdeur, new() { NotifyActionEnum.OpenCloseAchterdeurOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseTuindeur, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseTuindeur, new() { NotifyActionEnum.OpenCloseTuindeurOmroepen });
        CreateNotificationForOpenClose(_entities.BinarySensor.OpencloseAchterdeurgarage, scheduler, TimeSpan.FromMinutes(1), NotifyTagEnum.OpenCloseAchterdeurgarage, new() { NotifyActionEnum.OpenCloseAchterdeurgarageOmroepen });

        const string notifyHousePrefix = "[NotifyHouse]";

        _entities.Calendar.Local.StateAllChanges().Subscribe(state =>
        {
            //The state changes to 'on' for the active event
            if (state.New?.State != "on")
            {
                return;
            }
            var title = state.New!.Attributes!.FriendlyName;
            if (title == null)
            {
                return;
            }

            if (title.StartsWith(notifyHousePrefix))
            {
                var message = title.Substring(notifyHousePrefix.Length + 1);
                //notify.NotifyHouse(message);
                notify.NotifyGsmKen("Calendar notify test", message, NotifyPriorityEnum.high);
            }
        });
    }

    private void SetBeddenAlarmKidsSchedule(string time)
    {
        _logger.LogInformation($"SetBeddenAlarmKidsSchedule {time}");
        beddenAlarmKidsSchedule?.Dispose();
        beddenAlarmKidsSchedule = _scheduler.RunDaily(TimeSpan.Parse(time), () =>
        {
            if (_settingsProvider.BeddenAlarmKids)
            {
                notify.NotifyHouse("Attentie, Damon en Caitlyn jullie mogen je bed aan zetten.");
            }
        });
    }

    private void CreateNotificationForOpenClose(BinarySensorEntity entity, IScheduler scheduler, TimeSpan throttle, NotifyTagEnum tag, List<NotifyActionEnum>? actions = null)
    {
        string message = $"{entity.Attributes?.FriendlyName} is open for {throttle.TotalMinutes} minutes";
        entity.StateChanges()
            .Throttle(throttle, scheduler)
            .Where(x => x.New?.State == "on")
            .Subscribe(x => notify.NotifyGsmKen("", message, NotifyPriorityEnum.high, tag, actions));

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
            //notify.NotifyGsmGreet("Ken lokatie", "Ken is vertrokken vanuit werk", NotifyPriorityEnum.high);
            //notify.NotifyHouse("Attentie, Ken is vertrokken vanuit werk");
            return;
        }
    }

    private void LocationChangedGreet(StateChange<PersonEntity, EntityState<PersonAttributes>> x)
    {
        if (x.Old == null)
        {
            return;
        }
    }
}