using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.Definition
{
    internal class AtomicStateNode : Statecharts.NET.Definition.AtomicStateNode
    {
        internal string _name;
        internal IEnumerable<OneOf<Action, ContextAction>> _entryActions = Enumerable.Empty<OneOf<Action, ContextAction>>();
        internal IEnumerable<OneOf<Action, ContextAction>> _exitActions = Enumerable.Empty<OneOf<Action, ContextAction>>();
        internal IEnumerable<Transition> _transitions = Enumerable.Empty<Transition>();

        public override string Name => _name;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions => _entryActions;
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions => _exitActions;
        public override IEnumerable<Transition> Transitions => _transitions;
        public override IEnumerable<Service> Services { get; }
    }

    internal class CompoundStateNode : Statecharts.NET.Definition.CompoundStateNode
    {
        internal string _name;
        internal IEnumerable<StateNode> _states;
        internal InitialTransition _initialTransition;
        internal IEnumerable<Transition> _transitions = Enumerable.Empty<Transition>();

        public override string Name => _name;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; }
        public override IEnumerable<Transition> Transitions => _transitions;
        public override IEnumerable<Service> Services { get; }
        public override IEnumerable<StateNode> States => _states;
        public override InitialTransition InitialTransition => _initialTransition;
        public override Option<OneOfUnion<Transition, UnguardedTransition, UnguardedContextTransition, GuardedTransition, GuardedContextTransition>> DoneTransition { get; }
    }
}
