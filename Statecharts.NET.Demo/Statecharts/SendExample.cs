using System;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;


namespace Statecharts.NET.Demos.Statecharts
{
    internal static class SendExample
    {
        private static readonly NamedDataEventFactory<string> ShowError = Define.EventWithData<string>("ShowError");

        internal static StatechartDefinition<FetchContext> Behaviour => Define.Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "SendExample"
                    .AsCompound()
                    .WithInitialState("selecting")
                    .WithStates(
                        "selecting"
                            .AsOrthogonal()
                            .WithStates(
                                "trafficlight"
                                    .AsCompound()
                                    .WithInitialState("notselected")
                                    .WithStates(
                                        "notselected"
                                            .WithTransitions(
                                                On("red").TransitionTo.Sibling("red"),
                                                On("yellow").TransitionTo.Self.WithActions(Send(ShowError("et voilà, a custom message appears"))))
                                            .WithInvocations(Define.Service.Task(async token =>
                                            {
                                                await System.Threading.Tasks.Task.Delay(3000, token);
                                                Console.WriteLine(1);
                                                await System.Threading.Tasks.Task.Delay(3000, token);
                                                Console.WriteLine(2);
                                                await System.Threading.Tasks.Task.Delay(3000, token);
                                                Console.WriteLine(3);
                                            })),
                                        "red",
                                        "yellow"),
                                "error"
                                    .WithTransitions(
                                        On(ShowError).TransitionTo.Child("some").WithActions<FetchContext>(Run<FetchContext, string>((_, message) => Console.WriteLine($"ERROR: {message}"))),
                                        On("HideError").TransitionTo.Child("none"))
                                    .AsCompound()
                                    .WithInitialState("none")
                                    .WithStates(
                                        "some",
                                        "none")),
                        "driving"));
    }
}
