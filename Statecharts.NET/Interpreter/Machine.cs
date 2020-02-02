using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    // TODO: CurrentConfig/CurrentState

    // TODO: roadmap: move Action to Definition and Unify | invoke Actions | invoke Services | rethink Events | unify Micro/Macrosteps

    public class Machine<TContext>
        where TContext : IEquatable<TContext>
    {
        private Queue<Model.Event> internalEvents = new Queue<Model.Event>(); // TODO: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
        private Queue<Model.Event> externalEvents = new Queue<Model.Event>();
        private StateConfiguration stateConfiguration;
        private TContext context;

        public ExecutableStatechart<TContext> StateChart { get; set; }

        // TODO: think whether this can be modeled with a StartedService to prevent doing stuff with an uninitialized Statechart
        public State<TContext> Start() => Start(StateConfiguration.NotInitialized);
        public State<TContext> Start(StateConfiguration configuration)
        {
            stateConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            context = StateChart.InitialContext;
            Console.WriteLine($"{Environment.NewLine}Starting...");
            var steps = Execute();
            return new State<TContext>(stateConfiguration, context);
        }
        public State<TContext> Send(NamedEvent @event)
        {
            Console.WriteLine($"{Environment.NewLine}Enqueuing {@event.EventName}...");
            externalEvents.Enqueue(@event);
            var steps = Execute();
            return new State<TContext>(stateConfiguration, context);
        }

        internal MacroStep ExecuteMicrosteps(IEnumerable<MicroStep> microSteps)
        {
            var executedSteps = new List<MicroStep>();
            foreach (var microStep in microSteps)
            {
                executedSteps.Add(ApplyStep(microStep));
                executedSteps.AddRange(Stabilize());
            }
            return new MacroStep(executedSteps);
        }

        private IEnumerable<MicroStep> Stabilize()
        {
            var steps = new List<MicroStep>();
            for (var microStep = CreateStabilizationStep(); microStep != null; microStep = CreateStabilizationStep())
                steps.Add(ApplyStep(microStep));
            return steps;
        }

        private StabilizationStep CreateStabilizationStep() =>
            StateChart.GetStateNodes(stateConfiguration)
                .GetUnstableStateNodes()
                .Select(stateNode => stateNode.Match(
                    atomic => null,
                    final => null, // TODO: correctly handle final states according to SCXML
                    compound => new StabilizationStep(compound.InitialTransition.Target.Append(compound.InitialTransition.Target.GetDescendants()).Select(a => a.Id)), // TODO: https://github.com/davidkpiano/xstate/issues/675
                    orthogonal => new StabilizationStep(orthogonal.StateNodes.Select(state => state.Id))))
                .FirstOrDefault();

        private MicroStep ApplyStep(MicroStep microStep)
        {
            void ApplyInitialStep(InitializationStep step)
            {
                var enteredStateNodes = StateChart.GetStateNode(step.RootStateId).Yield();
                var eventsRaisedByEntering = enteredStateNodes.Select(stateNode => ExecuteActionBlock(stateNode.EntryActions)).ToList();
                stateConfiguration = stateConfiguration.With(step.RootStateId);
            }
            void ApplyStabilizationStep(StabilizationStep step)
            {
                var enteredStateNodes = StateChart.GetStateNodes(step.EnteredStatesIds);
                var eventsRaisedByEntering = enteredStateNodes.Select(stateNode => ExecuteActionBlock(stateNode.EntryActions)).ToList();
                stateConfiguration = stateConfiguration.With(step.EnteredStatesIds);
            }
            void ApplyStep(IList<StateNodeId> enteredStatesKeys, IList<StateNodeId> exitedStatesKeys, Transition transition)
            {
                var enteredStateNodes = StateChart.GetStateNodes(enteredStatesKeys);
                var exitedStateNodes = StateChart.GetStateNodes(exitedStatesKeys);
                var eventsRaisedByExiting = exitedStateNodes.Select(stateNode => ExecuteActionBlock(stateNode.ExitActions)).ToList();
                var eventsRaisedOnTransition = transition.Match(forbidden => Enumerable.Empty<Model.IEvent>(), unguarded => ExecuteActionBlock(unguarded.Actions), guarded => ExecuteActionBlock(guarded.Actions));
                var eventsRaisedByEntering = enteredStateNodes.Select(stateNode => ExecuteActionBlock(stateNode.EntryActions)).ToList();
                stateConfiguration = stateConfiguration.Without(exitedStatesKeys).With(enteredStatesKeys);
            }

            microStep.Match<TContext>(
                ApplyInitialStep,
                ApplyStabilizationStep,
                immediate => ApplyStep(immediate.EnteredStatesIds.ToList(), immediate.ExitedStatesIds.ToList(), immediate.Transition),
                @event => ApplyStep(@event.EnteredStatesIds.ToList(), @event.ExitedStatesIds.ToList(), @event.Transition));
            return microStep; // TODO: raised/sent events
        }

        // TODO: https://github.com/davidkpiano/xstate/issues/603
        // TODO: raised Events
        private IEnumerable<Model.IEvent> ExecuteActionBlock(IEnumerable<Model.Action> actions)
        {
            var events = new List<Event>();

            foreach (var action in actions)
                action.Switch(
                    // TODO: actually execute the Actions
                    // TODO: where to get EventData from
                    send => { }, 
                    raise => { },
                    log => { },
                    assign => assign.Mutation(context, default),
                    sideEffect => sideEffect.Function(context, default));

            return events; // TODO: return actual raised events
        }

        // like 'sismic.execute_once'
        internal MacroStep ExecuteMacrostep()
        {
            var microSteps = ComputeMicroSteps().ToList();
            var macroStep = ExecuteMicrosteps(microSteps);
            return macroStep;
        }

        internal IEnumerable<MacroStep> Execute()
        {
            var steps = new List<MacroStep>();
            var macroStep = ExecuteMacrostep();
            while (macroStep.MicroSteps.Count() != 0) // TODO: make this prettier
            {
                steps.Add(macroStep);
                macroStep = ExecuteMacrostep();
            }
            return steps;
        }

        internal IEnumerable<MicroStep> ComputeMicroSteps()
        {
            if (stateConfiguration.IsNotInitialized) return new InitializationStep().Yield();
            var @event = SelectEvent();
            var transitions = SelectTransitions(@event);
            var computedSteps = CreateSteps(@event, transitions);
            return computedSteps;
        }

        private Model.Event SelectEvent()
            => internalEvents.Count > 0
                ? internalEvents.Dequeue()
                : externalEvents.Count > 0
                    ? externalEvents.Dequeue()
                    : null;
            //=> internalEvents.Concat(externalEvents).FirstOrDefault();

        private IEnumerable<MicroStep> CreateSteps(Model.Event @event, IEnumerable<Transition> transitions)
            => transitions.SelectMany(transition =>
                transition.GetTargets().Select(target =>
                {
                    if(@event == null) Debug.WriteLine("TODO: CreateSteps(...) was called with NULL event, remodel this"); // TODO: remodel
                    var lca = (transition.Source, target).LeastCommonAncestor();
                    var lastBeforeLCA = transition.Source.OneBeneath(lca);
                    var exited = lastBeforeLCA.Append(lastBeforeLCA.GetDescendants()).Where(stateConfiguration.Contains);
                    var entered = target.Append(target.AncestorsUntil(lca).Reverse());
                    return transition.Match<ForbiddenTransition, UnguardedTransition, GuardedTransition, MicroStep>( // TODO: refactor the similarity
                        forbidden => null, // TODO: WTF, needs remodelling
                        unguarded => new EventStep<TContext>(@event, transition, entered.Ids(), exited.Ids()), 
                        guarded => new EventStep<TContext>(@event, transition, entered.Ids(), exited.Ids()));
                }));

        // TODO: don't take all transitions (https://gitlab.com/scion-scxml/test-framework/blob/master/test/documentOrder/documentOrder0.scxml)
        private IEnumerable<Transition> SelectTransitions(OneOf<Model.Event, Model.CustomDataEvent> nextEvent)
        {
            bool Matches(OneOf<Model.Event, Model.CustomDataEvent> @event) => @event.Equals(nextEvent); // TODO: Equals vs. == (https://docs.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)?redirectedfrom=MSDN)
            bool IsEnabled(GuardedTransition guarded)
                => guarded.Guard.Match(
                    inline => false, // TODO: proper inState check
                    guard => guard.Condition.Invoke(context),
                    dataGuard => dataGuard.Condition.Invoke(context, null)); // TODO: pass Data to Event
            bool SourceStateIsActive(Transition transition)
                => stateConfiguration.Contains(transition.Source);
            bool TransitionShouldBeTaken(Transition transition) => transition.Match(
                    forbidden => false, // TODO: remove others if first was forbidden
                    unguarded =>Matches(unguarded.Event),
                    guarded => Matches(guarded.Event) && IsEnabled(guarded));

            return StateChart.Transitions
                .Where(SourceStateIsActive)
                .Where(TransitionShouldBeTaken);
        }
    }

    internal class MacroStep
    {
        public MacroStep(IEnumerable<MicroStep> microSteps)
        {
            MicroSteps = microSteps;
        }

        public IEnumerable<MicroStep> MicroSteps { get; }
    }

    internal static class MicroStepFunctions
    {
        internal static TResult Map<TContext, TResult>(
            this MicroStep microStep,
            Func<InitializationStep, TResult> fInitial,
            Func<StabilizationStep, TResult> fStabilization,
            Func<ImmediateStep<TContext>, TResult> fImmediate,
            Func<EventStep<TContext>, TResult> fEvent)
            where TContext : IEquatable<TContext>
        {
            switch (microStep)
            {
                case InitializationStep initial:
                    return fInitial(initial);
                case StabilizationStep stabilization:
                    return fStabilization(stabilization);
                case ImmediateStep<TContext> immediate:
                    return fImmediate(immediate);
                case EventStep<TContext> @event:
                    return fEvent(@event);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
        internal static void Match<TContext>(
            this MicroStep microStep,
            Action<InitializationStep> fInitial,
            Action<StabilizationStep> fStabilization,
            Action<ImmediateStep<TContext>> fImmediate,
            Action<EventStep<TContext>> fEvent)
            where TContext : IEquatable<TContext>
        {
            switch (microStep)
            {
                case InitializationStep initial:
                    fInitial(initial);
                    break;
                case StabilizationStep stabilization:
                    fStabilization(stabilization);
                    break;
                case ImmediateStep<TContext> immediate:
                    fImmediate(immediate);
                    break;
                case EventStep<TContext> @event:
                    fEvent(@event);
                    break;
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    internal abstract class MicroStep { }
    internal class InitializationStep : MicroStep
    {
        public StateNodeId RootStateId => new StateNodeId(new RootStateNodeKey(string.Empty)); // TODO: fix this
    }
    internal class StabilizationStep : MicroStep
    {
        public IEnumerable<StateNodeId> EnteredStatesIds { get; }

        public StabilizationStep(IEnumerable<StateNodeId> enteredStatesIds)
        {
            EnteredStatesIds = enteredStatesIds ?? throw new ArgumentNullException(nameof(enteredStatesIds));
        }
    }
    internal class ImmediateStep<TContext> : MicroStep
        where TContext : IEquatable<TContext>
    {
        public UnguardedTransition Transition { get; }
        public IEnumerable<StateNodeId> EnteredStatesIds { get; }
        public IEnumerable<StateNodeId> ExitedStatesIds { get; }

        public ImmediateStep(
            UnguardedTransition transition,
            IEnumerable<StateNodeId> enteredStatesIds,
            IEnumerable<StateNodeId> exitedStatesIds)
        {
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
            EnteredStatesIds = enteredStatesIds ?? throw new ArgumentNullException(nameof(enteredStatesIds));
            ExitedStatesIds = exitedStatesIds ?? throw new ArgumentNullException(nameof(exitedStatesIds));
        }
    }
    internal class EventStep<TContext> : MicroStep
        where TContext : IEquatable<TContext>
    {
        public Model.Event Event { get; }
        public Transition Transition { get; }
        public IEnumerable<StateNodeId> EnteredStatesIds { get; }
        public IEnumerable<StateNodeId> ExitedStatesIds { get; }

        public EventStep(
            Model.Event @event,
            Transition transition,
            IEnumerable<StateNodeId> enteredStatesIds,
            IEnumerable<StateNodeId> exitedStatesIds)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
            EnteredStatesIds = enteredStatesIds ?? throw new ArgumentNullException(nameof(enteredStatesIds));
            ExitedStatesIds = exitedStatesIds ?? throw new ArgumentNullException(nameof(exitedStatesIds));
        }
    }
}
