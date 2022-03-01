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

            LightControl.SetLight(Entities.Light.WoonkamerKamer, 0);
            LightControl.SetLight(Entities.Light.WoonkamerBureau, 0);
            LightControl.SetLight(Entities.Light.TabletScreen);
            LightControl.SetLight(Entities.Light.WoonkamerKisten);
            LightControl.SetLight(Entities.Light.WoonkamerValentijn);


            if (Helper.GetDayNightState(Entities) == DayNightEnum.Day)
            {
                LightControl.SetLight(Entities.Light.LightWoonWand, Constants.brightnessWandDay);
                LightControl.SetLight(Entities.Light.WoonkamerBoog, Constants.brightnessBoogDay);
            }
            else
            {
                LightControl.SetLight(Entities.Light.LightWoonWand, Constants.brightnessWandNight);
                LightControl.SetLight(Entities.Light.WoonkamerBoog, Constants.brightnessBoogNight);
            }

            //Entities.Switch.BinnenKerst.TurnOn();
            Twinkle.Start();
        }

        public void HouseStateAway()
        {
            Logger.LogInformation("Away");

            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Away.ToString());

            // buiten
            LightControl.SetLight(Entities.Light.BuitenachterLamp, 0);
            LightControl.SetLight(Entities.Light.BuitenachterSierverlichting, 0);
            Entities.Switch.SwitchInfinityMirror.TurnOff();
            Entities.Switch.BuitenachterFontein.TurnOff();
            Entities.Switch.BuitenachterGrondpomp.TurnOff();
            Entities.Switch.SwitchVliegenlamp.TurnOff();

            // boven
            LightControl.SetLight(Entities.Light.LightCabine, 0);
            Entities.Switch.SwitchSierCabine.TurnOff();

            // onder
            LightControl.SetLight(Entities.Light.WoonkamerBoog, 0);
            LightControl.SetLight(Entities.Light.WoonkamerKamer, 0);
            LightControl.SetLight(Entities.Light.WoonkamerBureau, 0);
            LightControl.SetLight(Entities.Light.TabletScreen, 0);
            LightControl.SetLight(Entities.Light.WoonkamerKisten, 0);
            LightControl.SetLight(Entities.Light.LightHal, 0);
            LightControl.SetLight(Entities.Light.KeukenKeukenlamp, 0);
            LightControl.SetLight(Entities.Light.WashalWashal, 0);
            LightControl.SetLight(Entities.Light.WcWclamp, 0);
            LightControl.SetLight(Entities.Light.BadkamerLamp, 0);
            LightControl.SetLight(Entities.Light.SpeelkamerLamp, 0);
            LightControl.SetLight(Entities.Light.LightWoonWand, 0);
            LightControl.SetLight(Entities.Light.WoonkamerValentijn, 0);

            //Entities.Switch.BinnenKerst.TurnOff();
        }

        public void HouseStateSleeping()
        {
            HouseStateAway();
            Logger.LogInformation("Sleeping");

            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Sleeping.ToString());

            Entities.Climate.Keuken.SetTemperature(15);
            LightControl.SetLight(Entities.Light.SlaapkamerNachtlampGreet, 1);
            LightControl.SetLight(Entities.Light.SlaapkamerNachtlampKen, 1);

            Entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
            Entities.Vacuum.DreameP20294b09RobotCleaner.Start();

            if (Entities.DeviceTracker.GsmGreet.State == "home")
            {
                Notify.NotifyGsmGreet("Pillen", "Vergeet je avondpillen niet :)");
            }
            if (Entities.DeviceTracker.GsmKen.State == "home")
            {
                Notify.NotifyGsmKen("Pillen", "Vergeet je avondpillen niet :)");
            }
        }
    }
}
