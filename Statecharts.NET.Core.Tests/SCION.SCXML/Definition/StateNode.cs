using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.Shared.Definition;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal interface StateNode
    {
        Statecharts.NET.Definition.StateNode AsDefinition();
    }
    internal class PartialStateNode : StateNode
    {
        internal string Name { get; set; }
        internal Option<string> InitialStateNodeName { get; set; }
        internal IList<Statecharts.NET.Definition.StateNode> Children { get; } = new List<Statecharts.NET.Definition.StateNode>();
        internal IList<Statecharts.NET.Definition.Transition> Transitions { get; } = new List<NET.Definition.Transition>();
        internal IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; set; }
        internal Option<OneOfUnion<NET.Definition.Transition, NET.Definition.UnguardedTransition, UnguardedContextTransition, NET.Definition.GuardedTransition, GuardedContextTransition>> DoneTransition { get; set; }

        public Statecharts.NET.Definition.StateNode AsDefinition() =>
            Children.Any()
                ? new CompoundStateNodeDefinition(
                    Name,
                    EntryActions,
                    null,
                    Transitions,
                    null,
                    Children,
                    new InitialTransition(new ChildTarget(InitialStateNodeName.ValueOr(Children.First().Name))),
                    DoneTransition) as Statecharts.NET.Definition.StateNode
                : new AtomicStateNodeDefinition(
                    Name,
                    EntryActions,
                    null,
                    Transitions,
                    null);
    }

    internal class FinalStateNode : StateNode
    {
        public string Name { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; set; }
        public IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; set; }
        public Statecharts.NET.Definition.StateNode AsDefinition() =>
            new FinalStateNodeDefinition(Name, EntryActions, ExitActions);
    }
}
