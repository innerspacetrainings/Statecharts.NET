using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;

namespace Statecharts.NET.XState
{
    public static class XStateExtensions
    {
        public static string AsXStateVisualizerV4Definition<TContext>(
            this StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>, IXStateSerializable
            => $"const machine = Machine({statechartDefinition.AsXStateV4Definition()});";
        public static string AsXStateVisualizerV5Definition<TContext>(
            this StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>, IXStateSerializable
            => throw new NotImplementedException();

        private static string AsXStateV4Definition<TContext>(
            this StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>, IXStateSerializable
            => ObjectValue(
                    ("id", statechartDefinition.Id),
                    ("context", statechartDefinition.InitialContext.AsJSObject())
                ).With(statechartDefinition.RootStateNode.AsJSProperty(statechartDefinition).Value as ObjectValue).AsString(); // this cast is necessary because of the way xstate merges the top-level state node with the machine definition

        private static JSProperty AsJSProperty<TContext>(
            this StatenodeDefinition stateNodeDefinition,
            StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>
            => stateNodeDefinition.CataFold<JSProperty>( // TODO: add actions, transitions
                atomic => ($"\"{atomic.Name}\"", atomic.Properties(statechartDefinition)),
                final => ($"\"{final.Name}\"", final.Properties(statechartDefinition).With(("type", "final"))),
                (compound, subDefinitions) => ($"\"{compound.Name}\"", compound.Properties(statechartDefinition).With(
                    ("initial", compound.InitialTransition.Target.StatenodeName),
                    ("states", subDefinitions))),
                (orthogonal, subDefinitions) => ($"\"{orthogonal.Name}\"", orthogonal.Properties(statechartDefinition).With(
                    ("type", "parallel"),
                    ("states", subDefinitions))));

        private static ObjectValue Properties<TContext>(
            this StatenodeDefinition definition,
            StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>
        {
            var properties = new List<JSProperty>();

            ArrayValue Targets(IEnumerable<Target> targets)
                => ArrayValue(targets.Select(target => target.Serialize(definition.Name)));
            string Event(IEventDefinition @event)
            {
                switch (@event)
                {
                    case ISendableEvent named: return $"\"{named.Name}\"";
                    case ImmediateEventDefinition _: return "\"\"";
                    case DelayedEventDefinition delayed: return ((int)delayed.Delay.TotalMilliseconds).ToString();
                    default: throw new Exception("oh shit");
                }
            }
            JSProperty Unguarded(string @event, IEnumerable<Target> targets, Option<JSProperty> actions)
                => (@event, actions.Match(
                    actionsProperty => ObjectValue(("target", Targets(targets)), actionsProperty),
                    () => ObjectValue(("target", Targets(targets)))));

            JSProperty Guarded(string @event, IEnumerable<Target> targets, Option<JSProperty> actions)
                => (@event, actions.Match(
                    actionsProperty => ObjectValue(("target", Targets(targets)), ("cond", SimpleValue("() => false", true)), actionsProperty),
                    () => ObjectValue(("target", Targets(targets)), ("cond", SimpleValue("() => false", true)))));

            // transitions
            var transitions = definition.Match(
                atomic => atomic.Transitions,
                final => Enumerable.Empty<TransitionDefinition>(),
                compound => compound.Transitions,
                orthogonal => orthogonal.Transitions)
                .Where(transition => transition.Match(_ => true, unguarded => !IsDelayed(unguarded.Event), unguarded => !IsDelayed(unguarded.Event), unguarded => !IsDelayed(unguarded.Event), guarded => !IsDelayed(guarded.Event), guarded => !IsDelayed(guarded.Event), guarded => !IsDelayed(guarded.Event)))
                .Select(transition => transition
                    .Match(
                        forbidden => (forbidden.Event.Name, SimpleValue("undefined", true)),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions.Serialize("actions")),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions.Serialize("actions")),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions.Serialize("actions")))).ToList();
            if(transitions.Any()) properties.Add(("on", ObjectValue(transitions)));

            bool IsDelayed(IEventDefinition eventDefinition) => eventDefinition is DelayedEventDefinition;
            var delayedTransitions = definition.Match(
                atomic => atomic.Transitions,
                final => Enumerable.Empty<TransitionDefinition>(),
                compound => compound.Transitions,
                orthogonal => orthogonal.Transitions)
                .Where(transition => transition.Match(_ => false, unguarded => IsDelayed(unguarded.Event), unguarded => IsDelayed(unguarded.Event), unguarded => IsDelayed(unguarded.Event), guarded => IsDelayed(guarded.Event), guarded => IsDelayed(guarded.Event), guarded => IsDelayed(guarded.Event)))
                .Select(transition => transition
                    .Match(
                        forbidden => (forbidden.Event.Name, SimpleValue("undefined", true)),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions.Serialize("actions")),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions.Serialize("actions")),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions.Serialize("actions")))).ToList();
            if (delayedTransitions.Any()) properties.Add(("after", ObjectValue(delayedTransitions)));

