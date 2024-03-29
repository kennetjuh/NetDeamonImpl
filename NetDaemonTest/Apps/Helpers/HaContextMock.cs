using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using NetDaemon.HassModel.Entities;

namespace NetDaemonTest.Apps.Helpers;

public class HaContextMockImpl : IHaContext
{
    public Dictionary<string, EntityState> _entityStates { get; } = new();
    public Subject<StateChange> StateAllChangeSubject { get; } = new();
    public Subject<Event> EventsSubject { get; } = new();

    public IObservable<StateChange> StateAllChanges() => StateAllChangeSubject;

    public EntityState? GetState(string entityId) => _entityStates.TryGetValue(entityId, out var result) ? result : null;

    public IReadOnlyList<Entity> GetAllEntities() => _entityStates.Keys.Select(s => new Entity(this, s)).ToList();

    public virtual void CallService(string domain, string service, ServiceTarget? target = null, object? data = null)
    { }

    public Area? GetAreaFromEntityId(string entityId) => null;

    public virtual void SendEvent(string eventType, object? data = null)
    { }

    public Task<JsonElement?> CallServiceWithResponseAsync(string domain, string service, ServiceTarget? target = null, object? data = null)
    {
        throw new NotImplementedException();
    }

    public IObservable<Event> Events => EventsSubject;
}

public class HaContextMock : Mock<HaContextMockImpl>
{
    public HaContextMock(MockBehavior behaviour)
        : base(behaviour)
    {
    }

    public void TriggerStateChange(Entity entity, string newStatevalue, object? attributes = null)
    {
        var newState = new EntityState { State = newStatevalue };
        if (attributes != null)
        {
            newState = newState with { AttributesJson = attributes.AsJsonElement() };
        }

        TriggerStateChange(entity.EntityId, newState);
    }

    public void TriggerStateChange(string entityId, EntityState newState)
    {
        var oldState = Object._entityStates.TryGetValue(entityId, out var current) ? current : null;
        Object._entityStates[entityId] = newState;
        Object.StateAllChangeSubject.OnNext(new StateChange(new Entity(Object, entityId), oldState, newState));
    }

    public void VerifyServiceCalled(Entity entity, string domain, string service)
    {
        Verify(m => m.CallService(domain, service,
            It.Is<ServiceTarget?>(s => s!.EntityIds!.SingleOrDefault() == entity.EntityId),
            null));
    }

    public void TriggerEvent(Event @event)
    {
        Object.EventsSubject.OnNext(@event);
    }
}

public static class TestExtensions
{
    public static JsonElement AsJsonElement(this object value)
    {
        var jsonString = JsonSerializer.Serialize(value);
        return JsonSerializer.Deserialize<JsonElement>(jsonString);
    }
}


