using System;
using System.Collections.Generic;

namespace Statecharts.NET.Definition
{
    public static class BaseEventDefinitionFunctions
    {
        public static TResult Map<TResult>(
            this BaseEventDefinition eventDefinition,
            Func<ImmediateEventDefinition, TResult> fImmediateEvent,
            Func<EventDefinition<Event>, TResult> fEvent,
            Func<ForbiddenEventDefinition, TResult> fForbiddenEvent)
        {
            switch(eventDefinition)
            {
                case ImmediateEventDefinition immediateEvent:
                    return fImmediateEvent(immediateEvent);
                case EventDefinition<Event> @event:
                    return fEvent(@event);
                case ForbiddenEventDefinition forbiddenEvent:
                    return fForbiddenEvent(forbiddenEvent);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }
    
    public abstract class BaseEventDefinition
    {
    }

    public class EventDefinition<TEvent> : BaseEventDefinition
        where TEvent : BaseEvent
    {
        public TEvent Event { get; set; }
        public IEnumerable<EventTransitionDefinition> Transitions { get; set; }
    }
    public class ImmediateEventDefinition : BaseEventDefinition
    {
        public IEnumerable<ImmediateTransitionDefinition> Transitions { get; set; }
    }
    public class ForbiddenEventDefinition : BaseEventDefinition {
        public Event Event { get; set; }
    }
}
