using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;

namespace Statecharts.NET.XState
{
    public static class XStateExtensions
    {
        public static string AsXStateVisualizerV4Definition<TContext>(
            this Definition.Statechart<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>, IXStateSerializable
            => $"const machine = Machine({statechartDefinition.AsXStateV4Definition()});";
        public static string AsXStateVisualizerV5Definition<TContext>(
            this Definition.Statechart<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>, IXStateSerializable
            => throw new NotImplementedException();

        private static string AsXStateV4Definition<TContext>(
            this Definition.Statechart<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>, IXStateSerializable
            => ObjectValue(
                    ("id", statechartDefinition.Id),
                    ("context", statechartDefinition.InitialContext.AsJSObject())
                ).With(statechartDefinition.RootStateNode.AsJSProperty(statechartDefinition).Value as ObjectValue).AsString(); // this cast is necessary because of the way xstate merges the top-level state node with the machine definition

        private static JSProperty AsJSProperty<TContext>(
            this Definition.StateNode stateNodeDefinition,
            Definition.Statechart<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>
            => stateNodeDefinition.CataFold<JSProperty>( // TODO: add actions, transitions
                atomic => (atomic.Name, atomic.Properties(statechartDefinition)),
                final => (final.Name, final.Properties(statechartDefinition).With(("type", "final"))),
                (compound, subDefinitions) => (compound.Name, compound.Properties(statechartDefinition).With(
                    ("initial", compound.InitialTransition.Target.Key.StateName),
                    ("states", subDefinitions))),
                (orthogonal, subDefinitions) => (orthogonal.Name, orthogonal.Properties(statechartDefinition).With(
                    ("type", "parallel"),
                    ("states", subDefinitions))));

        private static ObjectValue Properties<TContext>(
            this Definition.StateNode definition,
            Definition.Statechart<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>
        {
            var properties = new List<JSProperty>();

            ArrayValue Targets(IEnumerable<Model.Target> targets)
                => ArrayValue(targets.Select(target => target.Serialize(statechartDefinition)));
            string Event(Model.Event @event) =>
                @event.Match(custom => custom.EventName, immediate => "\"\"", delayed => "after");
            JSProperty Unguarded(OneOf<string, Model.Event> @event, IEnumerable<Model.Target> targets)
                => (@event.Match(e => e, Event), ObjectValue(("target", Targets(targets))));
            JSProperty Guarded(OneOf<string, Model.Event> @event, IEnumerable<Model.Target> targets)
                => (@event.Match(e => e, Event), ObjectValue(("target", Targets(targets)), ("cond", SimpleValue("() => false", true))));

            // transitions
            var transitions = definition.GetTransitions().Select(
                transition => transition
                    .Match(
                        forbidden => (forbidden.Event.EventName, SimpleValue("undefined", true)),
                        unguarded => Unguarded(unguarded.Event, unguarded.Targets),
                        unguarded => Unguarded(unguarded.Event, unguarded.Targets),
                        unguarded => Unguarded(new Model.NamedEvent("[THINK]"), unguarded.Targets),
                        guarded => Guarded(guarded.Event, guarded.Targets),
                        guarded => Guarded(guarded.Event, guarded.Targets),
                        guarded => Guarded(new Model.NamedEvent("[THINK]"), guarded.Targets))).ToList();
            if(transitions.Any()) properties.Add(("on", ObjectValue(transitions)));

            JSProperty MapDoneTransition(OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition> transition) =>
                transition.Match(
                    unguarded => Unguarded("onDone", unguarded.Targets),
                    unguarded => Unguarded("onDone", unguarded.Targets),
                    guarded => Guarded("onDone", guarded.Targets),
                    guarded => Guarded("onDone", guarded.Targets));

            // DoneTransition
            var onDoneTransition = definition.Match(
                _ => Option.None<JSProperty>(), 
                _ => Option.None<JSProperty>(),
                compound => compound.DoneTransition.Map(MapDoneTransition), 
                orthogonal => orthogonal.DoneTransition.Map(MapDoneTransition));
            onDoneTransition.SwitchSome(properties.Add);

            // actions
            var entryCount = definition.EntryActions.Count();
            var exitCount = definition.ExitActions.Count();
            if(entryCount > 0) properties.Add(("entry", entryCount == 1 ? "1 Action" : $"{entryCount} Actions"));
            if(exitCount > 0) properties.Add(("exit", exitCount == 1 ? "1 Action" : $"{exitCount} Actions"));

            IEnumerable<ObjectValue> MapServices(NonFinalStateNode stateNode) =>
                stateNode.Services.Select(service =>
                {
                    var idProperty = service.Id.Map<JSProperty>(id => ("id", id));
                    var onErrorProperty = service.OnErrorTransition.Map(transition => transition.Match(
                        unguarded => Unguarded("onError", unguarded.Targets),
                        unguarded => Unguarded("onError", unguarded.Targets)));
                    var onSuccessProperty = service.Match(
                        _ => Option.None<JSProperty>(),
                        taskService => taskService.OnSuccessDefinition.Map(transition => transition.Match(
                            unguarded => Unguarded("onDone", unguarded.Targets),
                            unguarded => Unguarded("onDone", unguarded.Targets))),
                        taskDataService => taskDataService.OnSuccessDefinition.Map(transition => transition.Match(
                            unguarded => Unguarded("onDone", unguarded.Targets),
                            unguarded => Unguarded("onDone", unguarded.Targets),
                            unguarded => Unguarded("onDone", unguarded.Targets))));

                    var serviceProperties = new[] {idProperty, onErrorProperty, onSuccessProperty}
                        .Where(property => property.HasValue).Select(property => property.ValueOr(default(JSProperty)));

                    return ObjectValue(serviceProperties);
                });

            // services
            var services = definition.Match(_ => Enumerable.Empty<ObjectValue>(), MapServices).ToList();
            if (services.Any()) properties.Add(("invoke", ArrayValue(services)));

            return properties;
        }
    }

    internal static class Extensions
    {
        internal static SimpleValue Serialize<TContext>(
            this Model.Target target,
            Definition.Statechart<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>
            => SimpleValue(target.Match(
                absolute => $"#{statechartDefinition.Id}.{string.Join(".", absolute.Id.Path.Select(key => key.Map(_ => null, named => named.StateName)) .Where(text => text != null))}",
                sibling => $"{sibling.Key.StateName}",
                child => $".{child.Key.StateName}"));
    }
}