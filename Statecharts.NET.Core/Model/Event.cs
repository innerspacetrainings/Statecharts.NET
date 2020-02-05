using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public interface IEvent : IEquatable<IEvent> { }

    public abstract class Event : OneOfBase<NamedEvent, ImmediateEvent, DelayedEvent>, IEvent
    {
        public virtual bool Equals(IEvent other) => this.Match(Equals, Equals, Equals);
    }

    public class NamedEvent : Event
    {
        public string EventName { get; }
        public NamedEvent(string eventName) => EventName = eventName;

        public override bool Equals(IEvent other) => other is NamedEvent @event && @event.EventName == EventName;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((NamedEvent) obj);
        }

        public override int GetHashCode() => EventName != null ? EventName.GetHashCode() : 0;
        public override string ToString() => $"@\"{EventName}\"";
    }
    public class ImmediateEvent : Event {
        public override bool Equals(IEvent other) => other is ImmediateEvent;
        public override string ToString() => "Immediately";
    }
    public class DelayedEvent : Event {
        public TimeSpan Delay { get; }
        public DelayedEvent(TimeSpan delay) => Delay = delay;

        public override bool Equals(IEvent other) => other == this; // TODO: think of this
    }
    public class CustomDataEvent : IEvent // TODO: think of this, probably inherit NamedEvent and only add Data
    {
        public string EventName { get; }
        public object Data { get; }

        public CustomDataEvent(string eventName, object data)
        {
            EventName = eventName;
            Data = data;
        }

        public virtual bool Equals(IEvent other) => other is CustomDataEvent @event && @event.EventName == EventName;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CustomDataEvent)obj);
        }

        public override int GetHashCode() => EventName != null ? EventName.GetHashCode() : 0;
    }
    public class ServiceSuccessEvent : IEvent {
        public bool Equals(IEvent other) => throw new NotImplementedException();
    }
    public class ServiceErrorEvent : IEvent
    {
        public bool Equals(IEvent other) => throw new NotImplementedException();
    }
    public class CompoundDoneEvent : IEvent {
        public bool Equals(IEvent other) => throw new NotImplementedException();
    }
}