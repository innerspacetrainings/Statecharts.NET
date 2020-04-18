using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET
{
    internal static class StatenodeExtensions
    {
        internal static IEnumerable<Statenode> GetParents(
            this Statenode statenode) =>
            statenode.Parent
                .Map(parent => parent.Append(parent.GetParents()))
                .ValueOr(Enumerable.Empty<Statenode>());
        internal static Option<Statenode> LeastCommonAncestor(
            this (Statenode first, Statenode second) pair) =>
            Enumerable.Intersect(pair.first.GetParents(), pair.second.GetParents()).FirstOrDefault().ToOption();

        internal static Statenode OneBeneath(
            this Statenode statenode, Statenode beneath)
            => statenode.Append(statenode.GetParents())
                .FirstOrDefault(parentStateNode => parentStateNode.Parent.Equals(beneath.ToOption()));
        internal static IEnumerable<Statenode> GetDescendants(
            this Statenode stateNode)
            => stateNode.CataFold(
                atomic => atomic.Yield() as IEnumerable<Statenode>,
                final => final.Yield() as IEnumerable<Statenode>,
                (compound, subStates) =>
                    compound.Append(subStates.SelectMany(a => a)),
                (orthogonal, subStates) =>
                    orthogonal.Append(subStates.SelectMany(a => a))).Except(stateNode.Yield());
        internal static IEnumerable<Statenode> AncestorsUntil(
            this Statenode stateNode, Statenode until)
            => stateNode.GetParents().TakeWhile(parentStateNode => !parentStateNode.Equals(until));

        internal static IEnumerable<Transition> GetTransitions(this Statenode statenode)
            => statenode.Match(final => Enumerable.Empty<Transition>(), nonFinal => nonFinal.Transitions);
    }

    internal static class MicrostepExtensions
    {
        internal static IEnumerable<Statenode> GetEnteredStateNodes(this IEnumerable<Microstep> microSteps)
            => microSteps.SelectMany(step => step.EnteredStatenodes);
        internal static IEnumerable<Statenode> GetExitedStateNodes(this IEnumerable<Microstep> microSteps)
            => microSteps.SelectMany(step => step.ExitedStatenodes);
    }

    public class Resolver
    {
        private static IEnumerable<Transition> SelectTransitions<TContext>(
            Model.ExecutableStatechart<TContext> statechart,
            object context,
            StateConfiguration stateConfiguration,
            IEvent @event)
            where TContext : IContext<TContext>
        {
            Option<Transition> FirstMatchingTransition(Statenode node) =>
                node.GetTransitions()
                    .Where(transition => @event.Equals(transition.Event))
                    .FirstOrDefault(transition => transition.IsEnabled(context, @event.Data)).ToOption();

            return statechart
                .GetActiveStatenodes(stateConfiguration)
                .OrderByDescending(statenode => statenode.Depth)
                .Aggregate(
                    (excluded: Enumerable.Empty<Statenode>(), transitions: Enumerable.Empty<Transition>()),
                     (tuple, current) =>
                         tuple.excluded.Contains(current)
                             ? tuple
                             : FirstMatchingTransition(current).Match(
                                 transition => (
                                     excluded: tuple.excluded.Concat(current.GetParents()),
                                     transitions: transition.IsForbidden
                                         ? tuple.transitions
                                         : tuple.transitions.Append(transition)),
                                 () => tuple))
                .transitions;
        }

        private static IEnumerable<Microstep> ComputeMicrosteps(
            IEnumerable<Transition> transitions,
            object context,
            StateConfiguration stateConfiguration,
            IEvent @event)
        {
            var enabled = transitions.Where(transition => transition.IsEnabled(context, @event.Data));
            var result = enabled.SelectMany(transition =>
                transition.Targets.Select(target =>
                {
                    var test = (transition.Source, target).LeastCommonAncestor();
                    var lcca = test.Value;
                    var lastBeforeLeastCompoundCommonAncestor = transition.Source.OneBeneath(lcca);
                    var isChildTransition = target.GetParents().Contains(transition.Source);
                    var exited = isChildTransition
                        ? transition.Source.GetDescendants().Where(stateConfiguration.Contains)
                        : lastBeforeLeastCompoundCommonAncestor.Append(lastBeforeLeastCompoundCommonAncestor.GetDescendants()).Where(stateConfiguration.Contains);
                    var entered = isChildTransition
                        ? target.Append(target.AncestorsUntil(lastBeforeLeastCompoundCommonAncestor).Reverse())
                        : target.Append(target.AncestorsUntil(lcca).Reverse());

                    return new Microstep(@event, transition, entered, exited);
                }));

            return result.ToList();
        }

        [Pure]
        private static IEnumerable<Microstep> ResolveSingleEvent<TContext>(
            Model.ExecutableStatechart<TContext> statechart,
            object context,
            StateConfiguration stateConfiguration,
            IEvent @event)
            where TContext : IContext<TContext>
        {
            // TODO: this sucks
            var transitions = SelectTransitions(statechart, context, stateConfiguration, @event);
            var microsteps = ComputeMicrosteps(transitions, context, stateConfiguration, @event);
            return microsteps;
        }

        private static EventList Apply(
            Microstep microStep,
            object context,
            (Func<Model.Action, object, object, Option<OneOf<CurrentStep, NextStep>>> executeAction, Action<IEnumerable<Statenode>> stopExitedStatenodes) functions)
        {
            EventList ExecuteActionBlock(Actionblock actions)
            {
                var result = EventList.Empty();
                try
                {
                    foreach (var action in actions)
                        functions
                            .executeAction(action, context, microStep.Event.Data)
                            .SwitchSome(result.Enqueue);
                }
                catch (Exception e)
                {
                    result.EnqueueOnCurrentStep(new ExecutionErrorEvent(e));
                }
                return result;
            }

            var events = EventList.Empty();

            EventList ExecuteMultiple(IEnumerable<Actionblock> actionBlocks) =>
                EventList.From(actionBlocks.SelectMany(ExecuteActionBlock).ToList());
            void ExecuteExitActions() => events.AddRange(ExecuteMultiple(microStep.ExitedActionBlocks));
            void ExecuteTransitionActions() => events.AddRange(ExecuteActionBlock(microStep.TransitionActionBlock));
            void ExecuteEntryActions() => events.AddRange(ExecuteMultiple(microStep.EnteredActionBlocks));

            ExecuteExitActions();
            functions.stopExitedStatenodes(microStep.ExitedStatenodes);
            ExecuteTransitionActions();
            if(!(microStep.Event is DoneEvent && !microStep.Transition.Source.Parent.HasValue)) // TODO: remove this if after the OnRootDoneTransition is internal
                ExecuteEntryActions();

            return events;
        }
        
        // TODO: return FailableOption
        private static Option<OneOf<CurrentStep, NextStep>> SideffectFreeExecuteAction(Model.Action action, object context, object eventData) =>
            action.Match(
                send => ((OneOf<CurrentStep, NextStep>)new NextStep(send.Event)).ToOption(),
                raise => ((OneOf<CurrentStep, NextStep>)new CurrentStep(new NamedEvent(raise.EventName))).ToOption(),
                log => Option.None<OneOf<CurrentStep, NextStep>>(),
                assign =>
                {
                    assign.Mutation(context, eventData);
                    return Option.None<OneOf<CurrentStep, NextStep>>();
                },
                sideEffect => Option.None<OneOf<CurrentStep, NextStep>>(),
                startDelayedTransition => Option.None<OneOf<CurrentStep, NextStep>>());

        internal static OneOf<Macrostep<TContext>, Exception> ResolveMacrostep<TContext>(
            ExecutableStatechart<TContext> statechart,
            State<TContext> sourceState,
            IEvent macrostepEvent,
            (Func<Model.Action, object, object, Option<OneOf<CurrentStep, NextStep>>> executeAction, Action<IEnumerable<Statenode>> stopExitedStatenodes) functions)
            where TContext : IContext<TContext>
        {
            var microsteps = new List<Microstep>();
            var events = EventQueue.WithEvent(macrostepEvent);
            var stateConfiguration = sourceState.StateConfiguration;
            var context = sourceState.Context.CopyDeep();

            IReadOnlyCollection<Microstep> ResolveMicroSteps(IEvent @event) =>
                (@event is InitializeStatechartEvent
                    ? Microstep.InitializeStatechart(statechart.Rootnode).Yield()
                    : ResolveSingleEvent(statechart, context, stateConfiguration, @event)).ToList().AsReadOnly();
            void Execute(IEnumerable<Microstep> microSteps)
            {
                foreach (var step in microSteps)
                    foreach (var @event in Apply(step, context, functions))
                        @event.Switch(events.Enqueue, events.Enqueue);
            }
            void StabilizeIfNecessary(IReadOnlyCollection<Microstep> microSteps)
            {
                foreach (var enteredStateNode in microSteps.GetEnteredStateNodes())
                    enteredStateNode.Switch(
                        Functions.NoOp,
                        Functions.NoOp,
                        compound => events.EnqueueStabilizationEvent(compound.Id),
                        orthogonal => events.EnqueueStabilizationEvent(orthogonal.Id));
            }
            void EnqueueImmediateEvent(IReadOnlyCollection<Microstep> steps)
            {
                if(steps.Any()) events.EnqueueImmediateEvent();
            }
            void EnqueueDoneEvents()
            {
                var doneStatenodes = statechart
                    .GetActiveStatenodes(stateConfiguration)
                    .OrderByDescending(statenode => statenode.Depth)
                    .ThenBy(statenode => statenode.DocumentIndex)
                    .Aggregate(Enumerable.Empty<Statenode>(),
                    (done, statenode) => statenode.Match(
                        atomic => done, 
                        done.Append,
                        compound => compound.Statenodes.Intersect(done.Where(node => node.Match(final => true, nonFinal => false))).Any() ? done.Append(compound) : done,
                        orthogonal => orthogonal.Statenodes.All(done.Contains) ? done.Append(orthogonal) : done));

                foreach (var doneStatenode in doneStatenodes)
                    doneStatenode.Switch(
                        Functions.NoOp,
                        Functions.NoOp,
                        compound => events.EnqueueDoneEvent(compound.Id),
                        orthogonal => events.EnqueueDoneEvent(orthogonal.Id));
            }
            void UpdateStateConfiguration(IReadOnlyCollection<Microstep> microSteps) =>
                stateConfiguration = stateConfiguration.Without(microSteps.GetExitedStateNodes()).With(microSteps.GetEnteredStateNodes());
            void AddMicrosteps(IEnumerable<Microstep> steps) => microsteps.AddRange(steps);

            while (events.IsNotEmpty && events.NextIsInternal)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"events: {events}");
                Console.ResetColor();

                var @event = events.Dequeue();
                var steps = ResolveMicroSteps(@event);
                var isRootDoneEvent = @event.Equals(new DoneEvent(statechart.Rootnode.Id));
                    switch (@event) // TODO, improve this
                {
                    case ExecutionErrorEvent executionErrorEvent when !steps.Any():
                        return executionErrorEvent.Exception;
                    case ServiceErrorEvent serviceErrorEvent when !steps.Any():
                        return serviceErrorEvent.Exception;
                }

                Execute(steps);
                if (!isRootDoneEvent)
                {
                    StabilizeIfNecessary(steps);
                    EnqueueImmediateEvent(steps);
                }
                UpdateStateConfiguration(steps);
                if(!(@event is DoneEvent)) EnqueueDoneEvents(); // TODO: sometimes enqueuing happens multiple times
                AddMicrosteps(steps);
            }

            return new Macrostep<TContext>(new State<TContext>(stateConfiguration, context), events.NextStepEvents, microsteps);
        }


        public static OneOf<State<TContext>, Exception> ResolveNextState<TContext>(
            Model.ExecutableStatechart<TContext> statechart,
            State<TContext> state,
            ISendableEvent @event)
            where TContext : IContext<TContext> =>
            ResolveMacrostep(statechart, state, @event, (SideffectFreeExecuteAction, Functions.NoOp))
                .Match<OneOf<State<TContext>, Exception>>(macrostep => macrostep.State, exception => exception);

        public static OneOf<State<TContext>, Exception> ResolveInitialState<TContext>(
            ExecutableStatechart<TContext> statechart) where TContext : IContext<TContext> =>
            ResolveMacrostep(
                statechart,
                State<TContext>.Initial(statechart.InitialContext.CopyDeep()),
                new InitializeStatechartEvent(),
                (SideffectFreeExecuteAction, Functions.NoOp))
                .Match<OneOf<State<TContext>, Exception>>(macrostep => macrostep.State, exception => exception);
    }
}
