using System;
using System.Collections.Generic;

namespace Statecharts.NET.Definition
{
    public static class BaseEventDefinitionFunctions
    {
        public static TResult Map<TResult>(
            this BaseEventDefinition eventDefinition,
            Func<ImmediateEventDefinition, TResult> fImmediateEvent,
            Func<EventDefinition, TResult> fEvent,
            Func<ForbiddenEventDefinition, TResult> fForbiddenEvent)
        {
            switch(eventDefinition)
            {
                case ImmediateEventDefinition immediateEvent:
                    return fImmediateEvent(immediateEvent);
                case EventDefinition @event:
                    return fEvent(@event);
                case ForbiddenEventDefinition forbiddenEvent:
                    return fForbiddenEvent(forbiddenEvent);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public interface IEventDefinition { }
    
    public abstract class BaseEventDefinition : IEventDefinition
    {
    }

    public class EventDefinition : BaseEventDefinition
    {
        public BaseEvent Event { get; }
        public IEnumerable<EventTransitionDefinition> Transitions { get; set; }

        public EventDefinition(BaseEvent @event) => Event = @event;
    }
    public class ImmediateEventDefinition : BaseEventDefinition
    {
        public IEnumerable<ImmediateTransitionDefinition> Transitions { get; set; }
    }
    public class DoneEventDefinition : BaseEventDefinition
    {
        public IEnumerable<BaseTransitionDefinition> Transitions { get; set; } // TODO: correct generic type
    }
    public class DelayedEventDefinition : BaseEventDefinition
    {
        public TimeSpan Delay { get; }
        public IEnumerable<BaseTransitionDefinition> Transitions { get; set; } // TODO: correct generic type

        public DelayedEventDefinition(TimeSpan delay) => Delay = delay;
    }
    public class ForbiddenEventDefinition : BaseEventDefinition {
        public Event Event { get; set; }
    }
}
