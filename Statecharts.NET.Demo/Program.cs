using System;
using System.Collections.Generic;
using Statecharts.NET.Demos.Statecharts;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;
using static Statecharts.NET.Language.Keywords;
using Action = System.Action;
using Service = Statecharts.NET.Language.Service;
using Task = System.Threading.Tasks.Task;

namespace Statecharts.NET.Demo
{
    internal class FetchContext : IContext<FetchContext>, IXStateSerializable
    {
        public int Retries { get; set; }

        public bool Equals(FetchContext other) => other != null && Retries == other.Retries;

        ObjectValue IXStateSerializable.AsJSObject()
            => ObjectValue(("retries", Retries));

        public FetchContext CopyDeep() => new FetchContext { Retries = Retries };

        public override string ToString()
            => $"FetchContext: (Retries = {Retries})";
    }

    internal static class Program
    {
        ////private static readonly StatechartDefinition<FetchContext> FetchDefinition = Statechart
        ////    .WithInitialContext(new FetchContext {Retries = 0})
        ////    .WithRootState(
        ////        "fetch"
        ////            .AsCompound()
        ////            .WithInitialState("idle")
        ////            .WithStates(
        ////                "idle"
        ////                    .WithTransitions(
        ////                        Ignore("GATHERUSERDATA"),
        ////                        On("FETCH").TransitionTo.Sibling("loading"))
        ////                    .AsOrthogonal()
        ////                    .WithStates(
        ////                        "really".WithTransitions(
        ////                            On("YES").TransitionTo.Absolute("fetch", "loading"),
        ////                            On("NO").TransitionTo.Sibling("nana")),
        ////                        "nana".WithTransitions(On("SERIOUSLY").TransitionTo.Absolute("fetch", "failure"))),
        ////                "loading"
        ////                    .WithEntryActions<FetchContext>(
        ////                        Run(() => Console.WriteLine("parameterless Actions also compile *party*")),
        ////                        //Run(() => throw new Exception("haha, i killed you")),
        ////                        Raise("raise"),
        ////                        Send("send"),
        ////                        Log<FetchContext>(context => $"Entered loading state with context: {context}"))
        ////                    .WithTransitions(
        ////                        Immediately.If<FetchContext>(context => context.Retries >= 3).TransitionTo
        ////                            .Sibling("sheeeesh"),
        ////                        On("RESOLVE").TransitionTo.Sibling("success"),
        ////                        On("REJECT").TransitionTo.Sibling("failure")),
        ////                "success".AsFinal(),
        ////                "sheeeesh".AsFinal(),
        ////                "test".AsCompound().WithInitialState("1").WithStates("1").OnDone.TransitionTo
        ////                    .Sibling("success"),
        ////                "failure".WithTransitions(
        ////                        On("RETRY").TransitionTo.Sibling("loading")
        ////                            .WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++)))
        ////                    .WithInvocations(
        ////                        Language.Service.DefineTask(async token =>
        ////                        {
        ////                            Console.WriteLine("waiting 3 seconds");
        ////                            await System.Threading.Tasks.Task.Delay(3000, token);
        ////                            Console.WriteLine("waiting finished");
        ////                        }).OnSuccess.TransitionTo.Sibling("sheeeesh"),
        ////                        Language.Service.DefineActivity(
        ////                            () => Console.WriteLine("started"),
        ////                            () => Console.WriteLine("stopped")))));

        private static NamedEvent Increment => Event.Define("INCREMENT");
        private static NamedDataEventFactory<int> IncrementBy => Event.Define("INCREMENTBY").WithData<int>();

