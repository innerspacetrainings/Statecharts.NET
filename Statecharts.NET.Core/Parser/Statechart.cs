using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Statecharts.NET.Utilities;
using Statecharts.NET.Definition;
using Statecharts.NET.Interpreter;
using Statecharts.NET.Model;
using Action = System.Action;
using Task = System.Threading.Tasks.Task;

namespace Statecharts.NET
{
    public static class StateChartExtensions
    {
        public static Machine<TContext> Interpret<TContext>(
            this ExecutableStatechart<TContext> statechart)
            where TContext : IEquatable<TContext>
            => new Machine<TContext> {StateChart = statechart}; // TODO: use ctor
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
                    definition.InitialTransition.Actions.ValueOr(Enumerable.Empty<OneOf<Definition.Action, Definition.ContextAction>>()));
                return compound;
            }
            Interpreter.OrthogonalStateNode CreateOrthogonalStateNode(Definition.OrthogonalStateNode definition)
            {
                var orthogonal = new Interpreter.OrthogonalStateNode(parent, definition);
                orthogonal.StateNodes = ParseSubstateNodes(definition.States, orthogonal);
                return orthogonal;
            }

            return stateNodeDefinition.Match<Interpreter.StateNode>(
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
        private readonly IDictionary<StateNodeId, Interpreter.StateNode> _stateNodes;
        private Action _doneAction;
        public TContext InitialContext { get; }
        public IEnumerable<Interpreter.Transition> Transitions { get; }

        public ExecutableStatechart(Interpreter.StateNode rootNode, TContext initialContext) : base(rootNode)
        {
            Interpreter.StateNode ResolveTarget(Interpreter.StateNode fromStateNode, Target target)
                => target.Match(
                    absolute => GetStateNode(absolute.Id),
                    sibling => GetStateNode(StateNodeId.Make(fromStateNode.Parent, sibling.Key)),
                    child => GetStateNode(StateNodeId.Make(fromStateNode, child.Key)));
            IEnumerable<Interpreter.Transition> GetTransitions(Interpreter.StateNode stateNode)
                => stateNode.Transitions.Select(
                    transition => transition.Match<Interpreter.Transition>(
                        definition => new Interpreter.ForbiddenTransition(stateNode, definition.Event),
                        definition => new Interpreter.UnguardedTransition(stateNode, definition.Event, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions.ValueOr(Enumerable.Empty<Definition.Action>())),
                        definition => new Interpreter.UnguardedTransition(stateNode, definition.Event, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions.ValueOr(Enumerable.Empty<OneOf<Definition.Action, ContextAction>>())),
                        definition => new Interpreter.UnguardedTransition(stateNode, definition.Event, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions.ValueOr(Enumerable.Empty<OneOf<Definition.Action, ContextAction, ContextDataAction>>())),
                        definition => new Interpreter.GuardedTransition(stateNode, definition.Event, definition.Guard, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions.ValueOr(Enumerable.Empty<Definition.Action>())),
                        definition => new Interpreter.GuardedTransition(stateNode, definition.Event, definition.Guard, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions.ValueOr(Enumerable.Empty<OneOf<Definition.Action, ContextAction>>())),
                        definition => new Interpreter.GuardedTransition(stateNode, definition.Event, definition.Guard, definition.Targets.Select(target => ResolveTarget(stateNode, target)), definition.Actions.ValueOr(Enumerable.Empty<OneOf<Definition.Action, ContextAction, ContextDataAction>>()))));

            InitialContext = initialContext;
            RootNode.Transitions.Add(
                new Interpreter.UnguardedTransition(
                    RootNode,
                    DoneEvent,
                    RootNode.Yield(),
                    (new Definition.SideEffectAction(() => _doneAction?.Invoke()) as Definition.Action).Yield()));

            var descendants = rootNode.GetDescendants().ToArray();
            _stateNodes = rootNode.Append(descendants)
                .ToDictionary(stateNode => stateNode.Id, stateNode => stateNode);
            Transitions = rootNode.CataFold(
                GetTransitions,
                GetTransitions,
                (compound, children) => GetTransitions(compound).Concat(children.SelectMany(Functions.Identity)),
                (orthogonal, children) => GetTransitions(orthogonal).Concat(children.SelectMany(Functions.Identity))).ToList();
        }

        public IEnumerable<Interpreter.StateNode> GetStateNodes(StateConfiguration configuration)
            => GetStateNodes(configuration.StateNodeIds);
        public IEnumerable<Interpreter.StateNode> GetStateNodes(IEnumerable<StateNodeId> stateNodeIds)
            => stateNodeIds.Select(GetStateNode);

        internal Interpreter.StateNode GetStateNode(StateNodeId id)
            => _stateNodes[id];

        internal void RegisterDoneAction(System.Action action) => _doneAction = action;
        
        public Task Start() =>
            Start(CancellationToken.None);
        public Task Start(CancellationToken cancellationToken) =>
            new RunningStatechart<TContext>(this, cancellationToken).StartFromRootState();
        public Task Start(State<TContext> state) =>
            Start(state, CancellationToken.None);
        public Task Start(State<TContext> state, CancellationToken cancellationToken) =>
            new RunningStatechart<TContext>(this, cancellationToken).StartFrom(state);

        // TODO: think of this, should be like xstate.machine.transition
        [Pure]
        public State<TContext> ResolveNextState(State<TContext> state, ISendableEvent @event)
            => Resolve(state, @event).state;
        [Pure]
        public IEnumerable<MicroStep> ResolveSingleEvent(State<TContext> state, IEvent @event)
            => Resolve(state, @event).microSteps;

        [Pure]
        private (State<TContext> state, IEnumerable<MicroStep> microSteps) Resolve(State<TContext> state, IEvent @event)
        {
            var transitions = SelectTransitions(state, @event);
            var microsteps = ComputeMicrosteps(transitions, state, @event);
            return (null, microsteps);
        }

        private IEnumerable<Interpreter.Transition> SelectTransitions(State<TContext> state, IEvent nextEvent)
        {
            bool Matches(OneOf<Event, CustomDataEvent> @event) => @event.Match<IEvent>(e => e, e => e).Equals(nextEvent); // TODO: Equals vs. == (https://docs.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)?redirectedfrom=MSDN)
            bool IsEnabled(Interpreter.GuardedTransition guarded)
                => guarded.Guard.Match(
                    @in => false, // TODO: proper inState check
                    guard => guard.Condition.Invoke(state.Context),
                    dataGuard => dataGuard.Condition.Invoke(state.Context, null)); // TODO: pass Data to Event
            bool SourceStateIsActive(Interpreter.Transition transition)
                => state.StateConfiguration.Contains(transition.Source);
            bool TransitionShouldBeTaken(Interpreter.Transition transition) => transition.Match(
                forbidden => Matches(forbidden.Event),
                unguarded => Matches(unguarded.Event),
                guarded => Matches(guarded.Event) && IsEnabled(guarded));
            Interpreter.StateNode TransitionSource(Interpreter.Transition transition) => transition.Source;
            Interpreter.Transition FirstMatching(IGrouping<Interpreter.StateNode, Interpreter.Transition> transitions) => transitions.FirstOrDefault(TransitionShouldBeTaken);

            return Transitions
                .Where(SourceStateIsActive)
                .GroupBy(TransitionSource)
                .Select(FirstMatching)
                .WhereNotNull();
        }
        private IEnumerable<MicroStep> ComputeMicrosteps(IEnumerable<Interpreter.Transition> transitions, State<TContext> state, IEvent @event)
            => transitions
                .Where(transition => transition.Match(forbidden => false, unguarded => true, guarded => true))
                .SelectMany(transition =>
                transition.GetTargets().Select(target =>
                {
                    var lca = (transition.Source, target).LeastCommonAncestor();
                    var lastBeforeLCA = transition.Source.OneBeneath(lca);
                    var exited = lastBeforeLCA.Append(lastBeforeLCA.GetDescendants()).Where(state.StateConfiguration.Contains);
                    var entered = target.Append(target.AncestorsUntil(lca).Reverse());

                    return new EventStep(@event, transition, entered, exited);
                }));
    }
}
