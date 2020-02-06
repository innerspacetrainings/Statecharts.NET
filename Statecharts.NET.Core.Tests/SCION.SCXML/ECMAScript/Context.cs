using System;
using Jint;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    internal class ECMAScriptContext : IEquatable<ECMAScriptContext>
    {
        public Engine Engine { get; }

        public ECMAScriptContext(Engine engine) => Engine = engine;

        public bool Equals(ECMAScriptContext other) => other != null && Engine.Global.GetOwnProperties().Equals(other.Engine.Global.GetOwnProperties()); // TODO: validate that this works

        public override string ToString()
            => $"EcmaScriptContext: (Retries = TODO)"; // TODO: serialize the stuff
    }
}
