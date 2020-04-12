using System;
using System.Collections.Generic;
using System.Text;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Demos
{
    internal class NoContext : IContext<NoContext>
    {
        public bool Equals(NoContext other) => true;
        public NoContext CopyDeep() => new NoContext();
    }
}
