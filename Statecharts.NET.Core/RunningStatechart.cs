using System;
using System.Collections.Generic;
using System.Linq;
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

    internal class CancellationCollection
    {
        private readonly Dictionary<StatenodeId, CancellationTokenSource> _cancellationTokenSources;

        public CancellationCollection() => _cancellationTokenSources = new Dictionary<StatenodeId, CancellationTokenSource>();

        public CancellationToken GetToken(StatenodeId statenodeId)
        {
            if (!_cancellationTokenSources.ContainsKey(statenodeId))
                _cancellationTokenSources.Add(statenodeId, new CancellationTokenSource());
            return _cancellationTokenSources[statenodeId].Token;
        }

        public void TryCancel(StatenodeId statenodeId)
        {
            if (!_cancellationTokenSources.ContainsKey(statenodeId)) return;
            _cancellationTokenSources[statenodeId].Cancel();
            _cancellationTokenSources.Remove(statenodeId);
        }

        public void CancelAll()
        {
            foreach (var statenodeId in _cancellationTokenSources.Keys)
                _cancellationTokenSources[statenodeId].Cancel();
            _cancellationTokenSources.Clear();
        }
    }

    public class RunningStatechart<TContext> where TContext : IContext<TContext>
    {
        private readonly ExecutableStatechart<TContext> _statechart;
        private State<TContext> _currentState;
        private readonly TaskCompletionSource<object> _taskSource;
        private readonly CancellationCollection _cancellation;
        private bool _isFinished;

        // TODO: config object
        private readonly InterpreterOptions _options;

        public event Action<Macrostep<TContext>> OnMacroStep;
        public event Action<IEvent> OnEventOccurred;

        internal RunningStatechart(ExecutableStatechart<TContext> statechart, CancellationToken cancellationToken, InterpreterOptions options)
        {
            _statechart = statechart;
            _taskSource = new TaskCompletionSource<object>();
            _cancellation = new CancellationCollection();

            _statechart.Done = (context, eventData) => _isFinished = true;
            cancellationToken.Register(CompleteCancelled);
            // TODO: register failed ActionBlock that was not handled as cancelled

            _options = options;

            _isFinished = false;
        }

        public IEnumerable<string> NextEvents => _statechart
            .GetActiveStatenodes(_currentState.StateConfiguration)
            .SelectMany(statenode => statenode.GetTransitions())
            .Select(transition => transition.Event is ISendableEvent @event ? @event.Name : null)
            .WhereNotNull();

        public Task Start() => Start(
            StateConfiguration.Empty(),
            _statechart.InitialContext,
            () => HandleEvent(new InitializeStatechartEvent()));

        private Task Start(
            StateConfiguration stateConfiguration,
            TContext context,
            System.Action initAction)
        {
            _currentState = new State<TContext>(stateConfiguration, context.CopyDeep());
            StartServices(_statechart.GetActiveStatenodes(stateConfiguration));
            initAction();
            return _taskSource.Task;
        }

        private void HandleEvent(IEvent @event)
        {
            OnEventOccurred?.Invoke(@event);
            var events = EventQueue.WithEvent(@event);

            while (events.IsNotEmpty && !_isFinished)
            {
                var result = Resolver.ResolveMacrostep(_statechart, _currentState, events.Dequeue(), (ExecuteAction, StopExitedStatenodes));
                result.Switch(macrostep =>
                {
                    _currentState = macrostep.State;
                    foreach (var queuedEvent in macrostep.QueuedEvents) events.Enqueue(new NextStep(queuedEvent));
                    OnMacroStep?.Invoke(macrostep);
                    StartServices(macrostep.GetEnteredStateNodes());
                }, CompleteErrored);
            }

            if(_isFinished) CompleteSuccessfully();
        }

        public void Send(ISendableEvent @event) => HandleEvent(@event);

        private void StopExitedStatenodes(IEnumerable<Statenode> statenodes)
        {
            foreach (var statenode in statenodes)
                _cancellation.TryCancel(statenode.Id);
        }
        private Option<OneOf<CurrentStep, NextStep>> ExecuteAction(Action action, object context, object eventData)
        {
            var result = Option.None<OneOf<CurrentStep, NextStep>>();
            action.Switch(
                send => result = ((OneOf<CurrentStep, NextStep>)new NextStep(new NamedEvent(send.EventName))).ToOption(),
                raise => result = ((OneOf<CurrentStep, NextStep>)new CurrentStep(new NamedEvent(raise.EventName))).ToOption(),
                log => _options.Log(log.Message(context, eventData)),
                assign => assign.Mutation(context, eventData),
                sideEffect => sideEffect.Function(context, eventData),
                async startDelayed =>
                {
                    try
                    {
                        await _options.Wait(startDelayed.Delay, _cancellation.GetToken(startDelayed.StatenodeId));
                        HandleEvent(new DelayedEvent(startDelayed.StatenodeId, startDelayed.Delay));
                    }
                    catch (OperationCanceledException) { }
                });
            return result;
        }
        private async void StartServices(IEnumerable<Statenode> statenodes)
        {
            var entered = statenodes
                .Select(statenode => (statenode, services: statenode.Match(final => Enumerable.Empty<Service>(), nonFinal => nonFinal.Services)))
                .Where(entry => entry.services != null && entry.services.Any());

            var tasks = new List<Task>();
            foreach (var (stateNode, services) in entered)
            {
                tasks.AddRange(services.Select(async service =>
                {
                    try
                    {
                        var result = await service.Invoke(_cancellation.GetToken(stateNode.Id));
                        HandleEvent(new ServiceSuccessEvent(service.Id, result));
                    }
                    catch (Exception e)
                    {
                        HandleEvent(new ServiceErrorEvent(service.Id, e));
                    }
                }));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        private void Complete(System.Action finalAction)
        {
            _cancellation.CancelAll();
            finalAction();
        }
        private void CompleteSuccessfully() => Complete(() => _taskSource.TrySetResult(null));
        private void CompleteCancelled() => Complete(() => _taskSource.TrySetCanceled());
        private void CompleteErrored(Exception exception) => Complete(() => _taskSource.TrySetException(exception));
    }
}
