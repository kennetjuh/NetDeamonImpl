using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;

namespace NetDaemonImpl.apps
{
    [NetDaemonApp]
    public class HouseApp : MyNetDaemonBaseApp
    {
        private readonly ILightControl lightControl;
        private IDisposable? lastDayTask;
        private IDisposable? lastNightTask;

        public HouseApp(IHaContext haContext, IScheduler scheduler, ILogger<HouseApp> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IDayNightEvents dayNightEvents, IHouseStateEvents houseStateEvents)
            : base(haContext, scheduler, logger, settingsProvider)
        {
            this.lightControl = lightControl;

            _haContext.RegisterServiceCallBack<ChangeHouseStateData>("ChangeHouseState", (x) =>
            {
                if (Enum.TryParse<HouseStateEnum>(x.state, out var state))
                {
                    HandleHouseState(new HouseStateEvent(state, new(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));
                }
            });


            dayNightEvents.LastNightChangedEvent.Subscribe(x =>
            {
                lastDayTask?.Dispose();
                lastDayTask = scheduler.RunDaily(Helper.StringToDateTime(_entities.InputDatetime.Daynightlastdaytrigger.State).TimeOfDay.Add(TimeSpan.FromHours(1)), () =>
                {
                    // Halloween, kerst etc.
                });
            });

            dayNightEvents.LastDayChangedEvent.Subscribe(x =>
            {
                lastNightTask?.Dispose();
                lastNightTask = scheduler.RunDaily(Helper.StringToDateTime(_entities.InputDatetime.Daynightlastnighttrigger.State).TimeOfDay.Subtract(TimeSpan.FromHours(1)), () =>
                {
                    // Halloween, kerst etc.
                });

            });

            houseStateEvents.Event.Subscribe(HandleHouseState);
            //HandleHouseState(new HouseStateEvent(Helper.GetHouseState(_entities), new(Button.HouseVoordeur, ButtonEventType.Single, DateTime.MinValue)));

            dayNightEvents.DayNightEvent.Subscribe(HandleDayNight);
            HandleDayNight(new DayNightEvent(Helper.GetDayNightState(_entities)));
        }

        private void HandleDayNight(DayNightEvent x)
        {
            switch (x.State)
            {
                case DayNightEnum.Day:
                    Day();
                    break;
                case DayNightEnum.Night:
                    Night();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


        record ChangeHouseStateData(string state);

        private void HandleHouseState(HouseStateEvent x)
        {
            switch (x.State)
            {
                case HouseStateEnum.Awake:
                    Awake(x.button);
                    break;
                case HouseStateEnum.Away:
                    Away(x.button);
                    break;
                case HouseStateEnum.Sleeping:
                    Sleeping(x.button);
                    break;
                case HouseStateEnum.Holiday:
                    Holiday(x.button);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Night()
        {
            lightControl.SetLight(_entities.Light.WandlampVoordeur, 1);
        }

        private void Day()
        {
            lightControl.SetLight(_entities.Light.WandlampVoordeur, 0);
        }


        public void Awake(ButtonEvent buttonEvent)
        {
            _logger.LogInformation("Awake");
            _entities.InputText.Housestate.SetValue(HouseStateEnum.Awake.ToString());

            var actions = AwayAction();

            if (buttonEvent.Button == Button.HouseInkom && (DateTime.Now - buttonEvent.previousEvent) > TimeSpan.FromSeconds(2))
            {
                actions.AddOrUpdate(_entities.Light.Inkom.EntityId, () => lightControl.SetLight(_entities.Light.Inkom, 10));
            }
            actions.AddOrUpdate(_entities.Light.LightKeukenEiland.EntityId, () => lightControl.SetLight(_entities.Light.LightKeukenEiland, 50));
            actions.AddOrUpdate(_entities.Light.WoonkamerNis.EntityId, () => lightControl.SetLight(_entities.Light.WoonkamerNis, 10));

            actions.AddOrUpdate(_entities.Switch.PlugWoonkamerKast.EntityId, () => _entities.Switch.PlugWoonkamerKast.TurnOn());

            //Kerst
            actions.AddOrUpdate(_entities.Switch.PlugKerst1.EntityId, () => _entities.Switch.PlugKerst1.TurnOn());
            actions.AddOrUpdate(_entities.Switch.PlugKerst2.EntityId, () => _entities.Switch.PlugKerst2.TurnOn());
            actions.AddOrUpdate(_entities.Switch.PlugKerst3.EntityId, () => _entities.Switch.PlugKerst3.TurnOn());

            actions.ExecuteActions();
        }

        public void Away(ButtonEvent buttonEvent)
        {
            _logger.LogInformation("Away");
            _entities.InputText.Housestate.SetValue(HouseStateEnum.Away.ToString());

            AwayAction().ExecuteActions();
        }

        private ActionCollection AwayAction()
        {
            var actionCollection = new ActionCollection();
            actionCollection.AddOrUpdate(_entities.Light.WoonkamerBank.EntityId, () => lightControl.SetLight(_entities.Light.WoonkamerBank, 0));
            actionCollection.AddOrUpdate(_entities.Light.WoonkamerBureau.EntityId, () => lightControl.SetLight(_entities.Light.WoonkamerBureau, 0));
            actionCollection.AddOrUpdate(_entities.Light.WoonkamerNis.EntityId, () => lightControl.SetLight(_entities.Light.WoonkamerNis, 0));

            actionCollection.AddOrUpdate(_entities.Light.LightKeuken.EntityId, () => lightControl.SetLight(_entities.Light.LightKeuken, 0));
            actionCollection.AddOrUpdate(_entities.Light.LightKeukenEiland.EntityId, () => lightControl.SetLight(_entities.Light.LightKeukenEiland, 0));

            actionCollection.AddOrUpdate(_entities.Light.Eetkamer.EntityId, () => lightControl.SetLight(_entities.Light.Eetkamer, 0));

            actionCollection.AddOrUpdate(_entities.Light.Inkom.EntityId, () => lightControl.SetLight(_entities.Light.Inkom, 0));

            actionCollection.AddOrUpdate(_entities.Light.Washal.EntityId, () => lightControl.SetLight(_entities.Light.Washal, 0));


            actionCollection.AddOrUpdate(_entities.Light.LightBadkamer.EntityId, () => lightControl.SetLight(_entities.Light.LightBadkamer, 0));
            actionCollection.AddOrUpdate(_entities.Light.LightBadkamerNis.EntityId, () => lightControl.SetLight(_entities.Light.LightBadkamerNis, 0));

            actionCollection.AddOrUpdate(_entities.Switch.PlugKelder.EntityId, () => _entities.Switch.PlugKelder.TurnOff());
            actionCollection.AddOrUpdate(_entities.Switch.PlugWoonkamerKast.EntityId, () => _entities.Switch.PlugWoonkamerKast.TurnOff());

            actionCollection.AddOrUpdate(_entities.Light.HalBoven.EntityId, () => lightControl.SetLight(_entities.Light.HalBoven, 0));
            actionCollection.AddOrUpdate(_entities.Light.HalBovenZij.EntityId, () => lightControl.SetLight(_entities.Light.HalBovenZij, 0));
            actionCollection.AddOrUpdate(_entities.Light.SlaapkamerCaitlyn.EntityId, () => lightControl.SetLight(_entities.Light.SlaapkamerCaitlyn, 0));
            actionCollection.AddOrUpdate(_entities.Light.SlaapkamerDamon.EntityId, () => lightControl.SetLight(_entities.Light.SlaapkamerDamon, 0));
            actionCollection.AddOrUpdate(_entities.Light.SlaapkamerKen.EntityId, () => lightControl.SetLight(_entities.Light.SlaapkamerKen, 0));
            actionCollection.AddOrUpdate(_entities.Light.Rommelkamer.EntityId, () => lightControl.SetLight(_entities.Light.Rommelkamer, 0));

            actionCollection.AddOrUpdate(_entities.Light.LightVeranda.EntityId, () => lightControl.SetLight(_entities.Light.LightVeranda, 0));
            actionCollection.AddOrUpdate(_entities.Light.SfeerGarage.EntityId, () => lightControl.SetLight(_entities.Light.SfeerGarage, 0));
            actionCollection.AddOrUpdate(_entities.Light.LightGarageLinks.EntityId, () => lightControl.SetLight(_entities.Light.LightGarageLinks, 0));
            actionCollection.AddOrUpdate(_entities.Light.LightGarageRechts1.EntityId, () => lightControl.SetLight(_entities.Light.LightGarageRechts1, 0));
            actionCollection.AddOrUpdate(_entities.Light.LightGarageRechts2.EntityId, () => lightControl.SetLight(_entities.Light.LightGarageRechts2, 0));

            //Kerst
            actionCollection.AddOrUpdate(_entities.Switch.PlugKerst1.EntityId, () => _entities.Switch.PlugKerst1.TurnOff());
            actionCollection.AddOrUpdate(_entities.Switch.PlugKerst2.EntityId, () => _entities.Switch.PlugKerst2.TurnOff());
            actionCollection.AddOrUpdate(_entities.Switch.PlugKerst3.EntityId, () => _entities.Switch.PlugKerst3.TurnOff());

            return actionCollection;
        }

        public void Sleeping(ButtonEvent buttonEvent)
        {
            _logger.LogInformation("Sleeping");
            _entities.InputText.Housestate.SetValue(HouseStateEnum.Sleeping.ToString());

            var actions = AwayAction();

            actions.AddOrUpdate(_entities.Light.SlaapkamerKen.EntityId, () => lightControl.SetLight(_entities.Light.SlaapkamerKen, 1));

            actions.ExecuteActions();
        }

        public void Holiday(ButtonEvent buttonEvent)
        {
            _logger.LogInformation("Holiday");
            _entities.InputText.Housestate.SetValue(HouseStateEnum.Holiday.ToString());

            AwayAction().ExecuteActions();
        }
    }
}
