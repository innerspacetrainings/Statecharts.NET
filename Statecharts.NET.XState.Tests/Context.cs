using System;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;


namespace Statecharts.NET.XState.Tests
{
    class DemoContext : IEquatable<DemoContext>, IXStateSerializable
    {
        public bool Equals(DemoContext other) => true;
        public ObjectValue AsJSObject() => ObjectValue(("nothing", 42));
    }
}
