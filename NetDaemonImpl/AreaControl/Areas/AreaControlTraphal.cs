using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlTraphal : AreaControl
{
    private readonly LightEntity light;

    private const double minBrightness = 10;
    private const double maxBrightness = 255;

    public AreaControlTraphal(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.LightTraphal;
    }

    public override void ButtonPressed(string entityId, DeconzEventIdEnum eventId)
    {
        // Button press will always trigger Manual mode
        StopAfterTask();
        StopRunTask();
        mode = AreaModeEnum.Manual;

        // If the result of the button press is the light is on trigger the after for manual mode
        if (lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness))
        {
            StartAfterTask(delayProvider.MotionClearManual, () =>
            {
                mode = AreaModeEnum.Idle;
                lightControl.SetLight(light, 0);
            });
        }
        // The light is off, wait for a small timeout before going back to idle
        else
        {
            StartAfterTask(delayProvider.ManualOffTimeout, () =>
            {
                mode = AreaModeEnum.Idle;
            });
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
                lightControl.SetLight(light, 0);
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

            var brightness = lightControl.luxBasedBrightness.GetBrightness(minBrightness, maxBrightness);

            // Sensor 1 = boven
            if (entityId == entities.BinarySensor.MotionTraphal1.EntityId)
            {
                StartRunTask((Ct) =>
                {
                    lightControl.SetLight(entities.Light.LightTraphal3, brightness);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.LightTraphal2, brightness);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.LightTraphal1, brightness);
                });
            }
            else
            {
                StartRunTask((Ct) =>
                {
                    lightControl.SetLight(entities.Light.LightTraphal1, brightness);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.LightTraphal2, brightness);
                    DelayRunTaskAndCheckCancellation(delayProvider.MotionOnSequenceDelay, Ct);
                    lightControl.SetLight(entities.Light.LightTraphal3, brightness);
                });
            }

        }
    }
}
