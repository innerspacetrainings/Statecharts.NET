using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;


namespace Statecharts.NET.Demos.Statecharts
{
    class Playground
    {
        internal static StatechartDefinition<FetchContext> Behaviour => Define.Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "Playground"
                    .AsCompound()
                    .WithInitialState("Init")
                    .WithStates(
                        "Init"
                            .AsCompound()
                            .WithInitialState("First")
                            .WithStates(
                                "First".WithTransitions(On("click").TransitionTo.Absolute("Playground", "Init", "Second")),
                                "Second".WithInvocations(Define.Service.Task(token => Task.Delay(1000, token)).OnSuccess.TransitionTo.Sibling("Third")),
                                "Third".AsFinal())
                            .OnDone.TransitionTo.Sibling("Next"),
                        "Next"));
    }
}
