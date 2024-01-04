using NetDaemonInterface;

namespace NetDaemonImpl.Modules
{
    public class HouseState : IHouseState
    {
        private readonly ILogger<HouseState> Logger;
        private readonly Entities Entities;
        private readonly Services Services;
        private readonly ILightControl LightControl;
        private readonly ITwinkle Twinkle;
        private readonly INotify Notify;

        public HouseState(IServiceProvider provider, ILightControl lightControl, ITwinkle twinkle, INotify notify, ILogger<HouseState> logger)
        {
            LightControl = lightControl;
            Twinkle = twinkle;
            Notify = notify;
            Logger = logger;
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
            Services = new Services(haContext);
        }

        public void HouseStateAwake()
        {
            Logger.LogInformation("Awake");
            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Awake.ToString());

            LightControl.SetLight(Entities.Light.Kamerlamp, 0);
            LightControl.SetLight(Entities.Light.Bureaulamp, 0);
            Entities.Switch.BdfM107Screen.TurnOn();
            LightControl.SetLight(Entities.Light.WoonkamerKisten);

            if (Helper.GetDayNightState(Entities) == DayNightEnum.Day)
            {
                LightControl.SetLight(Entities.Light.Wandlampen, Constants.brightnessWandDay);
                LightControl.SetLight(Entities.Light.Booglamp, Constants.brightnessBoogDay);
            }
            else
            {
                LightControl.SetLight(Entities.Light.Wandlampen, Constants.brightnessWandNight);
                LightControl.SetLight(Entities.Light.Booglamp, Constants.brightnessBoogNight);
            }

            Twinkle.Start();

            // Kerst
            //Entities.Switch.Binnen1.TurnOn();
            //Entities.Switch.Binnen2.TurnOn();
        }

        public void HouseStateAway(HouseStateEnum state)
        {
            Logger.LogInformation("Away");

            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Away.ToString());

            // buiten
            LightControl.SetLight(Entities.Light.BuitenachterLamp, 0);
            LightControl.SetLight(Entities.Light.BuitenachterSierverlichting, 0);
            LightControl.SetLight(Entities.Light.LightHut, 0);
            LightControl.SetLight(Entities.Light.BuitenzijHutsier, 0);
            LightControl.SetLight(Entities.Light.BuitenachterHangstoel, 0);
            Entities.Switch.SwitchInfinityMirror.TurnOff();
            Entities.Switch.BuitenachterGrondpomp.TurnOff();
            Entities.Switch.SwitchVliegenlamp.TurnOff();
            Entities.Switch.SwitchFontein.TurnOff();

            // boven
            LightControl.SetLight(Entities.Light.Cabine, 0);
            LightControl.SetLight(Entities.Light.Halboven, 0);
            LightControl.SetLight(Entities.Light.Slaapkamer, 0);
            if (state != HouseStateEnum.Sleeping)
            {
                LightControl.SetLight(Entities.Light.SlaapkamerNachtlampen, 0);
            }
            LightControl.SetLight(Entities.Light.Slaapkamerkids, 0);
            Entities.Switch.SwitchSierCabine.TurnOff();
            Entities.Switch.CabineHeater.TurnOff();

            // onder
            LightControl.SetLight(Entities.Light.Booglamp, 0);
            LightControl.SetLight(Entities.Light.Kamerlamp, 0);
            LightControl.SetLight(Entities.Light.Bureaulamp, 0);
            Entities.Switch.BdfM107Screen.TurnOff();
            LightControl.SetLight(Entities.Light.WoonkamerKisten, 0);
            LightControl.SetLight(Entities.Light.Hal, 0);
            LightControl.SetLight(Entities.Light.KeukenKeukenlamp, 0);
            LightControl.SetLight(Entities.Light.Washal, 0);
            LightControl.SetLight(Entities.Light.WcWclamp, 0);
            LightControl.SetLight(Entities.Light.Badkamer, 0);
            LightControl.SetLight(Entities.Light.SpeelkamerLamp, 0);
            LightControl.SetLight(Entities.Light.Wandlampen, 0);

            // Kerst
            //Entities.Switch.Binnen1.TurnOff();
            //Entities.Switch.Binnen2.TurnOff();
        }

        public void HouseStateSleeping()
        {
            HouseStateAway(HouseStateEnum.Sleeping);
            Logger.LogInformation("Sleeping");

            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Sleeping.ToString());

            Entities.Climate.Keuken.SetTemperature(15);
            LightControl.SetLight(Entities.Light.NachtlampGreet, 1);
            LightControl.SetLight(Entities.Light.NachtlampKen, 1);

            Entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
            Entities.Vacuum.DreameP20294b09RobotCleaner.Start();

            if (Entities.DeviceTracker.GsmGreet.State == "home")
            {
                Notify.NotifyGsmGreet("Pillen", "Vergeet je avondpillen niet :)", NotifyPriorityEnum.high);
            }
            if (Entities.DeviceTracker.GsmKen.State == "home")
            {
                //Notify.NotifyGsmKen("Pillen", "Vergeet je avondpillen niet :)", NotifyPriorityEnum.high);
            }
        }

        public void HouseStateHoliday()
        {
            HouseStateAway(HouseStateEnum.Away);
            Logger.LogInformation("Holiday");

            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Holiday.ToString());
        }
    }
}
