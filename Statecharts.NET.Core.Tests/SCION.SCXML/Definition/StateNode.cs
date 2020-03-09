using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.Shared.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal interface IStatenodeDefinition
    {
        StatenodeDefinition AsDefinition();
    }
    internal class PartialStateNode : IStatenodeDefinition
    {
        internal string Name { get; set; }
        internal Option<string> InitialStateNodeName { get; set; }
        internal IList<StatenodeDefinition> Children { get; } = new List<StatenodeDefinition>();
        internal IList<TransitionDefinition> Transitions { get; } = new List<TransitionDefinition>();
        internal IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; set; }
        internal DoneTransitionDefinition DoneTransition { get; set; }

        public StatenodeDefinition AsDefinition() =>
            Children.Any()
                ? new TestCompoundStatenodeDefinition(
                    Name,
                    EntryActions,
                    null,
                    Transitions,
                    null,
                    Children,
                    new InitialCompoundTransitionDefinition(new ChildTarget(InitialStateNodeName.ValueOr(Children.First().Name))), 
                    DoneTransition.ToOption()) as StatenodeDefinition
                : new TestAtomicStatenodeDefinition(
                    Name,
                    EntryActions,
                    null,
                    Transitions,
                    null);
    }

    internal class FinalStateNode : IStatenodeDefinition
    {
        public string Name { get; set; }
        public IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; set; }
        public IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; set; }
        public StatenodeDefinition AsDefinition() =>
            new TestFinalStatenodeDefinition(Name, EntryActions, ExitActions);
    }
}
