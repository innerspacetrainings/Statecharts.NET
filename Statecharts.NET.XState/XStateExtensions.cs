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
        public static string AsXStateDefinition<TContext>(
            this Definition.Statechart<TContext> statechartDefinition, string prefix = "")
            where TContext : IEquatable<TContext>, IXStateSerializable
            => WrapInMachine(
                prefix,
                ObjectValue(
                    ("id", statechartDefinition.Id),
                    ("context", statechartDefinition.InitialContext.AsJSObject())
                ).With(statechartDefinition.RootStateNode.AsJSProperty(statechartDefinition).Value as ObjectValue)); // this cast is necessary because of the way xstate merges the top-level state node with the machine definition

        private static string WrapInMachine(string prefix, ObjectValue @object)
            => $"const {prefix}Machine = Machine({@object.AsString()})";

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
            ArrayValue Targets(IEnumerable<Model.Target> targets)
                => ArrayValue(targets.Select(target => target.Serialize(statechartDefinition)));
            string Event(Model.Event @event) =>
                @event.Match(custom => custom.EventName, immediate => "\"\"", delayed => "after");
            JSProperty Unguarded(Model.Event @event, IEnumerable<Model.Target> targets)
                => (Event(@event), ObjectValue(("target", Targets(targets))));
            JSProperty Guarded(Model.Event @event, IEnumerable<Model.Target> targets)
                => (Event(@event), ObjectValue(("target", Targets(targets)), ("cond", SimpleValue("() => false", true))));

            List <JSProperty> properties = new List<JSProperty>();

            if (definition.Transitions.Any())
                properties.Add(("on", ObjectValue(
                    definition.Transitions.Select(
                        transition => transition.Match<Definition.ForbiddenTransition, Definition.UnguardedTransition, Definition.UnguardedContextTransition, Definition.UnguardedContextDataTransition, Definition.GuardedTransition, Definition.GuardedContextTransition, Definition.GuardedContextDataTransition, JSProperty>(
                            forbidden => (forbidden.Event.EventName, "undefined"),
                            unguarded => Unguarded(unguarded.Event, unguarded.Targets),
                            unguarded => Unguarded(unguarded.Event, unguarded.Targets),
                            unguarded => Unguarded(new Model.CustomEvent("[THINK]"), unguarded.Targets),
                            guarded => Guarded(guarded.Event, guarded.Targets),
                            guarded => Guarded(guarded.Event, guarded.Targets),
                            guarded => Guarded(new Model.CustomEvent("[THINK]"), guarded.Targets))))));

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