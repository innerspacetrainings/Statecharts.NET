using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public interface IFinalStatenodeDefinition { }
    public interface IAtomicStatenodeDefinition { }
    public interface ICompoundStatenodeDefinition { }
    public interface IOrthogonalStatenodeDefinition { }

    public abstract class StatenodeDefinition :
        OneOfBase<AtomicStatenodeDefinition, FinalStatenodeDefinition, CompoundStatenodeDefinition, OrthogonalStatenodeDefinition>
    {
        public abstract string Name { get; }
        public abstract Option<string> UniqueIdentifier { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; }
        public abstract IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; }

        public override string ToString() => $"{Name} ({GetType().Name.Replace("StatenodeDefinition`1", string.Empty)})";

        public TResult CataFold<TResult>(
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
        public static Option<IEnumerable<OneOf<ExecutableAction, ContextActionDefinition>>> NoActions => Option.None<IEnumerable<OneOf<ExecutableAction, ContextActionDefinition>>>();
        public static Option<IEnumerable<TransitionDefinition>> NoTransitions => Option.None<IEnumerable<TransitionDefinition>>();
        public static Option<IEnumerable<ServiceDefinition>> NoServices => Option.None<IEnumerable<ServiceDefinition>>();
        public static Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, GuardedTransitionDefinition, GuardedContextTransitionDefinition>> NoDoneTransition =>
            Option.None<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, GuardedTransitionDefinition, GuardedContextTransitionDefinition>>();
        #endregion
    }

    public abstract class FinalStatenodeDefinition : StatenodeDefinition, IFinalStatenodeDefinition { }
    public abstract class NonFinalStatenodeDefinition : StatenodeDefinition
    {
        public abstract IEnumerable<TransitionDefinition> Transitions { get; }
        public abstract IEnumerable<ServiceDefinition> Services { get; }
    }
    public abstract class AtomicStatenodeDefinition : NonFinalStatenodeDefinition, IAtomicStatenodeDefinition { }
    public abstract class CompoundStatenodeDefinition : NonFinalStatenodeDefinition, ICompoundStatenodeDefinition
    {
        public abstract IEnumerable<StatenodeDefinition> Statenodes { get; }
        public abstract InitialCompoundTransitionDefinition InitialTransition { get; }
        public abstract Option<DoneTransitionDefinition> DoneTransition { get; } // TODO: think about done data
    }
    public abstract class OrthogonalStatenodeDefinition : NonFinalStatenodeDefinition, IOrthogonalStatenodeDefinition
    {
        public abstract IEnumerable<StatenodeDefinition> Statenodes { get; }
        public abstract Option<DoneTransitionDefinition> DoneTransition { get; } // TODO: think about done data
    }
    #endregion

    public abstract class Statenode : OneOfBase<AtomicStatenode, FinalStatenode, CompoundStatenode, OrthogonalStatenode>
    {
        public Option<Statenode> Parent { get; }
        public string Name { get; }
        public Option<string> UniqueIdentifier { get; }
        public int DocumentIndex { get; }
        public Actionblock EntryActions { get; private set; }
        public Actionblock ExitActions { get; }

        public StatenodeId Id { get; }
        internal int Depth { get; }

        protected Statenode(
            Statenode parent,
            string name,
            Option<string> uniqueIdentifier,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions)
        {
            Parent = parent.ToOption();
            Name = name;
            UniqueIdentifier = uniqueIdentifier;
            DocumentIndex = documentIndex;
            EntryActions = entryActions;
            ExitActions = exitActions;

            Id = new StatenodeId(Parent, name);
            Depth = Parent.Map(p => p.Depth + 1).ValueOr(0);
        }

        public override bool Equals(object other) => other is Statenode statenode && statenode.Id.Equals(Id);
        public override int GetHashCode() => Id.GetHashCode() ^ 217;

        internal void AddDelayedTransitionAction(IEnumerable<StartDelayedTransitionAction> actions) =>
            EntryActions = Actionblock.From(EntryActions.Concat(actions));

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

        public override string ToString() => $"{Id} ({Match(_ => "A", _ => "F", _ => "C", _ => "O")})";
    }

    public class FinalStatenode : Statenode
    {
        public FinalStatenode(
            Statenode parent,
            string name,
            Option<string> uniqueIdentifier,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions)
        {
        }
    }

    public abstract class NonFinalStatenode : Statenode
    {
        public IEnumerable<Transition> Transitions { get; internal set; }
        public IEnumerable<Service> Services { get; internal set; }

        protected NonFinalStatenode(
            Statenode parent,
            string name,
            Option<string> uniqueIdentifier,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions) { }
    }

    public class AtomicStatenode : NonFinalStatenode
    {
        public AtomicStatenode(
            Statenode parent,
            string name,
            Option<string> uniqueIdentifier,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions) { }
    }

    public class CompoundStatenode : NonFinalStatenode
    {
        public IEnumerable<Statenode> Statenodes { get; internal set; }

        public CompoundStatenode(
            Statenode parent,
            string name,
            Option<string> uniqueIdentifier,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions) { }
    }
    public class OrthogonalStatenode : NonFinalStatenode
    {
        public IEnumerable<Statenode> Statenodes { get; internal set; }

        public OrthogonalStatenode(
            Statenode parent,
            string name,
            Option<string> uniqueIdentifier,
            int documentIndex,
            Actionblock entryActions,
            Actionblock exitActions) : base(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions) { }
    }
}
