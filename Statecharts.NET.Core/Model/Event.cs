using System;

namespace Statecharts.NET.Model
{
    #region Definition
    public interface IEventDefinition { }
    public interface IDataEventDefinition : IEventDefinition
    {
        object Data { get; }
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
    public class ServiceSuccessEventDefinition : IEventDefinition
    {
        public override string ToString() => "Service.Success";
    }
    public class ServiceErrorEventDefinition : IEventDefinition
    {
        public override string ToString() => "Service.Error";
    }
    public class DoneEventDefinition : IEventDefinition
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
        public override string ToString() => $"{GetType().Name}({_id})";
    }
    public class ImmediateEvent : IEvent
    {
        public object Data => null;
        public bool Equals(IEvent other) => other is ImmediateEvent;
        public override string ToString() => "Immediate";
    }
    public class DelayedEvent : IdEvent<DelayedEvent>
    {
        private readonly StatenodeId _statenodeId;
        public override object Data { get; }
        public DelayedEvent(StatenodeId statenodeId, TimeSpan delay) :
            base($"{statenodeId}:after:{delay.TotalMilliseconds}")
        {
            _statenodeId = statenodeId;
            Data = delay;
        }
        public override string ToString() => $"Delayed ({_statenodeId}, {((TimeSpan) Data).TotalMilliseconds}ms)";
    }
    public class InitializeEvent : IEvent
    {
        public StatenodeId StatenodeId { get; }
        public object Data => null;
        public InitializeEvent(StatenodeId id) => StatenodeId = id; // TODO: probably limit this to Compund & Orthogonal
        public bool Equals(IEvent other) =>
            other is InitializeEvent initializeEvent && initializeEvent.StatenodeId.Equals(StatenodeId);
        public override string ToString() => $"Initialize Statenode ({StatenodeId})";
    }
    public class InitializeStatechartEvent : IEvent
    {
        public object Data => null;
        public bool Equals(IEvent other) => other is InitializeStatechartEvent;
        public override string ToString() => "Initialize Statechart";
    }
    public class ServiceSuccessEvent : IdEvent<ServiceSuccessEvent>
    {
        private readonly string _serviceId;
        public override object Data { get; }
        public ServiceSuccessEvent(string serviceId, object result) :
            base($"{serviceId}.success") // TODO: think of this key (lookup xstate)
        {
            _serviceId = serviceId;
            Data = result;
        }
        public override string ToString() => $"Service Success ({_serviceId})";
    }
    public class ServiceErrorEvent : IdEvent<ServiceErrorEvent>
    {
        private readonly string _serviceId;
        internal Exception Exception { get; }
        public override object Data => Exception;
        public ServiceErrorEvent(string serviceId, Exception exception) : base($"{serviceId}.error") // TODO: think of this key (lookup xstate)
        {
            _serviceId = serviceId;
            Exception = exception;
        }

        public override string ToString() => $"Service Error ({_serviceId}, {Exception.GetType().Name}: {Exception.Message})";
    }
    public class DoneEvent : IdEvent<DoneEvent> {
        private readonly StatenodeId _statenodeId;
        public DoneEvent(StatenodeId statenodeId) : base($"{statenodeId}.done") => _statenodeId = statenodeId;
        public override string ToString() => $"Done ({_statenodeId})";
    }
    public class ExecutionErrorEvent : IEvent
    {
        public Exception Exception { get; }
        public object Data => Exception;
        public ExecutionErrorEvent(Exception exception) => Exception = exception;
        public bool Equals(IEvent other) => other is ExecutionErrorEvent;
    }
    #endregion
    #region Sendable Events
    public interface ISendableEvent : IEvent
    {
        string Name { get; }
    }
    public interface INamedEvent : ISendableEvent { }
    public interface INamedDataEvent : ISendableEvent { }
    public class NamedEvent : IEventDefinition, INamedEvent
    {
        public string Name { get; }
        public object Data => null;
        public NamedEvent(string eventName) => Name = eventName;
        public bool Equals(IEvent other) => other is ISendableEvent @event && @event.Name == Name; // TODO: check whether INamedEvent would be better
        public override string ToString() => $"@{Name}";
    }
    public class NamedDataEvent<TData> : IDataEventDefinition, INamedDataEvent
    {
        private readonly TData _data;
        public string Name { get; }
        public object Data => _data;

        public NamedDataEvent(string eventName, TData data)
        {
            Name = eventName;
            _data = data;
        }
        public bool Equals(IEvent other) => other is ISendableEvent @event && @event.Name == Name; // TODO: check whether IDataNamedEvent would be better
        public override string ToString() => $"@{Name} (Data: {_data})";
    }
    #endregion
}