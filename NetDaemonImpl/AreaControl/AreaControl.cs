using NetDaemonInterface;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonImpl.AreaControl;

/// <summary>
/// Abstract base class used for as base for specific AreaControls
/// Provide helpers and functions to specific implementations of an area
/// </summary>
public abstract class AreaControl : IAreaControl
{
    internal AreaModeEnum mode = AreaModeEnum.Idle;
    internal CancellationTokenSource? CTSAfter;
    internal CancellationTokenSource? CTSRun;
    internal IEntities entities;
    internal IDelayProvider delayProvider;
    internal ILightControl lightControl;

    public AreaControl(IEntities entities, IDelayProvider delayProvider, ILightControl lightControl)
    {
        this.entities = entities;
        this.delayProvider = delayProvider;
        this.lightControl = lightControl;
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
    /// Helper to be used within a RunTask action, waits for a delay and checks for cancellation
    /// </summary>
    /// <param name="delay">The delay</param>
    /// <param name="ct">The cancellation token</param>
    internal void DelayRunTaskAndCheckCancellation(TimeSpan delay, CancellationToken ct)
    {
        Task.Delay(delay).Wait(ct);
        ct.ThrowIfCancellationRequested();
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

    /// <inheritdoc/>
    public virtual void ButtonPressed(string ButtonSensor, DeconzEventIdEnum eventId) { }

    /// <inheritdoc/>
    public virtual void MotionCleared(string MotionSensor) { }

    /// <inheritdoc/>
    public virtual void MotionDetected(string MotionSensor) { }
}