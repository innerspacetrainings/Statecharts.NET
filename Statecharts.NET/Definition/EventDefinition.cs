using System;
using System.Collections.Generic;

namespace Statecharts.NET.Definition
{
    public interface IEvent : IEquatable<IEvent> { }
    public interface IEvent<out TData> : IEvent
    {
        TData Data { get; }
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
    public class CustomEvent<TData> : CustomEvent, IEvent<TData>
        where TData : IEquatable<TData>
    {
        public TData Data { get; }
        public CustomEvent(string eventName, TData data) : base(eventName) => Data = data;
    }
    public class ImmediateEvent : IEvent {
        public bool Equals(IEvent other) => other is ImmediateEvent;
    }
    public class ServiceDoneEvent<TResult> : IEvent<TResult> { }
    public class CompoundDoneEvent : IEvent { }
    public class DelayedEvent : IEvent { }
}
