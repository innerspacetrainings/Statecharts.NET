using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Jint;
using Jint.Native;
using Statecharts.NET.Definition;

namespace Statecharts.NET.SCION.SCXML.Tests
{
    internal class EcmaScriptContext : IEquatable<EcmaScriptContext>
    {
        public Engine Engine { get; set; }

        public bool Equals(EcmaScriptContext other) => other != null && Engine == other.Engine; // TODO: not optimal

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
        internal static StatechartDefinition<EcmaScriptContext> TestXML(string scxmlDefinition)
        {
            var definition = XElement.Parse(scxmlDefinition);
            XName NSd(string name) => definition.GetDefaultNamespace() + name;
            var engine = new Engine();

            var datamodel = definition.Element(NSd("datamodel"));
            var context = datamodel != null ? GetInitialContext(datamodel, engine, NSd) : null;
            return new StatechartDefinition<EcmaScriptContext>() { InitialContext = context, StateNodeDefinition = GetStateNodeDefinition(definition, engine, NSd) };
        }

        private static BaseStateNodeDefinition<EcmaScriptContext> GetRootStateNodeDefinition(
            XElement definition,
            Engine engine,
            Func<string, XName> NSd)
        {
            var parallel = definition.Elements().FirstOrDefault(element => element.Name == NSd("parallel"));
            var topStates = definition.Elements().Where(element => element.Name == NSd("state") || element.Name == NSd("parallel"));
            var root = new XElement("state", new object[] { new XAttribute("id", "root")}.Concat(topStates));

            return parallel == null
                ? GetStateNodeDefinition(root, engine, NSd)
                : GetStateNodeDefinition(parallel, engine, NSd);
        }

        private static BaseStateNodeDefinition<EcmaScriptContext> GetStateNodeDefinition(XElement definition, Engine engine, Func<string, XName> NSd)
        {
            var events = definition.Elements(NSd("transition")).Select(e => new EventDefinition<Statecharts.NET.Event>()
            {
                Event = new NET.Event(e.Attribute("event")?.Value),
                Transitions = new[]
                {
                    new UnguardedEventTransitionDefinition()
                    {
                        Targets = new[]
                        {
                            new SiblingTargetDefinition()
                            {
                                Key = new NamedStateNodeKey(e.Attribute("target")?.Value)
                            }
                        }
                    }
                }
            });
            switch (definition)
            {
                case { } when definition.Name == NSd("scxml"):
                    return new CompoundStateNodeDefinition<EcmaScriptContext>()
                    {
                        Name = definition.Attribute("name")?.Value ?? "root",
                        InitialTransition = new InitialTransitionDefinition()
                        {
                            Target = new ChildTargetDefinition()
                            {
                                Key = new NamedStateNodeKey(definition.Attribute("initial")?.Value ?? definition.Substates(NSd).FirstOrDefault()?.Attribute("id")?.Value)
                            }
                        },
                        States = definition.Substates(NSd).Select(element => GetStateNodeDefinition(element, engine, NSd)),
                        Events = events
                    };
                case { } when definition.Name == NSd("state") && definition.Substates(NSd).Any():
                    return new CompoundStateNodeDefinition<EcmaScriptContext>()
                    {
                        Name = definition.Attribute("id")?.Value ?? "FUCK",
                        Events = events
                    };
                case { } when definition.Name == NSd("state") && !definition.Substates(NSd).Any():
                    return new AtomicStateNodeDefinition<EcmaScriptContext>()
                    {
                        Name = definition.Attribute("id")?.Value ?? "FUCK",
                        Events = events
                    };
                case { } when definition.Name == NSd("parallel"):
                    return new OrthogonalStateNodeDefinition<EcmaScriptContext>();
                case { } when definition.Name == NSd("final"):
                    return new FinalStateNodeDefinition<EcmaScriptContext>();
                default:
                    return null; // TODO: error handling
            }
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
