using NetDaemonInterface.Enums;
using System;

namespace NetDaemonInterface.Observable
{
    public interface IButtonEvents
    {
        IObservable<ButtonEvent> Event { get; }
    }
    public record ButtonEvent(Button Button, ButtonEventType Event, DateTime previousEvent);
}