using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public abstract class StatenodeDefinition :
        OneOfBase<AtomicStatenodeDefinition, FinalStatenodeDefinition, CompoundStatenodeDefinition, OrthogonalStatenodeDefinition>
    {
        public abstract string Name { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextActionDefinition>>> EntryActions { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextActionDefinition>>> ExitActions { get; }

        public override string ToString() => $"{Name} ({GetType().Name.Replace("Definition.StateNode`1", string.Empty)})";

        #region Construction Helper Methods
        public static Option<IEnumerable<OneOf<Action, ContextActionDefinition>>> NoActions => Option.None<IEnumerable<OneOf<Action, ContextActionDefinition>>>();
        public static Option<IEnumerable<TransitionDefinition>> NoTransitions => Option.None<IEnumerable<TransitionDefinition>>();
        public static Option<IEnumerable<ServiceDefinition>> NoServices => Option.None<IEnumerable<ServiceDefinition>>();
        public static Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, GuardedTransitionDefinition, GuardedContextTransitionDefinition>> NoDoneTransition =>
            Option.None<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, GuardedTransitionDefinition, GuardedContextTransitionDefinition>>();
        #endregion
    }

    public abstract class FinalStatenodeDefinition : StatenodeDefinition { }
    public abstract class NonFinalStatenodeDefinition : StatenodeDefinition
    {
        public abstract Option<IEnumerable<TransitionDefinition>> Transitions { get; }
        public abstract Option<IEnumerable<ServiceDefinition>> Services { get; }
    }
    public abstract class AtomicStatenodeDefinition : NonFinalStatenodeDefinition
    {
    }
    public abstract class CompoundStatenodeDefinition : NonFinalStatenodeDefinition
    {
        public abstract IEnumerable<StatenodeDefinition> States { get; }
        public abstract InitialCompoundTransitionDefinition InitialTransition { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, GuardedTransitionDefinition, GuardedContextTransitionDefinition>> DoneTransition { get; } // TODO: think about done data
    }
    public abstract class OrthogonalStatenodeDefinition : NonFinalStatenodeDefinition
    {
        public abstract IEnumerable<StatenodeDefinition> States { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, GuardedTransitionDefinition, GuardedContextTransitionDefinition>> DoneTransition { get; } // TODO: think about done data
    }
    #endregion
}
