using System;
using Jint;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    internal class ECMAScriptContext : IContext<ECMAScriptContext>
    {
        public Engine Engine { get; }

        public ECMAScriptContext(Engine engine) => Engine = engine;

        public bool Equals(ECMAScriptContext other) => other != null && Engine.Global.GetOwnProperties().Equals(other.Engine.Global.GetOwnProperties()); // TODO: validate that this works

        public ECMAScriptContext CopyDeep()
        {
            var engine = new Engine();
            foreach (var (key, value) in Engine.Global.GetOwnProperties())
                engine.Global.FastSetProperty(key, value);
            return new ECMAScriptContext(engine);
        }

        public override string ToString()
            => $"EcmaScriptContext: (Retries = TODO)"; // TODO: serialize the stuff
    }
}
