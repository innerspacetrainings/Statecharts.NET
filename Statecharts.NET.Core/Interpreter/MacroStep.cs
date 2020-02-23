using System.Collections;
using System.Collections.Generic;

namespace Statecharts.NET.Interpreter
{
    public class Macrostep : IEnumerable<MicroStep>
    {
        private readonly IEnumerable<MicroStep> _microSteps;
        public Macrostep(IEnumerable<MicroStep> microSteps) => _microSteps = microSteps;

        public IEnumerator<MicroStep> GetEnumerator() => _microSteps.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
