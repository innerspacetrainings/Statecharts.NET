using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class InitialTransition
    {
        public ChildTarget Target { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }

    public class Transition :
        OneOfBase<
            ForbiddenTransition,
            UnguardedTransition,
            UnguardedContextTransition,
            UnguardedContextDataTransition,
            GuardedTransition,
            GuardedContextTransition,
            GuardedContextDataTransition>
    { }

    public class ForbiddenTransition : Transition
    {
        public CustomEvent Event { get; }
        public ForbiddenTransition(string eventName) => Event = new CustomEvent(eventName);
    }
    public class UnguardedTransition : Transition
    {
        public Event Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
    public class UnguardedContextTransition : Transition
    {
        public Event Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> Actions { get; set; }
    }
    public class UnguardedContextDataTransition : Transition
    {
        public OneOf<Event, CustomDataEvent> Event { get; set; } // TODO: DataEvent
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<OneOf<Action, ContextAction, ContextDataAction>> Actions { get; set; }
    }
    public class GuardedTransition : Transition
    {
        public Event Event { get; set; }
        public Guard Guard { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
    public class GuardedContextTransition
    {
        public Event Event { get; set; }
        public OneOf<InStateGuard, ConditionContextGuard> Guard { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> Actions { get; set; }
    }
    public class GuardedContextDataTransition
    {
        public OneOf<Event, CustomDataEvent> Event { get; set; } // TODO: DataEvent
        public OneOf<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<OneOf<Action, ContextAction, ContextDataAction>> Actions { get; set; }
    }
}
