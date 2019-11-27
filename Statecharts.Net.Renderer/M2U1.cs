using System.Collections;
using Statecharts.NET.Types;
using static Statecharts.NET.DSL;

namespace Statecharts.Net.Renderer
{
    interface IEn : IEnumerator { }
    class M2U1
    {
        public static readonly Machine machine =
            Machine("M2U1")
            .WithInitial("wiping")
            .WithParallelStates(
                State("table")
                .WithInitial("unlocked")
                .WithSubStates(
                    State("locked"),
                    State("unlocked")
                    .WithInitial("pending")
                    .WithSubStates(
                        State("pending")
                        .WithTransitions(
                            On("pointtouched").ContinueTo("linefinished").WithDraftGuard("IsLineFinished")),
                        State("linefinished")
                        .WithTransitions(
                            ContinueTo(Sibling("pending")))))
                .WithTransitions(
                    On("lock").ContinueTo("locked")),
                State("progress")
                .WithInitial("cleaning")
                .WithSubStates(
                    State("cleaning"),
                    State("alllinescleaned").AsFinal())
                .WithTransitions(
                    On("pointtouched").ContinueTo("allinescleaned").WithDraftGuard("WereAllLinesCleaned")),
                State("speed")
                .WithInitial("correct")
                .WithSubStates(
                    State("correct").WithTransitions(On("movedtoofast").ContinueTo("toofast")),
                    State("toofast").WithTransitions(On("sloweddown").ContinueTo("correct"))),
                State("error")
                .WithInitial("none")
                .WithSubStates(
                    State("none")
                    .WithTransitions(
                        On("some.overcleanlane").ContinueTo("overcleanlane"),
                        ContinueTo(Child("some.wrongorder")).WithDraftGuard("IsWrongOrder"),
                        ContinueTo(Child("some.wrongstartlane")).WithDraftGuard("IsWrongStartlane"),
                        ContinueTo("some.wrongdirection").WithDraftGuard("IsWrongDirection")),
                    State("some")
                    .WithSubStates(
                        State("overcleanlane").AsFinal(),
                        State("wrongstartlane").AsFinal(),
                        State("wrongorder").AsFinal(),
                        State("wrongdirection").AsFinal())));
    }
}
