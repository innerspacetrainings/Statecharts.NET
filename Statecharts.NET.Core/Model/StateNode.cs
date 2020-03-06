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
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; }

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
        public abstract IEnumerable<TransitionDefinition> Transitions { get; }
        public abstract IEnumerable<ServiceDefinition> Services { get; }
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

    public abstract class Statenode : OneOfBase<AtomicStatenode, FinalStatenode, CompoundStatenode, OrthogonalStatenode>
    {
        public Option<Statenode> Parent { get; }
        public string Name { get; }
        public int DocumentIndex { get; }
        public Actionblock EntryActions { get; }
        public Actionblock ExitActions { get; }

        public StatenodeId Id { get; }
        internal int Depth { get; }


        public Statenode(
            Statenode parent,
            string name,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions)
        {
            Parent = parent.ToOption();
            Name = name;
            DocumentIndex = documentIndex;
            EntryActions = entryActions;
            ExitActions = exitActions;

            Id = Parent.Match(
                p => StatenodeId.DeriveFromParent(p, name),
                () => new RootStatenodeId(name));
            Depth = Parent.Match(
                p => p.Depth + 1,
                () => 0);
        }

        internal TResult Match<TResult>(Func<FinalStatenode, TResult> fFinalStatenode,
            Func<NonFinalStatenode, TResult> fNonFinalStatenode) =>
            Match(fNonFinalStatenode, fFinalStatenode, fNonFinalStatenode, fNonFinalStatenode);

        internal void Switch(Action<FinalStatenode> fFinalStatenode,
            Action<NonFinalStatenode> fNonFinalStatenode) =>
            Switch(fNonFinalStatenode, fFinalStatenode, fNonFinalStatenode, fNonFinalStatenode);

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

    public class FinalStatenode : Statenode
    {
        public FinalStatenode(Statenode parent, string name, int documentIndex, Actionblock entryActions, Actionblock exitActions) : base(parent, name, documentIndex, entryActions, exitActions)
        {
        }
    }

    public abstract class NonFinalStatenode : Statenode
    {
        public IEnumerable<Transition> Transitions { get; internal set; }

        protected NonFinalStatenode(
            Statenode parent,
            string name,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, documentIndex, entryActions, exitActions) { }
    }

    public class AtomicStatenode : NonFinalStatenode
    {
        public AtomicStatenode(
            Statenode parent,
            string name,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, documentIndex, entryActions, exitActions) { }
    }

    public class CompoundStatenode : NonFinalStatenode
    {
        public IEnumerable<Statenode> Statenodes { get; internal set; }

        public CompoundStatenode(Statenode parent, string name, int documentIndex, Actionblock entryActions, Actionblock exitActions) : base(parent, name, documentIndex, entryActions, exitActions) { }
    }
    public class OrthogonalStatenode : NonFinalStatenode
    {
        public IEnumerable<Statenode> Statenodes { get; internal set; }

        public OrthogonalStatenode(Statenode parent, string name, int documentIndex, Actionblock entryActions, Actionblock exitActions) : base(parent, name, documentIndex, entryActions, exitActions) { }
    }
}
