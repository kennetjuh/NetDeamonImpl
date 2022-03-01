using NetDaemonInterface;

namespace NetDaemonImpl.Modules
{
    public class HouseState : IHouseState
    {
        private readonly ILogger<HouseState> Logger;
        private readonly ISettingsProvider settingsProvider;
        private readonly IDayNight dayNight;
        private readonly Entities Entities;
        private readonly Services Services;
        private readonly ILightControl LightControl;
        private readonly ITwinkle Twinkle;
        private readonly INotify Notify;

        public HouseState(IServiceProvider provider, ILightControl lightControl, ITwinkle twinkle, INotify notify, ILogger<HouseState> logger, ISettingsProvider settingsProvider, IDayNight dayNight)
        {
            LightControl = lightControl;
            Twinkle = twinkle;
            Notify = notify;
            Logger = logger;
            this.settingsProvider = settingsProvider;
            this.dayNight = dayNight;
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
            Services = new Services(haContext);
        }

        public void HouseStateAwake()
        {
            Logger.LogInformation("Awake");
            Entities.InputText.Housestate.SetValue(HouseStateEnum.Awake.ToString());

            dayNight.ForceDayNight();
            Entities.InputBoolean.WatchdogBuiten.TurnOn();

            LightControl.SetLight(Entities.Light.Kamerlamp, 0);
            LightControl.SetLight(Entities.Light.Bureaulamp, 0);
            LightControl.SetLight(Entities.Light.Booglamp, Helper.GetDayNightState(Entities) == DayNightEnum.Day?Constants.brightnessBoogDay: Constants.brightnessBoogNight);
            

            Twinkle.Start();

            // Kerst
            //Entities.Switch.Binnen1.TurnOn();
            //Entities.Switch.Binnen2.TurnOn();
        }

        public void HouseStateAway(HouseStateEnum state)
        {
            Logger.LogInformation("Away");

            Entities.InputText.Housestate.SetValue(state.ToString());
            
            Entities.InputBoolean.WatchdogBuiten.TurnOn();

            // buiten
            LightControl.SetLight(Entities.Light.BuitenachterLamp, 0);
            LightControl.SetLight(Entities.Light.LightHut, 0);
            LightControl.SetLight(Entities.Light.BuitenachterHangstoel, 0);
            Entities.Switch.BuitenachterGrondpomp.TurnOff();
            Entities.Switch.SwitchFontein.TurnOff();
            Entities.InputBoolean.WatchdogBuiten.TurnOn();

            // boven
            LightControl.SetLight(Entities.Light.Cabine, 0);
            LightControl.SetLight(Entities.Light.Cabineplafond, 0);
            LightControl.SetLight(Entities.Light.Halboven, 0);
            LightControl.SetLight(Entities.Light.Slaapkamer, 0);
            if (state != HouseStateEnum.Sleeping)
            {
                LightControl.SetLight(Entities.Light.SlaapkamerNachtlampen, 0);
            }
            LightControl.SetLight(Entities.Light.Slaapkamerkids, 0);
            Entities.Switch.SwitchSierCabine.TurnOff();
            Entities.Switch.CabineHeater.TurnOff();
            Entities.InputBoolean.Cabinethermostat.TurnOff();

            // onder
            LightControl.SetLight(Entities.Light.Booglamp, 0);
            LightControl.SetLight(Entities.Light.Kamerlamp, 0);
            LightControl.SetLight(Entities.Light.Bureaulamp, 0);
            LightControl.SetLight(Entities.Light.Hal, 0);
            LightControl.SetLight(Entities.Light.KeukenKeukenlamp, 0);
            LightControl.SetLight(Entities.Light.Washal, 0);
            LightControl.SetLight(Entities.Light.WcWclamp, 0);
            LightControl.SetLight(Entities.Light.Badkamer, 0);
            LightControl.SetLight(Entities.Light.SpeelkamerLamp, 0);
            LightControl.SetLight(Entities.Light.Wandlampen, 0);

            LightControl.SetLight(Entities.Light.LightHalSfeer, 0);
            LightControl.SetLight(Entities.Light.SfeerlampKeuken, 0);

            // Kerst
            //Entities.Switch.Binnen1.TurnOff();
            //Entities.Switch.Binnen2.TurnOff();

            if (settingsProvider.JimmieAlarm)
            {
                if (Entities.DeviceTracker.GsmGreet.State == "home")
                {
                    Notify.NotifyGsmGreet("Jimmie", "Zit ie in de bench?", NotifyPriorityEnum.high);
                }
                if (Entities.DeviceTracker.GsmKen.State == "home")
                {
                    Notify.NotifyGsmKen("Jimmie", "Zit ie in de bench?", NotifyPriorityEnum.high);
                }
            }

            dayNight.CheckDayNight();
        }

        public void HouseStateSleeping()
        {
            HouseStateAway(HouseStateEnum.Sleeping);
            Logger.LogInformation("Sleeping");

            Entities.InputText.Housestate.SetValue(HouseStateEnum.Sleeping.ToString());

            Entities.Climate.Keuken.SetTemperature(19);
            //LightControl.SetLight(Entities.Light.NachtlampGreet, 1);
            //LightControl.SetLight(Entities.Light.NachtlampKen, 1);

            Entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
            Entities.Vacuum.DreameP20294b09RobotCleaner.Start();
        }

        public void HouseStateHoliday()
        {
            HouseStateAway(HouseStateEnum.Away);
            Logger.LogInformation("Holiday");

            Entities.InputText.Housestate.SetValue(HouseStateEnum.Holiday.ToString());
        }

        public void TvMode()
        {
            Entities.InputText.Housestate.SetValue(HouseStateEnum.Tv.ToString());

            // buiten
            Entities.InputBoolean.WatchdogBuiten.TurnOff();
            LightControl.SetLight(Entities.Light.BuitenachterLamp, 0);
            LightControl.SetLight(Entities.Light.BuitenachterHangstoel, 0);
            LightControl.SetLight(Entities.Light.BuitenachterFonteinlamp, 0);
            Entities.Switch.SwitchFontein.TurnOff();

            // onder
            LightControl.SetLight(Entities.Light.Booglamp, 0);
            LightControl.SetLight(Entities.Light.Kamerlamp, 0);
            LightControl.SetLight(Entities.Light.Bureaulamp, 0);
            LightControl.SetLight(Entities.Light.KeukenKeukenlamp, 0);
            LightControl.SetLight(Entities.Light.Wandlampen, 0);
            LightControl.SetLight(Entities.Light.SfeerlampKeuken, 0);
        }
    }
}
