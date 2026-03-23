using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonImpl.apps.Areas
{
    public abstract class AreaBase : MyNetDaemonBaseApp
    {
        protected readonly IDelayProvider delayProvider;
        protected readonly ILightControl lightControl;
        protected readonly IButtonEvents buttonEvents;
        private readonly IHouseStateEvents houseStateEvents;

        internal AreaModeEnum mode = AreaModeEnum.Idle;
        internal CancellationTokenSource? CTSAfter;
        internal CancellationTokenSource? CTSRun;

        public AreaBase(IHaContext haContext, IScheduler scheduler, ILogger logger, ISettingsProvider settingsProvider, IDelayProvider delayProvider, ILightControl lightControl, IButtonEvents deconzButtonEvents, IHouseStateEvents houseStateEvents) :
            base(haContext, scheduler, logger, settingsProvider)
        {
            this.delayProvider = delayProvider;
            this.lightControl = lightControl;
            this.buttonEvents = deconzButtonEvents;
            this.houseStateEvents = houseStateEvents;
        }

        /// <summary>
        /// The after task is designed for an action that needs to be executed after a delay.
        /// Starting a new after task will stop the currently running task
        /// </summary>
        /// <param name="Delay">The delay</param>
        /// <param name="action">The action</param>
        internal void StartAfterTask(TimeSpan Delay, Action action)
        {
            CTSAfter?.Cancel();
            CTSAfter = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    Task.Delay(Delay).Wait(CTSAfter.Token);
                    CTSAfter.Token.ThrowIfCancellationRequested();
                    action();
                }
                catch (OperationCanceledException)
                {
                    // Ignore
                }
            });
        }

        /// <summary>
        /// The RunTask is used to execute longer running tasks without blocking the return
        /// Starting a new run task will stop the currently running task
        /// </summary>
        /// <param name="action"></param>
        internal void StartRunTask(Action<CancellationToken> action)
        {
            CTSRun?.Cancel();
            CTSRun = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    action(CTSRun.Token);
                }
                catch (OperationCanceledException)
                {
                    // Ignore
                }
            });
        }

        /// <summary>
        /// Stops the currently running RunTask
        /// </summary>
        internal void StopAfterTask()
        {
            CTSAfter?.Cancel();
        }

        /// <summary>
        /// Stops the currently running RunTask
        /// </summary>
        internal void StopRunTask()
        {
            CTSRun?.Cancel();
        }

        public void ResetMotionState()
        {
            StopAfterTask();
            mode = AreaModeEnum.Idle;
        }

        public void SubscribeToDeconzButton(Button button)
        {
            buttonEvents.Event
           .Where(x => x.Button == button)
           .Subscribe(x =>
           {
               ButtonPressed(x);
           });
        }

        public void SubscribeToMotionSensor(BinarySensorEntity motionSensor)
        {
            motionSensor.StateChanges()
            .Subscribe(x =>
            {
                if (x.New?.State == "on")
                {
                    MotionDetected(motionSensor.EntityId);
                }
                else if (x.New?.State == "off")
                {
                    MotionCleared(motionSensor.EntityId);
                }
            });
        }

        public void SubscribeToOpenCloseSensor(BinarySensorEntity openCloseSensor)
        {
            openCloseSensor.StateChanges()
            .Subscribe(x =>
            {
                bool isOpen = x.New?.State == "on";
                OpenCloseChanged(openCloseSensor.EntityId, isOpen);
            });
        }

        public virtual void MotionCleared(string MotionSensor) { }

        /// <inheritdoc/>
        public virtual void MotionDetected(string MotionSensor) { }

        public virtual void OpenCloseChanged(string entityId, bool isOpen) { }

        protected void DefaultMotionManualButton(List<LightEntity> lightEntities)
        {
            StopAfterTask();
            // Button press will always trigger Manual mode
            mode = AreaModeEnum.Manual;
            StartAfterTask(delayProvider.ManualOffTimeout, () =>
            {
                if (lightEntities.All(x => x.IsOff()))
                {
                    mode = AreaModeEnum.Idle;
                }
            });
        }

        protected void DefaultMotionDetected(List<LightEntity> lightEntities, double brightness)
        {
            if (mode != AreaModeEnum.Manual)
            {
                StopAfterTask();
            }

            //Only trigger on Motion when the Area is in idle
            if (Helper.GetHouseState(_entities) == HouseStateEnum.Awake && Helper.GetDayNightState(_entities) == DayNightEnum.Night && mode == AreaModeEnum.Idle)
            {
                mode = AreaModeEnum.Motion;
                foreach (var light in lightEntities)
                {
                    if (brightness < 1 && brightness > 0)
                    {
                        lightControl.SetLightRgb(light, brightness * 255, [255, 255, 60]);
                    }
                    else
                    {
                        lightControl.SetLight(light, brightness);
                    }
                }
            }
        }

        protected void DefaultMotionCleared(List<LightEntity> lightEntities)
        {
            //Only create the after for motion cleared when the Area mode is Motion
            if (mode == AreaModeEnum.Motion)
            {
                StartAfterTask(delayProvider.MotionClear, () =>
                {
                    mode = AreaModeEnum.Idle;
                    foreach (var light in lightEntities)
                    {
                        lightControl.SetLight(light, 0);
                    }
                });
            }
        }

        public virtual void ButtonPressed(ButtonEvent buttonEvent) { }

        protected void SubscribeToHouseMotion()
        {
            SubscribeToMotionSensor(_entities.BinarySensor.MotionInkom);
            SubscribeToMotionSensor(_entities.BinarySensor.MotionKeuken);
            SubscribeToMotionSensor(_entities.BinarySensor.MotionEetkamer);
            houseStateEvents.Event.Subscribe(x => ResetMotionState());
        }
    }
}
