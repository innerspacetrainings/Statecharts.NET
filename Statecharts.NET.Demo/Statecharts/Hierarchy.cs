using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;

namespace Statecharts.NET.Demos.Statecharts
{
    internal static class Hierarchy
    {
        internal static StatechartDefinition<FetchContext> Hier2 => Define.Statechart
            .WithInitialContext(new FetchContext { Retries = 0 })
            .WithRootState(
                "hier2"
                    .AsCompound()
                    .WithInitialState("a")
                    .WithStates(
                        "a"
                            .WithTransitions(On("t").TransitionTo.Child("a2"))
                            .AsCompound()
                            .WithInitialState("a1")
                            .WithStates(
                                "a1"
                                    .WithTransitions(On("t").TransitionTo.Absolute("hier2", "b")),
                                "a2"),
                        "b"));
    }
}
