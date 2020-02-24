using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    public class MicroStep
    {
        public Model.IEvent Event { get; }
        public Transition Transition { get; }
        public IEnumerable<StateNode> EnteredStateNodes { get; }
        public IEnumerable<StateNode> ExitedStateNodes { get; }

        public EventStep(
            Model.IEvent @event,
            Transition transition,
            IEnumerable<StateNode> enteredStates,
            IEnumerable<StateNode> exitedStates)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
            EnteredStates = enteredStates ?? throw new ArgumentNullException(nameof(enteredStates));
            ExitedStates = exitedStates ?? throw new ArgumentNullException(nameof(exitedStates));
        }

        public IEnumerable<ActionBlock> EnteredActionBlocks =>
            EnteredStateNodes.Select(stateNode => stateNode.EntryActions);
        public IEnumerable<ActionBlock> ExitedActionBlocks =>
            ExitedStateNodes.Select(stateNode => stateNode.ExitActions);
        public ActionBlock TransitionActionBlock =>
            Transition.Actions;
    }
}
