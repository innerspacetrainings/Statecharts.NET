using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    // TODO: CurrentConfig/CurrentState

    public class Service<TContext>
        where TContext : IEquatable<TContext>
    {
        private Queue<BaseEvent> internalEvents = new Queue<BaseEvent>(); // TODO: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
        private Queue<BaseEvent> externalEvents = new Queue<BaseEvent>();
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
        public State<TContext> Send(Event @event)
        {
            Console.WriteLine($"{Environment.NewLine}Enqueuing {@event.Type}...");
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

        private StabilizationStep CreateStabilizationStep()
        {
            foreach (var stateNode in StateChart.GetStateNodes(stateConfiguration).GetUnstableStateNodes())
            {
                switch (stateNode)
                {
                    case FinalStateNode<TContext> final:
                        return null; // TODO: correctly handle final states according to SCXML
                    case OrthogonalStateNode<TContext> orthogonal:
                        return new StabilizationStep(orthogonal.StateNodes.Select(state => state.Id));
                    case CompoundStateNode<TContext> compound:
                        // TODO: https://github.com/davidkpiano/xstate/issues/675
                        return new StabilizationStep(compound.InitialTransition.Target.Append(compound.InitialTransition.Target.GetDescendants()).Select(a => a.Id));
                    default: throw new Exception("NON EXHAUSTIVE SWITCH");
                }
            }
            return null;
        }

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
            void ApplyStep(IList<StateNodeId> enteredStatesKeys, IList<StateNodeId> exitedStatesKeys, Transition<TContext> transition)
            {
                var enteredStateNodes = StateChart.GetStateNodes(enteredStatesKeys);
                var exitedStateNodes = StateChart.GetStateNodes(exitedStatesKeys);
                var eventsRaisedByExiting = exitedStateNodes.Select(stateNode => ExecuteActionBlock(stateNode.ExitActions)).ToList();
                var eventsRaisedOnTransition = transition.Map(eventless => ExecuteActionBlock(eventless.Actions),eventful => ExecuteActionBlock(eventful.Actions));
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
        private IEnumerable<BaseEvent> ExecuteActionBlock(IEnumerable<Action> actions)
        {
            foreach (var action in actions)
            {
                switch (action)
                {
                    // TODO: execute the Actions
                    case LogAction logAction:
                        Debug.WriteLine(logAction.Label);
                        break;
                    case SideEffectAction<TContext> sideEffectAction:
                        sideEffectAction.Function(context);
                        break;
                    default:
                        Debug.WriteLine($"{action.GetType().Name} should be executed");
                        break;
                }
            }

            return Enumerable.Empty<BaseEvent>(); // TODO: return actual raised events
        }

        private IEnumerable<BaseEvent> ExecuteActionBlock(IEnumerable<EventAction> actions)
        {
            foreach (var action in actions)
            {
                switch (action)
                {
                    // TODO: execute the Actions
                    case AssignEventAction<TContext, int> assignEventAction:
                        assignEventAction.Mutation(context, new Event("TODO, sheeeeesh"));
                        break;
                    default:
                        Debug.WriteLine($"{action.GetType().Name} should be executed");
                        break;
                }
            }

            return Enumerable.Empty<BaseEvent>(); // TODO: return actual raised events
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

        private BaseEvent SelectEvent()
            => internalEvents.Count > 0
                ? internalEvents.Dequeue()
                : externalEvents.Count > 0
                    ? externalEvents.Dequeue()
                    : null;
            //=> internalEvents.Concat(externalEvents).FirstOrDefault();

        private IEnumerable<MicroStep> CreateSteps(BaseEvent @event,
            IEnumerable<BaseTransition<TContext>> transitions)
            => transitions.SelectMany(transition =>
                transition.GetTargets().Select(target =>
                {
                    if(@event == null) Debug.WriteLine("TODO: CreateSteps(...) was called with NULL event, remodel this"); // TODO: remodel
                    var lca = (transition.Source, target).LeastCommonAncestor();
                    var lastBeforeLCA = transition.Source.OneBeneath(lca);
                    var exited = lastBeforeLCA.Append(lastBeforeLCA.GetDescendants()).Where(stateConfiguration.Contains);
                    var entered = target.Append(target.AncestorsUntil(lca).Reverse());
                    return transition.Map<MicroStep, TContext>( // TODO: refactor the similarity
                        initial => throw new Exception("WTF is happening here..."), // TODO: remodel this
                        unguarded => new ImmediateStep<TContext>(new EventlessTransition<TContext>(unguarded.Source, unguarded.Targets, unguarded.Actions), entered.Ids(), exited.Ids()),
                        guarded => new ImmediateStep<TContext>(new EventlessTransition<TContext>(guarded.Source, guarded.Targets, guarded.Actions), entered.Ids(), exited.Ids()),
                        unguarded => new EventStep<TContext>(@event, new EventfulTransition<TContext>(unguarded.Source, unguarded.Targets, unguarded.Actions), entered.Ids(), exited.Ids()),
                        guarded => new EventStep<TContext>(@event, new EventfulTransition<TContext>(guarded.Source, guarded.Targets, guarded.Actions), entered.Ids(), exited.Ids()));
                }));

        // TODO: don't take all transitions (https://gitlab.com/scion-scxml/test-framework/blob/master/test/documentOrder/documentOrder0.scxml)
        private IEnumerable<BaseTransition<TContext>> SelectTransitions(BaseEvent nextEvent)
        {
            bool Matches(BaseEvent @event) => @event.Equals(nextEvent); // TODO: Equals vs. == (https://docs.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)?redirectedfrom=MSDN)
            bool IsEnabled(GuardedTransition guarded)
                => guarded.Guard.Map<bool, TContext>(
                    inline => inline.Condition.Invoke(context, nextEvent),
                    inState => false); // TODO: proper inState check
            bool SourceStateIsActive(BaseTransition<TContext> transition)
                => stateConfiguration.Contains(transition.Source);
            bool TransitionShouldBeTaken(BaseTransition<TContext> transition)
                => transition.Map(
                    _ => false, // TODO: check if this case gets ever hit
                    _ => true,
                    guarded => IsEnabled(guarded),
                    unguarded => Matches(unguarded.Event),
                    guarded => Matches(guarded.Event) && IsEnabled(guarded));

            if (nextEvent == null) Debug.WriteLine("TODO: SelectTransitions(...) was called with NULL event, remodel this"); // TODO: remodel

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
        public EventlessTransition<TContext> Transition { get; }
        public IEnumerable<StateNodeId> EnteredStatesIds { get; }
        public IEnumerable<StateNodeId> ExitedStatesIds { get; }

        public ImmediateStep(
            EventlessTransition<TContext> transition,
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
        public BaseEvent Event { get; }
        public EventfulTransition<TContext> Transition { get; }
        public IEnumerable<StateNodeId> EnteredStatesIds { get; }
        public IEnumerable<StateNodeId> ExitedStatesIds { get; }

        public EventStep(
            BaseEvent @event,
            EventfulTransition<TContext> transition,
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
