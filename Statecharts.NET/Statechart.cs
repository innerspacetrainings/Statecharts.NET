using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Extensions;
using Statecharts.NET.Interpreter;

namespace Statecharts.NET
{
    public static class StateChartExtensions
    {
        public static Service<TContext> Interpret<TContext>(
            this ExecutableStatechart<TContext> statechart)
            where TContext : IEquatable<TContext>
            => new Service<TContext> {StateChart = statechart};
    }

    public static class ParsedStatechartFunctions
    {
        public static ParsedStatechart<TContext> Parse<TContext>(this StatechartDefinition<TContext> stateChartDefinition)
            where TContext : IEquatable<TContext>
            => new ExecutableStatechart<TContext>(
                stateChartDefinition.StateNodeDefinition.Parse(null),
                stateChartDefinition.InitialContext);

        private static BaseStateNode<TContext> Parse<TContext>(
            this BaseStateNodeDefinition<TContext> stateNodeDefinition,
            BaseStateNode<TContext> parent)
            where TContext : IEquatable<TContext>
        {
            IEnumerable<BaseStateNode<TContext>> ParseSubstateNodes(
                IEnumerable<BaseStateNodeDefinition<TContext>> substateNodeDefinitions,
                BaseStateNode<TContext> recursedParent) =>
                substateNodeDefinitions.Select(substateDefinition => substateDefinition.Parse(recursedParent));

            switch (stateNodeDefinition)
            {
                case AtomicStateNodeDefinition<TContext> definition:
                    return new AtomicStateNode<TContext>(parent, definition);
                case FinalStateNodeDefinition<TContext> definition:
                    return new FinalStateNode<TContext>(parent, definition);
                case CompoundStateNodeDefinition<TContext> definition:
                    var compound = new CompoundStateNode<TContext>(parent, definition);
                    compound.StateNodes = ParseSubstateNodes(definition.States, compound);
                    compound.InitialTransition = new InitialTransition<TContext>(
                        compound,
                        compound.ResolveTarget(definition.InitialTransition.Target),
                        definition.InitialTransition.Actions);
                    return compound;
                case OrthogonalStateNodeDefinition<TContext> definition:
                    var orthogonal = new OrthogonalStateNode<TContext>(parent, definition);
                    orthogonal.StateNodes = ParseSubstateNodes(definition.States, orthogonal);
                    return orthogonal;
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public abstract class ParsedStatechart<TContext>
        where TContext : IEquatable<TContext>
    {
        public BaseStateNode<TContext> RootNode { get; set; }

        public string Id => RootNode.Key.GetType().Name; // TODO: get real name here

        public ParsedStatechart(BaseStateNode<TContext> rootNode)
        {
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }
    }

    class InvalidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        public IEnumerable<StatechartDefinitionError> Errors { get; }

        public InvalidStatechart(BaseStateNode<TContext> rootNode, IEnumerable<StatechartDefinitionError> errors) : base(rootNode)
        {
            Errors = errors;
        }
    }

    class ValidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        public IEnumerable<StatechartHole> Holes { get; }

        public ValidStatechart(BaseStateNode<TContext> rootNode, IEnumerable<StatechartHole> holes) : base(rootNode)
        {
            Holes = holes;
        }
    }

    public class ExecutableStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        private IDictionary<StateNodeId, BaseStateNode<TContext>> stateNodes;
        public TContext InitialContext { get; }
        public IEnumerable<BaseTransition<TContext>> Transitions { get; }

        public ExecutableStatechart(BaseStateNode<TContext> rootNode, TContext initialContext) : base(rootNode)
        {
            T Identity<T>(T t) => t;

            IEnumerable<BaseTransition<TContext>> GetTransitions(BaseStateNode<TContext> stateNode)
                => stateNode.EventDefinitions.SelectMany(eventDefinition =>
                    eventDefinition.Map<IEnumerable<BaseTransition<TContext>>>(
                        immediate => immediate.Transitions.Select(
                            transitionDefinition => transitionDefinition.Map(
                                unguarded => new UnguardedImmediateTransition<TContext>(
                                    stateNode,
                                    transitionDefinition.Targets.Select(target => ResolveTarget(stateNode, target)),
                                    unguarded.Actions) as BaseTransition<TContext>,
                                guarded => new GuardedImmediateTransition<TContext>(
                                    guarded.Guard,
                                    stateNode,
                                    transitionDefinition.Targets.Select(target => ResolveTarget(stateNode, target)),
                                    guarded.Actions) as BaseTransition<TContext>)),
                        @event => @event.Transitions.Select(
                                transitionDefinition => transitionDefinition.Map(
                                    unguarded => new UnguardedEventTransition<TContext>(
                                        stateNode,
                                        @event.Event,
                                        transitionDefinition.Targets.Select(target => ResolveTarget(stateNode, target)),
                                        unguarded.Actions) as BaseTransition<TContext>,
                                    guarded => new GuardedEventTransition<TContext>(
                                        stateNode,
                                        guarded.Guard,
                                        @event.Event,
                                        transitionDefinition.Targets.Select(target => ResolveTarget(stateNode, target)),
                                        guarded.Actions) as BaseTransition<TContext>)),
                        guarded => Enumerable.Empty<BaseTransition<TContext>>()));

            InitialContext = initialContext;
            stateNodes = rootNode.Append(rootNode.GetDescendants().ToArray())
                .ToDictionary(stateNode => stateNode.Id, stateNode => stateNode);
            Transitions = rootNode.CataFold(
                GetTransitions,
                GetTransitions,
                (compound, children) => GetTransitions(compound).Concat(children.SelectMany(Identity)),
                (orthogonal, children) => GetTransitions(orthogonal).Concat(children.SelectMany(Identity))).ToList();
        }

        public IEnumerable<BaseStateNode<TContext>> GetStateNodes(StateConfiguration configuration)
            => GetStateNodes(configuration.StateNodeIds);
        public IEnumerable<BaseStateNode<TContext>> GetStateNodes(IEnumerable<StateNodeId> stateNodeIds)
            => stateNodeIds.Select(id => stateNodes[id]);

        public BaseStateNode<TContext> GetStateNode(StateNodeId id)
            => stateNodes[id];

        private BaseStateNode<TContext> ResolveTarget(
            BaseStateNode<TContext> fromStateNode,
            BaseTargetDefinition targetDefinition)
            => targetDefinition.Map(
                absolute => GetStateNode(absolute.Id),
                sibling => GetStateNode(StateNode.MakeId(fromStateNode.Parent, sibling.Key)),
                child => GetStateNode(StateNode.MakeId(fromStateNode, child.Key)));
    }
}
