using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;
using Task = System.Threading.Tasks.Task;

namespace Statecharts.NET
{
    internal static class ReadabilityExtensions
    {
        internal static void CancelAll(this Dictionary<Statenode, CancellationTokenSource> cancellationTokens)
            => cancellationTokens.Values.ToList().ForEach(token => token.Cancel());
        internal static IEnumerable<Statenode> GetEnteredStateNodes<TContext>(this Macrostep<TContext> macrostep) where TContext : IContext<TContext> =>
            macrostep.Microsteps.SelectMany(microstep => microstep.EnteredStatenodes);
    }

    public class RunningStatechart<TContext> where TContext : IContext<TContext>
    {
        private readonly ExecutableStatechart<TContext> _statechart;
        private State<TContext> _currentState;
        private readonly TaskCompletionSource<object> _taskSource;
        private readonly Dictionary<Statenode, CancellationTokenSource> _serviceCancellationTokens;

        // TODO: config object
        private readonly ILogger _logger;

        internal RunningStatechart(ExecutableStatechart<TContext> statechart, CancellationToken cancellationToken)
        {
            _statechart = statechart;
            _taskSource = new TaskCompletionSource<object>();
            _serviceCancellationTokens = new Dictionary<Statenode, CancellationTokenSource>();

            _statechart.Done = (context, eventData) => CompleteSuccessfully(); // TODO: parameters
            cancellationToken.Register(CompleteCancelled);
            // TODO: register failed ActionBlock that was not handled as cancelled
        }

        public IEnumerable<string> NextEvents => _statechart
            .GetActiveStatenodes(_currentState.StateConfiguration)
            .SelectMany(statenode => statenode.GetTransitions())
            .Select(transition => transition.Event is NamedEvent named ? named.EventName : null)
            .WhereNotNull();

        public Task Start() => Start(
            new StateConfiguration(_statechart.Rootnode.Yield()),
            _statechart.InitialContext,
            () => HandleEvent(new InitializeEvent(_statechart.Rootnode.Id)));

        private Task Start(
            StateConfiguration stateConfiguration,
            TContext context,
            System.Action initAction)
        {
            _currentState = new State<TContext>(stateConfiguration, context.CopyDeep());
            initAction();
            return _taskSource.Task;
        }

        private void HandleEvent(IEvent @event)
        {
            var events = EventQueue.WithEvent(@event);

            while (events.IsNotEmpty)
            {
                var macrostep = Resolver.ResolveMacrostep(_statechart, _currentState, events.Dequeue(), (ExecuteAction, StopServices));
                _currentState = macrostep.State;
                foreach (var queuedEvent in macrostep.QueuedEvents) events.Enqueue(new NextStep(queuedEvent));
                Console.WriteLine("  State Config: " + string.Join(", ", _currentState.StateConfiguration.StateNodeIds));
                Console.WriteLine("  Context: " + _currentState.Context);
                StartServices(macrostep.GetEnteredStateNodes());
            }

            Console.WriteLine();
        }
        public void Send(ISendableEvent sentEvent) => HandleEvent(sentEvent);
        private void StopServices(IEnumerable<Statenode> statenodes)
        {
            foreach (var (stateNode, token) in statenodes
                .Select(stateNode => (stateNode, cancellationTokenSource: _serviceCancellationTokens.GetValue(stateNode)))
                .Where(o => o.cancellationTokenSource != null))
            {
                token.Cancel();
                _serviceCancellationTokens.Remove(stateNode);
            }
        }
        private Option<OneOf<CurrentStep, NextStep>> ExecuteAction(Action action, object context, object eventData)
        {
            var result = Option.None<OneOf<CurrentStep, NextStep>>();
            action.Switch(
                send => result = ((OneOf<CurrentStep, NextStep>)new NextStep(new NamedEvent(send.EventName))).ToOption(),
                raise => result = ((OneOf<CurrentStep, NextStep>)new CurrentStep(new NamedEvent(raise.EventName))).ToOption(),
                log => _logger?.Log(log.Message(context, eventData)),
                assign => assign.Mutation(context, eventData),
                sideEffect => sideEffect.Function(context, eventData));
            return result;
        }
        private void StartServices(IEnumerable<Statenode> statenodes)
        {
            var entered = statenodes
                .Select(statenode => (statenode, services: statenode.Match(final => Enumerable.Empty<Service>(), nonFinal => nonFinal.Services)))
                .Where(entry => entry.services != null && entry.services.Any());

            foreach (var (stateNode, services) in entered)
            {
                _serviceCancellationTokens.Add(stateNode, new CancellationTokenSource());
                foreach (var service in services)
                    service.Invoke(_serviceCancellationTokens[stateNode].Token).ContinueWith(task =>
                    {
                        switch (task.Status)
                        {
                            case TaskStatus.RanToCompletion:
                                HandleEvent(new ServiceSuccessEvent(service.Id, task.Result));
                                break;
                            case TaskStatus.Faulted:
                                HandleEvent(new ServiceErrorEvent(service.Id, task.Exception));
                                break;
                        }
                    }, _serviceCancellationTokens[stateNode].Token);
            }
        }

        private void Complete(System.Action finalAction)
        {
            _serviceCancellationTokens.CancelAll();
            finalAction();
        }
        private void CompleteSuccessfully() => Complete(() => _taskSource.TrySetResult(null));
        private void CompleteCancelled() => Complete(() => _taskSource.TrySetCanceled());
    }
}
