using System;
using System.Collections.Generic;
using System.Linq;
using Jint;
using Jint.Runtime.Descriptors;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    internal class ECMAScriptContext : IContext<ECMAScriptContext>
    {
        public Engine Engine { get; }
        private readonly IList<string> _systemProperties;

        private IEnumerable<string> AddedProperties =>
            Engine.Global.GetOwnProperties().Select(property => property.Key).Where(property => !_systemProperties.Contains(property));

        public ECMAScriptContext(Engine engine)
        {
            Engine = engine;
            _systemProperties = Engine.Global.GetOwnProperties().Select(property => property.Key).ToList();
        }

        public bool Equals(ECMAScriptContext other) => other != null && Engine.Global.GetOwnProperties().Equals(other.Engine.Global.GetOwnProperties()); // TODO: validate that this works

        public ECMAScriptContext CopyDeep()
        {
            ////var engine = new Engine();
            ////foreach (var (key, value) in Engine.Global.GetOwnProperties())
            ////    engine.Global.FastSetProperty(key, Engine.GetValue("").);
            ////return new ECMAScriptContext(engine);
            return this;
        }

        public override string ToString()
            => string.Join(Environment.NewLine, AddedProperties.Select(name => $"{name}: {Engine.GetValue(name)}"));

        public string AsString => ToString();
    }
}
