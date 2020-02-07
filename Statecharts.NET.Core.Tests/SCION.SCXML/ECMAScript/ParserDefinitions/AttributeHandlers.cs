﻿using Statecharts.NET.Tests.Definition;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions
{
    internal static class Attribute
    {
        internal static void SetStatechartInitial(Statechart statechart, string initialStateNode) =>
            statechart.InitialStateNodeName = initialStateNode.ToOption();
        internal static void SetStateNodeName(AtomicStateNode stateNode, string name) =>
            stateNode._name = name;
        public static void SetTransitionEvent(Transition transition, string @event) =>
            transition._event = @event;
        public static void SetTransitionTarget(Transition transition, string target) =>
            transition._target = target;
        public static void SetContextDataEntryId(ContextDataEntry entry, string id) =>
            entry.Id = id;
        public static void SetContextDataEntryExpression(ContextDataEntry entry, string expression) =>
            entry.ValueExpression = expression.ToOption();
        public static void SetLogExpression(LogAction logAction, string expression) =>
            logAction.Expression = expression;
        public static void SetLogLabel(LogAction logAction, string label) =>
            logAction.Label = label;
        public static void SetAssignProperty(AssignAction assignAction, string property) =>
            assignAction.Property = property;
        public static void SetAssignExpression(AssignAction assignAction, string expression) =>
            assignAction.Expression = expression;
    }
}
