using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public class InitialCompoundTransitionDefinition
    {
        public InitialCompoundTransitionDefinition(
            ChildTarget target,
            IEnumerable<OneOf<Action, ContextActionDefinition>> actions = null)
        {
            Target = target;
            Actions = actions;
        }

        public virtual ChildTarget Target { get; } // TODO: enable deep child targets
        public virtual IEnumerable<OneOf<Action, ContextActionDefinition>> Actions { get; }
    }
    public class DoneTransitionDefinition
    {
        public DoneTransitionDefinition(IEnumerable<Target> targets)
            : this(targets, Option.None<OneOf<InStateGuard, ConditionContextGuard>>(), null) { }
        public DoneTransitionDefinition(IEnumerable<Target> targets, OneOf<InStateGuard, ConditionContextGuard> guard)
            : this(targets, guard.ToOption(), null) { }
        public DoneTransitionDefinition(IEnumerable<Target> targets, IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
            : this(targets, Option.None<OneOf<InStateGuard, ConditionContextGuard>>(), actions) { }
        public DoneTransitionDefinition(IEnumerable<Target> targets, OneOf<InStateGuard, ConditionContextGuard> guard, IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
            : this(targets, guard.ToOption(), actions) { }
        private DoneTransitionDefinition(
            IEnumerable<Target> targets,
            Option<OneOf<InStateGuard, ConditionContextGuard>> guard,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
        {
            Targets = targets;
            Guard = guard;
            Actions = actions;
        }

        public virtual IEnumerable<Target> Targets { get; }
        public virtual Option<OneOf<InStateGuard, ConditionContextGuard>> Guard { get; }
        public virtual IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }

    public abstract class TransitionDefinition :
        OneOfBase<
            ForbiddenTransitionDefinition,
            UnguardedTransitionDefinition,
            UnguardedContextTransitionDefinition,
            UnguardedContextDataTransitionDefinition,
            GuardedTransitionDefinition,
            GuardedContextTransitionDefinition,
            GuardedContextDataTransitionDefinition>
    { }

    public sealed class ForbiddenTransitionDefinition : TransitionDefinition
    {
        public NamedEventDefinition Event { get; }
        public ForbiddenTransitionDefinition(string eventName) => Event = new NamedEventDefinition(eventName);
    }
    public abstract class UnguardedTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<ActionDefinition> Actions { get; }
    }
    public abstract class UnguardedContextTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public abstract class UnguardedContextDataTransitionDefinition : TransitionDefinition
    {
        public abstract IDataEventDefinition Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    public abstract class GuardedTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract InStateGuard Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<ActionDefinition> Actions { get; }
    }
    public abstract class GuardedContextTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public abstract class GuardedContextDataTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    #endregion
    #region Parsed

    public class Transition
    {
        public IEvent Event { get; }
        public Statenode Source { get; }
        public IEnumerable<Statenode> Targets { get; }

        public bool IsEnabled(object context, object eventData) => throw new NotImplementedException();
    }
    #endregion
}
