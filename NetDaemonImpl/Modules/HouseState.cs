using NetDaemonInterface;

namespace NetDaemonImpl.Modules
{
    public class HouseState : IHouseState
    {
        private readonly Entities Entities;
        private readonly Services Services;
        private readonly ILightControl lightControl;
        private readonly ITwinkle twinkle;
        private readonly INotify notify;

        public HouseState(IServiceProvider provider, ILightControl lightControl, ITwinkle twinkle, INotify notify)
        {
            this.lightControl = lightControl;
            this.twinkle = twinkle;
            this.notify = notify;
            var haContext = DiHelper.GetHaContext(provider);
            Entities = new Entities(haContext);
            Services = new Services(haContext);
        }

        public void HouseStateAwake()
        {
            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Awake.ToString());

            lightControl.SetLight(Entities.Light.WoonkamerKamer, 0);
            lightControl.SetLight(Entities.Light.WoonkamerBureau, 0);
            lightControl.SetLight(Entities.Light.TabletScreen);
            lightControl.SetLight(Entities.Light.WoonkamerKisten);

            if (Helper.GetDayNightState(Entities) == DayNightEnum.Day)
            {
                lightControl.SetLight(Entities.Light.LightWoonWand, 125);
                lightControl.SetLight(Entities.Light.WoonkamerBoog, 125);
            }
            else
            {
                lightControl.SetLight(Entities.Light.LightWoonWand, 70);
                lightControl.SetLight(Entities.Light.WoonkamerBoog, 70);
            }

            //Entities.Switch.BinnenKerst.TurnOn();
            twinkle.Start();
        }

        public void HouseStateAway()
        {
            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Away.ToString());

            // buiten
            lightControl.SetLight(Entities.Light.BuitenachterLamp, 0);
            lightControl.SetLight(Entities.Light.BuitenachterSierverlichting, 0);
            Entities.Switch.SwitchInfinityMirror.TurnOff();
            Entities.Switch.BuitenachterFontein.TurnOff();
            Entities.Switch.BuitenachterGrondpomp.TurnOff();
            Entities.Switch.SwitchVliegenlamp.TurnOff();

            // boven
            lightControl.SetLight(Entities.Light.LightCabine, 0);
            Entities.Switch.SwitchSierCabine.TurnOff();

            // onder
            lightControl.SetLight(Entities.Light.WoonkamerBoog, 0);
            lightControl.SetLight(Entities.Light.WoonkamerKamer, 0);
            lightControl.SetLight(Entities.Light.WoonkamerBureau, 0);
            lightControl.SetLight(Entities.Light.TabletScreen, 0);
            lightControl.SetLight(Entities.Light.WoonkamerKisten, 0);
            lightControl.SetLight(Entities.Light.LightHal, 0);
            lightControl.SetLight(Entities.Light.KeukenKeukenlamp, 0);
            lightControl.SetLight(Entities.Light.WashalWashal, 0);
            lightControl.SetLight(Entities.Light.WcWclamp, 0);
            lightControl.SetLight(Entities.Light.BadkamerLamp, 0);
            lightControl.SetLight(Entities.Light.SpeelkamerLamp, 0);
            lightControl.SetLight(Entities.Light.LightWoonWand, 0);

            //Entities.Switch.BinnenKerst.TurnOff();
        }

        public void HouseStateSleeping()
        {
            HouseStateAway();
            Entities.Sensor.Housestate.SetState(Services, HouseStateEnum.Sleeping.ToString());

            Entities.Climate.Keuken.SetTemperature(15);
            lightControl.SetLight(Entities.Light.SlaapkamerNachtlampGreet, 1);
            lightControl.SetLight(Entities.Light.SlaapkamerNachtlampKen, 1);

            Entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Strong");
            Entities.Vacuum.DreameP20294b09RobotCleaner.Start();

            notify.NotifyGsm("Pillen", "Vergeet je avondpillen niet :)");
        }
    }
}
