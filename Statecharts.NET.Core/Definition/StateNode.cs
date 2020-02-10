using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public static class StateNodeDefinitionFunctions
    {
        public static TResult CataFold<TResult>(
            this StateNode stateNode,
            Func<AtomicStateNode, TResult> fAtomic,
            Func<FinalStateNode, TResult> fFinal,
            Func<CompoundStateNode, IEnumerable<TResult>, TResult> fCompound,
            Func<OrthogonalStateNode, IEnumerable<TResult>, TResult> fOrthogonal)
        {
            TResult Recurse(StateNode recursedStateNode) =>
                recursedStateNode.CataFold(fAtomic, fFinal, fCompound, fOrthogonal);

            return stateNode.Match(
                fAtomic,
                fFinal,
                compound => fCompound(compound, compound.States.Select(Recurse)),
                orthogonal => fOrthogonal(orthogonal, orthogonal.States.Select(Recurse)));
        }

        public static IEnumerable<Transition> GetTransitions(this StateNode stateNode) =>
            stateNode.Match(
                final => StateNode.NoTransitions,
                nonFinal => nonFinal.Transitions).ValueOr(Enumerable.Empty<Transition>());
        public static IEnumerable<Service> GetServices(this StateNode stateNode) =>
            stateNode.Match(
                final => StateNode.NoServices,
                nonFinal => nonFinal.Services).ValueOr(Enumerable.Empty<Service>());

        public static TResult Match<TResult>(this StateNode stateNode, Func<FinalStateNode, TResult> final, Func<NonFinalStateNode, TResult> nonFinal) =>
            stateNode.Match(nonFinal, final, nonFinal, nonFinal);
    }

    public abstract class StateNode :
        OneOfBase<AtomicStateNode, FinalStateNode, CompoundStateNode, OrthogonalStateNode>
    {
        public abstract string Name { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextAction>>> EntryActions { get; }
        public abstract Option<IEnumerable<OneOf<Action, ContextAction>>> ExitActions { get; }

        public override string ToString() => $"{Name} ({GetType().Name.Replace("Definition.StateNode`1", string.Empty)})";

        public static Option<IEnumerable<OneOf<Action, ContextAction>>> NoActions => Option.None<IEnumerable<OneOf<Action, ContextAction>>>();
        public static Option<IEnumerable<Transition>> NoTransitions => Option.None<IEnumerable<Transition>>();
        public static Option<IEnumerable<Service>> NoServices => Option.None<IEnumerable<Service>>();
        public static Option<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> NoDoneTransition => Option.None<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>>();
    }

    public abstract class FinalStateNode : StateNode {}
    public abstract class NonFinalStateNode : StateNode
    {
        public abstract Option<IEnumerable<Transition>> Transitions { get; }
        public abstract Option<IEnumerable<Service>> Services { get; }
    }
    public abstract class AtomicStateNode : NonFinalStateNode
    {
    }
    public abstract class CompoundStateNode : NonFinalStateNode
    {
        public abstract IEnumerable<StateNode> States { get; }
        public abstract InitialTransition InitialTransition { get; }
        public abstract Option<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; } // TODO: think about done data
    }
    public abstract class OrthogonalStateNode : NonFinalStateNode
    {
        public abstract IEnumerable<StateNode> States { get; }
        public abstract Option<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; } // TODO: think about done data
    }
}
