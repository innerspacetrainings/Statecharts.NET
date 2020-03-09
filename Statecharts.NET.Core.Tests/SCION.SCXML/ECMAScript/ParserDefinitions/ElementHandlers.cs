using Jint.Native;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Utilities;
using AssignAction = Statecharts.NET.Tests.SCION.SCXML.Definition.AssignAction;
using LogAction = Statecharts.NET.Tests.SCION.SCXML.Definition.LogAction;
using Transition = Statecharts.NET.Tests.SCION.SCXML.Definition.Transition;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions
{
    internal static class Element
    {
        internal static void SetStatechartInitialContext(Statechart statechart, ECMAScriptContext initialContext)
            => statechart.InitialContext = initialContext.ToOption();
        internal static void StatechartAddStateNode(Statechart statechart, IStatenodeDefinition stateNode)
            => statechart.Statenodes.Add(stateNode.AsDefinition());
        internal static void StateNodeAddTransition(PartialStateNode stateNode, Transition transition)
            => stateNode.Transitions.Add(transition.AsTransitionDefinition());
        public static void StateNodeSetEntryActions(PartialStateNode stateNode, EntryActions entryActions)
            => stateNode.EntryActions = entryActions.Actions;
        public static void StateNodeSetEntryActions(FinalStateNode stateNode, EntryActions entryActions)
            => stateNode.EntryActions = entryActions.Actions;
        public static void StateNodeAddChildren(PartialStateNode stateNode, PartialStateNode children)
            => stateNode.Children.Add(children.AsDefinition());
        public static void ContextAddProperty(ECMAScriptContext context, ContextDataEntry entry)
            => context.Engine.SetValue(
                entry.Id,
                entry.ValueExpression
                    .Map(expression => $"({expression})")
                    .Map(expression => context.Engine.Execute(expression).GetCompletionValue())
                    .ValueOr(JsValue.Undefined));
        public static void TransitionAddLogAction(Transition transition, LogAction logAction)
            => transition.AddAction(logAction);
        public static void TransitionAddAssignAction(Transition transition, AssignAction assignAction)
            => transition.AddAction(assignAction);
        public static void EntryActionsAddLogAction(EntryActions entryActions, LogAction logAction)
            => entryActions.AddAction(logAction);
        public static void EntryActionsAddAssignAction(EntryActions entryActions, AssignAction assignAction)
            => entryActions.AddAction(assignAction);
    }
}
