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
}
