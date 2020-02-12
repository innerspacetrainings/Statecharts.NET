using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    internal abstract class MicroStep : OneOfBase<InitializationStep, StabilizationStep, ImmediateStep, EventStep> { }

    internal class InitializationStep : MicroStep
    {
        public StateNodeId RootStateId => new StateNodeId(new RootStateNodeKey(string.Empty)); // TODO: fix this
    }
    internal class StabilizationStep : MicroStep
    {
        public IEnumerable<StateNode> EnteredStates { get; }

        public StabilizationStep(IEnumerable<StateNode> enteredStates) =>
            EnteredStates = enteredStates ?? throw new ArgumentNullException(nameof(enteredStates));
    }
    internal class ImmediateStep : MicroStep
    {
        public UnguardedTransition Transition { get; }
        public IEnumerable<StateNode> EnteredStates { get; }
        public IEnumerable<StateNode> ExitedStates { get; }

        public ImmediateStep(
            UnguardedTransition transition,
            IEnumerable<StateNode> enteredStates,
            IEnumerable<StateNode> exitedStates)
        {
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
            EnteredStates = enteredStates ?? throw new ArgumentNullException(nameof(enteredStates));
            ExitedStates = exitedStates ?? throw new ArgumentNullException(nameof(exitedStates));
        }
    }
    internal class EventStep : MicroStep
    {
        public Model.Event Event { get; }
        public Transition Transition { get; }
        public IEnumerable<StateNode> EnteredStates { get; }
        public IEnumerable<StateNode> ExitedStates { get; }

        public EventStep(
            Model.Event @event,
            Transition transition,
            IEnumerable<StateNode> enteredStates,
            IEnumerable<StateNode> exitedStates)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
            EnteredStates = enteredStates ?? throw new ArgumentNullException(nameof(enteredStates));
            ExitedStates = exitedStates ?? throw new ArgumentNullException(nameof(exitedStates));
        }
    }
}
