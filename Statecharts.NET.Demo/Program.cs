using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;
using static Statecharts.NET.Language.Keywords;

namespace Statecharts.NET.Demo
{
    internal class FetchContext : IEquatable<FetchContext>, IXStateSerializable
    {
        public int Retries { get; set; }

        public bool Equals(FetchContext other) => other != null && Retries == other.Retries;

        ObjectValue IXStateSerializable.AsJSObject()
            => ObjectValue(("retries", Retries));

        public override string ToString()
            => $"FetchContext: (Retries = {Retries})";
    }

    internal static class Program
    {
        private static readonly Statechart<FetchContext> FetchDefinition = Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "fetch"
                    .AsCompound()
                    .WithInitialState("idle")
                    .WithStates(
                        "idle"
                            .WithTransitions(
                                Ignore("GATHERUSERDATA"),
                                On("FETCH").TransitionTo.Sibling("loading"))
                            .AsOrthogonal()
                            .WithStates(
                                "really".WithTransitions(
                                        On("YES").TransitionTo.Absolute("fetch", "loading"),
                                        On("NO").TransitionTo.Sibling("nana")),
                                "nana".WithTransitions(On("SERIOUSLY").TransitionTo.Absolute("fetch", "failure"))),
                        "loading"
                            .WithEntryActions(Run<FetchContext>(context => Console.WriteLine($"Entered loading state with context: {context}")))
                            .WithTransitions(
                                Immediately.If<FetchContext>(context => context.Retries >= 3).TransitionTo.Sibling("sheeeesh"),
                                On("RESOLVE").TransitionTo.Sibling("success"),
                                On("REJECT").TransitionTo.Sibling("failure")),
                        "success".AsFinal(),
                        "sheeeesh".AsFinal(),
                        "failure".WithTransitions(
                                On("RETRY").TransitionTo.Sibling("loading")
                                    .WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++)))));

        private static void Main()
        {
            var definition = FetchDefinition;
            Console.WriteLine(definition.AsXStateVisualizerV4Definition());

            var parsedStatechart = definition.Parse();
            Console.WriteLine($"Parsing the definition of the Statechart resulted in {parsedStatechart.GetType().Name}");

            switch (parsedStatechart)
            {
                case ExecutableStatechart<FetchContext> statechart:
                    var service = statechart.Interpret();
                    var state = service.Start();
                    Log(state);
                    while (true)
                    {
                        var eventType = Console.ReadLine();
                        state = service.Send(new CustomEvent(eventType?.ToUpper()));
                        Log(state);
                    }
                default:
                    Console.WriteLine("NOT EXECUTABLE");
                    break;
            }

        }

        private static void Log(State<FetchContext> state)
        {
            Console.WriteLine("StateConfig:");
            Console.WriteLine(string.Join(Environment.NewLine, state.StateConfiguration.StateNodeIds.Select(text => $"  {text}")));
        }
    }
}
