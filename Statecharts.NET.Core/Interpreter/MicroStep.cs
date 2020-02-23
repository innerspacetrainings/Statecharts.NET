using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    public abstract class MicroStep : OneOfBase<InitializationStep, StabilizationStep, ImmediateStep, EventStep>
    {
        public IEnumerable<StateNode> ExitedStates =>
            Match(
                _ => Enumerable.Empty<StateNode>(),
                _ => Enumerable.Empty<StateNode>(),
                _ => Enumerable.Empty<StateNode>(),
                eventStep => eventStep.ExitedStates);

        public IEnumerable<StateNode> EnteredStates =>
            Match(
                initializationStep => initializationStep.RootState.Yield(),
                stabilizationStep => stabilizationStep.EnteredStates,
                immediateStep => immediateStep.EnteredStates,
                eventStep => eventStep.ExitedStates);

        public Option<Transition> Transition =>
            Match(
                initializationStep => Option.None<Transition>(),
                stabilizationStep => Option.None<Transition>(),
                immediateStep => (immediateStep.Transition as Transition).ToOption(),
                eventStep => eventStep.Transition.ToOption());
    }

    public class InitializationStep : MicroStep
    {
        public InitializationStep(StateNode rootState) => RootState = rootState;
        public StateNode RootState { get; }
    }
    public class StabilizationStep : MicroStep
    {
        public IEnumerable<StateNode> EnteredStates { get; }

        public StabilizationStep(IEnumerable<StateNode> enteredStates) =>
            EnteredStates = enteredStates ?? throw new ArgumentNullException(nameof(enteredStates));
    }
    public class ImmediateStep : MicroStep
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
    public class EventStep : MicroStep
    {
        public Model.IEvent Event { get; }
        public Transition Transition { get; }
        public IEnumerable<StateNode> EnteredStates { get; }
        public IEnumerable<StateNode> ExitedStates { get; }

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
    }
}
