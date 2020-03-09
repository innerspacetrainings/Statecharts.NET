using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript
{
    internal static class Extensions
    {
        internal static IList<string> Ids(this State<ECMAScriptContext> state)
            => state.StateConfiguration.NonRootIds.ToList();
    }
}
