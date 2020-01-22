using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class InitialTransition
    {
        public ChildTarget Target { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> Actions { get; set; }
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
        public virtual Event Event { get; }
        public virtual IEnumerable<Target> Targets { get; }
        public virtual IEnumerable<Action> Actions { get; }
    }
    public class UnguardedContextTransition : Transition
    {
        public Event Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> Actions { get; set; }
    }
    public class UnguardedContextDataTransition : Transition
    {
        public OneOf<Event, CustomDataEvent> Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<OneOf<Action, ContextAction, ContextDataAction>> Actions { get; set; }
    }
    public class GuardedTransition : Transition
    {
        public virtual Event Event { get; }
        public virtual InStateGuard Guard { get; }
        public virtual IEnumerable<Target> Targets { get; }
        public virtual IEnumerable<Action> Actions { get; }
    }
    public class GuardedContextTransition
    {
        public virtual Event Event { get; }
        public virtual OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public virtual IEnumerable<Target> Targets { get; }
        public virtual IEnumerable<OneOf<Action, ContextAction>> Actions { get; }
    }
    public class GuardedContextDataTransition
    {
        public OneOf<Event, CustomDataEvent> Event { get; }
        public OneOf<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public IEnumerable<Target> Targets { get; }
        public IEnumerable<OneOf<Action, ContextAction, ContextDataAction>> Actions { get; }
    }
}
