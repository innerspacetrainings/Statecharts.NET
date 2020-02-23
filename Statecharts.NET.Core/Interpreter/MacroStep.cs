using System.Collections;
using System.Collections.Generic;

namespace Statecharts.NET.Interpreter
{
    public class Macrostep : IEnumerable<MicroStep>
    {
        private readonly IEnumerable<MicroStep> _microSteps;
        private readonly IList<ErrorStep> _errorSteps = new IList<ErrorStep>();
        public Macrostep(IEnumerable<MicroStep> microSteps) => _microSteps = microSteps;

        public IEnumerator<MicroStep> GetEnumerator()
        {
            foreach (var microStep in _microSteps)
                yield return microStep;
            foreach (var errorStep in _errorSteps)
                yield return errorStep;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void AddErrorStep(ErrorStep step) => _errorSteps.Add(step);
    }
}
