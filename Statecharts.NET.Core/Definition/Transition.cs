using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class InitialTransition
    {
        public InitialTransition(ChildTarget target) => Target = target;

        public InitialTransition(ChildTarget target, IEnumerable<OneOf<Action, ContextAction>> actions)
        {
            Target = target;
            Actions = actions.ToOption();
        }

        public virtual ChildTarget Target { get; }
        public virtual Option<IEnumerable<OneOf<Action, ContextAction>>> Actions { get; }
    }

    public static class TransitionDefinitionFunctions
    {
        public static IEnumerable<Target> GetTargets(this Transition transition) =>
            transition.Match(
                forbidden => Enumerable.Empty<Target>(),
                unguarded => unguarded.Targets,
                unguarded => unguarded.Targets,
                unguarded => unguarded.Targets,
                guarded => guarded.Targets,
                guarded => guarded.Targets,
                guarded => guarded.Targets);
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
        public NamedEvent Event { get; }
        public ForbiddenTransition(string eventName) => Event = new NamedEvent(eventName);
    }
    public abstract class UnguardedTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract Option<IEnumerable<Action>> Actions { get; }
    }
    public abstract class UnguardedContextTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextAction>>> Actions { get; }
    }
    public abstract class UnguardedContextDataTransition : Transition
    {
        public abstract OneOf<Event, CustomDataEvent> Event { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextAction, ContextDataAction>>> Actions { get; }
    }
    public abstract class GuardedTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract InStateGuard Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract Option<IEnumerable<Action>> Actions { get; }
    }
    public abstract class GuardedContextTransition : Transition
    {
        public abstract Event Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextAction>>> Actions { get; }
    }
    public abstract class GuardedContextDataTransition : Transition
    {
        public abstract OneOf<Event, CustomDataEvent> Event { get; }
        public abstract OneOf<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public abstract IEnumerable<Target> Targets { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextAction, ContextDataAction>>> Actions { get; }
    }
}