            JSProperty MapDoneTransition(DoneTransitionDefinition transition) =>
                transition.Guard.Match(
                    guarded => Guarded("onDone", transition.Targets, transition.Actions.Serialize("actions")),
                    () => Unguarded("onDone", transition.Targets, transition.Actions.Serialize("actions")));

            // DoneTransition
            var onDoneTransition = definition.Match(
                _ => Option.None<JSProperty>(), 
                _ => Option.None<JSProperty>(),
                compound => compound.DoneTransition.Map(MapDoneTransition), 
                orthogonal => orthogonal.DoneTransition.Map(MapDoneTransition));
            onDoneTransition.SwitchSome(properties.Add);

            // actions
            definition.EntryActions.Serialize("entry").SwitchSome(properties.Add);
            definition.ExitActions.Serialize("exit").SwitchSome(properties.Add);

            IEnumerable<ObjectValue> MapServices(NonFinalStatenodeDefinition stateNode) =>
                stateNode.Services.Select(service =>
                {
                    var idProperty = service.Id.Map<JSProperty>(id => ("id", id));
                    var onErrorProperty = service.OnErrorTransition.Map(transition => transition.Match(
                        unguarded => Unguarded("onError", unguarded.Targets, unguarded.Actions.Serialize("actions")),
                        unguarded => Unguarded("onError", unguarded.Targets, unguarded.Actions.Serialize("actions"))));
                    var onSuccessProperty = service.Match(
                        _ => Option.None<JSProperty>(),
                        taskService => taskService.OnSuccessDefinition.Map(transition => transition.Match(
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions.Serialize("actions")),
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions.Serialize("actions")))),
                        taskDataService => taskDataService.OnSuccessDefinition.Map(transition => transition.Match(
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions.Serialize("actions")),
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions.Serialize("actions")),
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions.Serialize("actions")))));

                    var serviceProperties = new[] {idProperty, onErrorProperty, onSuccessProperty}
                        .Where(property => property.HasValue).Select(property => property.ValueOr(default(JSProperty)));

                    return ObjectValue(serviceProperties);
                });

            // services
            var services = definition.Match(MapServices, _ => Enumerable.Empty<ObjectValue>(), MapServices, MapServices).ToList(); // TODO: probably build Match for NonFinalStatenode
            if (services.Any()) properties.Add(("invoke", services));

            return properties;
        }
    }

    internal static class Extensions
    {
        internal static SimpleValue Serialize(
            this Target target,
            string sourceStatenodeName)
            => SimpleValue(target.Match(
                absolute => $"#{string.Join(".", absolute.Id.Values)}",
                sibling => $"{sibling.StatenodeName}",
                child => $".{child.StatenodeName}",
                self => sourceStatenodeName,
                uniqueIdentifier => $"#{uniqueIdentifier.Id}"));

        internal static Option<JSProperty> Serialize(
            this IEnumerable<ActionDefinition> actions,
            string key)
        {
            IEnumerable<JSValue> GetVisualizableActions()
                => actions?.Select(action => action.Match(
                                send => $"send(\"{send.Event.Name}\")",
                                raise => $"raise(\"{raise.Event.Name}\")",
                                log => $"log({log.Label})",
                                _ => null,
                                _ => null))
                    .WhereNotNull()
                    .Select(stringRepresantion => SimpleValue(stringRepresantion, true));
            var actionsCount = actions?.Count() ?? 0;
            return actionsCount == 0
                ? Option.None<JSProperty>()
                : JSProperty(key,
                    ArrayValue(SimpleValue(actionsCount == 1 ? "1 Action" : $"{actionsCount} Actions")
                        .Append(GetVisualizableActions()).WhereNotNull())).ToOption();
        }

        internal static Option<JSProperty> Serialize(
            this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions,
            string key) =>
            Serialize(actions?.Select(action => action.Match(Functions.Identity, _ => null)).WhereNotNull(), key);

        internal static Option<JSProperty> Serialize(
            this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> actions,
            string key) =>
            Serialize(actions?.Select(action => action.Match(Functions.Identity, _ => null, _ => null)).WhereNotNull(), key);
    }
}