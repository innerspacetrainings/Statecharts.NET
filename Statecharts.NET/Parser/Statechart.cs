using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Interpreter;
using Statecharts.NET.Utilities;

namespace Statecharts.NET
{
    public static class StateChartExtensions
    {
        public static Service<TContext> Interpret<TContext>(
            this ExecutableStatechart<TContext> statechart)
            where TContext : IEquatable<TContext>
            => new Service<TContext> {StateChart = statechart}; // TODO: use ctor
    }

    public static class ParsedStatechartFunctions
    {
        public static ParsedStatechart<TContext> Parse<TContext>(this Definition.Statechart<TContext> stateChartDefinition)
            where TContext : IEquatable<TContext>
            => new ExecutableStatechart<TContext>(
                stateChartDefinition.RootStateNode.Parse(null),
                stateChartDefinition.InitialContext);

        private static Interpreter.StateNode Parse(
            this Definition.StateNode stateNodeDefinition,
            Interpreter.StateNode parent)
        {
            IEnumerable<Interpreter.StateNode> ParseSubstateNodes(
                IEnumerable<Definition.StateNode> substateNodeDefinitions,
                Interpreter.StateNode recursedParent) =>
                substateNodeDefinitions.Select(substateDefinition => substateDefinition.Parse(recursedParent));

            Interpreter.CompoundStateNode CreateCompoundStateNode(Definition.CompoundStateNode definition)
            {
                var compound = new Interpreter.CompoundStateNode(parent, definition);
                compound.StateNodes = ParseSubstateNodes(definition.States, compound);
                compound.InitialTransition = new Interpreter.InitialTransition(
                    compound,
                    compound.ResolveTarget(definition.InitialTransition.Target),
                    definition.InitialTransition.Actions);
                return compound;
            }
            Interpreter.OrthogonalStateNode CreateOrthogonalStateNode(Definition.OrthogonalStateNode definition)
            {
                var orthogonal = new Interpreter.OrthogonalStateNode(parent, definition);
                orthogonal.StateNodes = ParseSubstateNodes(definition.States, orthogonal);
                return orthogonal;
            }

            return stateNodeDefinition.Match<Definition.AtomicStateNode, Definition.FinalStateNode, Definition.CompoundStateNode, Definition.OrthogonalStateNode, Interpreter.StateNode>(
                definition => new Interpreter.AtomicStateNode(parent, definition),
                definition => new Interpreter.FinalStateNode(parent, definition),
                CreateCompoundStateNode,
                CreateOrthogonalStateNode);
        }
    }

    public abstract class ParsedStatechart<TContext>
        where TContext : IEquatable<TContext>
    {
        public Interpreter.StateNode RootNode { get; set; }

        public string Id => RootNode.Key.GetType().Name; // TODO: get real name here

        public ParsedStatechart(Interpreter.StateNode rootNode)
        {
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }
    }

    class InvalidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        public IEnumerable<StatechartDefinitionError> Errors { get; }

        public InvalidStatechart(Interpreter.StateNode rootNode, IEnumerable<StatechartDefinitionError> errors) : base(rootNode)
        {
            Errors = errors;
        }
    }

    class ValidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        public IEnumerable<StatechartHole> Holes { get; }

        public ValidStatechart(Interpreter.StateNode rootNode, IEnumerable<StatechartHole> holes) : base(rootNode)
        {
            Holes = holes;
        }
    }

    public class ExecutableStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        private IDictionary<StateNodeId, Interpreter.StateNode> stateNodes;
        public TContext InitialContext { get; }
        public IEnumerable<Interpreter.Transition> Transitions { get; }

        public ExecutableStatechart(Interpreter.StateNode rootNode, TContext initialContext) : base(rootNode)
        {
            T Identity<T>(T t) => t;

            IEnumerable<Interpreter.Transition> GetTransitions(Interpreter.StateNode stateNode)
                => stateNode.Transitions.Select(
                    transition => transition.Match<Definition.ForbiddenTransition, Definition.UnguardedTransition, Definition.UnguardedContextTransition, Definition.UnguardedContextDataTransition, Definition.GuardedTransition, Definition.GuardedContextTransition, Definition.GuardedContextDataTransition, Interpreter.Transition>(
                        definition => new Interpreter.ForbiddenTransition(stateNode, definition.Event),
                        definition => new Interpreter.UnguardedTransition(stateNode, definition.Event, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions),
                        definition => new Interpreter.UnguardedTransition(stateNode, definition.Event, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions),
                        definition => new Interpreter.UnguardedTransition(stateNode, definition.Event, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions),
                        definition => new Interpreter.GuardedTransition(stateNode, definition.Event, definition.Guard, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions),
                        definition => new Interpreter.GuardedTransition(stateNode, definition.Event, definition.Guard, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions),
                        definition => new Interpreter.GuardedTransition(stateNode, definition.Event, definition.Guard, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions)));

            InitialContext = initialContext;
            stateNodes = rootNode.Append(rootNode.GetDescendants().ToArray())
                .ToDictionary(stateNode => stateNode.Id, stateNode => stateNode);
            Transitions = rootNode.CataFold(
                GetTransitions,
                GetTransitions,
                (compound, children) => GetTransitions(compound).Concat(children.SelectMany(Identity)),
                (orthogonal, children) => GetTransitions(orthogonal).Concat(children.SelectMany(Identity))).ToList();
        }

        public IEnumerable<Interpreter.StateNode> GetStateNodes(StateConfiguration configuration)
            => GetStateNodes(configuration.StateNodeIds);
        public IEnumerable<Interpreter.StateNode> GetStateNodes(IEnumerable<StateNodeId> stateNodeIds)
            => stateNodeIds.Select(id => stateNodes[id]);

        public Interpreter.StateNode GetStateNode(StateNodeId id)
            => stateNodes[id];

        private Interpreter.StateNode ResolveTarget(
            Interpreter.StateNode fromStateNode,
            Model.Target target)
            => target.Match(
                absolute => GetStateNode(absolute.Id),
                sibling => GetStateNode(StateNodeId.Make(fromStateNode.Parent, sibling.Key)),
                child => GetStateNode(StateNodeId.Make(fromStateNode, child.Key)));
    }
}
