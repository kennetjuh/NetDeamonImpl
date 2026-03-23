using System;

namespace NetDaemonInterface.Observable
{
    public interface IDayNightEvents
    {
        IObservable<DayNightEvent> DayNightEvent { get; }
        IObservable<object> LastDayChangedEvent { get; }
        IObservable<object> LastNightChangedEvent { get; }
    }
    public record DayNightEvent(DayNightEnum State);
}