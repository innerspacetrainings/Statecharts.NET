using System.Collections;
using System.Collections.Generic;

namespace Statecharts.NET.Interpreter
{
    public class MacroStep : IEnumerable<MicroStep>
    {
        private readonly List<MicroStep> _microSteps = new List<MicroStep>();
        public IEnumerator<MicroStep> GetEnumerator()
            => _microSteps.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal void Add(IEnumerable<MicroStep> microSteps) => _microSteps.AddRange(microSteps);
    }
}
