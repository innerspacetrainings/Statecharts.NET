using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Jint;
using Jint.Native;
using Statecharts.NET.Tests.Definition;
using Statecharts.NET.Tests.SCION.SCXML.Definition;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    internal class EcmaScriptContext : IEquatable<EcmaScriptContext>
    {
        public Engine Engine { get; set; }

        public bool Equals(EcmaScriptContext other) => other != null && Engine.Global.GetOwnProperties().Equals(other.Engine.Global.GetOwnProperties()); // TODO: validate that this works

        public override string ToString()
            => $"EcmaScriptContext: (Retries = TODO)"; // TODO: serialize the stuff
    }

    internal static class XmlExtensions
    {
        internal static IEnumerable<XElement> Substates(this XElement definition, Func<string, XName> NSd)
            => definition.Elements().Where(element => new [] {"state", "parallel", "final"}.Select(NSd).Contains(element.Name));
    }

    static class SCXMLEcmascript
    {
        private static Dictionary<string, Func<object>> elementConstructors =
            new Dictionary<string, Func<object>>
            {
                { "scxml", ConstructStatechart },
                { "datamodel", ConstructContext },
                { "state", ConstructAtomicState }
            };
        private static Dictionary<(Type, string), Action<object, string>> attributeSetters =
            new Dictionary<(Type, string), Action<object, string>>
            {
                { (typeof(Statechart), "xmlns"), IntentionallyIgnore },
                { (typeof(Statechart), "version"), IntentionallyIgnore },
                { (typeof(Statechart), "datamodel"), IntentionallyIgnore },
                { (typeof(Statechart), "initial"), EraseType<Statechart>(SetStatechartInitialAttribute) },
                { (typeof(AtomicStateNode), "id"), EraseType<AtomicStateNode>(SetStateNodeName) }
    };
        private static Dictionary<(Type, Type), Action<object, object>> elementSetters =
            new Dictionary<(Type, Type), Action<object, object>>
            {
                { (typeof(Statechart), typeof(EcmaScriptContext)), EraseTypes<Statechart, EcmaScriptContext>(SetStatechartInitialContext) },
                { (typeof(Statechart), typeof(AtomicStateNode)), EraseTypes<Statechart, AtomicStateNode>(AddStateNode) } // TODO: switch to StateNode
            };

        private static void SetStatechartInitialContext(Statechart statechart, EcmaScriptContext initialContext)
        { }
        //=> statechart.InitialContext = initialContext;

        private static void AddStateNode(Statechart statechart, AtomicStateNode initialContext)
        { }


        private static object ConstructStatechart() => new Statechart();
        private static object ConstructContext() => new EcmaScriptContext {Engine = new Engine()};
        private static object ConstructAtomicState() => new AtomicStateNode();

        private static Action<object, string> EraseType<T>(Action<T, string> setter)
            => (@object, value) => setter((T) @object, value);
        private static Action<object, object> EraseTypes<T1, T2>(Action<T1, T2> setter)
            => (object1, object2) => setter((T1)object1, (T2)object2);
        private static void IntentionallyIgnore(object o, string value) { }

        private static void SetStatechartInitialAttribute(Statechart statechart,
            string initialStateNode)
        { }
            //=> ((CompoundStateNode) statechart.RootStateNode).InitialTransition = new InitialTransition(new ChildTarget(initialStateNode));

        private static void SetStateNodeName(AtomicStateNode stateNode, string name) =>
            stateNode.SetName(name);

        internal static Statecharts.NET.Definition.Statechart<EcmaScriptContext> ParseStatechart(string scxmlDefinition)
        {
            ////XName NSd(string name) => definition.GetDefaultNamespace() + name; TODO: use this
            static object RecurseElement(object parent, XElement xElement)
            {
                var element = elementConstructors[xElement.Name.LocalName]();
                foreach (var attribute in xElement.Attributes())
                    attributeSetters[(element.GetType(), attribute.Name.LocalName)](element, attribute.Value);
                foreach(var children in xElement.Elements())
                    RecurseElement(element, children);
                if(parent != null) elementSetters[(parent.GetType(), element.GetType())](parent, element);
                return element;
            }

            var statechart = RecurseElement(null, XElement.Parse(scxmlDefinition)) as Statechart;
            return new Statecharts.NET.Definition.Statechart<EcmaScriptContext>(new EcmaScriptContext(), new Tests.Definition.AtomicStateNode());
        }

        private static EcmaScriptContext GetInitialContext(XElement datamodel, Engine engine, Func<string, XName> NSd)
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

            return new EcmaScriptContext {Engine = engine};
        }
    }
}
