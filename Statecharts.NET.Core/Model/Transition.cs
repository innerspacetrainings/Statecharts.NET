using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public class InitialCompoundTransitionDefinition
    {
        public InitialCompoundTransitionDefinition(
            ChildTarget target,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions = null)
        {
            Target = target;
            Actions = actions;
        }

        public virtual ChildTarget Target { get; } // TODO: enable deep child targets
        public virtual IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public class DoneTransitionDefinition
    {
        public DoneTransitionDefinition(IEnumerable<Target> targets)
            : this(targets, Option.None<OneOfUnion<Guard, InStateGuard, ConditionContextGuard>>(), null) { }
        public DoneTransitionDefinition(IEnumerable<Target> targets, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard)
            : this(targets, guard.ToOption(), null) { }
        public DoneTransitionDefinition(IEnumerable<Target> targets, IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
            : this(targets, Option.None<OneOfUnion<Guard, InStateGuard, ConditionContextGuard>>(), actions) { }
        public DoneTransitionDefinition(IEnumerable<Target> targets, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
            : this(targets, guard.ToOption(), actions) { }
        private DoneTransitionDefinition(
            IEnumerable<Target> targets,
            Option<OneOfUnion<Guard, InStateGuard, ConditionContextGuard>> guard,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
        {
            Targets = targets;
            Guard = guard;
            Actions = actions;
        }

        public virtual IEnumerable<Target> Targets { get; }
        public virtual Option<OneOfUnion<Guard, InStateGuard, ConditionContextGuard>> Guard { get; }
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
        public ISendableEvent Event { get; }
        public ForbiddenTransitionDefinition(ISendableEvent @event) => Event = @event;
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
        public abstract OneOfUnion<Guard, InStateGuard, ConditionContextGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public abstract class GuardedContextDataTransitionDefinition : TransitionDefinition
    {
        public abstract IDataEventDefinition Event { get; }
        public abstract OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    #endregion
    #region Parsed

    public class Transition
    {
        public IEvent Event { get; }
        public ParsedStatenode Source { get; }
        public IEnumerable<ParsedStatenode> Targets { get; }
        public Actionblock Actions { get; }
        public Option<Guard> Guard { get; }
        public bool IsForbidden { get; }

        internal Transition(IEvent @event, ParsedStatenode source, IEnumerable<ParsedStatenode> targets, Actionblock actions, Option<Guard> guard, bool isForbidden)
        {
            Event = @event;
            Source = source;
            Targets = targets;
            Actions = actions;
            Guard = guard;
            IsForbidden = isForbidden;
        }

        public bool IsEnabled(object context, object eventData) => Guard.Match(
            guard => guard.Match(
                inState => throw new NotImplementedException(),
                conditionContext => conditionContext.Condition(context),
                conditionContextData => conditionContextData.Condition(context, eventData)),
            () => true);

        public override string ToString() => $"{Source}: {Event} {(IsForbidden ? "🚫" : Guard.HasValue ? "❓" : "✔")} to [{string.Join(",", Targets)}] ({Actions.Count()} Actions)";
    }
    #endregion
}
