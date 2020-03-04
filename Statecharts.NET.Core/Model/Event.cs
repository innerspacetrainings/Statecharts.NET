using System;

namespace Statecharts.NET.Model
{
    public interface ISendableEventDefinition : IEventDefinition
    {
        string Name { get; }
    }
    public interface ISendableEvent : IEvent
    {
        string Name { get; }
    }
    public interface ISendableDataEventDefinition<TData> : IDataEventDefinition
    {
        string Name { get; }
    }
    public interface ISendableDataEvent : IEvent
    {
        string Name { get; }
        object Data { get; }
    }


    #region Definition
    public interface IEventDefinition { }
    public interface IDataEventDefinition : IEventDefinition
    {
        object Data { get; }
    }
    public class NamedEventDefinition : IEventDefinition // TODO: ISendableEventDefinition
    {
        public string Name { get; }
        public NamedEventDefinition(string eventName) => Name = eventName;
        public override string ToString() => $"@\"{Name}\"";
    }
    public class NamedDataEventDefinition : NamedEventDefinition, IDataEventDefinition
    {
        public object Data { get; }
        public NamedDataEventDefinition(string eventName, object data) : base(eventName) => Data = data;
        public override string ToString() => $"@\"{Name}\" (Data={Data})";
    }
    public class ImmediateEventDefinition : IEventDefinition
    {
        public override string ToString() => "Immediately";
    }
    public class DelayedEventDefinition : IEventDefinition
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
    #region Executable
    public interface IEvent : IEquatable<IEvent>
    {
        object Data { get; }
    }
    public class IdEvent<TActualEvent> : IEvent
        where TActualEvent : IdEvent<TActualEvent>
    {
        private readonly string _id;
        public virtual object Data => null;
        protected IdEvent(string id) => _id = id;
        public bool Equals(IEvent other) => other is TActualEvent @event && @event._id == _id;
    }
    public class NamedEvent : IEvent
    {
        public string EventName { get; }
        public object Data { get; }

        public NamedEvent(string eventName, object data = null)
        {
            EventName = eventName;
            Data = data;
        }

        public bool Equals(IEvent other) => other is NamedEvent @event && @event.EventName == EventName;
    }
    public class ImmediateEvent : IEvent
    {
        public object Data => null;
        public bool Equals(IEvent other) => other is ImmediateEvent;
    }
    public class DelayedEvent : IdEvent<DelayedEvent>
    {
        public DelayedEvent(Statenode statenode, DelayedEventDefinition definition) :
            base($"{statenode.Key}:after:{definition.Delay.TotalMilliseconds}") { } // TODO: think of this key (lookup xstate)
    }
    public class InitializeEvent : IEvent
    {
        public object Data => null;
        public bool Equals(IEvent other) => other is InitializeEvent;
    }
    public class ServiceSuccessEvent : IdEvent<ServiceSuccessEvent>
    {
        public ServiceSuccessEvent(string serviceId) : base($"{serviceId}.success") { } // TODO: think of this key (lookup xstate)
    }
    public class ServiceErrorEvent : IdEvent<ServiceErrorEvent>
    {
        public ServiceErrorEvent(string serviceId) : base($"{serviceId}.error") { } // TODO: think of this key (lookup xstate)
    }
    public class DoneEvent : IdEvent<DoneEvent> {
        public DoneEvent(Statenode statenode) : base($"{statenode}.done") { } // TODO: think of this key (lookup xstate)
    }
    public class ExecutionErrorEvent : IEvent
    {
        public Exception Exception { get; }
        public object Data => null;
        public ExecutionErrorEvent(Exception exception) => Exception = exception;
        public bool Equals(IEvent other) => other is ExecutionErrorEvent;
    }
    #endregion
}