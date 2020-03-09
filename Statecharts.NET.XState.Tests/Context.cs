using System;
using Statecharts.NET.Interfaces;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;


namespace Statecharts.NET.XState.Tests
{
    class DemoContext : IContext<DemoContext>, IXStateSerializable
    {
        public bool Equals(DemoContext other) => true;
        public ObjectValue AsJSObject() => ObjectValue(("nothing", 42));
        public DemoContext CopyDeep() => new DemoContext();
    }
}
