using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;
using static Statecharts.NET.Language.Keywords;
using Service = Statecharts.NET.Definition.Service;

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
                            .WithEntryActions<FetchContext>(
                                Run<FetchContext>(context => Console.WriteLine($"Entered loading state with context: {context}")),
                                Run(() => Console.WriteLine("parameterless Actions also compile *party*")),
                                //Run(() => throw new Exception("haha, i killed you")),
                                Raise("raise"),
                                Send("send"),
                                Log<FetchContext>(context => $"Entered loading state with context: {context}"))
                            .WithTransitions(
                                Immediately.If<FetchContext>(context => context.Retries >= 3).TransitionTo.Sibling("sheeeesh"),
                                On("RESOLVE").TransitionTo.Sibling("success"),
                                On("REJECT").TransitionTo.Sibling("failure")),
                        "success".AsFinal(),
                        "sheeeesh".AsFinal(),
                        "failure".WithTransitions(
                                On("RETRY").TransitionTo.Sibling("loading")
                                    .WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++)))
                            .WithInvocations(
                                Language.Service.DefineTask(async token => {
                                    Console.WriteLine(0);
                                    await System.Threading.Tasks.Task.Delay(1000);
                                    Console.WriteLine(1);
                                }).OnSuccess.TransitionTo.Sibling("sheeeesh"),
                                Language.Service.DefineActivity(
                                    () => Console.WriteLine("started"),
                                    () => Console.WriteLine("stopped")))));

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
                    var started = service.Start();
                    LogState(started.State);
                    while (true)
                    {
                        var eventType = Console.ReadLine();
                        var state = service.Send(new NamedEvent(eventType?.ToUpper()));
                        LogState(state);
                    }
                default:
                    Console.WriteLine("NOT EXECUTABLE");
                    break;
            }

        }

        private static void LogState(State<FetchContext> state)
        {
            Console.WriteLine("StateConfig:");
            Console.WriteLine(string.Join(Environment.NewLine, state.StateConfiguration.StateNodeIds.Select(text => $"  {text}")));
        }
    }
}
