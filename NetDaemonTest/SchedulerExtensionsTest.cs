using Microsoft.Reactive.Testing;
using NetDaemonImpl.Extensions;
using Xunit;

namespace NetDaemonTest;

public class SchedulerExtensionsTest
{
    [Fact]
    public void RunDaily_TimeInFuture_SchedulesForToday()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var executed = false;
        var futureTime = scheduler.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

        // Act
        scheduler.RunDaily(futureTime, () => executed = true);
        scheduler.AdvanceBy(TimeSpan.FromHours(1).Ticks);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void RunDaily_TimeInPast_SchedulesForTomorrow()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var executed = false;

        // Start the scheduler at noon
        scheduler.AdvanceTo(DateTimeOffset.Now.Date.AddHours(12).Ticks);
        var pastTime = TimeSpan.FromHours(6); // 6 AM is in the past

        // Act
        scheduler.RunDaily(pastTime, () => executed = true);

        // Advance 18 hours (to 6 AM next day)
        scheduler.AdvanceBy(TimeSpan.FromHours(18).Ticks);

        // Assert
        Assert.True(executed);
    }
}
