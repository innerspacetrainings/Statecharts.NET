using System;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;


namespace Statecharts.NET.Demos.Statecharts
{
    internal static class Invalid
    {
        internal static StatechartDefinition<FetchContext> Behaviour => Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "example"
                    .AsCompound()
                    .WithInitialState("a")
                    .WithStates(
                        "a".WithTransitions(
                            On("1").TransitionTo.Child("uuups"),
                            On("2").TransitionTo.Sibling("c")),
                        "b"));
    }
}
