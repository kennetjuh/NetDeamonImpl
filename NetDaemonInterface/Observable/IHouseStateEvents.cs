using System;

namespace NetDaemonInterface.Observable
{
    public record HouseStateEvent(HouseStateEnum State, ButtonEvent button);

    public interface IHouseStateEvents
    {
        IObservable<HouseStateEvent> Event { get; }
    }
}