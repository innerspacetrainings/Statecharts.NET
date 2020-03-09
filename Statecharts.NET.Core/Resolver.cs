﻿using System;
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
            this (Statenode first, Statenode second) pair)
        {
            var par1 = pair.first.GetParents();
            var par2 = pair.second.GetParents();

            var intersection = Enumerable.Intersect(par1, par2).FirstOrDefault();
            return intersection.ToOption();
        }

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
                    .Where(transition => transition.IsEnabled(context, @event.Data))
                    .FirstOrDefault(transition => @event.Equals(transition.Event)).ToOption();

             return statechart
                .GetActiveStatenodes(stateConfiguration)
                .Aggregate(
                     (excluded: Enumerable.Empty<Statenode>(), transitions: Enumerable.Empty<Transition>()),
                     (tuple, current) =>
                         tuple.excluded.Contains(current)
                             ? tuple
                             : FirstMatchingTransition(current).Match(
                                 transition => (
                                     excluded: tuple.excluded.Concat(current.GetParents()),
                                     transitions: tuple.transitions.Append(transition)),
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
            var initialized = enabled.Where(transition => !(@event is InitializeEvent && transition.Targets.All(stateConfiguration.Contains)));
            var result = initialized.SelectMany(transition =>
                transition.Targets.Select(target =>
                {
                    var test = (transition.Source, target).LeastCommonAncestor();
                    var lca = test.Value; // TODO: this might fuck up
                    var lastBeforeLeastCommonAncestor = transition.Source.OneBeneath(lca);
                    var exited = lastBeforeLeastCommonAncestor
                        .Append(lastBeforeLeastCommonAncestor.GetDescendants()).Where(stateConfiguration.Contains);
                    var entered = target.Append(target.AncestorsUntil(lca).Reverse());

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
            (Func<Model.Action, object, object, Option<OneOf<CurrentStep, NextStep>>> executeAction, Action<IEnumerable<Statenode>> stopServices) functions)
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
            functions.stopServices(microStep.ExitedStatenodes);
            ExecuteTransitionActions();
            ExecuteEntryActions();

            return events;
        }
        
        private static Option<OneOf<CurrentStep, NextStep>> SideffectFreeExecuteAction(Model.Action action, object context, object eventData) =>
            action.Match(
                send => ((OneOf<CurrentStep, NextStep>)new NextStep(new NamedEvent(send.EventName))).ToOption(),
                raise => ((OneOf<CurrentStep, NextStep>)new CurrentStep(new NamedEvent(raise.EventName))).ToOption(),
                log => Option.None<OneOf<CurrentStep, NextStep>>(),
                assign =>
                {
                    assign.Mutation(context, eventData);
                    return Option.None<OneOf<CurrentStep, NextStep>>();
                },
                sideEffect => Option.None<OneOf<CurrentStep, NextStep>>());

        internal static Macrostep<TContext> ResolveMacrostep<TContext>(
            ExecutableStatechart<TContext> statechart,
            State<TContext> sourceState,
            IEvent macrostepEvent,
            (Func<Model.Action, object, object, Option<OneOf<CurrentStep, NextStep>>> executeAction, Action<IEnumerable<Statenode>> stopServices) functions)
            where TContext : IContext<TContext>
        {
            var microsteps = new List<Microstep>();
            var events = EventQueue.WithEvent(macrostepEvent);
            var stateConfiguration = sourceState.StateConfiguration;
            var context = sourceState.Context.CopyDeep();

            IReadOnlyCollection<Microstep> ResolveMicroSteps(IEvent @event) =>
                ResolveSingleEvent(statechart, context, stateConfiguration, @event).ToList().AsReadOnly();
            void Execute(IEnumerable<Microstep> microSteps)
            {
                foreach (var step in microSteps)
                    foreach (var @event in Apply(step, context, functions))
                        @event.Switch(events.Enqueue, events.Enqueue);
            }
            void StabilizeIfNecessary(IReadOnlyCollection<Microstep> microSteps)
            {
                if (microSteps.GetEnteredStateNodes().Any()) events.EnqueueStabilizationEvent();
            }
            void EnqueueDoneEvents(IReadOnlyCollection<Microstep> microSteps)
            {
                // TODO: check state finishing and enqueue
            }
            void UpdateStateConfiguration(IReadOnlyCollection<Microstep> microSteps) =>
                stateConfiguration = stateConfiguration.Without(microSteps.GetExitedStateNodes()).With(microSteps.GetEnteredStateNodes());
            void AddMicrosteps(IEnumerable<Microstep> steps) => microsteps.AddRange(steps);

            while (events.IsNotEmpty && events.NextIsInternal)
            {
                var steps = ResolveMicroSteps(events.Dequeue());
                Execute(steps);
                StabilizeIfNecessary(steps);
                EnqueueDoneEvents(steps);
                UpdateStateConfiguration(steps);
                AddMicrosteps(steps);
            }

            return new Macrostep<TContext>(new State<TContext>(stateConfiguration, context), events.NextStepEvents, microsteps);
        }


        public static State<TContext> ResolveNextState<TContext>(
            Model.ExecutableStatechart<TContext> statechart,
            State<TContext> state,
            ISendableEvent @event)
            where TContext : IContext<TContext> =>
            ResolveMacrostep(statechart, state, @event, (SideffectFreeExecuteAction, Functions.NoOp)).State;

        public static State<TContext> ResolveInitialState<TContext>(
            ExecutableStatechart<TContext> statechart) where TContext : IContext<TContext> =>
            ResolveMacrostep(
                statechart,
                new State<TContext>(new StateConfiguration(statechart.Rootnode.Yield()), statechart.InitialContext.CopyDeep()),
                new InitializeEvent(),
                (SideffectFreeExecuteAction, Functions.NoOp)).State;
    }
}
