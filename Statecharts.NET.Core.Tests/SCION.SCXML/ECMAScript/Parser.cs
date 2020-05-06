using System.Collections.Generic;
using System.Xml.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions;
using AssignAction = Statecharts.NET.Tests.SCION.SCXML.Definition.AssignAction;
using LogAction = Statecharts.NET.Tests.SCION.SCXML.Definition.LogAction;
using RaiseAction = Statecharts.NET.Tests.SCION.SCXML.Definition.RaiseAction;
using Transition = Statecharts.NET.Tests.SCION.SCXML.Definition.Transition;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript
{
    internal static class SCXMLECMAScriptParser
    {
        private static Dictionary<string, System.Func<object>> ElementConstructors =>
            new Dictionary<string, System.Func<object>>
            {
                { "scxml", Construct.Statechart },
                { "datamodel", Construct.Context },
                { "state", Construct.PartialStateNode },
                { "parallel", Construct.OrthogonalStateNode },
                { "final", Construct.FinalStateNode },
                { "transition", Construct.Transition },
                { "initial", Construct.InitialTransition },
                { "data", Construct.ContextDataEntry },
                { "onentry", Construct.EntryActions },
                { "onexit", Construct.ExitActions },
                { "log", Construct.LogAction },
                { "assign", Construct.AssignAction },
                { "raise", Construct.RaiseAction }
    };
        private static Dictionary<(System.Type, string), System.Action<object, string>> AttributeSetters =>
            new Dictionary<(System.Type, string), System.Action<object, string>>
            {
                { (typeof(Statechart), "xmlns"), IntentionallyIgnore },
                { (typeof(Statechart), "version"), IntentionallyIgnore },
                { (typeof(Statechart), "datamodel"), IntentionallyIgnore },
                { (typeof(Statechart), "ns0"), IntentionallyIgnore },
                { (typeof(Statechart), "initial"), EraseType<Statechart>(Attribute.SetStatechartInitial) },
                { (typeof(Statechart), "name"), EraseType<Statechart>(Attribute.SetStatechartName) },
                { (typeof(PartialStateNode), "id"), EraseType<PartialStateNode>(Attribute.SetStateNodeName) },
                { (typeof(PartialStateNode), "initial"), EraseType<PartialStateNode>(Attribute.SetStatenodeInitial) },
                { (typeof(FinalStateNode), "id"), EraseType<FinalStateNode>(Attribute.SetStateNodeName) },
                { (typeof(Transition), "event"), EraseType<Transition>(Attribute.SetTransitionEvent) },
                { (typeof(Transition), "target"), EraseType<Transition>(Attribute.SetTransitionTarget) },
                { (typeof(Transition), "cond"), EraseType<Transition>(Attribute.SetTransitionCondition) },
                { (typeof(ContextDataEntry), "id"), EraseType<ContextDataEntry>(Attribute.SetContextDataEntryId) },
                { (typeof(ContextDataEntry), "expr"), EraseType<ContextDataEntry>(Attribute.SetContextDataEntryExpression) },
                { (typeof(LogAction), "expr"), EraseType<LogAction>(Attribute.SetLogExpression) },
                { (typeof(LogAction), "label"), EraseType<LogAction>(Attribute.SetLogLabel) },
                { (typeof(AssignAction), "location"), EraseType<AssignAction>(Attribute.SetAssignProperty) },
                { (typeof(AssignAction), "expr"), EraseType<AssignAction>(Attribute.SetAssignExpression) },
                { (typeof(RaiseAction), "event"), EraseType<RaiseAction>(Attribute.SetRaiseEvent) },
            };
        private static Dictionary<(System.Type, System.Type), System.Action<object, object>> ElementSetters =>
            new Dictionary<(System.Type, System.Type), System.Action<object, object>>
            {
                { (typeof(Statechart), typeof(ECMAScriptContext)), EraseTypes<Statechart, ECMAScriptContext>(Element.SetStatechartInitialContext) },
                { (typeof(Statechart), typeof(PartialStateNode)), EraseTypes<Statechart, IStatenodeDefinition>(Element.StatechartAddStateNode) },
                { (typeof(Statechart), typeof(FinalStateNode)), EraseTypes<Statechart, IStatenodeDefinition>(Element.StatechartAddStateNode) },
                { (typeof(PartialStateNode), typeof(Transition)), EraseTypes<PartialStateNode, Transition>(Element.StateNodeAddTransition) },
                { (typeof(PartialStateNode), typeof(EntryActions)), EraseTypes<PartialStateNode, EntryActions>(Element.StateNodeSetEntryActions) },
                { (typeof(PartialStateNode), typeof(ExitActions)), EraseTypes<PartialStateNode, ExitActions>(Element.StateNodeSetExitActions) },
                { (typeof(PartialStateNode), typeof(PartialStateNode)), EraseTypes<PartialStateNode, PartialStateNode>(Element.StateNodeAddChildren) },
                { (typeof(PartialStateNode), typeof(InitialTransition)), EraseTypes<PartialStateNode, InitialTransition>(Element.StateNodeSetInitialTransition) },
                { (typeof(FinalStateNode), typeof(EntryActions)), EraseTypes<FinalStateNode, EntryActions>(Element.StateNodeSetEntryActions) },
                { (typeof(ECMAScriptContext), typeof(ContextDataEntry)), EraseTypes<ECMAScriptContext, ContextDataEntry>(Element.ContextAddProperty) },
                { (typeof(Transition), typeof(LogAction)), EraseTypes<Transition, LogAction>(Element.TransitionAddLogAction) },
                { (typeof(Transition), typeof(AssignAction)), EraseTypes<Transition, AssignAction>(Element.TransitionAddAssignAction) },
                { (typeof(Transition), typeof(RaiseAction)), EraseTypes<Transition, RaiseAction>(Element.TransitionAddRaiseAction) },
                { (typeof(InitialTransition), typeof(Transition)), EraseTypes<InitialTransition, Transition>(Element.InitialTransitionAddTransition) },
                { (typeof(EntryActions), typeof(LogAction)), EraseTypes<EntryActions, LogAction>(Element.EntryActionsAddLogAction) },
                { (typeof(EntryActions), typeof(AssignAction)), EraseTypes<EntryActions, AssignAction>(Element.EntryActionsAddAssignAction) },
                { (typeof(EntryActions), typeof(RaiseAction)), EraseTypes<EntryActions, RaiseAction>(Element.EntryActionsAddRaiseAction) },
                { (typeof(ExitActions), typeof(LogAction)), EraseTypes<ExitActions, LogAction>(Element.ExitActionsAddLogAction) },
                { (typeof(ExitActions), typeof(AssignAction)), EraseTypes<ExitActions, AssignAction>(Element.ExitActionsAddAssignAction) },
                { (typeof(ExitActions), typeof(RaiseAction)), EraseTypes<ExitActions, RaiseAction>(Element.ExitActionsAddRaiseAction) }
        };

        internal static StatechartDefinition<ECMAScriptContext> ParseStatechart(string scxmlDefinition)
        {
            static object RecurseElement(object parent, XElement xElement)
            {
                static object Construct(XElement xElement)
                {
                    var name = xElement.Name.LocalName;
                    try { return ElementConstructors[name](); }
                    catch (KeyNotFoundException) { throw new System.Exception($"No Constructor registered for \"{name}\""); }
                }
                static void SetAttribute(object element, XAttribute attribute)
                {
                    var name = attribute.Name.LocalName;
                    try { AttributeSetters[(element.GetType(), name)](element, attribute.Value); }
                    catch (KeyNotFoundException) { throw new System.Exception($"Attribute-Setter {element.GetType().Name}.Set(\"{name}\") missing"); }
                }
                static void SetElement(object parent, object element)
                {
                    try { ElementSetters[(parent.GetType(), element.GetType())](parent, element); }
                    catch (KeyNotFoundException) { throw new System.Exception($"Element-Setter {parent.GetType().Name}.Set({element.GetType().Name}) missing"); }
                }

                var element = Construct(xElement);
                foreach (var attribute in xElement.Attributes())
                    SetAttribute(element, attribute);
                foreach(var children in xElement.Elements())
                    RecurseElement(element, children);
                if(parent != null) SetElement(parent, element);
                return element;
            }

            var statechart = RecurseElement(null, XElement.Parse(scxmlDefinition)) as Statechart;
            return statechart?.AsStatechartDefinition();
        }

        #region Helpers
        private static void IntentionallyIgnore(object o, string value) { }
        private static System.Action<object, string> EraseType<T>(System.Action<T, string> setter)
            => (@object, value) => setter((T) @object, value);
        private static System.Action<object, object> EraseTypes<T1, T2>(System.Action<T1, T2> setter)
            => (object1, object2) => setter((T1)object1, (T2)object2);
        #endregion
    }
}
