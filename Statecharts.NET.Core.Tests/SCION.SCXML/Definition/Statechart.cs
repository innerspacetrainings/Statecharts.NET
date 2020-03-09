using System.Collections.Generic;
using System.Linq;
using Jint;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.Shared.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class Statechart
    {
        internal Option<string> Name { get; set; }
        internal Option<string> InitialStateNodeName { get; set; }
        internal Option<ECMAScriptContext> InitialContext { get; set; }
        internal IList<StatenodeDefinition> Statenodes { get; } = new List<StatenodeDefinition>();

        internal StatechartDefinition<ECMAScriptContext> AsStatechartDefinition()
        {
            var initialContext = InitialContext.ValueOr(new ECMAScriptContext(new Engine()));
            var rootStateNode = new TestCompoundStatenodeDefinition(
                Name.ValueOr("root"),
                null,
                null,
                null,
                null,
                Statenodes,
                new InitialCompoundTransitionDefinition(new ChildTarget(InitialStateNodeName.ValueOr(Statenodes.First().Name))),
                Option.None<DoneTransitionDefinition>());

            return new StatechartDefinition<ECMAScriptContext>(initialContext, rootStateNode);
        }
    }
}
