using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using static Statecharts.NET.Language.Keywords;

namespace Statecharts.NET.Language.Builders
{
    internal class StatenodeDefinitionData
    {
        public string Name { get; }
        internal IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions { get; set; }
        internal IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions { get; set; }
        internal IEnumerable<TransitionDefinition> Transitions { get; set; }
        internal IEnumerable<ServiceDefinition> Services { get; set; }
        internal InitialCompoundTransitionDefinition InitialTransition { get; set; }
        internal IEnumerable<StatenodeDefinition> States { get; set; }

        public StatenodeDefinitionData(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EntryActions = Enumerable.Empty<OneOf<Model.ActionDefinition, ContextActionDefinition>>();
            ExitActions = Enumerable.Empty<OneOf<Model.ActionDefinition, ContextActionDefinition>>();
            Transitions = Enumerable.Empty<TransitionDefinition>();
            Services = Enumerable.Empty<ServiceDefinition>();
            States = Enumerable.Empty<StatenodeDefinition>();
        }
    }

    public class StatenodeWithName : StatenodeWithEntryActions
    {
        public StatenodeWithName(string name) : base(name) { }

        public StatenodeWithEntryActions WithEntryActions(
            ActionDefinition action,
            params ActionDefinition[] actions)
        {
            Definition.EntryActions = action.Append(actions)
                .Select<ActionDefinition, OneOf<Model.ActionDefinition, ContextActionDefinition>>(a => a.ToDefinitionAction());
            return this;
        }
        public StatenodeWithEntryActions WithEntryActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
        {
            Definition.EntryActions = action.Append(actions)
                .Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition>>(
                    contextlessAction => contextlessAction.ToDefinitionAction(),
                    contextAction => contextAction.ToDefinitionAction()));
            return this;
        }
    }
    public class StatenodeWithEntryActions : StatenodeWithExitActions
    {
        internal StatenodeWithEntryActions(string name) : base(name) { }

        public StatenodeWithExitActions WithExitActions(
            ActionDefinition action,
            params ActionDefinition[] actions)
        {
            Definition.ExitActions = action.Append(actions)
                .Select<ActionDefinition, OneOf<Model.ActionDefinition, ContextActionDefinition>>(a => a.ToDefinitionAction());
            return this;
        }
        public StatenodeWithExitActions WithExitActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
        {
            Definition.ExitActions = action.Append(actions)
                .Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition>>(
                    contextlessAction => contextlessAction.ToDefinitionAction(),
                    contextAction => contextAction.ToDefinitionAction()));
            return this;
        }
    }
    public class StatenodeWithExitActions : StatenodeWithTransitions
    {
        internal StatenodeWithExitActions(string name) : base(name) { }

        public StatenodeWithTransitions WithTransitions(
            TransitionDefinition transitionDefinition,
            params TransitionDefinition[] transitionDefinitions) =>
            WithTransitions(transitionDefinition.Append(transitionDefinitions));

        public StatenodeWithTransitions WithTransitions(IEnumerable<TransitionDefinition> transitionDefinitions)
        {
            Definition.Transitions = transitionDefinitions;
            return this;
        }

        public FinalStatenode AsFinal() => new FinalStatenode(Definition);
    }
    public class StatenodeWithTransitions : StatenodeWithInvocations
    {
        internal StatenodeWithTransitions(string name) : base(name) { }

        public StatenodeWithInvocations WithInvocations(
            ServiceDefinition service,
            params ServiceDefinition[] services)
        {
            Definition.Services = service.Append(services);
            return this;
        }
    }
    public class StatenodeWithInvocations : AtomicStatenodeDefinition
    {
        private protected StatenodeDefinitionData Definition { get; }

        internal StatenodeWithInvocations(string name) => Definition = new StatenodeDefinitionData(name);

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;

        public CompoundStatenode AsCompound() => new CompoundStatenode(Definition);
        public OrthogonalStatenode AsOrthogonal() => new OrthogonalStatenode(Definition);
    }

    public class FinalStatenode : FinalStatenodeDefinition
    {
        private StatenodeDefinitionData Definition { get; }

        internal FinalStatenode(StatenodeDefinitionData data)
            => Definition = data;

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
    }

    public class CompoundStatenode
    {
        internal StatenodeDefinitionData Definition { get; }

        internal CompoundStatenode(StatenodeDefinitionData data)
            => Definition = data;

        public CompoundStatenodeWithInitialState WithInitialState(string stateName)
        {
            Definition.InitialTransition = new InitialCompoundTransitionDefinition(Child(stateName));
            return new CompoundStatenodeWithInitialState(this);
        }
    }
    public class CompoundStatenodeWithInitialState
    {
        internal StatenodeDefinitionData Definition { get; }

        internal CompoundStatenodeWithInitialState(CompoundStatenode compound)
            => Definition = compound.Definition;

        public CompoundStatenodeWithInitialActions WithInitialActions(
            OneOf<Model.ActionDefinition, ContextActionDefinition> action,
            params OneOf<Model.ActionDefinition, ContextActionDefinition>[] actions)
        {
            Definition.InitialTransition = new InitialCompoundTransitionDefinition(Definition.InitialTransition.Target, action.Append(actions));
            return new CompoundStatenodeWithInitialActions(this);
        }

        public CompoundStatenodeWithSubstates WithStates(
            OneOf<string, StatenodeDefinition> state,
            params OneOf<string, StatenodeDefinition>[] states) =>
            WithStates(state.Append(states));
        public CompoundStatenodeWithSubstates WithStates(IEnumerable<OneOf<string, StatenodeDefinition>> states) =>
            WithStates(states.Select(definition => definition.Match(name => new StatenodeWithName(name), valid => valid)));
        public CompoundStatenodeWithSubstates WithStates(IEnumerable<string> states) =>
            WithStates(states.Select(name => new StatenodeWithName(name)));
        public CompoundStatenodeWithSubstates WithStates(IEnumerable<StatenodeDefinition> states)
        {
            Definition.States = states;
            return new CompoundStatenodeWithSubstates(this);
        }
    }
    public class CompoundStatenodeWithInitialActions
    {
        internal StatenodeDefinitionData Definition { get; }

        internal CompoundStatenodeWithInitialActions(CompoundStatenodeWithInitialState compound)
            => Definition = compound.Definition;

        public CompoundStatenodeWithSubstates WithStates(
            OneOf<string, StatenodeDefinition> state,
            params OneOf<string, StatenodeDefinition>[] states) =>
            WithStates(state.Append(states));
        public CompoundStatenodeWithSubstates WithStates(IEnumerable<OneOf<string, StatenodeDefinition>> states) =>
            WithStates(states.Select(definition => definition.Match(name => new StatenodeWithName(name), valid => valid)));
        public CompoundStatenodeWithSubstates WithStates(IEnumerable<string> states) =>
            WithStates(states.Select(name => new StatenodeWithName(name)));
        public CompoundStatenodeWithSubstates WithStates(IEnumerable<StatenodeDefinition> states)
        {
            Definition.States = states;
            return new CompoundStatenodeWithSubstates(this);
        }
    }
    public class CompoundStatenodeWithSubstates : CompoundStatenodeDefinition
    {
        internal StatenodeDefinitionData Definition { get; }

        internal CompoundStatenodeWithSubstates(CompoundStatenodeWithInitialState compoundWithInitialState)
            => Definition = compoundWithInitialState.Definition;
        internal CompoundStatenodeWithSubstates(CompoundStatenodeWithInitialActions compoundWithInitialActions)
            => Definition = compoundWithInitialActions.Definition;

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;
        public override InitialCompoundTransitionDefinition InitialTransition => Definition.InitialTransition;
        public override IEnumerable<StatenodeDefinition> Statenodes => Definition.States;
        public override Option<DoneTransitionDefinition> DoneTransition => Option.None<DoneTransitionDefinition>();

        public CompoundStatenodeWithOnDone OnDone => new CompoundStatenodeWithOnDone(this);
    }
    public class CompoundStatenodeWithOnDone
    {
        internal StatenodeDefinitionData Definition { get; }

        public CompoundStatenodeWithOnDone(CompoundStatenodeWithSubstates compound) => Definition = compound.Definition;

        public CompoundStatenodeWithDoneTransitionTo TransitionTo => new CompoundStatenodeWithDoneTransitionTo(this);
    }
    public class CompoundStatenodeWithDoneTransitionTo
    {
        internal StatenodeDefinitionData Definition { get; }

        public CompoundStatenodeWithDoneTransitionTo(CompoundStatenodeWithOnDone compound) => Definition = compound.Definition;

        public CompoundStatenodeWithDoneTransition Child(string stateName, params string[] childStatenodesNames) =>
            new CompoundStatenodeWithDoneTransition(this, Keywords.Child(stateName, childStatenodesNames));
        public CompoundStatenodeWithDoneTransition Sibling(string stateName, params string[] childStatenodesNames) =>
            new CompoundStatenodeWithDoneTransition(this, Keywords.Sibling(stateName, childStatenodesNames));
        public CompoundStatenodeWithDoneTransition Absolute(string statechartName, params string[] childStatenodesNames) =>
            new CompoundStatenodeWithDoneTransition(this, Keywords.Absolute(statechartName, childStatenodesNames));
        public CompoundStatenodeWithDoneTransition Target(Target target) =>
            new CompoundStatenodeWithDoneTransition(this, target);
        public CompoundStatenodeWithDoneTransition Multiple(Target target, params Target[] targets) =>
            new CompoundStatenodeWithDoneTransition(this, target, targets);
    }
    public class CompoundStatenodeWithDoneTransition : CompoundStatenodeDefinition
    {
        internal StatenodeDefinitionData Definition { get; }
        internal UnguardedWithTarget DoneTransitionBuilder { get; }

        public CompoundStatenodeWithDoneTransition(CompoundStatenodeWithDoneTransitionTo compound, Target target, params Target[] targets)
        {
            Definition = compound.Definition;
            DoneTransitionBuilder = WithEvent.OnDone().TransitionTo.Multiple(target, targets);
        }

        public CompoundStatenodeWithDoneTransitionWithActions WithActions(ActionDefinition action, params ActionDefinition[] actions) =>
            new CompoundStatenodeWithDoneTransitionWithActions(this, action, actions);

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => Definition.States;
        public override InitialCompoundTransitionDefinition InitialTransition => Definition.InitialTransition;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }
    public class CompoundStatenodeWithDoneTransitionWithActions : CompoundStatenodeDefinition
    {
        internal StatenodeDefinitionData Definition { get; }
        internal UnguardedWithActions DoneTransitionBuilder { get; }
        public CompoundStatenodeWithDoneTransitionWithActions(CompoundStatenodeWithDoneTransition compound, ActionDefinition action, ActionDefinition[] actions)
        {
            Definition = compound.Definition;
            DoneTransitionBuilder = compound.DoneTransitionBuilder.WithActions(action, actions);
        }

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => Definition.States;
        public override InitialCompoundTransitionDefinition InitialTransition => Definition.InitialTransition;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }

    public class OrthogonalStatenode
    {
        internal StatenodeDefinitionData Definition { get; }

        internal OrthogonalStatenode(StatenodeDefinitionData data)
            => Definition = data;

        public OrthogonalStatenodeWithStates WithStates(
            OneOf<string, StatenodeDefinition> state,
            params OneOf<string, StatenodeDefinition>[] states) =>
            WithStates(state.Append(states));
        public OrthogonalStatenodeWithStates WithStates(IEnumerable<OneOf<string, StatenodeDefinition>> states) =>
            WithStates(states.Select(definition => definition.Match(name => new StatenodeWithName(name), valid => valid)));
        public OrthogonalStatenodeWithStates WithStates(IEnumerable<string> states) =>
            WithStates(states.Select(name => new StatenodeWithName(name)));
        public OrthogonalStatenodeWithStates WithStates(IEnumerable<StatenodeDefinition> states)
        {
            Definition.States = states;
            return new OrthogonalStatenodeWithStates(this);
        }
    }
    public class OrthogonalStatenodeWithStates : OrthogonalStatenodeDefinition
    {
        internal StatenodeDefinitionData Definition { get; }

        internal OrthogonalStatenodeWithStates(OrthogonalStatenode orthogonal)
            => Definition = orthogonal.Definition;

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => Definition.States;
        public override Option<DoneTransitionDefinition> DoneTransition => Option.None<DoneTransitionDefinition>();
        public OrthogonalStatenodeWithOnDone OnDone => new OrthogonalStatenodeWithOnDone(this);
    }
    public class OrthogonalStatenodeWithOnDone
    {
        internal StatenodeDefinitionData Definition { get; }

        public OrthogonalStatenodeWithOnDone(OrthogonalStatenodeWithStates orthogonal) => Definition = orthogonal.Definition;

        public OrthogonalStatenodeWithDoneTransitionTo TransitionTo => new OrthogonalStatenodeWithDoneTransitionTo(this);
    }
    public class OrthogonalStatenodeWithDoneTransitionTo
    {
        internal StatenodeDefinitionData Definition { get; }

        public OrthogonalStatenodeWithDoneTransitionTo(OrthogonalStatenodeWithOnDone orthogonal) => Definition = orthogonal.Definition;

        public OrthogonalStatenodeWithDoneTransition Child(string stateName, params string[] childStatenodesNames) =>
            new OrthogonalStatenodeWithDoneTransition(this, Keywords.Child(stateName, childStatenodesNames));
        public OrthogonalStatenodeWithDoneTransition Sibling(string stateName, params string[] childStatenodesNames) =>
            new OrthogonalStatenodeWithDoneTransition(this, Keywords.Sibling(stateName, childStatenodesNames));
        public OrthogonalStatenodeWithDoneTransition Absolute(string statechartName, params string[] childStatenodesNames) =>
            new OrthogonalStatenodeWithDoneTransition(this, Keywords.Absolute(statechartName, childStatenodesNames));
        public OrthogonalStatenodeWithDoneTransition Target(Target target) =>
            new OrthogonalStatenodeWithDoneTransition(this, target);
        public OrthogonalStatenodeWithDoneTransition Multiple(Target target, params Target[] targets) =>
            new OrthogonalStatenodeWithDoneTransition(this, target, targets);
    }
    public class OrthogonalStatenodeWithDoneTransition : OrthogonalStatenodeDefinition
    {
        internal StatenodeDefinitionData Definition { get; }
        internal UnguardedWithTarget DoneTransitionBuilder { get; }

        public OrthogonalStatenodeWithDoneTransition(OrthogonalStatenodeWithDoneTransitionTo orthogonal, Target target, params Target[] targets)
        {
            Definition = orthogonal.Definition;
            DoneTransitionBuilder = WithEvent.OnDone().TransitionTo.Multiple(target, targets);
        }

        public OrthogonalStatenodeWithDoneTransitionWithActions WithActions(ActionDefinition action, params ActionDefinition[] actions) =>
            new OrthogonalStatenodeWithDoneTransitionWithActions(this, action, actions);

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => Definition.States;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }
    public class OrthogonalStatenodeWithDoneTransitionWithActions : OrthogonalStatenodeDefinition
    {
        internal StatenodeDefinitionData Definition { get; }
        internal UnguardedWithActions DoneTransitionBuilder { get; }
        public OrthogonalStatenodeWithDoneTransitionWithActions(OrthogonalStatenodeWithDoneTransition orthogonal, ActionDefinition action, ActionDefinition[] actions)
        {
            Definition = orthogonal.Definition;
            DoneTransitionBuilder = orthogonal.DoneTransitionBuilder.WithActions(action, actions);
        }

        public override string Name => Definition.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> EntryActions => Definition.EntryActions;
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> ExitActions => Definition.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => Definition.Transitions;
        public override IEnumerable<ServiceDefinition> Services => Definition.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => Definition.States;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }
}
