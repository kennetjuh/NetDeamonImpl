using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlWashal : AreaControl
{
    private readonly LightEntity light;
    private const double minBrightness = 10;
    private const double maxBrightness = 255;

    public AreaControlWashal(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.Washal;
    }

    public override void ButtonPressed(string entityId, DeconzEventIdEnum eventId)
    {
        StopAfterTask();
        // Button press will always trigger Manual mode
        mode = AreaModeEnum.Manual;

        if (!lightControl.ButtonDefaultLuxBased(eventId, light, minBrightness, maxBrightness))
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
            lightControl.SetLight(light, lightControl.LuxBasedBrightness.GetBrightness(minBrightness, maxBrightness));
        }
    }
}
