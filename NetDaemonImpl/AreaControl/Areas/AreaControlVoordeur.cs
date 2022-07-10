using NetDaemonInterface;

namespace NetDaemonImpl.AreaControl.Areas;

public class AreaControlVoordeur : AreaControl
{
    private readonly LightEntity light;

    public AreaControlVoordeur(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl) : base(entities, delayProvider, lightControl)
    {
        light = entities.Light.WandlampBuiten;
    }

    public override void MotionCleared(string entityId)
    {
        //Only create the after for motion cleared when the Area mode is Motion
        if (mode == AreaModeEnum.Motion)
        {
            StartAfterTask(delayProvider.MotionClear, () =>
            {
                mode = AreaModeEnum.Idle;
                lightControl.SetLight(light, Constants.brightnessWandVoor);
            });
        }
    }

    public override void MotionDetected(string entityId)
    {
        //Only trigger on Motion when the Area is in idle and the light is on (night)
        if (light.IsOn() && mode == AreaModeEnum.Idle)
        {
            mode = AreaModeEnum.Motion;
            lightControl.SetLight(light, Constants.brightnessWandVoorMotion);
        }
    }


}
