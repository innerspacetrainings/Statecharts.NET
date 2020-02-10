using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.Shared.Definition
{
    internal class AtomicStateNodeDefinition : Statecharts.NET.Definition.AtomicStateNode
    {
        public AtomicStateNodeDefinition(
            string name,
            IEnumerable<OneOf<Action, ContextAction>> entryActions,
            IEnumerable<OneOf<Action, ContextAction>> exitActions,
            IEnumerable<Transition> transitions,
            IEnumerable<Service> services)
        {
            Name = name;
            EntryActions = entryActions.ToOption();
            ExitActions = exitActions.ToOption();
            Transitions = transitions.ToOption();
            Services = services.ToOption();
        }

        public override string Name { get; }
        public override Option<IEnumerable<OneOf<Action, ContextAction>>> EntryActions { get; }
        public override Option<IEnumerable<OneOf<Action, ContextAction>>> ExitActions { get; }
        public override Option<IEnumerable<Transition>> Transitions { get; }
        public override Option<IEnumerable<Service>> Services { get; }
    }

    internal class CompoundStateNodeDefinition : Statecharts.NET.Definition.CompoundStateNode
    {
        public CompoundStateNodeDefinition(
            string name,
            IEnumerable<OneOf<Action, ContextAction>> entryActions,
            IEnumerable<OneOf<Action, ContextAction>> exitActions,
            IEnumerable<Transition> transitions,
            IEnumerable<Service> services,
            IEnumerable<StateNode> states,
            InitialTransition initialTransition,
            Option<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> doneTransition)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EntryActions = entryActions.ToOption();
            ExitActions = exitActions.ToOption();
            Transitions = transitions.ToOption();
            Services = services.ToOption();
            States = states;
            InitialTransition = initialTransition ?? throw new ArgumentNullException(nameof(initialTransition));
            DoneTransition = doneTransition;
        }

        public override string Name { get; }

        public override Option<IEnumerable<OneOf<Action, ContextAction>>> EntryActions { get; }
        public override Option<IEnumerable<OneOf<Action, ContextAction>>> ExitActions { get; }
        public override Option<IEnumerable<Transition>> Transitions { get; }
        public override Option<IEnumerable<Service>> Services { get; }
        public override IEnumerable<StateNode> States { get; }
        public override InitialTransition InitialTransition { get; }
        public override Option<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; }
    }

    internal class FinalStateNodeDefinition : Statecharts.NET.Definition.FinalStateNode
    {
        public FinalStateNodeDefinition(
            string name,
            IEnumerable<OneOf<Action, ContextAction>> entryActions,
            IEnumerable<OneOf<Action, ContextAction>> exitActions)
        {
            Name = name;
            EntryActions = entryActions.ToOption();
            ExitActions = exitActions.ToOption();
        }

        public override string Name { get; }
        public override Option<IEnumerable<OneOf<Action, ContextAction>>> EntryActions { get; }
        public override Option<IEnumerable<OneOf<Action, ContextAction>>> ExitActions { get; }
    }
}
