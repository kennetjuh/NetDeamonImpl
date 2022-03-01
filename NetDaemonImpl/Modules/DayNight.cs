using NetDaemonInterface;

namespace NetDaemonImpl.Modules
{
    public class DayNight : IDayNight
    {
        private readonly ILogger<DayNight> Logger;
        private readonly ISettingsProvider settingsProvider;
        private readonly Entities Entities;
        private readonly Services Services;
        private readonly ILightControl LightControl;
        private readonly ILuxBasedBrightness luxBasedBrightness;
        private SunEntity SunEntity;

        public DayNight(IServiceProvider provider, ILightControl LightControl, ILogger<DayNight> logger, ISettingsProvider settingsProvider, ILuxBasedBrightness luxBasedBrightness)
        {
            this.LightControl = LightControl;
            Logger = logger;
            this.settingsProvider = settingsProvider;
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
            Services = new Services(haContext);
            this.luxBasedBrightness = luxBasedBrightness;
            SunEntity = Entities.Sun.Sun;
        }

        public void SetSunEntity(SunEntity sunEntity)
        {
            SunEntity = sunEntity;
        }

        public void CheckDayNight()
        {
            var elevation = SunEntity.Attributes?.Elevation;
            var isRising = SunEntity.Attributes?.Rising;
            var lux = luxBasedBrightness.GetLux();
            var current = Helper.GetDayNightState(Entities);

            if (current == DayNightEnum.Day && isRising == false && elevation < 0 && lux < 30)
            {
                Entities.InputDatetime.Daynightlastnighttrigger.SetDatetime(time: DateTime.Now.ToString(Constants.dateTime_TimeFormat));
                Entities.InputText.Daynight.SetValue(DayNightEnum.Night.ToString());
                Night(false);
            }
            else if (current == DayNightEnum.Night && isRising == true && elevation > -5 && lux > 20)
            {
                Entities.InputDatetime.Daynightlastdaytrigger.SetDatetime(time: DateTime.Now.ToString(Constants.dateTime_TimeFormat));
                Entities.InputText.Daynight.SetValue(DayNightEnum.Day.ToString());
                Day(false);
            }
        }

        public void ForceDayNight()
        {
            if (Helper.GetDayNightState(Entities) == DayNightEnum.Night) { Night(true); } else { Day(true); }
        }

        private void Night(bool force)
        {
            LightControl.SetLight(Entities.Light.BuitenopritWandlamp, 50);
            LightControl.SetLight(Entities.Light.WandlampBuiten, 50);
            Entities.Switch.BuitenvoorGrondspots.TurnOn();
            LightControl.SetLight(Entities.Light.SfeerlampHalboven, settingsProvider.BrightnessSfeerlampBovenNight);

            if (Entities.Light.Wandlampen.IsOn() || force)
            {
                LightControl.SetLight(Entities.Light.Wandlampen, Constants.brightnessWandNight);
            }
            if (Entities.Light.LightHalSfeer.IsOn() || force)
            {
                LightControl.SetLight(Entities.Light.LightHalSfeer, settingsProvider.BrightnessSfeerlampHalNight);
            }
            if (Entities.Light.SfeerlampKeuken.IsOn() || force)
            {
                LightControl.SetLight(Entities.Light.SfeerlampKeuken, settingsProvider.BrightnessSfeerlampKeukenNight);
            }
            WatchdogBuiten();
        }

        private void Day(bool force)
        {
            LightControl.SetLight(Entities.Light.BuitenopritWandlamp, 0);
            LightControl.SetLight(Entities.Light.WandlampBuiten, 0);
            Entities.Switch.BuitenvoorGrondspots.TurnOff();
            LightControl.SetLight(Entities.Light.SfeerlampHalboven, settingsProvider.BrightnessSfeerlampBovenDay);

            if (Entities.Light.Wandlampen.IsOn() || force)
            {
                LightControl.SetLight(Entities.Light.Wandlampen, Constants.brightnessWandDay);
            }
            if (Entities.Light.LightHalSfeer.IsOn() || force)
            {
                LightControl.SetLight(Entities.Light.LightHalSfeer, settingsProvider.BrightnessSfeerlampHalDay);
            }
            if (Entities.Light.SfeerlampKeuken.IsOn() || force)
            {
                LightControl.SetLight(Entities.Light.SfeerlampKeuken, settingsProvider.BrightnessSfeerlampKeukenDay);
            }
            WatchdogBuiten();
        }

        public void WatchdogBuiten()
        {
            if (Helper.GetDayNightState(Entities) == DayNightEnum.Day)
            {
                LightControl.SetLight(Entities.Light.GrondlampZij, 0);
                LightControl.SetLight(Entities.Light.BuitenachterFonteinlamp, 0);
                LightControl.SetLight(Entities.Light.WandlampHut, 0);
            }
            else
            {
                LightControl.SetLight(Entities.Light.GrondlampZij, Constants.brightnessBuitenZij);
                LightControl.SetLight(Entities.Light.BuitenachterFonteinlamp, Constants.brightnessFontein);
                LightControl.SetLight(Entities.Light.WandlampHut, Constants.brightnessHutWand);
            }
        }
    }
}
