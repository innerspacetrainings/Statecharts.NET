using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Model
{
    public class Actionblock : IEnumerable<ExecutableAction>
    {
        private readonly IEnumerable<ExecutableAction> _actions;
        private Actionblock(IEnumerable<ExecutableAction> actions = null) => _actions =
            actions ?? Enumerable.Empty<ExecutableAction>();

        internal static Actionblock Empty() => new Actionblock();
        internal static Actionblock From(IEnumerable<ExecutableAction> actions) => new Actionblock(actions);

        public IEnumerator<ExecutableAction> GetEnumerator() => _actions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"{this.Count()} ({string.Join(", ", _actions)})";
    }
}
