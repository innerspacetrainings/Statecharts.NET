using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using static Statecharts.NET.Language.Keywords;

namespace Statecharts.NET.Language.Builders.StateNode
{
    internal class DefinitionData
    {
        public string Name { get; }
        internal IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; set; }
        internal IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; set; }
        internal IEnumerable<TransitionDefinition> Transitions { get; set; }
        internal IEnumerable<ServiceDefinition> Services { get; set; }
        internal InitialCompoundTransitionDefinition InitialTransition { get; set; }
        internal IEnumerable<StatenodeDefinition> States { get; set; }

        public DefinitionData(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EntryActions = Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition>>();
            ExitActions = Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition>>();
            Transitions = Enumerable.Empty<TransitionDefinition>();
            Services = Enumerable.Empty<ServiceDefinition>();
            States = Enumerable.Empty<StatenodeDefinition>();
        }
    }

    public class WithName : WithEntryActions
    {
        public WithName(string name) : base(name) { }

        public WithEntryActions WithEntryActions(
            Language.Action action,
            params Language.Action[] actions)
        {
            DefinitionData.EntryActions = action.Append(actions)
                .Select<Language.Action, OneOf<ActionDefinition, ContextActionDefinition>>(a => a.ToDefinitionAction());
            return this;
        }
        public WithEntryActions WithEntryActions<TContext>(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions)
        {
            DefinitionData.EntryActions = action.Append(actions)
                .Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                    contextlessAction => contextlessAction.ToDefinitionAction(),
                    contextAction => contextAction.ToDefinitionAction()));
            return this;
        }
    }
    public class WithEntryActions : WithExitActions
    {
        internal WithEntryActions(string name) : base(name) { }

        public WithExitActions WithExitActions(
            Language.Action action,
            params Language.Action[] actions)
        {
            DefinitionData.ExitActions = action.Append(actions)
                .Select<Language.Action, OneOf<ActionDefinition, ContextActionDefinition>>(a => a.ToDefinitionAction());
            return this;
        }
        public WithExitActions WithExitActions<TContext>(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions)
        {
            DefinitionData.ExitActions = action.Append(actions)
                .Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                    contextlessAction => contextlessAction.ToDefinitionAction(),
                    contextAction => contextAction.ToDefinitionAction()));
            return this;
        }
    }
    public class WithExitActions : WithTransitions
    {
        internal WithExitActions(string name) : base(name) { }

        public WithTransitions WithTransitions(
            TransitionDefinition transitionDefinition,
            params TransitionDefinition[] transitionDefinitions) =>
            WithTransitions(transitionDefinition.Append(transitionDefinitions));

        public WithTransitions WithTransitions(IEnumerable<TransitionDefinition> transitionDefinitions)
        {
            DefinitionData.Transitions = transitionDefinitions;
            return this;
        }

        public Final AsFinal() => new Final(DefinitionData);
    }
    public class WithTransitions : WithInvocations
    {
        internal WithTransitions(string name) : base(name) { }

        public WithInvocations WithInvocations(
            ServiceDefinition service,
            params ServiceDefinition[] services)
        {
            DefinitionData.Services = service.Append(services);
            return this;
        }
    }
    public class WithInvocations : AtomicStatenodeDefinition
    {
        private protected DefinitionData DefinitionData { get; }

        internal WithInvocations(string name) => DefinitionData = new DefinitionData(name);

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;

        public Compound AsCompound() => new Compound(DefinitionData);
        public Orthogonal AsOrthogonal() => new Orthogonal(DefinitionData);
    }

    public class Final : FinalStatenodeDefinition
    {
        private DefinitionData DefinitionData { get; }

        internal Final(DefinitionData data)
            => DefinitionData = data;

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
    }

    public class Compound
    {
        internal DefinitionData DefinitionData { get; }

        internal Compound(DefinitionData data)
            => DefinitionData = data;

        public CompoundWithInitialState WithInitialState(string stateName)
        {
            DefinitionData.InitialTransition = new InitialCompoundTransitionDefinition(Child(stateName));
            return new CompoundWithInitialState(this);
        }
    }
    public class CompoundWithInitialState
    {
        internal DefinitionData DefinitionData { get; }

        internal CompoundWithInitialState(Compound compound)
            => DefinitionData = compound.DefinitionData;

        public CompoundWithInitialActions WithInitialActions(
            OneOf<ActionDefinition, ContextActionDefinition> action,
            params OneOf<ActionDefinition, ContextActionDefinition>[] actions)
        {
            DefinitionData.InitialTransition = new InitialCompoundTransitionDefinition(DefinitionData.InitialTransition.Target, action.Append(actions));
            return new CompoundWithInitialActions(this);
        }

        public CompoundWithStates WithStates(
            OneOf<string, StatenodeDefinition> state,
            params OneOf<string, StatenodeDefinition>[] states) =>
            WithStates(state.Append(states));
        public CompoundWithStates WithStates(IEnumerable<OneOf<string, StatenodeDefinition>> states) =>
            WithStates(states.Select(definition => definition.Match(name => new WithName(name), valid => valid)));
        public CompoundWithStates WithStates(IEnumerable<string> states) =>
            WithStates(states.Select(name => new WithName(name)));
        public CompoundWithStates WithStates(IEnumerable<StatenodeDefinition> states)
        {
            DefinitionData.States = states;
            return new CompoundWithStates(this);
        }
    }
    public class CompoundWithInitialActions
    {
        internal DefinitionData DefinitionData { get; }

        internal CompoundWithInitialActions(CompoundWithInitialState compound)
            => DefinitionData = compound.DefinitionData;

        public CompoundWithStates WithStates(
            OneOf<string, StatenodeDefinition> state,
            params OneOf<string, StatenodeDefinition>[] states) =>
            WithStates(state.Append(states));
        public CompoundWithStates WithStates(IEnumerable<OneOf<string, StatenodeDefinition>> states) =>
            WithStates(states.Select(definition => definition.Match(name => new WithName(name), valid => valid)));
        public CompoundWithStates WithStates(IEnumerable<string> states) =>
            WithStates(states.Select(name => new WithName(name)));
        public CompoundWithStates WithStates(IEnumerable<StatenodeDefinition> states)
        {
            DefinitionData.States = states;
            return new CompoundWithStates(this);
        }
    }
    public class CompoundWithStates : CompoundStatenodeDefinition
    {
        internal DefinitionData DefinitionData { get; }

        internal CompoundWithStates(CompoundWithInitialState compoundWithInitialState)
            => DefinitionData = compoundWithInitialState.DefinitionData;
        internal CompoundWithStates(CompoundWithInitialActions compoundWithInitialActions)
            => DefinitionData = compoundWithInitialActions.DefinitionData;

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;
        public override InitialCompoundTransitionDefinition InitialTransition => DefinitionData.InitialTransition;
        public override IEnumerable<StatenodeDefinition> Statenodes => DefinitionData.States;
        public override Option<DoneTransitionDefinition> DoneTransition => Option.None<DoneTransitionDefinition>();

        public CompoundWithOnDone OnDone => new CompoundWithOnDone(this);
    }
    public class CompoundWithOnDone
    {
        internal DefinitionData DefinitionData { get; }

        public CompoundWithOnDone(CompoundWithStates compound) => DefinitionData = compound.DefinitionData;

        public CompoundWithDoneTransitionTo TransitionTo => new CompoundWithDoneTransitionTo(this);
    }
    public class CompoundWithDoneTransitionTo
    {
        internal DefinitionData DefinitionData { get; }

        public CompoundWithDoneTransitionTo(CompoundWithOnDone compound) => DefinitionData = compound.DefinitionData;

        public CompoundWithDoneTransition Child(string stateName, params string[] childStatenodesNames) =>
            new CompoundWithDoneTransition(this, Keywords.Child(stateName, childStatenodesNames));
        public CompoundWithDoneTransition Sibling(string stateName, params string[] childStatenodesNames) =>
            new CompoundWithDoneTransition(this, Keywords.Sibling(stateName, childStatenodesNames));
        public CompoundWithDoneTransition Absolute(string statechartName, params string[] childStatenodesNames) =>
            new CompoundWithDoneTransition(this, Keywords.Absolute(statechartName, childStatenodesNames));
        public CompoundWithDoneTransition Target(Target target) =>
            new CompoundWithDoneTransition(this, target);
        public CompoundWithDoneTransition Multiple(Model.Target target, params Model.Target[] targets) =>
            new CompoundWithDoneTransition(this, target, targets);
    }
    public class CompoundWithDoneTransition : CompoundStatenodeDefinition
    {
        internal DefinitionData DefinitionData { get; }
        internal UnguardedWithTarget DoneTransitionBuilder { get; }

        public CompoundWithDoneTransition(CompoundWithDoneTransitionTo compound, Model.Target target, params Model.Target[] targets)
        {
            DefinitionData = compound.DefinitionData;
            DoneTransitionBuilder = WithEvent.OnDone().TransitionTo.Multiple(target, targets);
        }

        public CompoundWithDoneTransitionWithActions WithActions(Language.Action action, params Language.Action[] actions) =>
            new CompoundWithDoneTransitionWithActions(this, action, actions);

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => DefinitionData.States;
        public override InitialCompoundTransitionDefinition InitialTransition => DefinitionData.InitialTransition;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }
    public class CompoundWithDoneTransitionWithActions : CompoundStatenodeDefinition
    {
        internal DefinitionData DefinitionData { get; }
        internal UnguardedWithActions DoneTransitionBuilder { get; }
        public CompoundWithDoneTransitionWithActions(CompoundWithDoneTransition compound, Language.Action action, Language.Action[] actions)
        {
            DefinitionData = compound.DefinitionData;
            DoneTransitionBuilder = compound.DoneTransitionBuilder.WithActions(action, actions);
        }

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => DefinitionData.States;
        public override InitialCompoundTransitionDefinition InitialTransition => DefinitionData.InitialTransition;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }

    public class Orthogonal
    {
        internal DefinitionData DefinitionData { get; }

        internal Orthogonal(DefinitionData data)
            => DefinitionData = data;

        public OrthogonalWithStates WithStates(
            OneOf<string, StatenodeDefinition> state,
            params OneOf<string, StatenodeDefinition>[] states) =>
            WithStates(state.Append(states));
        public OrthogonalWithStates WithStates(IEnumerable<OneOf<string, StatenodeDefinition>> states) =>
            WithStates(states.Select(definition => definition.Match(name => new WithName(name), valid => valid)));
        public OrthogonalWithStates WithStates(IEnumerable<string> states) =>
            WithStates(states.Select(name => new WithName(name)));
        public OrthogonalWithStates WithStates(IEnumerable<StatenodeDefinition> states)
        {
            DefinitionData.States = states;
            return new OrthogonalWithStates(this);
        }
    }
    public class OrthogonalWithStates : OrthogonalStatenodeDefinition
    {
        internal DefinitionData DefinitionData { get; }

        internal OrthogonalWithStates(Orthogonal orthogonal)
            => DefinitionData = orthogonal.DefinitionData;

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => DefinitionData.States;
        public override Option<DoneTransitionDefinition> DoneTransition => Option.None<DoneTransitionDefinition>();
        public OrthogonalWithOnDone OnDone => new OrthogonalWithOnDone(this);
    }
    public class OrthogonalWithOnDone
    {
        internal DefinitionData DefinitionData { get; }

        public OrthogonalWithOnDone(OrthogonalWithStates orthogonal) => DefinitionData = orthogonal.DefinitionData;

        public OrthogonalWithDoneTransitionTo TransitionTo => new OrthogonalWithDoneTransitionTo(this);
    }
    public class OrthogonalWithDoneTransitionTo
    {
        internal DefinitionData DefinitionData { get; }

        public OrthogonalWithDoneTransitionTo(OrthogonalWithOnDone orthogonal) => DefinitionData = orthogonal.DefinitionData;

        public OrthogonalWithDoneTransition Child(string stateName, params string[] childStatenodesNames) =>
            new OrthogonalWithDoneTransition(this, Keywords.Child(stateName, childStatenodesNames));
        public OrthogonalWithDoneTransition Sibling(string stateName, params string[] childStatenodesNames) =>
            new OrthogonalWithDoneTransition(this, Keywords.Sibling(stateName, childStatenodesNames));
        public OrthogonalWithDoneTransition Absolute(string statechartName, params string[] childStatenodesNames) =>
            new OrthogonalWithDoneTransition(this, Keywords.Absolute(statechartName, childStatenodesNames));
        public OrthogonalWithDoneTransition Target(Target target) =>
            new OrthogonalWithDoneTransition(this, target);
        public OrthogonalWithDoneTransition Multiple(Model.Target target, params Model.Target[] targets) =>
            new OrthogonalWithDoneTransition(this, target, targets);
    }
    public class OrthogonalWithDoneTransition : OrthogonalStatenodeDefinition
    {
        internal DefinitionData DefinitionData { get; }
        internal UnguardedWithTarget DoneTransitionBuilder { get; }

        public OrthogonalWithDoneTransition(OrthogonalWithDoneTransitionTo orthogonal, Model.Target target, params Model.Target[] targets)
        {
            DefinitionData = orthogonal.DefinitionData;
            DoneTransitionBuilder = WithEvent.OnDone().TransitionTo.Multiple(target, targets);
        }

        public OrthogonalWithDoneTransitionWithActions WithActions(Language.Action action, params Language.Action[] actions) =>
            new OrthogonalWithDoneTransitionWithActions(this, action, actions);

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => DefinitionData.States;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }
    public class OrthogonalWithDoneTransitionWithActions : OrthogonalStatenodeDefinition
    {
        internal DefinitionData DefinitionData { get; }
        internal UnguardedWithActions DoneTransitionBuilder { get; }
        public OrthogonalWithDoneTransitionWithActions(OrthogonalWithDoneTransition orthogonal, Language.Action action, Language.Action[] actions)
        {
            DefinitionData = orthogonal.DefinitionData;
            DoneTransitionBuilder = orthogonal.DoneTransitionBuilder.WithActions(action, actions);
        }

        public override string Name => DefinitionData.Name;
        public override Option<string> UniqueIdentifier => Option.None<string>();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<TransitionDefinition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<ServiceDefinition> Services => DefinitionData.Services;
        public override IEnumerable<StatenodeDefinition> Statenodes => DefinitionData.States;
        public override Option<DoneTransitionDefinition> DoneTransition => new DoneTransitionDefinition(DoneTransitionBuilder.Targets).ToOption(); // TODO: improve this
    }
}
