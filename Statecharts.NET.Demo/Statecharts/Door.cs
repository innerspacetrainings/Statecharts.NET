using System;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;


namespace Statecharts.NET.Demos.Statecharts
{
    internal static class Door
    {
        private static FinalStatenodeDefinition Broken => "Broken".AsFinal();
        internal static StatechartDefinition<FetchContext> Behaviour => Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "door"
                    .WithEntryActions(Run(() => Console.WriteLine("dooring")))
                    .WithTransitions(On("SMASHED").TransitionTo.Child("Broken"))
                    .AsCompound()
                    .WithInitialState("closed")
                    .WithStates(
                        "closed".WithTransitions(On("OPEN").TransitionTo.Sibling("open")),
                        "open".WithTransitions(On("CLOSE").TransitionTo.Sibling("closed")),
                        Broken));
    }
}
