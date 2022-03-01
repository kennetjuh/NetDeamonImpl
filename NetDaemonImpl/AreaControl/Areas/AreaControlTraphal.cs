using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlTraphal : AreaControl
{
    private readonly LightEntity LightWand;
    private readonly LightEntity Light;

    private const double minBrightnessWand = 50;
    private const double maxBrightnessWand = 255;

    private const double minBrightness = 1;
    private const double maxBrightness = 150;

    public AreaControlTraphal(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        LightWand = entities.Light.TraphalWand;
        Light = entities.Light.Traphal;
    }

    public override void ButtonPressed(string entityId, DeconzEventIdEnum eventId)
    {
        // Button press will always trigger Manual mode
        StopAfterTask();
        StopRunTask();
        mode = AreaModeEnum.Manual;

        if (LightWand.IsOn() && eventId == DeconzEventIdEnum.Single)
        {
            lightControl.SetLight(Light, 0);
            lightControl.SetLight(LightWand, 0);

            StartAfterTask(delayProvider.ManualOffTimeout, () =>
            {
                mode = AreaModeEnum.Idle;
            });
        }
        else
        {
            lightControl.ButtonDefaultLuxBased(eventId, LightWand, minBrightnessWand, maxBrightnessWand);
            lightControl.ButtonDefaultLuxBased(eventId, Light, minBrightness, maxBrightness);
        }
    }

    public override void MotionCleared(string entityId)
    {
        //Only create the after for motion cleared when the Area mode is Motion
        if (mode == AreaModeEnum.Motion)
        {
            StartAfterTask(delayProvider.MotionClear, () =>
            {
                mode = AreaModeEnum.Idle;
                lightControl.SetLight(LightWand, 0);
                lightControl.SetLight(Light, 0);
            });
        }
    }

    public override void MotionDetected(string entityId)
    {
        if (mode != AreaModeEnum.Manual)
        {
            StopAfterTask();
        }

        //Only trigger on Motion when the Area is in idle
        if (mode == AreaModeEnum.Idle)
        {
            mode = AreaModeEnum.Motion;

            var brightnessWand = lightControl.LuxBasedBrightness.GetBrightness(minBrightnessWand, maxBrightnessWand);
            var brightness = lightControl.LuxBasedBrightness.GetBrightness(minBrightness, maxBrightness);

            // Sensor 1 = boven
            if (entityId == entities.BinarySensor.MotionTraphal1.EntityId)
            {
                StartRunTask((Ct) =>
                {
                    lightControl.SetLight(entities.Light.Traphal3, brightnessWand);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.Traphal2, brightnessWand);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.Traphal1, brightnessWand);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.Traphal, brightness);
                });
            }
            else
            {
                StartRunTask((Ct) =>
                {
                    lightControl.SetLight(entities.Light.Traphal, brightness);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.Traphal1, brightnessWand);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.Traphal2, brightnessWand);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.Traphal3, brightnessWand);
                });
            }

        }
    }
}
