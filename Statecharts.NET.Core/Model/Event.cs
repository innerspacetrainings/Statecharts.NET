using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public interface IEventDefinition : IEquatable<IEventDefinition> { }
    public interface IDataEventDefinition : IEventDefinition
    {
        object Data { get; }
    }
    public class NamedEventDefinition
    {
        public string EventName { get; }
        public NamedEventDefinition(string eventName) => EventName = eventName;
        public override string ToString() => $"@\"{EventName}\"";
    }
    public class NamedDataEventDefinition : NamedEventDefinition
    {
        public object Data { get; }
        public NamedDataEventDefinition(string eventName, object data) : base(eventName) => Data = data;
        public override string ToString() => $"@\"{EventName}\" (Data={Data})";
    }
    public class ImmediateEventDefinition
    {
        public override string ToString() => "Immediately";
    }
    public class DelayedEventDefinition
    {
        public TimeSpan Delay { get; }
        public DelayedEventDefinition(TimeSpan delay) => Delay = delay;
        public override string ToString() => $"After: {Delay.TotalSeconds} seconds";
    }
    public class ServiceSuccessEventDefinition
    {
        public override string ToString() => "Service.Success";
    }
    public class ServiceErrorEventDefinition
    {
        public override string ToString() => "Service.Error";
    }
    public class DoneEventDefinition
    {
        public override string ToString() => "Done";
    }
    #endregion


    // TODO: take a look into this
    public interface IEvent : IEquatable<IEvent> { }
    public interface ISendableEvent : IEvent { }

    public abstract class Event : OneOfBase<NamedEvent, ImmediateEvent, DelayedEvent>, IEvent
    {
        public virtual bool Equals(IEvent other) => this.Match(Equals, Equals, Equals);
    }

    public class NamedEvent : Event, ISendableEvent
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
    public class DelayedEvent : Event, ISendableEvent {
        public TimeSpan Delay { get; }
        public DelayedEvent(TimeSpan delay) => Delay = delay;

        public override bool Equals(IEvent other) => other == this; // TODO: think of this
    }
    public class CustomDataEvent : ISendableEvent // TODO: think of this, probably inherit NamedEvent and only add Data
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
    public class ServiceSuccessEvent : ISendableEvent {
        public bool Equals(IEvent other) => throw new NotImplementedException();
    }
    public class ServiceErrorEvent : ISendableEvent
    {
        public bool Equals(IEvent other) => throw new NotImplementedException();
    }
    public class CompoundDoneEvent : IEvent {
        public bool Equals(IEvent other) => throw new NotImplementedException();
    }
}