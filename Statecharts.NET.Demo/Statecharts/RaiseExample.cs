using System;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;
using Service = Statecharts.NET.Language.Service;

namespace Statecharts.NET.Demos.Statecharts
{
    internal static class RaiseExample
    {
        internal static StatechartDefinition<FetchContext> Behaviour => Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "RaiseExample"
                    .AsCompound()
                    .WithInitialState("Inactive")
                    .WithStates(
                        "Inactive"
                            .WithTransitions(On("start").TransitionTo.Sibling("Loading")),
                        "Loading"
                            .WithTransitions(On("stop").TransitionTo.Sibling("Inactive"))
                            .WithInvocations(Service.DefineTask(token => System.Threading.Tasks.Task.Delay(5000, token))
                                .OnSuccess.TransitionTo.Sibling("ReportLoadingFinished").WithActions<FetchContext>(
                                    Assign<FetchContext>(context => context.Retries = 42),
                                    Raise("test?"))),
                        "ReportLoadingFinished"
                            .WithEntryActions(Raise("CancelPlaying"))
                            .WithTransitions(Immediately.TransitionTo.Sibling("Playing")),
                        "Playing"
                            .WithTransitions(On("CancelPlaying").TransitionTo.Sibling("Inactive"))));
    }
}
