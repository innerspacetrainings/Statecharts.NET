using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.SCION.SCXML.Tests;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    internal static class Extensions
    {
        internal static IEnumerable<string> Ids(this State<EcmaScriptContext> state)
            => state.StateConfiguration.StateNodeIds.Select(id => string.Join(".", id.Path.Select(key => key.Map(_ => null, named => named.StateName)).Where(key => key != null))).Where(id => !string.IsNullOrEmpty(id));
    }
}
