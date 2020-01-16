using System;
using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Definition
{
    public static class StateNodeDefinitionFunctions
    {
        public static TResult CataFold<TResult>(
            this IBaseStateNodeDefinition stateNodeDefinition,
            Func<IAtomicStateNodeDefinition, TResult> fAtomic,
            Func<IFinalStateNodeDefinition, TResult> fFinal,
            Func<ICompoundStateNodeDefinition, IEnumerable<TResult>, TResult> fCompound,
            Func<IOrthogonalStateNodeDefinition, IEnumerable<TResult>, TResult> fOrthogonal)
        {
            TResult Recurse(IBaseStateNodeDefinition recursedStateNodeDefinition) =>
                recursedStateNodeDefinition.CataFold(fAtomic, fFinal, fCompound, fOrthogonal);

            switch (stateNodeDefinition)
            {
                case IAtomicStateNodeDefinition atomic:
                    return fAtomic(atomic);
                case IFinalStateNodeDefinition final:
                    return fFinal(final);
                case ICompoundStateNodeDefinition compound:
                    return fCompound(compound, compound.States.Select(Recurse));
                case IOrthogonalStateNodeDefinition orthogonal:
                    return fOrthogonal(orthogonal, orthogonal.States.Select(Recurse));
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public interface IBaseStateNodeDefinition
    {
        string Name { get; }
        IEnumerable<IEventDefinition> Events { get; }
        IEnumerable<Action> EntryActions { get; }
        IEnumerable<Action> ExitActions { get; }
        IEnumerable<IActivity> Activities { get; }
    }

    public interface INonFinalStateNodeDefinition : IBaseStateNodeDefinition
    {
        IEnumerable<IBaseServiceDefinition> Services { get; }
    }
    public interface IAtomicStateNodeDefinition : INonFinalStateNodeDefinition {}
    public interface INonAtomicStateNodeDefinition : INonFinalStateNodeDefinition
    {
        IEnumerable<IBaseStateNodeDefinition> States { get; }
    }
    public interface ICompoundStateNodeDefinition : INonAtomicStateNodeDefinition
    {
        InitialTransitionDefinition InitialTransition { get; }
    }
    public interface IOrthogonalStateNodeDefinition : INonAtomicStateNodeDefinition {}
    public interface IFinalStateNodeDefinition : IBaseStateNodeDefinition {}
}
