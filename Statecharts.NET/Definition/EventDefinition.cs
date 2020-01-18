using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public interface IEvent : IEquatable<IEvent> { }

    public class Event : OneOfBase<CustomEvent, CustomDataEvent, ImmediateEvent, DelayedEvent, ServiceDoneEvent, CompoundDoneEvent>, IEvent {
        public bool Equals(IEvent other) => this.Match(Equals, Equals, Equals, Equals, Equals, Equals);
    }

    public class CustomEvent : IEvent
    {
        public string EventName { get; }
        public CustomEvent(string eventName) => EventName = eventName;

        public virtual bool Equals(IEvent other) => other is CustomEvent @event && @event.EventName == EventName;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CustomEvent) obj);
        }

        public override int GetHashCode() => EventName != null ? EventName.GetHashCode() : 0;
    }
    public class CustomDataEvent : CustomEvent
    {
        public object Data { get; }
        public CustomDataEvent(string eventName, object data) : base(eventName) => Data = data;
    }
    public class ImmediateEvent : IEvent {
        public bool Equals(IEvent other) => other is ImmediateEvent;
    }
    public class DelayedEvent : IEvent {
        public TimeSpan Delay { get; }
        public DelayedEvent(TimeSpan delay) => Delay = delay;

        public bool Equals(IEvent other) => other == this; // TODO: think of this
    }
    public class ServiceDoneEvent : IEvent { }
    public class CompoundDoneEvent : IEvent { }
}
