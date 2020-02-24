using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Statecharts.NET.Interpreter
{
    public class ActionBlock : IEnumerable<Model.Action>
    {
        public IEnumerator<Model.Action> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
