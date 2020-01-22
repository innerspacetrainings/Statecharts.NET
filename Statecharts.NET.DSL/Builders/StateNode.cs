using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Language.Service;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using static Statecharts.NET.Language.Keywords;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET.Language.StateNode
{
    internal class DefinitionData
    {
        public string Name { get; }
        public IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; set; }
        public IEnumerable<Definition.Transition> Transitions { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public IEnumerable<Model.Service> Services { get; set; }
        public Definition.InitialTransition InitialTransition { get; set; }
        public IEnumerable<Definition.StateNode> States { get; set; }

        public DefinitionData(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public class WithName : WithEntryActions
    {
        public WithName(string name) : base(name) { }

        public WithEntryActions WithEntryActions(
            OneOf<Action, ContextAction> action,
            params OneOf<Action, ContextAction>[] actions)
        {
            DefinitionData.EntryActions = action.Append(actions);
            return this;
        }
    }
    public class WithEntryActions : WithExitActions
    {
        internal WithEntryActions(string name) : base(name) { }

        public WithExitActions WithExitActions(
            OneOf<Action, ContextAction> action,
            params OneOf<Action, ContextAction>[] actions)
        {
            DefinitionData.ExitActions = action.Append(actions);
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
    }
    public class WithTransitions : WithActivities
    {
        internal WithTransitions(string name) : base(name) { }

        public WithActivities WithActivities(
            Activity activity,
            params Activity[] activities)
        {
            DefinitionData.Activities = activity.Append(activities);
            return this;
        }
    }

    public class WithActivities : WithServices
    {
        internal WithActivities(string name) : base(name) { }

        public WithServices WithServices(
            OneOf<ServiceLogic, Model.Service> service,
            params OneOf<ServiceLogic, Model.Service>[] services)
        {
            DefinitionData.Services = service.Append(services).Select(
                definition => definition.Match(
                    logic => new WithLogic(logic),
                    valid => valid));
            return this;
        }
    }
    public class WithServices : Definition.AtomicStateNode
    {
        private protected DefinitionData DefinitionData { get; }

        internal WithServices(string name) => DefinitionData = new DefinitionData(name);

        public override string Name => DefinitionData.Name;
        public override IEnumerable<Definition.Transition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<Activity> Activities => DefinitionData.Activities;
        public override IEnumerable<Model.Service> Services => DefinitionData.Services;

        public Final AsFinal() => new Final(DefinitionData);
        public Compound AsCompound() => new Compound(DefinitionData);
        public Orthogonal AsOrthogonal() => new Orthogonal(DefinitionData);
    }

    public class Final : Definition.FinalStateNode
    {
        private DefinitionData DefinitionData { get; }

        internal Final(DefinitionData data)
            => DefinitionData = data;

        public override string Name => DefinitionData.Name;
        public override IEnumerable<Definition.Transition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<Activity> Activities => DefinitionData.Activities;
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
            OneOf<Action, ContextAction> action,
            params OneOf<Action, ContextAction>[] actions)
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
        private DefinitionData DefinitionData { get; }

        internal CompoundWithStates(CompoundWithInitialState compoundWithInitialState)
            => DefinitionData = compoundWithInitialState.DefinitionData;
        internal CompoundWithStates(CompoundWithInitialActions compoundWithInitialActions)
            => DefinitionData = compoundWithInitialActions.DefinitionData;

        public override string Name => DefinitionData.Name;
        public override IEnumerable<Definition.Transition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<Activity> Activities => DefinitionData.Activities;
        public override IEnumerable<Model.Service> Services => DefinitionData.Services;
        public override Definition.InitialTransition InitialTransition => DefinitionData.InitialTransition;
        public override IEnumerable<Definition.StateNode> States => DefinitionData.States;
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
        public override IEnumerable<Definition.Transition> Transitions => DefinitionData.Transitions;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions => DefinitionData.EntryActions;
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions => DefinitionData.ExitActions;
        public override IEnumerable<Activity> Activities => DefinitionData.Activities;
        public override IEnumerable<Model.Service> Services => DefinitionData.Services;
        public override IEnumerable<Definition.StateNode> States => DefinitionData.States;
    }
}
