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
                atomic => (atomic.Name, atomic.Properties(statechartDefinition)),
                final => (final.Name, final.Properties(statechartDefinition).With(("type", "final"))),
                (compound, subDefinitions) => (compound.Name, compound.Properties(statechartDefinition).With(
                    ("initial", compound.InitialTransition.Target.StatenodeName),
                    ("states", subDefinitions))),
                (orthogonal, subDefinitions) => (orthogonal.Name, orthogonal.Properties(statechartDefinition).With(
                    ("type", "parallel"),
                    ("states", subDefinitions))));

        private static ObjectValue Properties<TContext>(
            this StatenodeDefinition definition,
            StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>
        {
            var properties = new List<JSProperty>();

            ArrayValue Targets(IEnumerable<Target> targets)
                => ArrayValue(targets.Select(target => target.Serialize(definition.Name, statechartDefinition)));
            string Event(IEventDefinition @event)
            {
                switch (@event)
                {
                    case ISendableEvent named: return named.Name;
                    case ImmediateEventDefinition _: return "\"\"";
                    case DelayedEventDefinition _: return "after";
                    default: throw new Exception("oh shit");
                }
            }
            JSProperty Unguarded(string @event, IEnumerable<Target> targets, int? actionsCount)
                => (@event, actionsCount == 0
                    ? ObjectValue(("target", Targets(targets)))
                    : ObjectValue(("target", Targets(targets)), ("actions", SimpleValue(actionsCount == 1 ? "1 Action" : $"{actionsCount} Actions"))));
            JSProperty Guarded(string @event, IEnumerable<Target> targets, int actionsCount)
                => (@event, actionsCount == 0
                    ? ObjectValue(("target", Targets(targets)), ("cond", SimpleValue("() => false", true)))
                    : ObjectValue(("target", Targets(targets)), ("cond", SimpleValue("() => false", true)), ("actions", SimpleValue(actionsCount == 1 ? "1 Action" : $"{actionsCount} Actions"))));

            // transitions
            var transitions = definition.Match(
                atomic => atomic.Transitions,
                final => Enumerable.Empty<TransitionDefinition>(),
                compound => compound.Transitions,
                orthogonal => orthogonal.Transitions).Select(
                transition => transition
                    .Match(
                        forbidden => (forbidden.Event.Name, SimpleValue("undefined", true)),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                        unguarded => Unguarded(Event(unguarded.Event), unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions?.Count() ?? 0),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions?.Count() ?? 0),
                        guarded => Guarded(Event(guarded.Event), guarded.Targets, guarded.Actions?.Count() ?? 0))).ToList();
            if(transitions.Any()) properties.Add(("on", ObjectValue(transitions)));

            JSProperty MapDoneTransition(DoneTransitionDefinition transition) =>
                transition.Guard.Match(
                    guarded => Guarded("onDone", transition.Targets, transition.Actions?.Count() ?? 0),
                    () => Unguarded("onDone", transition.Targets, transition.Actions?.Count() ?? 0));

            // DoneTransition
            var onDoneTransition = definition.Match(
                _ => Option.None<JSProperty>(), 
                _ => Option.None<JSProperty>(),
                compound => compound.DoneTransition.Map(MapDoneTransition), 
                orthogonal => orthogonal.DoneTransition.Map(MapDoneTransition));
            onDoneTransition.SwitchSome(properties.Add);

            // actions
            var entryCount = definition.EntryActions?.Count() ?? 0;
            var exitCount = definition.ExitActions?.Count() ?? 0;
            if(entryCount > 0) properties.Add(("entry", entryCount == 1 ? "1 Action" : $"{entryCount} Actions"));
            if(exitCount > 0) properties.Add(("exit", exitCount == 1 ? "1 Action" : $"{exitCount} Actions"));

            IEnumerable<ObjectValue> MapServices(NonFinalStatenodeDefinition stateNode) =>
                stateNode.Services.Select(service =>
                {
                    var idProperty = service.Id.Map<JSProperty>(id => ("id", id));
                    var onErrorProperty = service.OnErrorTransition.Map(transition => transition.Match(
                        unguarded => Unguarded("onError", unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                        unguarded => Unguarded("onError", unguarded.Targets, unguarded.Actions?.Count() ?? 0)));
                    var onSuccessProperty = service.Match(
                        _ => Option.None<JSProperty>(),
                        taskService => taskService.OnSuccessDefinition.Map(transition => transition.Match(
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions?.Count() ?? 0))),
                        taskDataService => taskDataService.OnSuccessDefinition.Map(transition => transition.Match(
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions?.Count() ?? 0),
                            unguarded => Unguarded("onDone", unguarded.Targets, unguarded.Actions?.Count() ?? 0))));

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
        internal static SimpleValue Serialize<TContext>(
            this Target target,
            string sourceStatenodeName,
            StatechartDefinition<TContext> statechartDefinition)
            where TContext : IContext<TContext>
            => SimpleValue(target.Match(
                absolute => $"#{statechartDefinition.Id}." + string.Join(".", absolute.Id.Values),
                sibling => $"{sibling.StatenodeName}",
                child => $".{child.StatenodeName}",
                self => sourceStatenodeName));
    }
}