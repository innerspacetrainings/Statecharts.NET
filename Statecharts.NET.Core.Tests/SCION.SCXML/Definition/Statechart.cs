using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
using Statecharts.NET.Definition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class Statechart
    {
        internal Option<string> Name { get; set; }
        internal Option<string> InitialStateNodeName { get; set; }
        internal Option<EcmaScriptContext> InitialContext { get; set; }
        internal IList<StateNode> StateNodes { get; } = new List<StateNode>();

        internal void AddStateNade(StateNode stateNode) => StateNodes.Add(stateNode);

        internal Statechart<EcmaScriptContext> AsRealDefinition()
        {
            var initialContext = InitialContext.ValueOr(new EcmaScriptContext(new Engine()));
            var rootStateNode = new Tests.Definition.CompoundStateNode
            {
                _name = Name.ValueOr("root"),
                _states = StateNodes,
                _initialTransition = new InitialTransition(new ChildTarget(InitialStateNodeName.ValueOr(StateNodes.First().Name)))
            };

            return new Statechart<EcmaScriptContext>(initialContext, rootStateNode);
        }
    }
}
