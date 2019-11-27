using System;
using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Definition
{
    public static class StateNodeDefinitionFunctions
    {
        public static TResult CataFold<TContext, TResult>(
            this BaseStateNodeDefinition<TContext> stateNodeDefinition,
            Func<AtomicStateNodeDefinition<TContext>, TResult> fAtomic,
            Func<FinalStateNodeDefinition<TContext>, TResult> fFinal,
            Func<CompoundStateNodeDefinition<TContext>, IEnumerable<TResult>, TResult> fCompound,
            Func<OrthogonalStateNodeDefinition<TContext>, IEnumerable<TResult>, TResult> fOrthogonal)
            where TContext : IEquatable<TContext>
        {
            TResult Recurse(BaseStateNodeDefinition<TContext> recursedStateNodeDefinition) =>
                recursedStateNodeDefinition.CataFold(fAtomic, fFinal, fCompound, fOrthogonal);

            switch (stateNodeDefinition)
            {
                case AtomicStateNodeDefinition<TContext> atomic:
                    return fAtomic(atomic);
                case FinalStateNodeDefinition<TContext> final:
                    return fFinal(final);
                case CompoundStateNodeDefinition<TContext> compound:
                    return fCompound(compound, compound.States.Select(Recurse));
                case OrthogonalStateNodeDefinition<TContext> orthogonal:
                    return fOrthogonal(orthogonal, orthogonal.States.Select(Recurse));
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public abstract class BaseStateNodeDefinition<TContext>
    {
        public string Name { get; set; }
        public IEnumerable<BaseEventDefinition> Events { get; set; }
        public IEnumerable<Action> EntryActions { get; set; }
        public IEnumerable<Action> ExitActions { get; set; }
    }

    public class CompoundStateNodeDefinition<TContext> : BaseStateNodeDefinition<TContext>
    {
        public InitialTransitionDefinition InitialTransition { get; set; }
        public IEnumerable<BaseStateNodeDefinition<TContext>> States { get; set; }
    }

    public class OrthogonalStateNodeDefinition<TContext> : BaseStateNodeDefinition<TContext>
    {
        public IEnumerable<BaseStateNodeDefinition<TContext>> States { get; set; }
    }

    public class FinalStateNodeDefinition<TContext> : BaseStateNodeDefinition<TContext> { }

    public class AtomicStateNodeDefinition<TContext> : BaseStateNodeDefinition<TContext> { }
}
