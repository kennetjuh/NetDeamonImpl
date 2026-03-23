using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using System.Linq;
using System.Reactive.Subjects;

namespace NetDaemonImpl.IObservable
{
    public class HouseStateEvents : IHouseStateEvents
    {
        private readonly ILogger<HouseStateEvents> logger;
        private readonly Subject<HouseStateEvent> _subject = new();

        public IObservable<HouseStateEvent> Event => _subject.AsObservable();


        public HouseStateEvents(IServiceProvider provider, ILogger<HouseStateEvents> logger, ILuxBasedBrightness luxBasedBrightness, IButtonEvents deconzButtonEvents)
        {

            this.logger = logger;
            var haContext = DiHelper.GetHaContext(provider);
            var entities = new Entities(haContext);

            deconzButtonEvents.Event
                .Where(x => x.Button == Button.HouseInkom || x.Button == Button.HouseVoordeur)
                .Subscribe(HandleDeconzButtonEvent);
        }

        private void HandleDeconzButtonEvent(ButtonEvent x)
        {
            try
            {
                switch (x.Event)
                {
                    case ButtonEventType.Single:
                        _subject.OnNext(new HouseStateEvent(HouseStateEnum.Awake, x));
                        break;
                    case ButtonEventType.Double:
                        _subject.OnNext(new HouseStateEvent(HouseStateEnum.Away, x));
                        break;
                    case ButtonEventType.LongPress:
                        _subject.OnNext(new HouseStateEvent(HouseStateEnum.Sleeping, x));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling Deconz button event for HouseStateEvents");
            }
        }
    }
}
