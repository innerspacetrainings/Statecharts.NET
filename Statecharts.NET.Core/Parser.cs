using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET
{
    internal static class ActionDefinitionExtensions
    {
        private static Actionblock NullsafeConvert(IEnumerable<Action> actions) =>
            actions != null
                ? Actionblock.From(actions)
                : Actionblock.Empty();

        internal static Actionblock Convert(this IEnumerable<ActionDefinition> actions) =>
            NullsafeConvert(actions?.Select(Action.From));
        internal static Actionblock Convert(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions) =>
            NullsafeConvert(actions?.Select(Action.From));
        internal static Actionblock Convert(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> actions) =>
            NullsafeConvert(actions?.Select(Action.From));
    }

    internal static class TargetExtensions
    {
        private static OneOf<StatenodeId, string> StatenodeIdFor(StatenodeId sourceId, Target target) =>
            target.Match<OneOf<StatenodeId, string>>(
                absolute => absolute.Id,
                sibling => sourceId.Sibling(sibling.StatenodeName, sibling.ChildStatenodesNames),
                child => sourceId.Child(child.StatenodeName, child.ChildStatenodesNames),
                self => sourceId,
                uniquelyIdentified => uniquelyIdentified.Id);
        internal static IEnumerable<Statenode> GetTargetStatenodes(this IEnumerable<Target> targets, StatenodeId sourceId, Func<OneOf<StatenodeId, string>, Statenode> getStatenode) =>
            targets.Select(target => getStatenode(StatenodeIdFor(sourceId, target)));
        internal static Statenode GetTargetStatenode(this Target target, StatenodeId sourceId, Func<OneOf<StatenodeId, string>, Statenode> getStatenode) =>
            getStatenode(StatenodeIdFor(sourceId, target));
    }

    internal static class EventDefinitionExtensions
    {
        internal static IEvent Convert(this IEventDefinition eventDefinition, Statenode source, ServiceDefinition serviceDefinition, int serviceIndex)
        {
            switch (eventDefinition)
            {
                case INamedDataEvent definition: return new NamedDataEvent<object>(definition.Name, definition.Data); // TODO: check this
                case INamedEvent definition: return new NamedEvent(definition.Name);
                case ImmediateEventDefinition _: return new ImmediateEvent();
                case DelayedEventDefinition definition: return new DelayedEvent(source.Id, definition.Delay);
                case ServiceSuccessEventDefinition _: return new ServiceSuccessEvent(serviceDefinition.GetId(source.Id, serviceIndex), null);
                case ServiceErrorEventDefinition _: return new ServiceErrorEvent(serviceDefinition.GetId(source.Id, serviceIndex), null);
                case DoneEventDefinition _: return new DoneEvent(source.Id);
                default: throw new Exception("it would be easier to interpret a Statechart if a proper event mapping is defined ;)");
            }
        }
    }

    internal static class TransitionDefinitionExtensions
    {
        private static Transition Convert(this TransitionDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode, ServiceDefinition serviceDefinition, int serviceIndex) =>
            definition.Match(
                forbidden => new Transition(forbidden.Event, source, Enumerable.Empty<Statenode>(), Actionblock.Empty(), Option.None<Guard>(), true),
                unguarded => new Transition(unguarded.Event.Convert(source, serviceDefinition, serviceIndex), source, unguarded.Targets.GetTargetStatenodes(source.Id, getStatenode), unguarded.Actions.Convert(), Option.None<Guard>(), false),
                unguarded => new Transition(unguarded.Event.Convert(source, serviceDefinition, serviceIndex), source, unguarded.Targets.GetTargetStatenodes(source.Id, getStatenode), unguarded.Actions.Convert(), Option.None<Guard>(), false),
                unguarded => new Transition(unguarded.Event.Convert(source, serviceDefinition, serviceIndex), source, unguarded.Targets.GetTargetStatenodes(source.Id, getStatenode), unguarded.Actions.Convert(), Option.None<Guard>(), false),
                guarded => new Transition(guarded.Event.Convert(source, serviceDefinition, serviceIndex), source, guarded.Targets.GetTargetStatenodes(source.Id, getStatenode), guarded.Actions.Convert(), (guarded.Guard as Guard).ToOption(), false),
                guarded => new Transition(guarded.Event.Convert(source, serviceDefinition, serviceIndex), source, guarded.Targets.GetTargetStatenodes(source.Id, getStatenode), guarded.Actions.Convert(), guarded.Guard.AsBase().ToOption(), false),
                guarded => new Transition(guarded.Event.Convert(source, serviceDefinition, serviceIndex), source, guarded.Targets.GetTargetStatenodes(source.Id, getStatenode), guarded.Actions.Convert(), guarded.Guard.AsBase().ToOption(), false));
        private static Transition Convert(this InitialCompoundTransitionDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode) =>
            new Transition(new InitializeEvent(source.Id), source, definition.Target.GetTargetStatenode(source.Id, getStatenode).Yield(), definition.Actions.Convert(), Option.None<Guard>(), false);
        private static Transition Convert(this DoneTransitionDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode) =>
            new Transition(new DoneEvent(source.Id), source, definition.Targets.GetTargetStatenodes(source.Id, getStatenode), definition.Actions.Convert(), definition.Guard.Map(guard => guard.AsBase()), false);

        private static IEnumerable<Transition> GetNonFinalStatenodeTransitions(this NonFinalStatenodeDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode)
        {
            IEnumerable<(ServiceDefinition serviceDefinition, TransitionDefinition transitionDefinition, int serviceIndex)> GetServiceTransitionDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions) =>
                serviceDefinitions?.SelectMany((serviceDefinition, index) =>
                    serviceDefinition.Match<IEnumerable<(ServiceDefinition, TransitionDefinition, int)>>(
                        activity => activity.OnErrorTransition.Map(transitionDefinition => transitionDefinition.AsBase())
                            .Yield().WhereSome().Select(transitionDefinition => (serviceDefinition, transitionDefinition, index)),
                        task => new[] { task.OnSuccessDefinition, task.OnErrorTransition }.WhereSome()
                            .Select(transitionDefinition => (serviceDefinition, transitionDefinition.AsBase(), index)),
                        dataTask => new[]
                        {
                            dataTask.OnSuccessDefinition.Map(_ => _.AsBase()),
                            dataTask.OnErrorTransition.Map(_ => _.AsBase())
                        }.WhereSome().Select(transitionDefinition => (serviceDefinition, transitionDefinition, index))))
                ?? Enumerable.Empty<(ServiceDefinition, TransitionDefinition, int)>();

            var serviceTransitions = GetServiceTransitionDefinitions(definition.Services)
                .Select(mapped => mapped.transitionDefinition.Convert(source, getStatenode, mapped.serviceDefinition, mapped.serviceIndex));
            var regularTransitions = (definition.Transitions ?? Enumerable.Empty<TransitionDefinition>()).Select(transitionDefinition =>
                transitionDefinition.Convert(source, getStatenode, null, 0)); // TODO, hate this `null`

            return regularTransitions.Concat(serviceTransitions);
        }

        internal static IEnumerable<Transition> ConvertTransitions(this AtomicStatenodeDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode) =>
            definition.GetNonFinalStatenodeTransitions(source, getStatenode);
        internal static IEnumerable<Transition> ConvertTransitions(this CompoundStatenodeDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode)
        {
            var initial = definition.InitialTransition.Convert(source, getStatenode);
            var done = definition.DoneTransition.Map(doneTransitionDefinition => doneTransitionDefinition.Convert(source, getStatenode)).Yield().WhereSome();
            var transitions = definition.GetNonFinalStatenodeTransitions(source, getStatenode);

            return initial.Append(done).Concat(transitions);
        }
        internal static IEnumerable<Transition> ConvertTransitions(this OrthogonalStatenodeDefinition definition, Statenode source, Func<OneOf<StatenodeId, string>, Statenode> getStatenode)
        {
            var done = definition.DoneTransition.Map(doneTransitionDefinition => doneTransitionDefinition.Convert(source, getStatenode)).Yield().WhereSome();
            var transitions = definition.GetNonFinalStatenodeTransitions(source, getStatenode);

            return done.Concat(transitions);
        }
        internal static Transition GetInitialTransition(this OrthogonalStatenode statenode) =>
            new Transition(new InitializeEvent(statenode.Id), statenode, statenode.Statenodes, Actionblock.Empty(), Option.None<Guard>(), false);
    }

    internal static class ServiceDefinitionExtensions
    {
        internal static string GetId(this ServiceDefinition serviceDefinition, StatenodeId statenodeId, int serviceIndex)
            => serviceDefinition.Id.ValueOr($"{statenodeId.String}:service#{serviceIndex}");
        internal static IEnumerable<Service> Convert(this IEnumerable<ServiceDefinition> serviceDefinitions, NonFinalStatenode statenode)
        {
            Service CreateServiceFromActivity(ActivityServiceDefinition service, string id) =>
                new Service(id, token =>
                {
                    token.Register(service.Activity.Stop);
                    service.Activity.Start(); // TODO: handle failure
                    return new TaskCompletionSource<object>().Task; // TODO: check if token and TaskCompletionSource have to be linked
                });

            return serviceDefinitions?.Select((serviceDefinition, index) => serviceDefinition.Match(
                activity => CreateServiceFromActivity(activity, serviceDefinition.GetId(statenode.Id, index)),
                task => new Service(serviceDefinition.GetId(statenode.Id, index), async cancellationToken =>
                {
                    await task.Task(cancellationToken);
                    return default; // TODO: document why this is used this way
                }),
                dataTask => new Service(serviceDefinition.GetId(statenode.Id, index), dataTask.TaskDelegate)))
                ?? Enumerable.Empty<Service>();
        }
    }

    public static class Parser
    {
        private static (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) ParseStatenode(
            StatenodeDefinition definition,
            Statenode parent,
            IDictionary<StatenodeId, StatenodeDefinition> definitions,
            int documentIndex)
        {
            IEnumerable<(Statenode statenode, StatenodeDefinition definition)> ParseChildren(
                IEnumerable<StatenodeDefinition> substateNodeDefinitions,
                Statenode recursedParent) =>
                substateNodeDefinitions.Select((substateDefinition, index) =>
                    (ParseStatenode(substateDefinition, recursedParent, definitions, index).root, substateDefinition));

            var name = definition.Name;
            var uniqueIdentifier = definition.UniqueIdentifier;
            var entryActions = definition.EntryActions.Convert();
            var exitActions = definition.ExitActions.Convert();

            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateAtomicStatenode(AtomicStatenodeDefinition atomicDefinition)
            {
                var statenode = new AtomicStatenode(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions);
                statenode.Services = atomicDefinition.Services.Convert(statenode);
                definitions.Add(statenode.Id, atomicDefinition);
                return (statenode, definitions);
            }
            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateFinalStatenode(FinalStatenodeDefinition finalDefinition)
            {
                var statenode = new FinalStatenode(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions);
                definitions.Add(statenode.Id, finalDefinition);
                return (statenode, definitions);
            }
            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateCompoundStatenode(CompoundStatenodeDefinition compoundDefinition)
            {
                var statenode = new CompoundStatenode(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions);
                var children = ParseChildren(compoundDefinition.Statenodes, statenode).ToList();
                var statenodes = children.Select(child => child.statenode);

                statenode.Statenodes = statenodes;
                statenode.Services = compoundDefinition.Services.Convert(statenode);
                definitions.Add(statenode.Id, compoundDefinition);

                return (statenode, definitions);
            }
            (Statenode root, IDictionary<StatenodeId, StatenodeDefinition> definitions) CreateOrthogonalStatenode(OrthogonalStatenodeDefinition orthogonalDefinition)
            {
                var statenode = new OrthogonalStatenode(parent, name, uniqueIdentifier, documentIndex, entryActions, exitActions);
                var children = ParseChildren(orthogonalDefinition.Statenodes, statenode).ToList();
                var statenodes = children.Select(child => child.statenode);

                statenode.Statenodes = statenodes;
                statenode.Services = orthogonalDefinition.Services.Convert(statenode);
                definitions.Add(statenode.Id, orthogonalDefinition);

                return (statenode, definitions);
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
            Statenode GetStatenode(OneOf<StatenodeId, string> id) => id.Match(
                statenodeId => lookup[statenodeId].statenode,
                uniqueId => lookup.Values.FirstOrDefault(tuple => tuple.statenode.UniqueIdentifier.Equals(uniqueId.ToOption())).statenode);

            foreach (var (definition, statenode) in lookup.Values)
            {
                // TODO: this is fucking hacky 😢
                statenode.Switch(
                    atomic => atomic.Transitions = (definition as AtomicStatenodeDefinition).ConvertTransitions(statenode, GetStatenode),
                    Functions.NoOp,
                    compound => compound.Transitions = (definition as CompoundStatenodeDefinition).ConvertTransitions(statenode, GetStatenode),
                    orthogonal => orthogonal.Transitions = (definition as OrthogonalStatenodeDefinition).ConvertTransitions(statenode, GetStatenode).Append(orthogonal.GetInitialTransition()));
            }
        }

        private static void ParseAndSetDelayedTransitions(IEnumerable<Statenode> statenodes)
        {
            IEnumerable<StartDelayedTransitionAction> GetDelayedActions(NonFinalStatenode nonFinalStatenode) =>
                nonFinalStatenode.Transitions.Select(transition =>
                    transition.Event is DelayedEvent delayed
                        ? new StartDelayedTransitionAction(nonFinalStatenode.Id, (TimeSpan)delayed.Data)
                        : null).WhereNotNull();

            foreach (var statenode in statenodes)
                statenode.AddDelayedTransitionAction(statenode.Match(_ => Enumerable.Empty<StartDelayedTransitionAction>(), GetDelayedActions));
        }

        private static void SetRootDoneTransition<TContext>(ExecutableStatechart<TContext> statechart) where TContext : IContext<TContext>
        {
            statechart.Rootnode.Switch(
                Functions.NoOp,
                root =>
                    root.Transitions =
                        new Transition(new DoneEvent(root.Id), // TODO: make this transition internal
                            root,
                            root.Yield(),
                            Actionblock.From(new SideEffectAction((context, eventData) => statechart.Done?.Invoke((TContext)context, eventData)).Yield()),
                            Option.None<Guard>(),
                            false).Yield().Concat(root.Transitions));
        }

        // TODO: return actual ParsedStatechart based on results from parsing
        public static ParsedStatechart<TContext> Parse<TContext>(StatechartDefinition<TContext> definition)
            where TContext : IContext<TContext>
        {
            var (rootnode, definitions) = ParseStatenode(definition.RootStateNode, null, new Dictionary<StatenodeId, StatenodeDefinition>(),  0);
            var lookup = CreateLookup(rootnode, definitions);
            var statenodes = lookup.Values
                .Select(value => value.statenode)
                .OrderByDescending(statenode => statenode.Depth)
                .ThenBy(statenode => statenode.DocumentIndex)
                .ToDictionary(statenode => statenode.Id);

            ParseAndSetTransitions(lookup);
            ParseAndSetDelayedTransitions(statenodes.Values);

            var statechart = new ExecutableStatechart<TContext>(
                rootnode,
                definition.InitialContext.CopyDeep(),
                statenodes);

            SetRootDoneTransition(statechart);

            return statechart;
        }
    }
}
