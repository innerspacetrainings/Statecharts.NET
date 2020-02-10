using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Tests.Shared.Definition;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class PartialStateNode
    {
        internal string Name { get; set; }
        internal IList<StateNode> Children { get; } = new List<NET.Definition.StateNode>();
        internal IList<Statecharts.NET.Definition.Transition> Transitions { get; } = new List<NET.Definition.Transition>();
        internal IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; set; }
        internal Option<OneOfUnion<NET.Definition.Transition, NET.Definition.UnguardedTransition, UnguardedContextTransition, NET.Definition.GuardedTransition, GuardedContextTransition>> DoneTransition { get; set; }

        internal Statecharts.NET.Definition.StateNode AsDefinition() =>
            Children.Any()
                ? new CompoundStateNodeDefinition(
                    Name,
                    EntryActions,
                    null,
                    Transitions,
                    null,
                    Children,
                    null,
                    DoneTransition) as StateNode
                : new AtomicStateNodeDefinition(
                    Name,
                    EntryActions,
                    null,
                    Transitions,
                    null);
    }
}
