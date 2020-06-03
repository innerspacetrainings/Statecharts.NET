using System;
using System.Collections.Generic;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language.Builders;
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
            public static StatechartDefinitionWithInitialContext<TContext> WithInitialContext<TContext>(TContext initialContext)
                where TContext : IContext<TContext>
                => new StatechartDefinitionWithInitialContext<TContext>(initialContext);
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
            public static SideEffectActionDefinition SideEffect(System.Action effect) =>
                new SideEffectActionDefinition(effect);
            public static SideEffectActionDefinition<TContext> SideEffectWithContext<TContext>(Action<TContext> effect) =>
                new SideEffectActionDefinition<TContext>(effect);
            public static SideEffectActionDefinition<TContext, TData> SideEffectWithContextAndData<TContext, TData>(Action<TContext, TData> effect) =>
                new SideEffectActionDefinition<TContext, TData>(effect);
            public static AssignActionDefinition<TContext> Assign<TContext>(Action<TContext> mutation)
                => new AssignActionDefinition<TContext>(mutation);
            public static AssignActionDefinition<TContext, TData> AssignWithData<TContext, TData>(Action<TContext, TData> mutation)
                => new AssignActionDefinition<TContext, TData>(mutation);
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
            OneOf<TaskDelegate, TaskServiceDefinition> first,
            OneOf<TaskDelegate, TaskServiceDefinition> second,
            params OneOf<TaskDelegate, TaskServiceDefinition>[] remaining)
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

        public static WithNamedEvent On(string eventName)
            => new WithNamedEvent(eventName);
        public static WithNamedEvent On(NamedEvent @event)
            => new WithNamedEvent(@event);
        public static WithNamedDataEvent<TEventData> On<TEventData>(NamedDataEventFactory<TEventData> factory)
            => new WithNamedDataEvent<TEventData>(factory(default));
        public static WithEvent Immediately
            => WithEvent.Immediately();
        public static WithEvent After(TimeSpan delay)
            => WithEvent.Delayed(delay);

        public static ChildTarget Child(string statenodeName, params string[] childStatenodesNames)
            => new ChildTarget(statenodeName, childStatenodesNames);
        public static SiblingTarget Sibling(string statenodeName, params string[] childStatenodesNames)
            => new SiblingTarget(statenodeName, childStatenodesNames);
        public static AbsoluteTarget Absolute(string statechartName, params string[] childStatenodeNames) =>
            new AbsoluteTarget(StatenodeId.Absolute(statechartName.Append(childStatenodeNames)));

        public static SendActionDefinition Send(string eventName)
            => Send(new NamedEvent(eventName));
        public static SendActionDefinition Send(ISendableEvent @event)
            => new SendActionDefinition(@event);
        public static RaiseActionDefinition Raise(string eventName)
            => Raise(new NamedEvent(eventName));
        public static RaiseActionDefinition Raise(ISendableEvent @event)
            => new RaiseActionDefinition(@event);
        public static LogActionDefinition Log(string label)
            => new LogActionDefinition(label);
        public static LogActionDefinition<TContext> Log<TContext>(Func<TContext, string> message)
            => new LogActionDefinition<TContext>(message);
        public static LogActionDefinition<TContext, TData> Log<TContext, TData>(Func<TContext, TData, string> message)
            => new LogActionDefinition<TContext, TData>(message);
        public static AssignActionDefinition Assign(Action mutation)
            => new AssignActionDefinition(mutation);
        public static AssignActionDefinition<TContext> Assign<TContext>(Action<TContext> mutation)
            => new AssignActionDefinition<TContext>(mutation);
        public static AssignActionDefinition<TContext, TData> Assign<TContext, TData>(Action<TContext, TData> mutation)
            => new AssignActionDefinition<TContext, TData>(mutation);
        public static SideEffectActionDefinition Run(Action sideEffect)
            => new SideEffectActionDefinition(sideEffect);
        public static SideEffectActionDefinition<TContext> Run<TContext>(Action<TContext> sideEffect)
            => new SideEffectActionDefinition<TContext>(sideEffect);
        public static SideEffectActionDefinition<TContext, TData> Run<TContext, TData>(Action<TContext, TData> sideEffect)
            => new SideEffectActionDefinition<TContext, TData>(sideEffect);
    }
    public static class Helpers
    {
        public static StatenodeWithEntryActions WithEntryActions(
            this string name,
            ActionDefinition action,
            params ActionDefinition[] entryActions)
            => new StatenodeWithName(name).WithEntryActions(action, entryActions);
        public static StatenodeWithEntryActions WithEntryActions<TContext>(
            this string name,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] entryActions)
            => new StatenodeWithName(name).WithEntryActions(action, entryActions);
        public static StatenodeWithExitActions WithExitActions(
            this string name,
            ActionDefinition action,
            params ActionDefinition[] exitActions)
            => new StatenodeWithName(name).WithExitActions(action, exitActions);
        public static StatenodeWithExitActions WithExitActions<TContext>(
            this string name,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] exitActions)
            => new StatenodeWithName(name).WithExitActions(action, exitActions);
        public static StatenodeWithTransitions WithTransitions(
            this string name,
            TransitionDefinition transition,
            params TransitionDefinition[] transitions)
            => new StatenodeWithName(name).WithTransitions(transition, transitions);
        public static StatenodeWithTransitions WithTransitions(
            this string name,
            IEnumerable<TransitionDefinition> transitions)
            => new StatenodeWithName(name).WithTransitions(transitions);
        public static StatenodeWithInvocations WithInvocations(
            this string name,
            ServiceDefinition service,
            params ServiceDefinition[] services)
            => new StatenodeWithName(name).WithInvocations(service, services);
        public static StatenodeDefinition AsStatenodeDefinition(this string name)
            => new StatenodeWithName(name);
        public static FinalStatenode AsFinal(this string name)
            => new StatenodeWithName(name).AsFinal();
        public static CompoundStatenode AsCompound(this string name)
            => new StatenodeWithName(name).AsCompound();
        public static OrthogonalStatenode AsOrthogonal(this string name)
            => new StatenodeWithName(name).AsOrthogonal();
    }
}
