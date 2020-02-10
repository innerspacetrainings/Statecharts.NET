using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Utilities;
using static Statecharts.NET.Language.Keywords;

namespace Statecharts.NET.Language.Builders.StateNode
{
    internal class DefinitionData
    {
        public string Name { get; }
        internal IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> EntryActions { get; set; }
        internal IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> ExitActions { get; set; }
        internal IEnumerable<Definition.Transition> Transitions { get; set; }
        internal IEnumerable<Definition.Service> Services { get; set; }
        internal Definition.InitialTransition InitialTransition { get; set; }
        internal IEnumerable<Definition.StateNode> States { get; set; }

        public DefinitionData(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EntryActions = Enumerable.Empty<OneOf<Definition.Action, Definition.ContextAction>>();
            ExitActions = Enumerable.Empty<OneOf<Definition.Action, Definition.ContextAction>>();
            Transitions = Enumerable.Empty<Definition.Transition>();
            Services = Enumerable.Empty<Definition.Service>();
            States = Enumerable.Empty<Definition.StateNode>();
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
                .Select<Language.Action, OneOf<Definition.Action, Definition.ContextAction>>(a => a.ToDefinitionAction());
            return this;
        }
        public WithEntryActions WithEntryActions<TContext>(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions)
        {
            DefinitionData.EntryActions = action.Append(actions)
                .Select(a => a.Match<OneOf<Definition.Action, Definition.ContextAction>>(
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
                .Select<Language.Action, OneOf<Definition.Action, Definition.ContextAction>>(a => a.ToDefinitionAction());
            return this;
        }
        public WithExitActions WithExitActions<TContext>(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions)
        {
            DefinitionData.ExitActions = action.Append(actions)
                .Select(a => a.Match<OneOf<Definition.Action, Definition.ContextAction>>(
                    contextlessAction => contextlessAction.ToDefinitionAction(),
                    contextAction => contextAction.ToDefinitionAction()));
            return this;
        }
    }
    public class WithExitActions : WithTransitions
    {
        internal WithExitActions(string name) : base(name) { }

        public WithTransitions WithTransitions(
            Definition.Transition transitionDefinition,
            params Definition.Transition[] transitionDefinitions)
        {
            DefinitionData.Transitions = transitionDefinition.Append(transitionDefinitions);
            return this;
        }

        public Final AsFinal() => new Final(DefinitionData);
    }
    public class WithTransitions : WithInvocations
    {
        internal WithTransitions(string name) : base(name) { }

        public WithInvocations WithInvocations(
            Definition.Service service,
            params Definition.Service[] services)
        {
            DefinitionData.Services = service.Append(services);
            return this;
        }
    }
    public class WithInvocations : Definition.AtomicStateNode
    {
        private protected DefinitionData DefinitionData { get; }

        internal WithInvocations(string name) => DefinitionData = new DefinitionData(name);

        public override string Name => DefinitionData.Name;
        public override Option<IEnumerable<Definition.Transition>> Transitions => DefinitionData.Transitions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> EntryActions => DefinitionData.EntryActions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> ExitActions => DefinitionData.ExitActions.ToOption();
        public override Option<IEnumerable<Definition.Service>> Services => DefinitionData.Services.ToOption();

        public Compound AsCompound() => new Compound(DefinitionData);
        public Orthogonal AsOrthogonal() => new Orthogonal(DefinitionData);
    }

    public class Final : Definition.FinalStateNode
    {
        private DefinitionData DefinitionData { get; }

        internal Final(DefinitionData data)
            => DefinitionData = data;

        public override string Name => DefinitionData.Name;
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> EntryActions => DefinitionData.EntryActions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> ExitActions => DefinitionData.ExitActions.ToOption();
    }

    public class Compound
    {
        internal DefinitionData DefinitionData { get; }

        internal Compound(DefinitionData data)
            => DefinitionData = data;

        public CompoundWithInitialState WithInitialState(string stateName)
        {
            DefinitionData.InitialTransition = new Definition.InitialTransition(Child(stateName));
            return new CompoundWithInitialState(this);
        }
    }
    public class CompoundWithInitialState
    {
        internal DefinitionData DefinitionData { get; }

        internal CompoundWithInitialState(Compound compound)
            => DefinitionData = compound.DefinitionData;

        public CompoundWithInitialActions WithInitialActions(
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] actions)
        {
            DefinitionData.InitialTransition = new Definition.InitialTransition(DefinitionData.InitialTransition.Target, action.Append(actions));
            return new CompoundWithInitialActions(this);
        }

        public CompoundWithStates WithStates(
            OneOf<string, Definition.StateNode> state,
            params OneOf<string, Definition.StateNode>[] states)
        {
            DefinitionData.States = state.Append(states).Select(
                definition => definition.Match(name => new WithName(name), valid => valid));
            return new CompoundWithStates(this);
        }
    }
    public class CompoundWithInitialActions
    {
        internal DefinitionData DefinitionData { get; }

        internal CompoundWithInitialActions(CompoundWithInitialState compound)
            => DefinitionData = compound.DefinitionData;

        public CompoundWithStates WithStates(
            OneOf<string, Definition.StateNode> state,
            params OneOf<string, Definition.StateNode>[] states)
        {
            DefinitionData.States = state.Append(states).Select(
                definition => definition.Match(name => new WithName(name), valid => valid));
            return new CompoundWithStates(this);
        }
    }
    public class CompoundWithStates : Definition.CompoundStateNode
    {
        internal DefinitionData DefinitionData { get; }

