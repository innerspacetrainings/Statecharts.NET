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
    }

    public class StateNode :
        OneOfBase<AtomicStateNode, FinalStateNode, CompoundStateNode, OrthogonalStateNode>
    {
        public string Name { get; }
        public IEnumerable<Transition> Transitions { get; }
        public IEnumerable<Action> EntryActions { get; }
        public IEnumerable<Action> ExitActions { get; }
        public IEnumerable<Activity> Activities { get; }
    }

    public class AtomicStateNode : StateNode
    {
        public IEnumerable<Service> Services { get; }
    }
    public class FinalStateNode : StateNode {}
    public class CompoundStateNode : StateNode
    {
        public IEnumerable<Service> Services { get; }
        public IEnumerable<StateNode> States { get; }
        public InitialTransition InitialTransition { get; }
    }

    public class OrthogonalStateNode : StateNode
    {
        public IEnumerable<Service> Services { get; }
        public IEnumerable<StateNode> States { get; }
    }
}
