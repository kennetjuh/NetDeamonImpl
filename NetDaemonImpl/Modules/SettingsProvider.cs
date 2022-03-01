using NetDaemonInterface;

namespace NetDaemonImpl.Modules
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly Entities Entities;

        public SettingsProvider(IServiceProvider provider)
        {
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
        }

        public int BrightnessSfeerlampWoonkamerDay => (int?)Entities.InputNumber.Brightnesssfeerlampwoonkamerday.State ?? 40;

        public int BrightnessSfeerlampWoonkamerNight => (int?)Entities.InputNumber.Brightnesssfeerlampwoonkamernight.State ?? 10;

        public int BrightnessSfeerlampHalDay => (int?)Entities.InputNumber.Brightnesssfeerlamphalday.State ?? 40;

        public int BrightnessSfeerlampHalNight => (int?)Entities.InputNumber.Brightnesssfeerlamphalnight.State ?? 1;

        public int BrightnessSfeerlampKeukenDay => (int?)Entities.InputNumber.Brightnesssfeerlampkeukenday.State ?? 40;

        public int BrightnessSfeerlampKeukenNight => (int?)Entities.InputNumber.Brightnesssfeerlampkeukennight.State ?? 1;

        public int BrightnessSfeerlampBovenDay => (int?)Entities.InputNumber.Brightnesssfeerlampbovenday.State ?? 40;

        public int BrightnessSfeerlampBovenNight => (int?)Entities.InputNumber.Brightnesssfeerlampbovennight.State ?? 1;

        public bool BeddenAlarmKids => Entities.InputBoolean.Beddenalarmkids.IsOn();

        public bool JimmieAlarm => Entities.InputBoolean.Jimmiealarm.IsOn();
    }
}
