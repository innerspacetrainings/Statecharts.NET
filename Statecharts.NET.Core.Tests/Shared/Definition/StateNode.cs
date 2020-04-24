using System;
using System.Collections.Generic;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.Shared.Definition
{
    internal class TestAtomicStatenodeDefinition : AtomicStatenodeDefinition
    {
        public TestAtomicStatenodeDefinition(
            string name,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> entryActions,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> exitActions,
            IEnumerable<TransitionDefinition> transitions,
            IEnumerable<ServiceDefinition> services)
        {
            Name = name;
            EntryActions = entryActions;
            ExitActions = exitActions;
            Transitions = transitions;
            Services = services;
        }

        public override string Name { get; }
        public override Option<string> UniqueIdentifier => Name.ToOption();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; }
        public override IEnumerable<TransitionDefinition> Transitions { get; }
        public override IEnumerable<ServiceDefinition> Services { get; }
    }

    internal class TestCompoundStatenodeDefinition : CompoundStatenodeDefinition
    {
        public TestCompoundStatenodeDefinition(
            string name,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> entryActions,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> exitActions,
            IEnumerable<TransitionDefinition> transitions,
            IEnumerable<ServiceDefinition> services,
            IEnumerable<StatenodeDefinition> statenodes,
            InitialCompoundTransitionDefinition initialTransition,
            Option<DoneTransitionDefinition> doneTransition)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EntryActions = entryActions;
            ExitActions = exitActions;
            Transitions = transitions;
            Services = services;
            Statenodes = statenodes;
            InitialTransition = initialTransition ?? throw new ArgumentNullException(nameof(initialTransition));
            DoneTransition = doneTransition;
        }

        public override string Name { get; }
        public override Option<string> UniqueIdentifier => Name.ToOption();

        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; }
        public override IEnumerable<TransitionDefinition> Transitions { get; }
        public override IEnumerable<ServiceDefinition> Services { get; }
        public override IEnumerable<StatenodeDefinition> Statenodes { get; }
        public override InitialCompoundTransitionDefinition InitialTransition { get; }
        public override Option<DoneTransitionDefinition> DoneTransition { get; }
    }

    internal class TestFinalStatenodeDefinition : FinalStatenodeDefinition
    {
        public TestFinalStatenodeDefinition(
            string name,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> entryActions,
            IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> exitActions)
        {
            Name = name;
            EntryActions = entryActions;
            ExitActions = exitActions;
        }

        public override string Name { get; }
        public override Option<string> UniqueIdentifier => Name.ToOption();
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; }
    }
}
