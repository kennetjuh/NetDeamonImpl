using NetDaemonInterface;
using NetDaemonInterface.Observable;
using System.Linq;
using System.Reactive.Subjects;

namespace NetDaemonImpl.IObservable
{
    public class DayNightEvents : IDayNightEvents
    {
        private readonly ILogger<DayNightEvents> logger;
        private readonly Entities entities;
        private readonly SunEntity SunEntity;
        private readonly Subject<DayNightEvent> _subject = new();
        private readonly Subject<object> _lastDaySubject = new();
        private readonly Subject<object> _lastNightSubject = new();
        private readonly ILuxBasedBrightness luxBasedBrightness;

        public IObservable<DayNightEvent> DayNightEvent => _subject.AsObservable();
        public IObservable<object> LastDayChangedEvent => _lastDaySubject.AsObservable();
        public IObservable<object> LastNightChangedEvent => _lastNightSubject.AsObservable();

        public DayNightEvents(IServiceProvider provider, ILogger<DayNightEvents> logger, ILuxBasedBrightness luxBasedBrightness, IHouseStateEvents houseStateEvents)
        {
            this.logger = logger;
            this.luxBasedBrightness = luxBasedBrightness;
            var haContext = DiHelper.GetHaContext(provider);
            entities = new Entities(haContext);
            SunEntity = entities.Sun.Sun;
            SunEntity.StateAllChanges()
                    .Where(x => x.Old?.Attributes?.Elevation != x.New?.Attributes?.Elevation)
                    .Subscribe(x => CheckDayNight());

            entities.Sensor.SensorLuxBuiten.StateChanges()
                .Subscribe(x => CheckDayNight());

            houseStateEvents.Event.Subscribe(x =>
            {
                CheckDayNight();
            });

            CheckDayNight();
        }

        private void CheckDayNight()
        {
            try
            {
                var elevation = SunEntity.Attributes?.Elevation;
                var isRising = SunEntity.Attributes?.Rising;
                var lux = luxBasedBrightness.GetLux();
                var current = Helper.GetDayNightState(entities);

                if (current == DayNightEnum.Day && isRising == false && elevation < 0 && lux < 30)
                {
                    entities.InputDatetime.Daynightlastnighttrigger.SetDatetime(time: DateTime.Now.ToString(Constants.dateTime_TimeFormat));
                    entities.InputText.Daynight.SetValue(DayNightEnum.Night.ToString());
                    _lastNightSubject.OnNext(0);
                    _subject.OnNext(new(DayNightEnum.Night));
                    logger.LogInformation($"DayNight changed to Night. Elevation: {elevation}, Lux: {lux}");

                }
                else if (current == DayNightEnum.Night && isRising == true && elevation > -5 && lux > 20)
                {
                    entities.InputDatetime.Daynightlastdaytrigger.SetDatetime(time: DateTime.Now.ToString(Constants.dateTime_TimeFormat));
                    entities.InputText.Daynight.SetValue(DayNightEnum.Day.ToString());
                    _lastDaySubject.OnNext(0);
                    _subject.OnNext(new(DayNightEnum.Day));
                    logger.LogInformation($"DayNight changed to Day. Elevation: {elevation}, Lux: {lux}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CheckDayNight");
            }
        }

    }
}
