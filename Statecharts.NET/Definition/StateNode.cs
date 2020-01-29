using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
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
    }

    public abstract class StateNode :
        OneOfBase<AtomicStateNode, FinalStateNode, CompoundStateNode, OrthogonalStateNode>
    {
        public abstract string Name { get; }
        public abstract IEnumerable<OneOf<Model.Action, ContextAction>> EntryActions { get; }
        public abstract IEnumerable<OneOf<Model.Action, ContextAction>> ExitActions { get; }

        public TResult Match<TResult>(Func<FinalStateNode, TResult> final, Func<NonFinalStateNode, TResult> nonFinal) =>
            this.Match(nonFinal, final, nonFinal, nonFinal);
    }
    public abstract class FinalStateNode : StateNode {}
    public abstract class NonFinalStateNode : StateNode
    {
        public abstract IEnumerable<Transition> Transitions { get; }
    }
    public abstract class AtomicStateNode : NonFinalStateNode
    {
        public abstract IEnumerable<Service> Services { get; }
    }
    public abstract class CompoundStateNode : NonFinalStateNode
    {
        public abstract IEnumerable<Service> Services { get; }
        public abstract IEnumerable<StateNode> States { get; }
        public abstract InitialTransition InitialTransition { get; }
        public abstract Option<OneOf<UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; } // TODO: think about done data
    }
    public abstract class OrthogonalStateNode : NonFinalStateNode
    {
        public abstract IEnumerable<Service> Services { get; }
        public abstract IEnumerable<StateNode> States { get; }
        public abstract Option<OneOf<UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; } // TODO: think about done data
    }
}
