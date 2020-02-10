using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
using Statecharts.NET.Definition;
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
        internal IList<Statecharts.NET.Definition.StateNode> StateNodes { get; } = new List<Statecharts.NET.Definition.StateNode>();

        internal Statechart<ECMAScriptContext> AsStatechartDefinition()
        {
            var initialContext = InitialContext.ValueOr(new ECMAScriptContext(new Engine()));
            var rootStateNode = new CompoundStateNodeDefinition(
                Name.ValueOr("root"),
                null,
                null,
                null,
                null,
                StateNodes,
                new InitialTransition(new ChildTarget(InitialStateNodeName.ValueOr(StateNodes.First().Name))),
                Option.None<OneOfUnion<
                        Statecharts.NET.Definition.Transition,
                        Statecharts.NET.Definition.UnguardedTransition,
                        Statecharts.NET.Definition.UnguardedContextTransition,
                        Statecharts.NET.Definition.GuardedTransition,
                        Statecharts.NET.Definition.GuardedContextTransition>>());

            return new Statechart<ECMAScriptContext>(initialContext, rootStateNode);
        }
    }
}