        private static readonly StatechartDefinition<FetchContext> DemoDefinition = Statechart
            .WithInitialContext(new FetchContext {Retries = 0})
            .WithRootState(
                "demo"
                    .WithEntryActions(Log("NOW THIS WORKS AS WELL :party:"))
                    .AsCompound()
                    .WithInitialState("initial")
                    .WithStates(
                        "initial"
                            .WithEntryActions<FetchContext>(Assign<FetchContext>(context => context.Retries += 20))
                            .WithTransitions(
                            On("START").TransitionTo.Sibling("multiplechoice"),
                            On(IncrementBy).TransitionTo.Self
                                .WithActions<FetchContext>(Assign<FetchContext, int>((context, amount) =>
                                    context.Retries += amount)),
                            On(Increment).TransitionTo.Self
                                .WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++)))
                            .AsCompound()
                            .WithInitialState("1")
                            .WithStates(
                                "1".WithTransitions(Ignore(Increment)),
                                "2"),
                        "multiplechoice".WithTransitions(On("RETRY").TransitionTo.Child("initial"))
                            .AsCompound().WithInitialState("initial").WithStates(
                                "initial".WithTransitions(
                                        On("START").TransitionTo.Sibling("selecting"))
                                    .WithInvocations(Service.DefineActivity(() => Console.WriteLine("start"),
                                        () => Console.WriteLine("stop"))),
                                "selecting".WithTransitions(
                                    On("CORRECT").TransitionTo.Sibling("solved")),
                                "solved".AsFinal())
                            .OnDone.TransitionTo.Sibling("final"),
                        "timeout".WithTransitions(
                            On("COMPLETE").TransitionTo.Sibling("final")),
                        "final".AsFinal()));

        private static readonly StatechartDefinition<FetchContext> TestDefinition = Statechart
            .WithInitialContext(new FetchContext {Retries = 0})
            .WithRootState(
                "test"
                    .AsCompound()
                    .WithInitialState("initial")
                    .WithStates(
                        "initial"
                            .WithTransitions(
                                On("START").TransitionTo.Sibling("controllers"),
                                After(1.6.Seconds()).TransitionTo.Sibling("final")),
                        "controllers"
                            .AsOrthogonal()
                            .WithStates(
                                "right"
                                    .AsCompound()
                                    .WithInitialState("notpressed")
                                    .WithStates(
                                        "notpressed".WithTransitions(On("RPRESSED").TransitionTo.Sibling("pressed")),
                                        "pressed".AsFinal()),
                                "left"
                                    .AsCompound()
                                    .WithInitialState("notpressed")
                                    .WithStates(
                                        "notpressed".WithTransitions(On("LPRESSED").TransitionTo.Sibling("pressed")),
                                        "pressed".AsFinal()))
                            .OnDone.TransitionTo.Sibling("final"),
                        "final".AsFinal()));

        private static readonly Dictionary<string, Action> _statecharts = new Dictionary<string, Action>
        {
            {"Door", Run(Door.Behaviour)},
            {"SendExample", Run(SendExample.Behaviour)},
            {"Assign", Run(DemoDefinition)}
    };

        private static Action Run<TContext>(StatechartDefinition<TContext> definition)
            where TContext : IContext<TContext>, IXStateSerializable => () =>
            {
                Console.WriteLine(definition.AsXStateVisualizerV4Definition() + Environment.NewLine);

                var statechart = Parser.Parse(definition) as ExecutableStatechart<FetchContext>;
                var running = Interpreter.Interpret(statechart,
                    new InterpreterOptions(wait: (lapse, token) =>
                        Task.Delay((int) lapse.TotalMilliseconds / 1, token)));

                running.OnMacroStep += macrostep =>
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine();
                    Console.WriteLine(
                        $" Statenodes: {string.Join(", ", macrostep.State.StateConfiguration.StateNodeIds)}");
                    Console.WriteLine($"    Context: {macrostep.State.Context}");
                    Console.WriteLine($"Next events: {string.Join(", ", running.NextEvents)}");
                    Console.ResetColor();
                };

                running.Start().ContinueWith(_ => Environment.Exit(0));

                while (true)
                {
                    Console.Write("@");
                    var eventType = Console.ReadLine();
                    switch (eventType)
                    {
                        case { } when eventType.StartsWith("INCREMENTBY"):
                            running.Send(IncrementBy(int.Parse(eventType.Substring(11))));
                            break;
                        case null: break;
                        default:
                            running.Send(new NamedEvent(eventType));
                            break;
                    }
                }

                // ReSharper disable once FunctionNeverReturns
            };

        private static void Main()
        {
            _statecharts["Assign"]();
        }
    }
}