        internal CompoundWithStates(CompoundWithInitialState compoundWithInitialState)
            => DefinitionData = compoundWithInitialState.DefinitionData;
        internal CompoundWithStates(CompoundWithInitialActions compoundWithInitialActions)
            => DefinitionData = compoundWithInitialActions.DefinitionData;

        public override string Name => DefinitionData.Name;
        public override Option<IEnumerable<Definition.Transition>> Transitions => DefinitionData.Transitions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> EntryActions => DefinitionData.EntryActions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> ExitActions => DefinitionData.ExitActions.ToOption();
        public override Option<IEnumerable<Definition.Service>> Services => DefinitionData.Services.ToOption();
        public override Definition.InitialTransition InitialTransition => DefinitionData.InitialTransition;
        public override IEnumerable<Definition.StateNode> States => DefinitionData.States;
        public override Option<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition =>
            Option.None<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>>();

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

        public CompoundWithDoneTransition Child(string stateName) =>
            new CompoundWithDoneTransition(this, Keywords.Child(stateName));
        public CompoundWithDoneTransition Sibling(string stateName) =>
            new CompoundWithDoneTransition(this, Keywords.Sibling(stateName));
        public CompoundWithDoneTransition Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new CompoundWithDoneTransition(this, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public CompoundWithDoneTransition Multiple(Model.Target target, params Model.Target[] targets) =>
            new CompoundWithDoneTransition(this, target, targets);
    }
    public class CompoundWithDoneTransition : Definition.CompoundStateNode
    {
        internal DefinitionData DefinitionData { get; }
        internal Transition.WithTarget DoneTransitionBuilder { get; }

        public CompoundWithDoneTransition(CompoundWithDoneTransitionTo compound, Model.Target target, params Model.Target[] targets)
        {
            DefinitionData = compound.DefinitionData;
            DoneTransitionBuilder = WithEvent.OnCompoundDone().TransitionTo.Multiple(target, targets);
        }

        public CompoundWithDoneTransitionWithActions WithActions(Language.Action action, params Language.Action[] actions) =>
            new CompoundWithDoneTransitionWithActions(this, action, actions);

        public override string Name => DefinitionData.Name;
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> EntryActions => DefinitionData.EntryActions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> ExitActions => DefinitionData.ExitActions.ToOption();
        public override Option<IEnumerable<Definition.Transition>> Transitions => DefinitionData.Transitions.ToOption();
        public override Option<IEnumerable<Definition.Service>> Services => DefinitionData.Services.ToOption();
        public override IEnumerable<Definition.StateNode> States => DefinitionData.States;
        public override InitialTransition InitialTransition => DefinitionData.InitialTransition;
        public override Option<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition =>
            Option.From<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>>(DoneTransitionBuilder);
    }
    public class CompoundWithDoneTransitionWithActions : Definition.CompoundStateNode
    {
        internal DefinitionData DefinitionData { get; }
        internal Transition.WithActions DoneTransitionBuilder { get; }
        public CompoundWithDoneTransitionWithActions(CompoundWithDoneTransition compound, Language.Action action, Language.Action[] actions)
        {
            DefinitionData = compound.DefinitionData;
            DoneTransitionBuilder = compound.DoneTransitionBuilder.WithActions(action, actions);
        }

        public override string Name => DefinitionData.Name;
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> EntryActions => DefinitionData.EntryActions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> ExitActions => DefinitionData.ExitActions.ToOption();
        public override Option<IEnumerable<Definition.Transition>> Transitions => DefinitionData.Transitions.ToOption();
        public override Option<IEnumerable<Definition.Service>> Services => DefinitionData.Services.ToOption();
        public override IEnumerable<Definition.StateNode> States => DefinitionData.States;
        public override InitialTransition InitialTransition => DefinitionData.InitialTransition;
        public override Option<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition =>
            Option.From<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>>(DoneTransitionBuilder);
    }

    public class Orthogonal
    {
        internal DefinitionData DefinitionData { get; }

        internal Orthogonal(DefinitionData data)
            => DefinitionData = data;

        public OrthogonalWithStates WithStates(
            OneOf<string, Definition.StateNode> state,
            params OneOf<string, Definition.StateNode>[] states)
        {
            DefinitionData.States = state.Append(states).Select(
                definition => definition.Match(name => new WithName(name), valid => valid));
            return new OrthogonalWithStates(this);
        }
    }
    public class OrthogonalWithStates : Definition.OrthogonalStateNode
    {
        private DefinitionData DefinitionData { get; }

        internal OrthogonalWithStates(Orthogonal orthogonal)
            => DefinitionData = orthogonal.DefinitionData;

        public override string Name => DefinitionData.Name;
        public override Option<IEnumerable<Definition.Transition>> Transitions => DefinitionData.Transitions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> EntryActions => DefinitionData.EntryActions.ToOption();
        public override Option<IEnumerable<OneOf<Definition.Action, Definition.ContextAction>>> ExitActions => DefinitionData.ExitActions.ToOption();
        public override Option<IEnumerable<Definition.Service>> Services => DefinitionData.Services.ToOption();
        public override IEnumerable<Definition.StateNode> States => DefinitionData.States;
        public override Option<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition =>
            Option.None<OneOfUnion<Definition.Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>>();

        public object OnDone => throw new NotImplementedException();
    }
}
