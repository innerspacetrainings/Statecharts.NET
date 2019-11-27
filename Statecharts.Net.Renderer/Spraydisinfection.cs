using System;
using Statecharts.NET.Types;
using static Statecharts.NET.DSL;

namespace Statecharts.Net.Renderer
{
    class SprayDisinfection
    {
        public static readonly IBaseState bottle1 = State("bottle1")
                    .WithInitial("ontable")
                    .WithSubStates(
                        State("ontable")
                        .WithTransitions(On("bottle1_picked_up").ContinueTo("picked_up")),
                        State("picked_up")
                        .WithTransitions(
                            IfIn("bottle2.picked_up").ContinueTo("#spraydisinfection.disinfecting")));

        public static readonly Machine machine =
            Machine("spraydisinfection")
            .WithInitial("training_explanation")
            .WithStates(


                //State("jo talking").WaitFor(new PlayAudio("asdfasdf")).TransitionToNext(),
                //State("show on terminal").WaitFor(new ShowStartscreenOnTerminal()).TransitionToNext(),



                //State("jo talking").JoTalk("asdf").TransitionToNext(),
                State("training_explanation")
                .WithTransitions(
                    On("actions_finished").ContinueTo("started"))
                .WithEntryActions(
                    new ShowStartscreenOnTerminal(),
                    new PlayAudio("CRB_disinfect_training_1"),
                    new PlayAudio("CRB_disinfect_training_2")),
                //SendEvent("actions_finished")),
                State("started")
                .WithParallelStates(
                    bottle1,
                    State("bottle2")
                    .WithInitial("ontable")
                    .WithSubStates(
                        State("ontable")
                        .WithTransitions(On("bottle2_picked_up").ContinueTo("picked_up")),
                        State("picked_up")
                        .WithTransitions(
                            IfIn("bottle1.picked_up").ContinueTo("#spraydisinfection.disinfecting")))),
                State("disinfecting").AsFinal());
    }

    internal class PlayAudio : ISyncAction
    {
        public PlayAudio(string id)
        {
            this.Id = id;
        }

        public string Id { get; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }

    internal class ShowStartscreenOnTerminal : ISyncAction
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
