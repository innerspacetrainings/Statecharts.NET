using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;

namespace Statecharts.NET.XState
{
    public static class XStateExtensions
    {
        public static string AsXStateDefinition<TContext>(
            this StatechartDefinition<TContext> statechartDefinition, string prefix = "")
            where TContext : IEquatable<TContext>, IXStateSerializable
            => WrapInMachine(
                prefix,
                ObjectValue(
                    ("id", statechartDefinition.Id),
                    ("context", statechartDefinition.InitialContext.AsJSObject())
                ).With(statechartDefinition.StateNodeDefinition.AsJSProperty(statechartDefinition).Value as ObjectValue)); // this cast is necessary because of the way xstate merges the top-level state node with the machine definition

        private static string WrapInMachine(string prefix, ObjectValue @object)
            => $"const {prefix}Machine = Machine({@object.AsString()})";

        private static JSProperty AsJSProperty<TContext>(
            this IBaseStateNodeDefinition stateNodeDefinition,
            StatechartDefinition<TContext> statechartDefinition)
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
            this IBaseStateNodeDefinition definition,
            StatechartDefinition<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>
        {
            ArrayValue Targets(BaseTransitionDefinition transitionDefinition)
                => ArrayValue(transitionDefinition.Targets.Select(
                    targetDefinition => targetDefinition.Serialize(statechartDefinition)));
            ObjectValue Unguarded<TUnguardedTransitionDefinition>(TUnguardedTransitionDefinition unguarded)
                where TUnguardedTransitionDefinition : BaseTransitionDefinition, UnguardedTransitionDefinition
                => ObjectValue(("target", Targets(unguarded)));
            ObjectValue Guarded<TGuardedTransitionDefinition>(TGuardedTransitionDefinition guarded)
                where TGuardedTransitionDefinition : BaseTransitionDefinition, GuardedTransitionDefinition
                => ObjectValue(("target", Targets(guarded)), ("cond", SimpleValue("() => false", true)));

            List <JSProperty> properties = new List<JSProperty>();

            if (definition.Transitions != null && definition.Transitions.Any()) // TODO: probably do this null check somewhere else
                properties.Add(("on", ObjectValue(
                    definition.Transitions.Select(
                        e => e.Map<JSProperty>(
                            immediateEventDefinition => ("\"\"", ArrayValue(
                                immediateEventDefinition.Transitions.Select(
                                    def => def.Map(Unguarded, Guarded)))),
                            eventDefinition => (eventDefinition.Event.Type, ArrayValue(
                                eventDefinition.Transitions.Select(
                                    def => def.Map(Unguarded, Guarded)))),
                            forbiddenEventDefinition => (forbiddenEventDefinition.Event.Type, "undefined"))))));

            return properties;
        }
    }

    internal static class Extensions
    {
        internal static SimpleValue Serialize<TContext>(
            this BaseTargetDefinition targetDefinition,
            StatechartDefinition<TContext> statechartDefinition)
            where TContext : IEquatable<TContext>
            => SimpleValue(targetDefinition.Map(
                absolute => $"#{statechartDefinition.Id}.{string.Join(".", absolute.Id.Path.Select(key => key.Map(_ => null, named => named.StateName)) .Where(text => text != null))}",
                sibling => $"{sibling.Key.StateName}",
                child => $".{child.Key.StateName}"));
    }
}