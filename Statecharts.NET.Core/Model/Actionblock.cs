using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Model
{
    public class Actionblock : IEnumerable<Action>
    {
        private readonly IEnumerable<Action> _actions;
        private Actionblock(IEnumerable<Action> actions = null) => _actions =
            actions ?? Enumerable.Empty<Action>();

        internal static Actionblock Empty() => new Actionblock();
        internal static Actionblock From(IEnumerable<Action> actions) => new Actionblock(actions);

        public IEnumerator<Action> GetEnumerator() => _actions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
