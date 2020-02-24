using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET.Interpreter
{
    public class ActionBlock : IEnumerable<Model.Action>
    {
        private readonly IEnumerable<Action> _actions;
        private ActionBlock(IEnumerable<Model.Action> actions = null) => _actions =
            actions ?? Enumerable.Empty<Model.Action>();

        internal static ActionBlock Empty() => new ActionBlock();
        internal static ActionBlock From(IEnumerable<Model.Action> actions) => new ActionBlock(actions);

        public IEnumerator<Model.Action> GetEnumerator() => _actions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
