using System.Collections.Generic;

namespace Statecharts.NET.Interpreter
{
    internal class MacroStep
    {
        public MacroStep(IEnumerable<MicroStep> microSteps) => MicroSteps = microSteps;

        public IEnumerable<MicroStep> MicroSteps { get; }
    }
}
