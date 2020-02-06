using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Jint;
using Jint.Native;
using Statecharts.NET.Tests.Definition;
using Statecharts.NET.Tests.SCION.SCXML.Definition;
using Statecharts.NET.Tests.SCION.SCXML.ECMAScript.ParserDefinitions;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript
{
    static class SCXMLECMAScriptParser
    {
        private static Dictionary<string, System.Func<object>> ElementConstructors =>
            new Dictionary<string, System.Func<object>>
            {
                { "scxml", Construct.Statechart },
                { "datamodel", Construct.Context },
                { "state", Construct.AtomicState },
                { "transition", Construct.Transition }
            };
        private static Dictionary<(System.Type, string), System.Action<object, string>> AttributeSetters =>
            new Dictionary<(System.Type, string), System.Action<object, string>>
            {
                { (typeof(Statechart), "xmlns"), IntentionallyIgnore },
                { (typeof(Statechart), "version"), IntentionallyIgnore },
                { (typeof(Statechart), "datamodel"), IntentionallyIgnore },
                { (typeof(Statechart), "initial"), EraseType<Statechart>(Attribute.SetStatechartInitial) },
                { (typeof(AtomicStateNode), "id"), EraseType<AtomicStateNode>(Attribute.SetStateNodeName) },
                { (typeof(Transition), "event"), EraseType<Transition>(Attribute.SetTransitionEvent) },
                { (typeof(Transition), "target"), EraseType<Transition>(Attribute.SetTransitionTarget) }
            };
        private static Dictionary<(System.Type, System.Type), System.Action<object, object>> ElementSetters =>
            new Dictionary<(System.Type, System.Type), System.Action<object, object>>
            {
                { (typeof(Statechart), typeof(ECMAScriptContext)), EraseTypes<Statechart, ECMAScriptContext>(Element.SetStatechartInitialContext) },
                { (typeof(Statechart), typeof(AtomicStateNode)), EraseTypes<Statechart, Statecharts.NET.Definition.StateNode>(Element.StatechartAddStateNode) },
                { (typeof(AtomicStateNode), typeof(Transition)), EraseTypes<AtomicStateNode, Transition>(Element.StateNodeAddTransition) }
            };

        internal static Statecharts.NET.Definition.Statechart<ECMAScriptContext> ParseStatechart(string scxmlDefinition)
        {
            static object RecurseElement(object parent, XElement xElement)
            {
                static object Construct(XElement xElement)
                {
                    var name = xElement.Name.LocalName;
                    try { return ElementConstructors[name](); }
                    catch (System.Exception) { throw new System.Exception($"No Constructor registered for \"{name}\""); }
                }
                static void SetAttribute(object element, XAttribute attribute)
                {
                    var name = attribute.Name.LocalName;
                    try { AttributeSetters[(element.GetType(), name)](element, attribute.Value); }
                    catch (System.Exception) { throw new System.Exception($"No Attribute setter registered on \"{element.GetType()}\" for \"{name}\""); }
                }
                static void SetElement(object parent, object element)
                {
                    try { ElementSetters[(parent.GetType(), element.GetType())](parent, element); }
                    catch (System.Exception) { throw new System.Exception($"No Element setter registered on \"{parent.GetType()}\" for \"{element.GetType()}\""); }
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
        
        // TODO: remove
        private static ECMAScriptContext GetInitialContext(XElement datamodel, Engine engine, System.Func<string, XName> NSd)
        {
            var properties = datamodel.Elements().Where(element => element.Name == NSd("data"));
            foreach (var property in properties)
            {
                var id = property.Attribute("id")?.Value;
                var valueExpression = property.Attribute("expr")?.Value;
                var test = engine.Execute("(() => ({p1: 'v1', p2: 'v2'}))()").GetCompletionValue().ToObject();
                var test2 = engine.Execute("(function() {return 3;})()").GetCompletionValue().ToObject();
                if (id != null)
                    engine.SetValue(
                        id,
                        valueExpression != null
                            ? engine.Execute(valueExpression).GetCompletionValue()
                            : JsValue.Undefined);
            }

            return new ECMAScriptContext(engine);
        }
    }
}
