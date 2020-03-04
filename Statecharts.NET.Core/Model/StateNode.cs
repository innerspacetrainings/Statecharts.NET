using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string ToString() => $"{Name} ({GetType().Name.Replace("StatenodeDefinition`1", string.Empty)})";

        internal TResult CataFold<TResult>(
            Func<AtomicStatenodeDefinition, TResult> fAtomic,
            Func<FinalStatenodeDefinition, TResult> fFinal,
            Func<CompoundStatenodeDefinition, IEnumerable<TResult>, TResult> fCompound,
            Func<OrthogonalStatenodeDefinition, IEnumerable<TResult>, TResult> fOrthogonal)
        {
            TResult Recurse(StatenodeDefinition recursedStateNode) =>
                recursedStateNode.CataFold(fAtomic, fFinal, fCompound, fOrthogonal);

            return Match(
                fAtomic,
                fFinal,
                compound => fCompound(compound, compound.Statenodes.Select(Recurse)),
                orthogonal => fOrthogonal(orthogonal, orthogonal.Statenodes.Select(Recurse)));
        }

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
        public abstract IEnumerable<StatenodeDefinition> Statenodes { get; }
        public abstract InitialCompoundTransitionDefinition InitialTransition { get; }
        public abstract Option<DoneTransitionDefinition> DoneTransition { get; } // TODO: think about done data
    }
    public abstract class OrthogonalStatenodeDefinition : NonFinalStatenodeDefinition
    {
        public abstract IEnumerable<StatenodeDefinition> Statenodes { get; }
        public abstract Option<DoneTransitionDefinition> DoneTransition { get; } // TODO: think about done data
    }
    #endregion

    public class Statenode : OneOfBase<AtomicStatenode, FinalStatenode, CompoundStatenode, OrthogonalStatenode>
    {
        public Option<Statenode> Parent { get; }
        public string Name { get; }
        public StatenodeId Id { get; }
        internal int Depth { get; }

        public IEnumerable<Transition> Transitions { get; }
        public Actionblock EntryActions { get; }
        public Actionblock ExitActions { get; }
        public IEnumerable<Service> Services { get; }

        public Statenode(
            Statenode parent,
            string name,
            IEnumerable<Transition> transitions,
            Actionblock entryActions,
            Actionblock exitActions,
            IEnumerable<Service> services)
        {
            Parent = parent.ToOption();
            Name = name;
            Transitions = transitions;
            EntryActions = entryActions;
            ExitActions = exitActions;
            Services = services;

            Id = Parent.Match(
                p => StatenodeId.DeriveFromParent(p, name),
                () => new RootStatenodeId(name));
            Depth = Parent.Match(
                p => p.Depth + 1,
                () => 0);
        }

        internal TResult CataFold<TResult>(
            Func<AtomicStatenode, TResult> fAtomic,
            Func<FinalStatenode, TResult> fFinal,
            Func<CompoundStatenode, IEnumerable<TResult>, TResult> fCompound,
            Func<OrthogonalStatenode, IEnumerable<TResult>, TResult> fOrthogonal)
        {
            TResult Recurse(Statenode recursedStateNode) =>
                recursedStateNode.CataFold(fAtomic, fFinal, fCompound, fOrthogonal);

            return Match(
                fAtomic,
                fFinal,
                compound => fCompound(compound, compound.Statenodes.Select(Recurse)),
                orthogonal => fOrthogonal(orthogonal, orthogonal.Statenodes.Select(Recurse)));
        }
    }

    public class AtomicStatenode : Statenode { }
    public class FinalStatenode : Statenode { }
    public class CompoundStatenode : Statenode { }
    public class OrthogonalStatenode : Statenode { }
}
