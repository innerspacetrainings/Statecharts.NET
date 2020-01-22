using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class InitialTransition
    {
        public InitialTransition(ChildTarget target) => Target = target;

        public InitialTransition(ChildTarget target, IEnumerable<OneOf<Action, ContextAction>> actions)
        {
            Target = target;
            Actions = actions;
        }

        public virtual ChildTarget Target { get; }
        public virtual IEnumerable<OneOf<Action, ContextAction>> Actions { get; }
    }

    public abstract class Transition :
        OneOfBase<
            ForbiddenTransition,
            UnguardedTransition,
            UnguardedContextTransition,
            UnguardedContextDataTransition,
            GuardedTransition,
            GuardedContextTransition,
            GuardedContextDataTransition>
    { }

    public sealed class ForbiddenTransition : Transition
    {
        public CustomEvent Event { get; }
        public ForbiddenTransition(string eventName) => Event = new CustomEvent(eventName);
    }
    public abstract class UnguardedTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<Action> Actions { get; }
    }
    public abstract class UnguardedContextTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextAction>> Actions { get; }
    }
    public abstract class UnguardedContextDataTransition : Transition
    {
        public abstract OneOf<Event, CustomDataEvent> Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextAction, ContextDataAction>> Actions { get; }
    }
    public abstract class GuardedTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract InStateGuard Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<Action> Actions { get; }
    }
    public abstract class GuardedContextTransition
    {
        public abstract Event Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextAction>> Actions { get; }
    }
    public abstract class GuardedContextDataTransition
    {
        public abstract OneOf<Event, CustomDataEvent> Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextAction, ContextDataAction>> Actions { get; }
    }
}
