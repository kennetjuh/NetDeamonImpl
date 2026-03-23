using NetDaemonInterface;
using NetDaemonInterface.Observable;
using System.Threading.Tasks;

namespace NetDaemonImpl.apps
{
    [NetDaemonApp]
    public class AlarmApp : MyNetDaemonBaseApp
    {
        private INotify notify;

        public AlarmApp(IHaContext haContext, IScheduler scheduler, ILogger<HouseApp> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IDayNightEvents dayNightEvents, IHouseStateEvents houseStateEvents, INotify notify)
            : base(haContext, scheduler, logger, settingsProvider)
        {
            this.notify = notify;
            _entities.InputBoolean.Alarm.StateChanges().Subscribe(x =>
            {
                HandleAlarmToggle(x.New?.State == "on" ? AlarmEnum.Armed : AlarmEnum.Disarmed);
            });

            HandleAlarmToggle(Helper.GetAlarmState(_entities));

            _entities.BinarySensor.OpencloseKelderOpening
                .StateChanges().Subscribe(x =>
                {
                    bool isOpen = x.New?.State == "on";
                    if (isOpen)
                    {
                        AlarmStart();
                    }
                    else
                    {
                        AlarmStop();
                    }
                });
            _entities.BinarySensor.OpencloseInkomOpening
                .StateChanges().Subscribe(x =>
                {
                    bool isOpen = x.New?.State == "on";
                    if (isOpen)
                    {
                        AlarmStart();
                    }
                });
        }

        private void HandleAlarmToggle(AlarmEnum alarm)
        {
            if (alarm == AlarmEnum.Armed)
            {
                AlarmPulse(200);
            }
            else
            {
                AlarmStop();
                AlarmPulse(200);
                Task.Delay(100).Wait();
                AlarmPulse(200);
            }
        }

        private void AlarmPulse(int miliseconds = 500)
        {            
            _entities.Select.SireneVolume.SelectOption("low");
            _entities.Siren.Sirene.TurnOn();
            Task.Delay(miliseconds).Wait();
            _entities.Siren.Sirene.TurnOff();
        }

        private void AlarmStart()
        {
            if (Helper.GetAlarmState(_entities) != AlarmEnum.Armed)
            {
                return;
            }
            notify.NotifyHouse("Alarm geactiveerd!");
            notify.NotifyGsmAlarm();
            _entities.Select.SireneVolume.SelectOption("high");
            _entities.Siren.Sirene.TurnOn();
        }

        private void AlarmStop()
        {
            _entities.Siren.Sirene.TurnOff();
        }
    }
}

