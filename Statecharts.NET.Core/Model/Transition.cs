using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public class InitialCompoundTransitionDefinition
    {
        public InitialCompoundTransitionDefinition(ChildTarget target) => Target = target;

        public InitialCompoundTransitionDefinition(ChildTarget target, IEnumerable<OneOf<Action, ContextActionDefinition>> actions)
        {
            Target = target;
            Actions = actions;
        }

        public virtual ChildTarget Target { get; } // TODO: enable deep child targets
        public virtual IEnumerable<OneOf<Action, ContextActionDefinition>> Actions { get; }
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
        public abstract IEnumerable<Action> Actions { get; }
    }
    public abstract class UnguardedContextTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextActionDefinition>> Actions { get; }
    }
    public abstract class UnguardedContextDataTransitionDefinition : TransitionDefinition
    {
        public abstract IDataEventDefinition Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    public abstract class GuardedTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract InStateGuard Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<Action> Actions { get; }
    }
    public abstract class GuardedContextTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextActionDefinition>> Actions { get; }
    }
    public abstract class GuardedContextDataTransitionDefinition : TransitionDefinition
    {
        public abstract IEventDefinition Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract IEnumerable<OneOf<Action, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    #endregion
}
