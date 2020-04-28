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
        private readonly bool _isParallel;
        internal string Name { get; set; }
        internal Option<OneOf<InitialTransition, string>> Initial { get; set; }
        internal IList<StatenodeDefinition> Children { get; } = new List<StatenodeDefinition>();
        internal IList<TransitionDefinition> Transitions { get; } = new List<TransitionDefinition>();
        internal IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> EntryActions { get; set; }
        internal IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ExitActions { get; set; }
        internal DoneTransitionDefinition DoneTransition { get; set; }

        public PartialStateNode(bool isParallel) => _isParallel = isParallel;

        public StatenodeDefinition AsDefinition() =>
            Children.Any()
                ? _isParallel
                    ? new TestOrthogonalStatenodeDefinition(
                        Name,
                        EntryActions,
                        ExitActions,
                        Transitions,
                        null,
                        Children,
                        DoneTransition.ToOption()) as StatenodeDefinition
                    : new TestCompoundStatenodeDefinition(
                        Name,
                        EntryActions,
                        ExitActions,
                        Transitions,
                        null,
                        Children,
                        Initial.Match(
                            some => some.Match(transition => transition.ToDefinition(),
                                statenodeName => new InitialCompoundTransitionDefinition(new ChildTarget(statenodeName))),
                            () => new InitialCompoundTransitionDefinition(new ChildTarget(Children.First().Name))), 
                        DoneTransition.ToOption())
                : new TestAtomicStatenodeDefinition(
                    Name,
                    EntryActions,
                    ExitActions,
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
