using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    // TODO: rethink events | continuation (services, final) | fix guards/immediate

    public class Machine<TContext>
        where TContext : IEquatable<TContext>
    {
        private EventQueue events = new EventQueue();
        private StateConfiguration stateConfiguration;
        private Dictionary<StateNode, CancellationTokenSource> serviceCancellationTokens = new Dictionary<StateNode, CancellationTokenSource>();
        private TContext context;

        private ILogger logger = new ConsoleLogger();
        private TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();

        public ExecutableStatechart<TContext> StateChart { get; set; }

        // TODO: think whether this can be modeled with a StartedMachine to prevent doing stuff with an uninitialized Statechart
        public StartResult<TContext> Start() => Start(StateConfiguration.NotInitialized);
        public StartResult<TContext> Start(StateConfiguration configuration)
        {
            stateConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            context = StateChart.InitialContext;
            var steps = Execute();
            return new StartResult<TContext>(new State<TContext>(stateConfiguration, context), taskSource.Task);
        }

        public State<TContext> Send(Model.NamedEvent @event)
            => Send(@event as ISendableEvent);
        private State<TContext> Send(ISendableEvent @event)
        {
            Console.WriteLine($"{@event} sent");
            events.EnqueueExternal(@event);
            var steps = Execute();
            return new State<TContext>(stateConfiguration, context);
        }

        // like 'sismic.execute_once'
        internal IList<MacroStep> Execute()
        {
            var steps = new List<MacroStep>();
            do steps.Add(ExecuteMacrostep()); while (!events.IsEmpty);
            return steps;
        }
        internal MacroStep ExecuteMacrostep()
        {
            var microSteps = ComputeMicroSteps().ToList();
            var macroStep = ExecuteMicrosteps(microSteps);
            // TODO: (Service) invoke all services here
            var enteredStateNodes = macroStep.EnteredStateNodes
                .Select(stateNode => (stateNode, services: stateNode.Services))
                .Where(entry => entry.services.Any());

            foreach (var (stateNode, services) in enteredStateNodes)
            {
                var cts = new CancellationTokenSource();
                foreach (var service in services)
                {
                    var s = service.Invoke(cts.Token);
                    s.ContinueWith(task =>
                    {
                        switch (task.Status)
                        {
                            case TaskStatus.RanToCompletion:
                                Send(new NamedEvent($"service.finished {task.Result}"));
                                break;
                            case TaskStatus.Faulted:
                                Send(new NamedEvent("service.failed"));
                                break;
                        }
                    }, cts.Token);
                }

                serviceCancellationTokens.Add(stateNode, cts);
            }

            return macroStep;
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
                    compound => new StabilizationStep(compound.InitialTransition.Target.Append(compound.InitialTransition.Target.GetDescendants())), // TODO: https://github.com/davidkpiano/xstate/issues/675
                    orthogonal => new StabilizationStep(orthogonal.StateNodes)))
                .FirstOrDefault();

        private MicroStep ApplyStep(MicroStep microStep)
        {
            void ExecuteEntryActionsFor(params StateNode[] stateNodes)
            {
                foreach (var stateNode in stateNodes)
                    ExecuteActionBlock(stateNode.EntryActions);
            }
            void ExecuteExitActionsFor(params StateNode[] stateNodes)
            {
                foreach (var stateNode in stateNodes)
                    ExecuteActionBlock(stateNode.ExitActions);
            }
            void ApplyInitialStep(InitializationStep step)
            {
                ExecuteEntryActionsFor(step.RootState);
                stateConfiguration = stateConfiguration.With(step.RootState.Id);
            }
            void ApplyStabilizationStep(StabilizationStep step)
            {
                ExecuteEntryActionsFor(step.EnteredStates.ToArray());
                stateConfiguration = stateConfiguration.With(step.EnteredStates.Ids());
            }
            void ApplyStep(IList<StateNode> enteredStates, IList<StateNode> exitedStates, Transition transition)
            {
                ExecuteExitActionsFor(exitedStates.ToArray());
                foreach (var token in exitedStates.Select(sn => serviceCancellationTokens.GetValue(sn)).WhereNotNull())
                    token.Cancel();
                foreach (var stateNode in exitedStates)
                    serviceCancellationTokens.Remove(stateNode);
                ExecuteActionBlock(transition.Actions);
                ExecuteEntryActionsFor(enteredStates.ToArray());
                stateConfiguration = stateConfiguration.Without(exitedStates.Ids()).With(enteredStates.Ids());
            }

            microStep.Switch(
                ApplyInitialStep,
                ApplyStabilizationStep,
                immediate => ApplyStep(immediate.EnteredStates.ToList(), immediate.ExitedStates.ToList(), immediate.Transition),
                @event => ApplyStep(@event.EnteredStates.ToList(), @event.ExitedStates.ToList(), @event.Transition));
            return microStep; // TODO: raised/sent events
        }

        private void ExecuteActionBlock(IEnumerable<Model.Action> actions)
        {
            foreach (var action in actions)
                try
                {
                    // TODO: where to get EventData from
                    action.Switch(
                        send => events.EnqueueExternal(new Model.NamedEvent(send.EventName)),
                        raise => events.EnqueueInternal(new Model.NamedEvent(raise.EventName)),
                        log => logger.Log(log.Message(context, default)),
                        assign => assign.Mutation(context, default),
                        sideEffect => sideEffect.Function(context, default));
                }
                catch (Exception exception)
                {
                    // TODO: error handling: place Event 'error.execution' (with Exception as Data) on internal Event Queue, only raise the exception, if no handler was defined
                    // TODO: System.InvalidOperationException: 'An attempt was made to transition a task to a final state when it had already completed.'
                    taskSource.SetException(exception);
                }
        }

        internal IEnumerable<MicroStep> ComputeMicroSteps()
        {
            if (stateConfiguration.IsNotInitialized) return new InitializationStep(StateChart.RootNode).Yield();
            var @event = events.Dequeue();
            var transitions = SelectTransitions(@event);
            var computedSteps = CreateSteps(@event, transitions);
            return computedSteps;
        }

        private IEnumerable<MicroStep> CreateSteps(IEvent @event, IEnumerable<Transition> transitions)
            => transitions.SelectMany(transition =>
                transition.GetTargets().Select(target =>
                {
                    var lca = (transition.Source, target).LeastCommonAncestor();
                    var lastBeforeLCA = transition.Source.OneBeneath(lca);
                    var exited = lastBeforeLCA.Append(lastBeforeLCA.GetDescendants()).Where(stateConfiguration.Contains);
                    var entered = target.Append(target.AncestorsUntil(lca).Reverse());

                    MicroStep NoStep() => null;
                    EventStep EventStep() => new EventStep(@event, transition, entered, exited);
                    
                    return transition.Match(
                        forbidden => NoStep(),
                        unguarded => EventStep(),
                        guarded => EventStep());
                })).Where(step => step != null);

        private IEnumerable<Transition> SelectTransitions(IEvent nextEvent)
        {
            bool Matches(OneOf<Event, CustomDataEvent> @event) => @event.Match<IEvent>(e => e, e => e).Equals(nextEvent); // TODO: Equals vs. == (https://docs.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)?redirectedfrom=MSDN)
            bool IsEnabled(GuardedTransition guarded)
                => guarded.Guard.Match(
                    @in => false, // TODO: proper inState check
                    guard => guard.Condition.Invoke(context),
                    dataGuard => dataGuard.Condition.Invoke(context, null)); // TODO: pass Data to Event
            bool SourceStateIsActive(Transition transition)
                => stateConfiguration.Contains(transition.Source);
            bool TransitionShouldBeTaken(Transition transition) => transition.Match(
                    forbidden => Matches(forbidden.Event),
                    unguarded => Matches(unguarded.Event),
                    guarded => Matches(guarded.Event) && IsEnabled(guarded));
            StateNode TransitionSource(Transition transition) => transition.Source;
            Transition FirstMatching(IGrouping<StateNode, Transition> transitions) => transitions.FirstOrDefault(TransitionShouldBeTaken);

            return StateChart.Transitions
                .Where(SourceStateIsActive)
                .GroupBy(TransitionSource)
                .Select(FirstMatching)
                .WhereNotNull();
        }
    }
}
