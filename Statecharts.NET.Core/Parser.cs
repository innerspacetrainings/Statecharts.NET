using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET
{
    internal static class ConversionExtensions
    {
        private static Actionblock NullsafeConvert(IEnumerable<Action> actions) =>
            actions != null
                ? Actionblock.From(actions)
                : Actionblock.Empty();

        internal static Actionblock Convert(this IEnumerable<ActionDefinition> actions) =>
            NullsafeConvert(actions.Select(Action.From));
        internal static Actionblock Convert(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions) =>
            NullsafeConvert(actions.Select(Action.From));
        internal static Actionblock Convert(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> actions) =>
            NullsafeConvert(actions.Select(Action.From));
    }

    internal static class TargetExtensions
    {
        private static StatenodeId StatenodeIdFor(StatenodeId sourceId, Target target) =>
            target.Match(absolute => absolute.Id, sibling => sourceId.Sibling(sibling.StatenodeName), child => sourceId.Child(child.StatenodeName));
        internal static IEnumerable<Statenode> GetTargetStatenodes(this IEnumerable<Target> targets, StatenodeId sourceId, Func<StatenodeId, Statenode> getStatenode) =>
            targets.Select(target => getStatenode(StatenodeIdFor(sourceId, target)));
        internal static Statenode GetTargetStatenode(this Target target, StatenodeId sourceId, Func<StatenodeId, Statenode> getStatenode) =>
            getStatenode(StatenodeIdFor(sourceId, target));
    }

    internal static class EventExtensions
    {
        internal static IEvent Convert(this IEventDefinition eventDefinition, Statenode source)
        {
            switch (eventDefinition)
            {
                case NamedDataEventDefinition definition: return new NamedEvent(definition.Name, definition.Data); // TODO: check this
                case NamedEventDefinition definition: return new NamedEvent(definition.Name);
                case ImmediateEventDefinition _: return new ImmediateEvent();
                case DelayedEventDefinition definition: return new DelayedEvent(source, definition.Delay);
                case ServiceSuccessEventDefinition _: throw new NotImplementedException();
                case ServiceErrorEventDefinition _: throw new NotImplementedException();
                case DoneEventDefinition _: return new DoneEvent(source);
                default: throw new Exception("it would be easier to interpret a Statechart if a proper event mapping is defined ;)");
            }
        }
    }

    internal static class TransitionDefinitionExtensions
    {
        private static Transition Convert(this TransitionDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode) =>
            definition.Match(
                forbidden => new Transition(forbidden.Event.Convert(source), source, Enumerable.Empty<Statenode>(), Actionblock.Empty(), (new ConditionContextGuard(_ => false) as Guard).ToOption()),
                unguarded => new Transition(unguarded.Event.Convert(source), source, unguarded.Targets.GetTargetStatenodes(source.Id, getStatenode), unguarded.Actions.Convert(), Option.None<Guard>()),
                unguarded => new Transition(unguarded.Event.Convert(source), source, unguarded.Targets.GetTargetStatenodes(source.Id, getStatenode), unguarded.Actions.Convert(), Option.None<Guard>()),
                unguarded => new Transition(unguarded.Event.Convert(source), source, unguarded.Targets.GetTargetStatenodes(source.Id, getStatenode), unguarded.Actions.Convert(), Option.None<Guard>()),
                guarded => new Transition(guarded.Event.Convert(source), source, guarded.Targets.GetTargetStatenodes(source.Id, getStatenode), guarded.Actions.Convert(), (guarded.Guard as Guard).ToOption()),
                guarded => new Transition(guarded.Event.Convert(source), source, guarded.Targets.GetTargetStatenodes(source.Id, getStatenode), guarded.Actions.Convert(), guarded.Guard.AsBase().ToOption()),
                guarded => new Transition(guarded.Event.Convert(source), source, guarded.Targets.GetTargetStatenodes(source.Id, getStatenode), guarded.Actions.Convert(), guarded.Guard.AsBase().ToOption()));
        private static Transition Convert(this InitialCompoundTransitionDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode) =>
            new Transition(new InitializeEvent(), source, definition.Target.GetTargetStatenode(source.Id, getStatenode).Yield(), definition.Actions.Convert(), Option.None<Guard>());
        private static Transition Convert(this DoneTransitionDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode) =>
            new Transition(new DoneEvent(source), source, definition.Targets.GetTargetStatenodes(source.Id, getStatenode), definition.Actions.Convert(), definition.Guard.Map(guard => guard.AsBase()));

        private static IEnumerable<Transition> GetNonFinalStatenodeTransitions(this NonFinalStatenodeDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode)
        {
            IEnumerable<TransitionDefinition> GetServiceTransitionDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions) =>
                serviceDefinitions.SelectMany(serviceDefinition =>
                    serviceDefinition.Match(
                        activity => activity.OnErrorTransition.Map(transitionDefinition => transitionDefinition.AsBase())
                            .Yield().WhereSome(),
                        task => new[] { task.OnSuccessDefinition, task.OnErrorTransition }.WhereSome()
                            .Select(transitionDefinition => transitionDefinition.AsBase()),
                        dataTask => new[]
                        {
                            dataTask.OnSuccessDefinition.Map(_ => _.AsBase()),
                            dataTask.OnErrorTransition.Map(_ => _.AsBase())
                        }.WhereSome()));

            return definition.Transitions.Concat(GetServiceTransitionDefinitions(definition.Services))
                .Select(transitionDefinition => transitionDefinition.Convert(source, getStatenode));
        }

        internal static IEnumerable<Transition> ConvertTransitions(this AtomicStatenodeDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode) =>
            definition.GetNonFinalStatenodeTransitions(source, getStatenode);
        internal static IEnumerable<Transition> ConvertTransitions(this CompoundStatenodeDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode)
        {
            var initial = definition.InitialTransition.Convert(source, getStatenode);
            var done = definition.DoneTransition.Map(doneTransitionDefinition => doneTransitionDefinition.Convert(source, getStatenode)).Yield().WhereSome();
            var transitions = definition.GetNonFinalStatenodeTransitions(source, getStatenode);

            return initial.Append(done).Concat(transitions);
        }
        internal static IEnumerable<Transition> ConvertTransitions(this OrthogonalStatenodeDefinition definition, Statenode source, Func<StatenodeId, Statenode> getStatenode)
        {
            var done = definition.DoneTransition.Map(doneTransitionDefinition => doneTransitionDefinition.Convert(source, getStatenode)).Yield().WhereSome();
            var transitions = definition.GetNonFinalStatenodeTransitions(source, getStatenode);

            return done.Concat(transitions);
        }
    }

    public static class Parser
    {
        private static (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) ParseStatenode(
            StatenodeDefinition definition,
            Statenode parent,
            int documentIndex)
        {
            IEnumerable<(Statenode statenode, StatenodeDefinition definition)> ParseChildren(
                IEnumerable<StatenodeDefinition> substateNodeDefinitions,
                Statenode recursedParent) =>
                substateNodeDefinitions.Select((substateDefinition, index) =>
                    (ParseStatenode(substateDefinition, recursedParent, documentIndex + index).root, substateDefinition));

            var name = definition.Name;
            var entryActions = definition.EntryActions.Convert();
            var exitActions = definition.ExitActions.Convert();

            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateAtomicStatenode(AtomicStatenodeDefinition atomicDefinition)
            {
                var statenode = new AtomicStatenode(parent, name, documentIndex, entryActions, exitActions);
                return (statenode, new Dictionary<StatenodeId, StatenodeDefinition> {{statenode.Id, atomicDefinition}});
            }
            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateFinalStatenode(FinalStatenodeDefinition finalDefinition)
            {
                var statenode = new FinalStatenode(parent, name, documentIndex, entryActions, exitActions);
                return (statenode, new Dictionary<StatenodeId, StatenodeDefinition> { { statenode.Id, finalDefinition } });
            }
            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateCompoundStatenode(CompoundStatenodeDefinition compoundDefinition)
            {
                var statenode = new CompoundStatenode(parent, name, documentIndex, entryActions, exitActions);
                var children = ParseChildren(compoundDefinition.Statenodes, statenode).ToList();
                var statenodes = children.Select(child => child.statenode);
                var definitions = children.Select(child => (child.statenode.Id, child.definition));

                statenode.Statenodes = statenodes;

                return (statenode, (statenode.Id, compoundDefinition as StatenodeDefinition).Append(definitions).ToDictionary(kvp => kvp.Id, kvp => kvp.Item2));
            }
            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateOrthogonalStatenode(OrthogonalStatenodeDefinition orthogonalDefinition)
            {
                var statenode = new OrthogonalStatenode(parent, name, documentIndex, entryActions, exitActions);
                var children = ParseChildren(orthogonalDefinition.Statenodes, statenode).ToList();
                var statenodes = children.Select(child => child.statenode);
                var definitions = children.Select(child => (child.statenode.Id, child.definition));

                statenode.Statenodes = statenodes;

                return (statenode, (statenode.Id, orthogonalDefinition as StatenodeDefinition).Append(definitions).ToDictionary(kvp => kvp.Id, kvp => kvp.Item2));
            }

            return definition.Match(
                CreateAtomicStatenode,
                CreateFinalStatenode,
                CreateCompoundStatenode, 
                CreateOrthogonalStatenode);
        }

        private static IDictionary<StatenodeId, (StatenodeDefinition definition, Statenode statenode)> CreateLookup(
            Statenode rootnode,
            IDictionary<StatenodeId, StatenodeDefinition> definitions) =>
            rootnode
                .CataFold<IEnumerable<Statenode>>(
                    atomic => atomic.Yield(),
                    final => final.Yield(),
                    (compound, children) => compound.Append(children.SelectMany(Functions.Identity)),
                    (orthogonal, children) => orthogonal.Append(children.SelectMany(Functions.Identity)))
                .ToDictionary(statenode => statenode.Id, statenode => (definitions[statenode.Id], statenode));

        private static void ParseAndSetTransitions(IDictionary<StatenodeId, (StatenodeDefinition definition, Statenode statenode)> lookup)
        {
            Statenode GetStatenode(StatenodeId id) => lookup[id].statenode;

            foreach (var (definition, statenode) in lookup.Values)
            {
                // TODO: this is fucking hacky 😢
                statenode.Switch(
                    atomic => atomic.Transitions = (definition as AtomicStatenodeDefinition).ConvertTransitions(statenode, GetStatenode),
                    Functions.NoOp,
                    compound => compound.Transitions = (definition as CompoundStatenodeDefinition).ConvertTransitions(statenode, GetStatenode),
                    orthogonal => orthogonal.Transitions = (definition as OrthogonalStatenodeDefinition).ConvertTransitions(statenode, GetStatenode));
            }
        }

        // TODO: return actual ParsedStatechart based on results from parsing
        public static ParsedStatechart<TContext> Parse<TContext>(StatechartDefinition<TContext> definition)
            where TContext : IContext<TContext>
        {
            var (rootnode, definitions) = ParseStatenode(definition.RootStateNode, null, 0);
            var lookup = CreateLookup(rootnode, definitions);
            var statenodes = lookup.Values
                .Select(value => value.statenode)
                .OrderByDescending(statenode => statenode.Depth)
                .ThenBy(statenode => statenode.DocumentIndex)
                .ToDictionary(statenode => statenode.Id);

            ParseAndSetTransitions(lookup);

            return new ExecutableStatechart<TContext>(
                rootnode,
                definition.InitialContext.CopyDeep(),
                statenodes);
        }
    }
}
