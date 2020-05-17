using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Statecharts.NET.Demo;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities.Time;
using static Statecharts.NET.Language.Keywords;
using ActionDefinition = Statecharts.NET.Language.ActionDefinition;

namespace Statecharts.NET.Demos.Statecharts
{
    public static class Showcase
    {
        public static void Definitions()
        {
            // Events
            var Start = Define.Event("Start");
            var IncrementBy = Define.EventWithData<int>("IncrementBy");

            // Actions
            var SideEffect1 = Define.Action.SideEffect(() => Console.WriteLine("I'm a Side Effect"));
            var SideEffect2 = Define.Action.SideEffectWithContext<FetchContext>(context => Console.WriteLine($"I have access to the context {context.Retries}"));
            var SideEffect3 = Define.Action.SideEffectWithContextAndData<FetchContext, int>((context, amount) => Console.WriteLine($"I have access to the context {context.Retries} and some data {amount}"));
            var Assign1 = Define.Action.Assign<FetchContext>(context => context.Retries = 0);
            var Assign2 = Define.Action.AssignWithData<FetchContext, int>((context, amount) => context.Retries += amount);

            // Services
            var TaskService = Define.Service.Task(token => Task.Delay(TimeSpan.FromSeconds(3), token));
            var ActivityService = Define.Service.Activity(() => Console.WriteLine("started"), () => Console.WriteLine("stopped"));

            // Root Statenode
            var CompoundRoot = Define.Statechart
                .WithInitialContext(new NoContext())
                .WithRootState(
                    "example"
                        .AsCompound()
                        .WithInitialState("first")
                        .WithStates("first", "second"));
            var OrthogonalRoot = Define.Statechart
                .WithInitialContext(new NoContext())
                .WithRootState(
                    "example"
                        .AsOrthogonal()
                        .WithStates("a", "b"));

            // Transitions
            var TransitionsExample = "example".WithTransitions(
                On("EventName").TransitionTo.Self,
                On(Start).TransitionTo.Self,
                On(IncrementBy).TransitionTo.Self,
                
                On("dummy").If<FetchContext>(context => context.Retries > 10).TransitionTo.Self,
                On(IncrementBy).If<FetchContext>((_, amount) => amount > 5).TransitionTo.Self,
                
                Ignore("EventName"),
                Ignore(Start),
                Ignore(IncrementBy),
                
                Immediately.TransitionTo.Self,
                
                After(3.Seconds()).TransitionTo.Self,
                
                On("dummy").TransitionTo.Self,
                On("dummy").TransitionTo.Child("child", "even", "deep", "children"),
                On("dummy").TransitionTo.Sibling("sibling", "even", "children", "of", "siblings"),
                On("dummy").TransitionTo.Target(Sibling("sibling")),
                On("dummy").TransitionTo.Target(Child("child")),
                On("dummy").TransitionTo.Absolute("rootstatenode", "children", "deeper", "..."),
                On("dummy").TransitionTo.Multiple(Child("paralle", "child1"), Child("parallel", "child2")),
                
                On("dummy").TransitionTo.Self.WithActions(SideEffect1, Log("and another one")));

            // Actions
            var ActionsExample = "example".WithEntryActions<FetchContext>(
                Run(() => Console.WriteLine("some arbitrary action")),
                Run<FetchContext>(context => Console.WriteLine($"some arbitrary action with {context}")),
                Log("logging a label"),
                Log<FetchContext>(context => $"logging some context {context}"),
                Assign<FetchContext>(context => context.Retries = 0));

            // Statenodes OnDone
            var OnCompoundDoneExample = "example"
                .AsCompound()
                .WithInitialState("first")
                .WithStates("first".AsFinal())
                .OnDone.TransitionTo.Sibling("sibling");
            var OnOrthogonalDoneExample = "example"
                .AsOrthogonal()
                .WithStates("first".AsFinal(), "second".AsFinal())
                .OnDone.TransitionTo.Sibling("sibling");

            // Services OnSuccess (OnError is currently missing :/)
            var TaskServiceExample = "example"
                .WithInvocations(TaskService.OnSuccess.TransitionTo.Sibling("sibling"));
        }

        private static StatenodeDefinition Numbered(int number, Target next) =>
            $"Statenode{number}"
                .WithTransitions(Immediately.TransitionTo.Target(next));

    }
}
