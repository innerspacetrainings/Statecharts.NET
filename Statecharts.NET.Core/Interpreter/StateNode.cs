using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    static class StateNodeFunctions
    {
        internal static TResult CataFold<TResult>(
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
                compound => fCompound(compound, compound.StateNodes.Select(Recurse)),
                orthogonal => fOrthogonal(orthogonal, orthogonal.StateNodes.Select(Recurse)));
        }

        internal static IEnumerable<StateNode> GetParents(
            this StateNode stateNode)
        => stateNode.HasParent
                ? stateNode.Parent.Append(stateNode.Parent.GetParents())
                : Enumerable.Empty<StateNode>();

        internal static StateNode LeastCommonAncestor(
            this (StateNode first, StateNode second) pair)
            => Enumerable.Intersect(
                pair.first.GetParents(),
                pair.second.GetParents()).FirstOrDefault();

        internal static StateNode OneBeneath(
            this StateNode stateNode, StateNode beneath)
            => stateNode.Append(stateNode.GetParents())
                .FirstOrDefault(parentStateNode => parentStateNode.Parent == beneath);

        internal static IEnumerable<StateNode> AncestorsUntil(
            this StateNode stateNode, StateNode until)
            => stateNode.GetParents().TakeWhile(parentStateNode => parentStateNode != until);

        internal static IEnumerable<StateNode> GetDescendants(
            this StateNode stateNode)
            => stateNode.CataFold(
                atomic => atomic.Yield() as IEnumerable<StateNode>,
                final => final.Yield() as IEnumerable<StateNode>,
                (compound, subStates) =>
                    compound.Append(subStates.SelectMany(a => a)),
                (orthogonal, subStates) =>
                    orthogonal.Append(subStates.SelectMany(a => a))).Except(stateNode.Yield());

        public static IEnumerable<StateNode> GetUnstableStateNodes(
            this IEnumerable<StateNode> stateNodes)
        {
            var stateNodesList = stateNodes.ToList();
            return stateNodesList
                .Except(stateNodesList.SelectMany(stateNode => stateNode.GetParents()).Distinct())
                .Where(stateNode => !(stateNode is AtomicStateNode));
        }

        public static IEnumerable<StateNodeId> Ids(
            this IEnumerable<StateNode> stateNodes)
            => stateNodes.Select(stateNode => stateNode.Id);

        internal static StateNode ResolveTarget(
            this StateNode sourceStateNode,
            OneOf<Model.SiblingTarget, Model.ChildTarget> target)
        {
            StateNode GetStateNode(StateNode stateNode, NamedStateNodeKey key)
                => stateNode.Match(
                    _ => throw new Exception("INVALID TARGET CONFIGURATION"),
                    _ => throw new Exception("INVALID TARGET CONFIGURATION"),
                    compound => compound.GetSubstate(key),
                    orthogonal => orthogonal.GetSubstate(key));

            return target.Match(
                sibling => GetStateNode(sourceStateNode.Parent, sibling.Key),
                child => GetStateNode(sourceStateNode, child.Key));
        }
    }
    
    public abstract class StateNode : OneOfBase<AtomicStateNode, FinalStateNode, CompoundStateNode, OrthogonalStateNode>
    {
        public StateNodeId Id { get; }
        public StateNodeKey Key { get; }
        internal int Depth { get; }
        public IEnumerable<Definition.Transition> Transitions { get; protected set; }
        public IEnumerable<Service> Services { get; }
        public StateNode Parent { get; }
        public IEnumerable<Model.Action> EntryActions { get; }
        public IEnumerable<Model.Action> ExitActions { get; }
        public bool HasParent => Parent != null;
        public string Name => Key.Map(_ => null, named => named.StateName);

        protected StateNode(StateNode parent, Definition.StateNode definition)
        {
            if (definition is null) throw new ArgumentNullException(nameof(definition));
            Parent = parent;
            Key = Parent == null ? new RootStateNodeKey(definition.Name) as StateNodeKey : new NamedStateNodeKey(definition.Name);
            Id = Parent == null ? new StateNodeId(new RootStateNodeKey(definition.Name)) : new StateNodeId(Parent.Id, Key);
            Depth = Parent?.Depth + 1 ?? 0;

            Transitions = definition.GetTransitions();
            definition.GetServices().SelectMany(service => service.Match<object>(
                activity => activity.OnErrorTransition.Yield(),
                task => task.OnSuccessDefinition.Append(task.OnErrorTransition),
                dataTask => dataTask.OnSuccessDefinition.Append(dataTask.OnErrorTransition)));
            Services = definition.GetServices().Select(Service.FromDefinition);

            EntryActions = definition.EntryActions.Map(actions => actions.ToModelActions()).ValueOr(Enumerable.Empty<Model.Action>());
            ExitActions = definition.ExitActions.Map(actions => actions.ToModelActions()).ValueOr(Enumerable.Empty<Model.Action>());
        }

        public override string ToString() => $"{Id} ({GetType().Name.Replace("Interpreter.StateNode`1", string.Empty).Replace("StateNode", string.Empty).ToLowerInvariant()})";
        public override int GetHashCode() => Key.GetHashCode();
        public override bool Equals(object obj) => obj is StateNode stateNode && Key.Equals(stateNode.Key);

        internal static StateNodeId MakeId(StateNode stateNode, NamedStateNodeKey key)
            => new StateNodeId(stateNode.Id, key);
    }
    public class FinalStateNode : StateNode
    {
        public FinalStateNode(StateNode parent, Definition.FinalStateNode definition) : base(parent, definition) { }
    }
    public abstract class NonFinalStateNode : StateNode
    {
        protected NonFinalStateNode(StateNode parent, Definition.NonFinalStateNode definition) : base(parent, definition) { }
    }
    public class AtomicStateNode : NonFinalStateNode
    {
        public AtomicStateNode(StateNode parent, Definition.AtomicStateNode definition) : base(parent, definition) { }
    }
    public class CompoundStateNode : NonFinalStateNode
    {
        public InitialTransition InitialTransition { get; internal set; }
        public IEnumerable<StateNode> StateNodes { get; internal set; }

        public CompoundStateNode(StateNode parent, Definition.CompoundStateNode definition) : base(parent, definition) =>
            Transitions = definition.DoneTransition.Match(
                doneTransition => Transitions.Prepend(doneTransition.AsBase()),
                () => Transitions);

        public StateNode GetSubstate(NamedStateNodeKey key)
            => StateNodes.FirstOrDefault(state => state.Key.Equals(key)) ?? throw new Exception("[THINK] WTF is happening");
    }
    public class OrthogonalStateNode : NonFinalStateNode
    {
        public IEnumerable<StateNode> StateNodes { get; internal set; }

        public OrthogonalStateNode(StateNode parent, Definition.OrthogonalStateNode definition) : base(parent, definition) { }

        public StateNode GetSubstate(NamedStateNodeKey key)
            => StateNodes.FirstOrDefault(state => state.Key.Equals(key)) ?? throw new Exception("[THINK] WTF is happening");
    }
}
