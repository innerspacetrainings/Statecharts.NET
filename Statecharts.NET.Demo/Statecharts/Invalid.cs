using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Language.Keywords;


namespace Statecharts.NET.Demos.Statecharts
{
    internal static class Invalid
    {
        internal static StatechartDefinition<NoContext> Behaviour => Statechart
            .WithInitialContext(new NoContext())
            .WithRootState(
                "example"
                    .AsCompound()
                    .WithInitialState("a")
                    .WithStates(
                        "a"
                            .WithTransitions(
                                On("1").TransitionTo.Child("uuups"),
                                On("2").TransitionTo.Sibling("c")),
                        "b"));
    }
}
