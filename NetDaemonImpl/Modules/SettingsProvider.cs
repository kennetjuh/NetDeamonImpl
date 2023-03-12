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

        public int BrightnessSfeerlampSpeelkamerDay => (int?)Entities.InputNumber.Brightnesssfeerlampspeelkamerday.State ?? 50;

        public int BrightnessSfeerlampSpeelkamerNight => (int?)Entities.InputNumber.Brightnesssfeerlampspeelkamernight.State ?? 10;

        public int BrightnessSfeerlampHalDay => (int?)Entities.InputNumber.Brightnesssfeerlamphalday.State ?? 50;

        public int BrightnessSfeerlampHalNight => (int?)Entities.InputNumber.Brightnesssfeerlamphalnight.State ?? 1;

        public int BrightnessSfeerlampWoonkamer1Day => throw new NotImplementedException();

        public int BrightnessSfeerlampWoonkamer1Night => throw new NotImplementedException();

        public int BrightnessSfeerlampWoonkamer2Day => throw new NotImplementedException();

        public int BrightnessSfeerlampWoonkamer2Night => throw new NotImplementedException();

        public int BrightnessSfeerlampKeukenDay => throw new NotImplementedException();

        public int BrightnessSfeerlampKeukenNight => throw new NotImplementedException();

        public int BrightnessSfeerlampBovenDay => throw new NotImplementedException();

        public int BrightnessSfeerlampBovenNight => throw new NotImplementedException();
    }
}
