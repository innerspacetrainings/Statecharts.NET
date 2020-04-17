using System;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;
using Service = Statecharts.NET.Language.Service;


namespace Statecharts.NET.Demos.Statecharts
{
    internal static class SendExample
    {
        internal static NamedDataEventFactory<string> ShowError = Event.Define("ShowError").WithData<string>();

        internal static StatechartDefinition<FetchContext> Behaviour => Statechart
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
                                            .WithInvocations(Service.DefineTask(token => System.Threading.Tasks.Task.Delay(10000, token))),
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
