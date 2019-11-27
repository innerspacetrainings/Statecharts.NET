using System;

namespace Statecharts.NET
{
    public abstract class BaseEvent : IEquatable<BaseEvent>
    {
        public abstract bool Equals(BaseEvent other);
        public abstract override int GetHashCode();
    }

    public class Event : BaseEvent
    {
        public Event(string type)
        {
            Type = type;
        }

        public string Type { get; }

        public override bool Equals(BaseEvent obj) => obj is Event @event && Type == @event.Type;
        public override int GetHashCode() => Type.GetHashCode();
    }

    public class DataEvent<TData> : Event
    {
        public DataEvent(string type, TData data) : base(type)
        {
            Data = data;
        }

        public TData Data { get; }

        public override bool Equals(BaseEvent obj) => obj is DataEvent<TData> @event && Type == @event.Type;
    }
}
