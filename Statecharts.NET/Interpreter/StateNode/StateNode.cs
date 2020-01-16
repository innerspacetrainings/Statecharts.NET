using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Extensions;

namespace Statecharts.NET.Interpreter
{
    static class StateNode
    {
        internal static StateNodeId MakeId<TContext>(
            BaseStateNode<TContext> stateNode, NamedStateNodeKey key)
            where TContext : IEquatable<TContext>
            => new StateNodeId(stateNode.Id, key);
    }

    static class StateNodeFunctions
    {
        internal static TResult Map<TContext, TResult>(
            this BaseStateNode<TContext> stateNode,
            Func<AtomicStateNode<TContext>, TResult> fAtomic,
            Func<FinalStateNode<TContext>, TResult> fFinal,
            Func<CompoundStateNode<TContext>, TResult> fCompound,
            Func<OrthogonalStateNode<TContext>, TResult> fOrthogonal)
            where TContext : IEquatable<TContext>
        {
            switch (stateNode)
            {
                case AtomicStateNode<TContext> atomic:
                    return fAtomic(atomic);
                case FinalStateNode<TContext> final:
                    return fFinal(final);
                case CompoundStateNode<TContext> compound:
                    return fCompound(compound);
                case OrthogonalStateNode<TContext> orthogonal:
                    return fOrthogonal(orthogonal);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        internal static TResult CataFold<TContext, TResult>(
            this BaseStateNode<TContext> stateNode,
            Func<AtomicStateNode<TContext>, TResult> fAtomic,
            Func<FinalStateNode<TContext>, TResult> fFinal,
            Func<CompoundStateNode<TContext>, IEnumerable<TResult>, TResult> fCompound,
            Func<OrthogonalStateNode<TContext>, IEnumerable<TResult>, TResult> fOrthogonal)
            where TContext : IEquatable<TContext>
        {
            TResult Recurse(BaseStateNode<TContext> recursedStateNode) =>
                recursedStateNode.CataFold(fAtomic, fFinal, fCompound, fOrthogonal);

            switch (stateNode)
            {
                case AtomicStateNode<TContext> atomic:
                    return fAtomic(atomic);
                case FinalStateNode<TContext> final:
                    return fFinal(final);
                case CompoundStateNode<TContext> compound:
                    return fCompound(compound, compound.StateNodes.Select(Recurse));
                case OrthogonalStateNode<TContext> orthogonal:
                    return fOrthogonal(orthogonal, orthogonal.StateNodes.Select(Recurse));
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        internal static IEnumerable<BaseStateNode<TContext>> GetParents<TContext>(
            this BaseStateNode<TContext> stateNode)
            where TContext : IEquatable<TContext>
        => stateNode.HasParent
                ? stateNode.Parent.Append(stateNode.Parent.GetParents())
                : Enumerable.Empty<BaseStateNode<TContext>>();

        internal static BaseStateNode<TContext> LeastCommonAncestor<TContext>(
            this (BaseStateNode<TContext> first, BaseStateNode<TContext> second) pair)
            where TContext : IEquatable<TContext>
            => Enumerable.Intersect(
                pair.first.GetParents(),
                pair.second.GetParents()).FirstOrDefault();

        internal static BaseStateNode<TContext> OneBeneath<TContext>(
            this BaseStateNode<TContext> stateNode, BaseStateNode<TContext> beneath)
            where TContext : IEquatable<TContext>
            => stateNode.Append(stateNode.GetParents())
                .FirstOrDefault(parentStateNode => parentStateNode.Parent == beneath);

        internal static IEnumerable<BaseStateNode<TContext>> AncestorsUntil<TContext>(
            this BaseStateNode<TContext> stateNode, BaseStateNode<TContext> until)
            where TContext : IEquatable<TContext>
            => stateNode.GetParents().TakeWhile(parentStateNode => parentStateNode != until);

        internal static IEnumerable<BaseStateNode<TContext>> GetDescendants<TContext>(
            this BaseStateNode<TContext> stateNode)
            where TContext : IEquatable<TContext>
            => stateNode.CataFold(
                atomic => atomic.Yield() as IEnumerable<BaseStateNode<TContext>>,
                final => final.Yield() as IEnumerable<BaseStateNode<TContext>>,
                (compound, subStates) =>
                    compound.Append(subStates.SelectMany(a => a)),
                (orthogonal, subStates) =>
                    orthogonal.Append(subStates.SelectMany(a => a))).Except(stateNode.Yield());

        public static IEnumerable<BaseStateNode<TContext>> GetUnstableStateNodes<TContext>(
            this IEnumerable<BaseStateNode<TContext>> stateNodes)
            where TContext : IEquatable<TContext>
        {
            var stateNodesList = stateNodes.ToList();
            return stateNodesList
                .Except(stateNodesList.SelectMany(stateNode => stateNode.GetParents()).Distinct())
                .Where(stateNode => !(stateNode is AtomicStateNode<TContext>));
        }

        public static IEnumerable<StateNodeId> Ids<TContext>(
            this IEnumerable<BaseStateNode<TContext>> stateNodes)
            where TContext : IEquatable<TContext>
            => stateNodes.Select(stateNode => stateNode.Id);

        internal static BaseStateNode<TContext> ResolveTarget<TContext>(
            this BaseStateNode<TContext> sourceStateNode,
            RelativeTargetDefinition targetDefinition)
            where TContext : IEquatable<TContext>
        {
            BaseStateNode<TContext>
                GetStateNode(BaseStateNode<TContext> stateNode, NamedStateNodeKey key)
                => stateNode.Map(
                    _ => throw new Exception("INVALID TARGET CONFIGURATION"),
                    _ => throw new Exception("INVALID TARGET CONFIGURATION"),
                    compound => compound.GetSubstate(key),
                    orthogonal => orthogonal.GetSubstate(key));

            return targetDefinition.Map(
                sibling => GetStateNode(sourceStateNode.Parent, targetDefinition.Key),
                child => GetStateNode(sourceStateNode, targetDefinition.Key));
        }
    }

    public abstract class BaseStateNode<TContext>
        where TContext : IEquatable<TContext>
    {
        public StateNodeId Id { get; }
        public StateNodeKey Key { get; }
        internal int Depth { get; }
        public IEnumerable<BaseEventDefinition> EventDefinitions { get; protected set; }
        public BaseStateNode<TContext> Parent { get; }
        public IEnumerable<Action> EntryActions { get; }
        public IEnumerable<Action> ExitActions { get; }
        public bool HasParent => Parent != null;
        public string Name => Key.Map(_ => null, named => named.StateName);

        protected BaseStateNode(BaseStateNode<TContext> parent, IBaseStateNodeDefinition definition)
        {
            if (definition is null) throw new ArgumentNullException(nameof(definition));
            Parent = parent;
            Key = Parent == null ? new RootStateNodeKey(definition.Name) as StateNodeKey : new NamedStateNodeKey(definition.Name);
            Id = Parent == null ? new StateNodeId(new RootStateNodeKey(definition.Name)) : new StateNodeId(Parent.Id, Key);
            Depth = Parent?.Depth + 1 ?? 0;

            EventDefinitions = definition.Events ?? Enumerable.Empty<BaseEventDefinition>();
            EntryActions = definition.EntryActions ?? Enumerable.Empty<Action>();
            ExitActions = definition.ExitActions ?? Enumerable.Empty<Action>();
        }

        public override string ToString() => $"{Id} ({GetType().Name.Replace("StateNode`1", string.Empty)})";
    }
}
