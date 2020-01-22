using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;

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
    }

    public abstract class StateNode :
        OneOfBase<AtomicStateNode, FinalStateNode, CompoundStateNode, OrthogonalStateNode>
    {
        public abstract string Name { get; }
        public abstract IEnumerable<Transition> Transitions { get; }
        public abstract IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; }
        public abstract IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; }
        public abstract IEnumerable<Activity> Activities { get; }
    }

    public abstract class AtomicStateNode : StateNode
    {
        public abstract IEnumerable<Service> Services { get; }
    }
    public abstract class FinalStateNode : StateNode {}
    public abstract class CompoundStateNode : StateNode
    {
        public abstract IEnumerable<Service> Services { get; }
        public abstract IEnumerable<StateNode> States { get; }
        public abstract InitialTransition InitialTransition { get; }
    }

    public abstract class OrthogonalStateNode : StateNode
    {
        public abstract IEnumerable<Service> Services { get; }
        public abstract IEnumerable<StateNode> States { get; }
    }
}
