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
            EntryActions = entryActions;
            ExitActions = exitActions;
            Transitions = transitions;
            Services = services;
        }

        public override string Name { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; }
        public override IEnumerable<Transition> Transitions { get; }
        public override IEnumerable<Service> Services { get; }
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
            Name = name;
            EntryActions = entryActions;
            ExitActions = exitActions;
            Transitions = transitions;
            Services = services;
            States = states;
            InitialTransition = initialTransition;
            DoneTransition = doneTransition;
        }

        public override string Name { get; }

        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; }
        public override IEnumerable<Transition> Transitions { get; }
        public override IEnumerable<Service> Services { get; }
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
            EntryActions = entryActions;
            ExitActions = exitActions;
        }

        public override string Name { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; }
    }
}
