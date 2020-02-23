using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;
using Task = System.Threading.Tasks.Task;

namespace Statecharts.NET.Interpreter
{
    internal static class ReadabilityExtensions
    {
        internal static Queue<T> AsQueue<T>(this T t) => new Queue<T>(t.Yield());
        [Pure] internal static IEnumerable<StateNode> GetEnteredStateNodes(this Macrostep macrostep) =>
            macrostep.Aggregate(Enumerable.Empty<StateNode>(), (entered, step) => step.Match(
                init => entered.Append(init.RootState),
                stabilization => entered.Concat(stabilization.EnteredStates),
                immediate => entered.Except(immediate.ExitedStates).Concat(immediate.EnteredStates),
                @event => entered.Except(@event.ExitedStates).Concat(@event.EnteredStates)));
        internal static void CancelAll(this Dictionary<StateNode, CancellationTokenSource> cancellationTokens)
            => cancellationTokens.Values.ToList().ForEach(token => token.Cancel());
    }

    public class RunningStatechart<TContext> where TContext : IEquatable<TContext>
    {
        private readonly ExecutableStatechart<TContext> _statechart;
        private readonly TaskCompletionSource<object> _taskSource;
        private readonly Dictionary<StateNode, CancellationTokenSource> _serviceCancellationTokens;

        private StateConfiguration _stateConfiguration;
        private TContext _context;

        // TODO: config object
        private readonly ILogger logger = new ConsoleLogger();

        private State<TContext> CurrentState => new State<TContext>(_stateConfiguration, _context);

        public RunningStatechart(ExecutableStatechart<TContext> statechart, CancellationToken cancellationToken)
        {
            _statechart = statechart;
            _taskSource = new TaskCompletionSource<object>();
            _serviceCancellationTokens = new Dictionary<StateNode, CancellationTokenSource>();

            cancellationToken.Register(() =>
            {
                _serviceCancellationTokens.CancelAll();
                _taskSource.TrySetCanceled();
            });
        }

        public void Send(ISendableEvent @event)
        {
            (IEnumerable<ISendableEvent> sentEvents, IEnumerable<Exception> executionErrors) Apply(MicroStep microstep)
            {
                (IEnumerable<ISendableEvent> events, Option<Exception> exception) ExecuteActionBlock(ActionBlock actions)
                {
                    var sentEvents = new Queue<ISendableEvent>();
                    var exception = Option.None<Exception>();
                    try
                    {
                        foreach (var action in actions)
                            // TODO: where to get EventData from
                            action.Switch(
                                send => sentEvents.Enqueue(new NamedEvent(send.EventName)),
                                raise => Functions.NoOp(),
                                log => logger.Log(log.Message(_context, default)),
                                assign => assign.Mutation(_context, default),
                                sideEffect => sideEffect.Function(_context, default));
                    }
                    catch (Exception e)
                    {
                        exception = e.ToOption();
                    }
                    return (sentEvents, exception);
                }
                (IEnumerable<ISendableEvent> sentEvents, IEnumerable<Exception> executionErrors) ExecuteActionBlocks(IEnumerable<ActionBlock> actionBlocks)
                {
                    var result = new List<(IEnumerable<ISendableEvent> sentEvents, Option<Exception> executionErrors)>();
                    foreach (var actions in actionBlocks)
                        result.Add(ExecuteActionBlock(actions));
                    return (result.SelectMany(o => o.sentEvents), result.Select(o => o.executionErrors).WhereSome());
                }
                void StopServices()
                {
                    foreach (var (stateNode, token) in microstep.ExitedStates
                        .Select(stateNode =>
                            (stateNode, cancellationTokenSource: _serviceCancellationTokens.GetValue(stateNode)))
                        .Where(o => o.cancellationTokenSource != null))
                    {
                        token.Cancel();
                        _serviceCancellationTokens.Remove(stateNode);
                    }
                }

                // (1) execute exit actions
                var (eventsFromExiting, exceptionsFromExiting) = ExecuteActionBlocks(microstep.ExitedStates.Select(stateNode => stateNode.ExitActions));
                // (2) stop running services
                StopServices();
                // (3) execute transition actions
                var (eventsFromTransition, exceptionFromTransition) = microstep.Transition.Match(transition => ExecuteActionBlock(transition.Actions), () => (Enumerable.Empty<ISendableEvent>(), Option.None<Exception>()));
                // (4) execute entry actions
                var (eventsFromEntering, exceptionsFromEntering) = ExecuteActionBlocks(microstep.EnteredStates.Select(state => state.EntryActions));

                return (
                    sentEvents: eventsFromEntering.Concat(eventsFromTransition).Concat(eventsFromExiting),
                    executionErrors: exceptionsFromEntering.Concat(exceptionFromTransition.YieldValue()).Concat(exceptionsFromExiting));
            }
            void StartServices(Macrostep macrostep)
            {
                var entered = macrostep
                    .GetEnteredStateNodes()
                    .Select(stateNode => (stateNode, services: stateNode.Services))
                    .Where(entry => entry.services.Any());

                foreach (var (stateNode, services) in entered)
                {
                    _serviceCancellationTokens.Add(stateNode, new CancellationTokenSource());
                    foreach (var service in services)
                        service.Invoke(_serviceCancellationTokens[stateNode].Token).ContinueWith(task =>
                        {
                            switch (task.Status)
                            {
                                case TaskStatus.RanToCompletion:
                                    Send(new ServiceSuccessEvent(service.Id, task.Result));
                                    break;
                                case TaskStatus.Faulted:
                                    Send(new ServiceErrorEvent(service.Id));
                                    break;
                            }
                        }, _serviceCancellationTokens[stateNode].Token);
                }
            }

            var events = @event.AsQueue();
            while (events.Any())
            {
                var macrostep = _statechart.ResolveMacrostep(CurrentState, events.Dequeue());
                foreach (var microstep in macrostep)
                {
                    // (1) execute actions & stop services
                    var (sentEvents, executionExceptions) = Apply(microstep);
                    foreach (var sentEvent in sentEvents) events.Enqueue(sentEvent);
                    // (2) update state configuration
                    _stateConfiguration = _stateConfiguration.Without(microstep.ExitedStates.Ids()).With(microstep.EnteredStates.Ids());
                    // (3) handle execution errors
                    //TODO: foreach (var exception in executionExceptions) macrostep.AddErrorStep(_statechart.ResolveError(CurrentState, new ExecutionErrorEvent(exception)));
                }
                StartServices(macrostep);
            }
        }

        internal Task StartFrom(State<TContext> state) => Start(
            state.StateConfiguration,
            state.Context,
            () => StartServices()); // TODO: think about this
        internal Task StartFromRootState() => Start(
            new StateConfiguration(_statechart.RootNode.Yield().Ids()),
            _statechart.InitialContext,
            () => Send(InitEvent)); // TODO: generate Init Event Transition when Parsing

        private Task Start(
            StateConfiguration stateConfiguration,
            TContext context,
            System.Action initAction)
        {
            _stateConfiguration = stateConfiguration;
            _context = context;

            initAction();

            return _taskSource.Task;
        }
    }
}
