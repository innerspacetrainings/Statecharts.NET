using System;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;
using static Statecharts.NET.Language.Keywords;

namespace Statecharts.NET.Demo
{
    internal class FetchContext : IContext<FetchContext>, IXStateSerializable
    {
        public int Retries { get; set; }

        public bool Equals(FetchContext other) => other != null && Retries == other.Retries;

        ObjectValue IXStateSerializable.AsJSObject()
            => ObjectValue(("retries", Retries));

        public FetchContext CopyDeep() => new FetchContext {Retries = Retries};

        public override string ToString()
            => $"FetchContext: (Retries = {Retries})";
    }

    internal static class Program
    {
        private static readonly StatechartDefinition<FetchContext> FetchDefinition = Statechart
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
                        "test".AsCompound().WithInitialState("1").WithStates("1").OnDone.TransitionTo.Sibling("success"),
                        "failure".WithTransitions(
                                On("RETRY").TransitionTo.Sibling("loading")
                                    .WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++)))
                            .WithInvocations(
                                Language.Service.DefineTask(async token => {
                                    Console.WriteLine("waiting 3 seconds");
                                    await System.Threading.Tasks.Task.Delay(3000, token);
                                    Console.WriteLine("waiting finished");
                                }).OnSuccess.TransitionTo.Sibling("sheeeesh"),
                                Language.Service.DefineActivity(
                                    () => Console.WriteLine("started"),
                                    () => Console.WriteLine("stopped")))));

        private static NamedEvent Increment => Event.Define("INCREMENT");
        private static NamedDataEventFactory<int> IncrementBy => Event.Define("INCREMENTBY").WithData<int>();

        private static readonly StatechartDefinition<FetchContext> DemoDefinition = Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "demo"
                    .AsCompound()
                    .WithInitialState("1")
                    .WithStates(
                        "1".WithTransitions(
                            On("START").TransitionTo.Sibling("mc"),
                            //On(IncrementBy).TransitionTo.Sibling("mc"),
                            On(Increment).TransitionTo.Self.WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++))),
                        "mc".WithTransitions(On("RETRY").TransitionTo.Child("initial"))
                            .AsCompound().WithInitialState("initial").WithStates(
                            "initial".WithTransitions(
                                On("START").TransitionTo.Sibling("selecting")),
                            "selecting".WithTransitions(
                                On("CORRECT").TransitionTo.Sibling("solved")),
                            "solved".AsFinal())
                            .OnDone.TransitionTo.Sibling("final"),
                        "final".AsFinal()));

        private static void Main()
        {
            var definition = DemoDefinition;
            Console.WriteLine(definition.AsXStateVisualizerV4Definition());

            var statechart = Parser.Parse(definition) as ExecutableStatechart<FetchContext>;
            var running = Interpreter.Interpret(statechart);

            running.OnMacroStep += macrostep =>
            {
                Console.WriteLine($"StateNodes: {string.Join(", ", macrostep.State.StateConfiguration.StateNodeIds)}");
                Console.WriteLine($"Context: {macrostep.State.Context}");
            };

            running.Start().ContinueWith(_ => Environment.Exit(0));

            while (true)
            {
                Console.WriteLine($"Next possible events: {string.Join(", ", running.NextEvents)}");
                var eventType = Console.ReadLine();
                running.Send(new NamedEvent(eventType?.ToUpper()));
            }
        }

        private static void LogState(State<FetchContext> state)
        {
            Console.WriteLine("StateConfig:");
            Console.WriteLine(string.Join(Environment.NewLine, state.StateConfiguration.StateNodeIds.Select(text => $"  {text}")));
        }
    }
}
