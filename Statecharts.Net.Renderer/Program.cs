using System;
using System.Linq;
using Statecharts.NET;
using Statecharts.NET.Types;
using static Statecharts.NET.DSL;

namespace Statecharts.Net.Renderer
{
    class Fetch {
        private static ISyncAction IncrementAttempts() => new IncrementAttempts();

        static readonly IBaseState idle =
            State("idle")
            .WithTransitions(
                On("fetch").ContinueTo("pending"));

        static readonly IBaseState pending =
            State("pending")
            .WithTransitions(
                On("resolve").ContinueTo("fulfilled"),
                On("reject").ContinueTo("rejected"),
                After(2000).ContinueTo("rejected"))
            .WithEntryActions(
                IncrementAttempts());

        static readonly IBaseState fulfilled =
            State("fulfilled")
            .WithInitial("first")
            .WithSubStates(
                State("first")
                    .WithTransitions(
                        On("next").ContinueTo("second")),
                State("second")
                    .WithTransitions(
                        On("next").ContinueTo("third")),
                State("third").AsFinal());

        static readonly IBaseState rejected =
            State("rejected")
            .WithInitial("can retry")
            .WithSubStates(
                State("can retry")
                .AsDecision(
                    IfDraft("attempts > 4").ContinueTo("failure")),
                State("failure")
                .AsFinal()
                .WithTransitions(On("retry").ContinueToSelf()))
            .WithTransitions(
                On("retry").ContinueTo("pending"));

        public static readonly Machine machine =
            Machine("fetch")
            .WithInitial("idle")
            .WithStates(idle, pending, fulfilled, rejected);
    }

    class Light
    {
        static readonly IBaseState green =
            State("green")
            .WithTransitions(
                On("timer").ContinueTo("yellow"));

        static readonly IBaseState yellow =
            State("yellow")
            .WithTransitions(
                On("timer").ContinueTo("red"));

        static readonly IBaseState red =
            State("red")
            .WithParallelStates(
                State("walkSign")
                .WithInitial("solid")
                .WithSubStates(
                    State("solid")
                    .WithTransitions(
                        On("countdown").ContinueTo("flashing")),
                    State("flashing")
                    .WithTransitions(
                        On("stop countdown").ContinueTo("solid"))),
                State("pedestrian")
                .WithInitial("walk")
                .WithSubStates(
                    State("walk")
                    .WithTransitions(
                        On("countdown").ContinueTo("wait")),
                    State("wait")
                    .WithTransitions(
                        On("stop countdown").ContinueTo("stop")),
                    State("stop")
                    .AsFinal()));

        public static readonly Machine machine =
            Machine("light")
            .WithInitial("green")
            .WithStates(green, yellow, red);
    }

    

    class Program
    {
        static void Main(string[] args)
        {
            PrintAsXState(M2U1.machine, "IsWrongOrder");
        }

        private static void PrintAsXState(Machine machine, params string[] guards) =>
            Console.WriteLine($"const machine = Machine({machine.XStateDescription}, {GetGuardsObject(guards)})");

        private static string GetGuardsObject(string[] guards)
            => "{ guards: { " + string.Join(",", guards.Select(guard => $"{guard}: () => false")) + " } }";
    }

    class IncrementAttempts : ISyncAction
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
