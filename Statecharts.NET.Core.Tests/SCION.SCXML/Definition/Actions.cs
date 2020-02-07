using System.Collections.Generic;
using Jint;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal abstract class ActionsArray
    {
        internal IList<OneOf<Action, ContextAction>> Actions { get; } = new List<OneOf<Action, ContextAction>>();

        internal void AddAction(LogAction logAction) =>
            Actions.Add(logAction.AsContextAction());
        internal void AddAction(AssignAction assignAction) =>
            Actions.Add(assignAction.AsContextAction());
    }

    internal class EntryActions : ActionsArray { }

    internal class LogAction
    {
        internal string Expression { get; set; }
        internal string Label { get; set; }

        public LogContextAction AsContextAction() => new LogContextAction(
            context => $"{Label}: {((ECMAScriptContext) context).Engine.Execute(Expression).GetCompletionValue().AsString()}");
    }
    internal class AssignAction
    {
        internal string Property { get; set; }
        internal string Expression { get; set; }

        public AssignContextAction AsContextAction() => new AssignContextAction(
            context => ((ECMAScriptContext)context).Engine.SetValue(
                Property,
                $"({((ECMAScriptContext)context).Engine.Execute(Expression).GetCompletionValue()})"));
    }
}
