using System;
using Jint;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Tests.Shared.Definition;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions
{
    internal static class Construct
    {
        internal static object Statechart() => new Statechart();
        internal static object Context() => new ECMAScriptContext(new Engine());
        internal static object PartialStateNode() => new PartialStateNode();
        internal static object OrthogonalStateNode() => throw new NotImplementedException();
        internal static object FinalStateNode() => new FinalStateNode();
        internal static object Transition() => new Transition();
        internal static object ContextDataEntry() => new ContextDataEntry();
        internal static object EntryActions() => new EntryActions();
        internal static object LogAction() => new LogAction();
        internal static object AssignAction() => new AssignAction();
    }
}
