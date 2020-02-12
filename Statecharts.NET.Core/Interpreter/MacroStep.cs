using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Interpreter
{
    internal class MacroStep
    {
        public MacroStep(IList<MicroStep> microSteps)
        {
            MicroSteps = microSteps;
            EnteredStateNodes = microSteps.Aggregate(Enumerable.Empty<StateNode>(), (entered, step) => step.Match(
                init => entered.Append(init.RootState),
                stabilization => entered.Concat(stabilization.EnteredStates),
                immediate => entered.Except(immediate.ExitedStates).Concat(immediate.EnteredStates),
                @event => entered.Except(@event.ExitedStates).Concat(@event.EnteredStates)));
        }

        public IEnumerable<MicroStep> MicroSteps { get; }
        public IEnumerable<StateNode> EnteredStateNodes { get; }
    }
}
