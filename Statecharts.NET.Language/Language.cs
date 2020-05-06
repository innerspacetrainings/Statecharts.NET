using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public delegate NamedDataEvent<TEventData> NamedDataEventFactory<TEventData>(TEventData data);

    public static class Define
    {
        public static class Statechart
        {
            public static Builders.StatechartDefinitionWithInitialContext<TContext> WithInitialContext<TContext>(TContext initialContext)
                where TContext : IContext<TContext>
                => new Builders.StatechartDefinitionWithInitialContext<TContext>(initialContext);
        }
        public static class Service
        {
            public static ActivityService Activity(System.Action start, System.Action stop) =>
                new ActivityService(new Activity(start, stop));
            public static ActivityService Activity(Activity activity) =>
                new ActivityService(activity);
            public static TaskService Task(TaskDelegate task) =>
                new TaskService(task);
        }
        public static class Action
        {
            public static SideEffectAction SideEffect(System.Action effect) =>
                new SideEffectAction(effect);
            public static SideEffectAction<TContext> SideEffectWithContext<TContext>(System.Action<TContext> effect) =>
                new SideEffectAction<TContext>(effect);
            public static SideEffectAction<TContext, TData> SideEffectWithContextAndData<TContext, TData>(System.Action<TContext, TData> effect) =>
                new SideEffectAction<TContext, TData>(effect);
            public static AssignAction Assign(System.Action mutation)
                => new AssignAction(mutation);
            public static AssignAction<TContext> AssignWithContext<TContext>(System.Action<TContext> mutation)
                => new AssignAction<TContext>(mutation);
            public static AssignAction<TContext, TData> AssignWithContextAndData<TContext, TData>(System.Action<TContext, TData> mutation)
                => new AssignAction<TContext, TData>(mutation);
        }
        #region Event
        public static NamedEvent Event(string eventName) =>
            new NamedEvent(eventName);
        public static NamedDataEventFactory<TEventData> EventWithData<TEventData>(string eventName) =>
            data => new NamedDataEvent<TEventData>(eventName, data);
        #endregion
    }

    public static class Keywords
    {
        public static TaskService Chain(
            OneOf<Model.TaskDelegate, TaskServiceDefinition> first,
            OneOf<Model.TaskDelegate, TaskServiceDefinition> second,
            params OneOf<Model.TaskDelegate, TaskServiceDefinition>[] remaining)
            => Define.Service.Task(async token =>
            {
                foreach (var wrappedTask in first.Append(second).Append(remaining))
                {
                    var task = wrappedTask.Match(Functions.Identity, service => service.Task);
                    await task(token);
                    token.ThrowIfCancellationRequested();
                }
            });

        public static ForbiddenTransitionDefinition Ignore(string eventName) =>
            new ForbiddenTransitionDefinition(new NamedEvent(eventName));
        public static ForbiddenTransitionDefinition Ignore(NamedEvent @event) =>
            new ForbiddenTransitionDefinition(@event);
        public static ForbiddenTransitionDefinition Ignore<TEventData>(NamedDataEventFactory<TEventData> factory) =>
            new ForbiddenTransitionDefinition(factory(default));

        public static Builders.Transition.WithNamedEvent On(string eventName)
            => new WithNamedEvent(eventName);
        public static Builders.Transition.WithNamedEvent On(NamedEvent @event)
            => new WithNamedEvent(@event);
        public static Builders.Transition.WithNamedDataEvent<TEventData> On<TEventData>(NamedDataEventFactory<TEventData> factory)
            => new WithNamedDataEvent<TEventData>(factory(default));
        public static Builders.Transition.WithEvent Immediately
            => Builders.Transition.WithEvent.Immediately();
        public static Builders.Transition.WithEvent After(TimeSpan delay)
            => Builders.Transition.WithEvent.Delayed(delay);

        public static ChildTarget Child(string stateNodeName)
            => new ChildTarget(stateNodeName);
        public static SiblingTarget Sibling(string stateNodeName)
            => new SiblingTarget(stateNodeName);
        public static AbsoluteTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new AbsoluteTarget(StatenodeId.Absolute(new []{stateChartName, stateNodeName}.Concat(stateNodeNames))); // TODO: fix this

        public static SendAction Send(string eventName)
            => Send(new NamedEvent(eventName));
        public static SendAction Send(ISendableEvent @event)
            => new SendAction(@event);
        public static RaiseAction Raise(string eventName)
            => Raise(new NamedEvent(eventName));
        public static RaiseAction Raise(ISendableEvent @event)
            => new RaiseAction(@event);
        public static LogAction Log(string label)
            => new LogAction(label);
        public static LogAction<TContext> Log<TContext>(Func<TContext, string> message)
            => new LogAction<TContext>(message);
        public static LogAction<TContext, TData> Log<TContext, TData>(Func<TContext, TData, string> message)
            => new LogAction<TContext, TData>(message);
        public static AssignAction Assign(System.Action mutation)
            => new AssignAction(mutation);
        public static AssignAction<TContext> Assign<TContext>(System.Action<TContext> mutation)
            => new AssignAction<TContext>(mutation);
        public static AssignAction<TContext, TData> Assign<TContext, TData>(System.Action<TContext, TData> mutation)
            => new AssignAction<TContext, TData>(mutation);
        public static SideEffectAction Run(System.Action sideEffect)
            => new SideEffectAction(sideEffect);
        public static SideEffectAction<TContext> Run<TContext>(System.Action<TContext> sideEffect)
            => new SideEffectAction<TContext>(sideEffect);
        public static SideEffectAction<TContext, TData> Run<TContext, TData>(System.Action<TContext, TData> sideEffect)
            => new SideEffectAction<TContext, TData>(sideEffect);
    }
    public static class Helpers
    {
        public static Builders.StateNode.WithEntryActions WithEntryActions(
            this string name,
            Language.Action action,
            params Language.Action[] entryActions)
            => new Builders.StateNode.WithName(name).WithEntryActions(action, entryActions);
        public static Builders.StateNode.WithEntryActions WithEntryActions<TContext>(
            this string name,
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] entryActions)
            => new Builders.StateNode.WithName(name).WithEntryActions(action, entryActions);
        public static Builders.StateNode.WithExitActions WithExitActions(
            this string name,
            Language.Action action,
            params Language.Action[] exitActions)
            => new Builders.StateNode.WithName(name).WithExitActions(action, exitActions);
        public static Builders.StateNode.WithExitActions WithExitActions<TContext>(
            this string name,
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] exitActions)
            => new Builders.StateNode.WithName(name).WithExitActions(action, exitActions);
        public static Builders.StateNode.WithTransitions WithTransitions(
            this string name,
            TransitionDefinition transition,
            params TransitionDefinition[] transitions)
            => new Builders.StateNode.WithName(name).WithTransitions(transition, transitions);
        public static Builders.StateNode.WithTransitions WithTransitions(
            this string name,
            IEnumerable<TransitionDefinition> transitions)
            => new Builders.StateNode.WithName(name).WithTransitions(transitions);
        public static Builders.StateNode.WithInvocations WithInvocations(
            this string name,
            ServiceDefinition service,
            params ServiceDefinition[] services)
            => new Builders.StateNode.WithName(name).WithInvocations(service, services);
        public static StatenodeDefinition AsStatenodeDefinition(this string name)
            => new Builders.StateNode.WithName(name);
        public static Builders.StateNode.Final AsFinal(this string name)
            => new Builders.StateNode.WithName(name).AsFinal();
        public static Builders.StateNode.Compound AsCompound(this string name)
            => new Builders.StateNode.WithName(name).AsCompound();
        public static Builders.StateNode.Orthogonal AsOrthogonal(this string name)
            => new Builders.StateNode.WithName(name).AsOrthogonal();
    }
}
