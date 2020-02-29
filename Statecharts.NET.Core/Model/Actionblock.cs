using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Model
{
    public class ActionBlock : IEnumerable<Action>
    {
        private readonly IEnumerable<Action> _actions;
        private ActionBlock(IEnumerable<Action> actions = null) => _actions =
            actions ?? Enumerable.Empty<Action>();

        internal static ActionBlock Empty() => new ActionBlock();
        internal static ActionBlock From(IEnumerable<Action> actions) => new ActionBlock(actions);

        public IEnumerator<Action> GetEnumerator() => _actions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
