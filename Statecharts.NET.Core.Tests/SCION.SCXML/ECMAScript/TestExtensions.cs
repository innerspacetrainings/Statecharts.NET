using System.Collections.Generic;
using System.Linq;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript
{
    internal static class Extensions
    {
        internal static IEnumerable<string> Ids(this State<ECMAScriptContext> state)
            => state.StateConfiguration.StateNodeIds.Select(id => string.Join(".", id.Path.Select(key => key.Map(_ => null, named => named.StateName)).Where(key => key != null))).Where(id => !string.IsNullOrEmpty(id));
    }
}
