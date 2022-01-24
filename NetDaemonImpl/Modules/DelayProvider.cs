using NetDaemonInterface;

namespace NetDaemonImpl.Modules
{
    public class DelayProvider : IDelayProvider
    {
        public TimeSpan MotionClear => TimeSpan.FromMinutes(1);

        public TimeSpan MotionClearManual => TimeSpan.FromMinutes(5);

        public TimeSpan ManualOffTimeout => TimeSpan.FromSeconds(5);

        public TimeSpan MotionOnSequenceDelay => TimeSpan.FromSeconds(1);

        public TimeSpan ModeCycleTimeout => TimeSpan.FromSeconds(5);

        public TimeSpan TwinkleDelay => TimeSpan.FromSeconds(120);
    }
}
