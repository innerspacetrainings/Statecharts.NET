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
                final => Enumerable.Empty<Transition>(),
                nonFinal => nonFinal.Transitions);
        public static IEnumerable<Service> GetServices(this StateNode stateNode) =>
            stateNode.Match(
                final => Enumerable.Empty<Service>(),
                nonFinal => nonFinal.Services);

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
        public abstract Option<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; } // TODO: think about done data
    }
}
