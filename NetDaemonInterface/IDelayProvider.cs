using System;

namespace NetDaemonInterface;

public interface IDelayProvider
{
    public TimeSpan MotionClear { get; }
    public TimeSpan MotionClearManual { get; }
    public TimeSpan ManualOffTimeout { get; }
    public TimeSpan MotionOnSequenceDelay { get; }
    public TimeSpan ModeCycleTimeout { get; }
    public TimeSpan TwinkleDelay { get; }
}
