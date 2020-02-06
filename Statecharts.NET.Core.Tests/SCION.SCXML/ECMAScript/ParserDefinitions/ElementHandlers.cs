using Statecharts.NET.Tests.Definition;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions
{
    internal static class Element
    {
        internal static void SetStatechartInitialContext(Statechart statechart, ECMAScriptContext initialContext)
            => statechart.InitialContext = initialContext.ToOption();
        internal static void StatechartAddStateNode(Statechart statechart, Statecharts.NET.Definition.StateNode stateNode)
            => statechart.AddStateNade(stateNode);

        internal static void StateNodeAddTransition(AtomicStateNode stateNode, Transition transition)
            => stateNode._transitions.Add(transition.AsTransitionDefinition());
    }
}
