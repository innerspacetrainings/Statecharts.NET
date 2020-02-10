using System.Collections.Generic;
using System.Xml.Linq;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions;

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
                { "data", Construct.ContextDataEntry },
                { "onentry", Construct.EntryActions },
                { "log", Construct.LogAction },
                { "assign", Construct.AssignAction }
            };
        private static Dictionary<(System.Type, string), System.Action<object, string>> AttributeSetters =>
            new Dictionary<(System.Type, string), System.Action<object, string>>
            {
                { (typeof(Statechart), "xmlns"), IntentionallyIgnore },
                { (typeof(Statechart), "version"), IntentionallyIgnore },
                { (typeof(Statechart), "datamodel"), IntentionallyIgnore },
                { (typeof(Statechart), "initial"), EraseType<Statechart>(Attribute.SetStatechartInitial) },
                { (typeof(PartialStateNode), "id"), EraseType<PartialStateNode>(Attribute.SetStateNodeName) },
                { (typeof(FinalStateNode), "id"), EraseType<FinalStateNode>(Attribute.SetStateNodeName) },
                { (typeof(Transition), "event"), EraseType<Transition>(Attribute.SetTransitionEvent) },
                { (typeof(Transition), "target"), EraseType<Transition>(Attribute.SetTransitionTarget) },
                { (typeof(ContextDataEntry), "id"), EraseType<ContextDataEntry>(Attribute.SetContextDataEntryId) },
                { (typeof(ContextDataEntry), "expr"), EraseType<ContextDataEntry>(Attribute.SetContextDataEntryExpression) },
                { (typeof(LogAction), "expr"), EraseType<LogAction>(Attribute.SetLogExpression) },
                { (typeof(LogAction), "label"), EraseType<LogAction>(Attribute.SetLogLabel) },
                { (typeof(AssignAction), "location"), EraseType<AssignAction>(Attribute.SetAssignProperty) },
                { (typeof(AssignAction), "expr"), EraseType<AssignAction>(Attribute.SetAssignExpression) }
        };
        private static Dictionary<(System.Type, System.Type), System.Action<object, object>> ElementSetters =>
            new Dictionary<(System.Type, System.Type), System.Action<object, object>>
            {
                { (typeof(Statechart), typeof(ECMAScriptContext)), EraseTypes<Statechart, ECMAScriptContext>(Element.SetStatechartInitialContext) },
                { (typeof(Statechart), typeof(PartialStateNode)), EraseTypes<Statechart, StateNode>(Element.StatechartAddStateNode) },
                { (typeof(Statechart), typeof(FinalStateNode)), EraseTypes<Statechart, StateNode>(Element.StatechartAddStateNode) },
                { (typeof(PartialStateNode), typeof(Transition)), EraseTypes<PartialStateNode, Transition>(Element.StateNodeAddTransition) },
                { (typeof(PartialStateNode), typeof(EntryActions)), EraseTypes<PartialStateNode, EntryActions>(Element.StateNodeSetEntryActions) },
                { (typeof(PartialStateNode), typeof(PartialStateNode)), EraseTypes<PartialStateNode, PartialStateNode>(Element.StateNodeAddChildren) },
                { (typeof(FinalStateNode), typeof(EntryActions)), EraseTypes<FinalStateNode, EntryActions>(Element.StateNodeSetEntryActions) },
                { (typeof(ECMAScriptContext), typeof(ContextDataEntry)), EraseTypes<ECMAScriptContext, ContextDataEntry>(Element.ContextAddProperty) },
                { (typeof(Transition), typeof(LogAction)), EraseTypes<Transition, LogAction>(Element.TransitionAddLogAction) },
                { (typeof(Transition), typeof(AssignAction)), EraseTypes<Transition, AssignAction>(Element.TransitionAddAssignAction) },
                { (typeof(EntryActions), typeof(LogAction)), EraseTypes<EntryActions, LogAction>(Element.EntryActionsAddLogAction) },
                { (typeof(EntryActions), typeof(AssignAction)), EraseTypes<EntryActions, AssignAction>(Element.EntryActionsAddAssignAction) }
        };

        internal static Statecharts.NET.Definition.Statechart<ECMAScriptContext> ParseStatechart(string scxmlDefinition)
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
