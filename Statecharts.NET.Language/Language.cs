using System;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public static class Statechart
    {
        public static Builders.Statechart<TContext> WithInitialContext<TContext>(TContext initialContext)
            where TContext : IContext<TContext>
            => new Builders.Statechart<TContext>(initialContext);
    }
    public static class Service
    {
        public static ActivityService DefineActivity(System.Action start, System.Action stop) =>
            new ActivityService(new Activity(start, stop));
        public static ActivityService DefineActivity(Model.Activity activity) =>
            new ActivityService(activity);
        public static TaskService DefineTask(Model.Task task) =>
            new TaskService(task);
    }
    public static class SideEffect
    {
        public static SideEffectAction Define(System.Action effect) =>
            new SideEffectAction(effect);
        public static SideEffectAction<TContext> Define<TContext>(System.Action<TContext> effect) =>
            new SideEffectAction<TContext>(effect);
        public static SideEffectAction<TContext, TData> Define<TContext, TData>(System.Action<TContext, TData> effect) =>
            new SideEffectAction<TContext, TData>(effect);
    }
    public static class Event
    {
        public static NamedEvent Define(string eventName) =>
            new NamedEvent(eventName);
        public static NamedEvent WithData<TEventData>(this NamedEvent @event) => // TODO: Event.Define("...")[.WithData<TEventData>()];
            new NamedEvent(@event.EventName, default);
    }

    public static class Keywords
    {
        public static TaskService Chain(
            OneOf<Model.Task, TaskServiceDefinition> first,
            OneOf<Model.Task, TaskServiceDefinition> second,
            params OneOf<Model.Task, TaskServiceDefinition>[] remaining)
            => Service.DefineTask(async token =>
            {
                foreach (var wrappedTask in first.Append(second).Append(remaining))
                {
                    var task = wrappedTask.Match(Functions.Identity, service => service.Task);
                    await task(token);
                    token.ThrowIfCancellationRequested();
                }
            });

        public static ForbiddenTransitionDefinition Ignore(string eventName) =>
            new ForbiddenTransitionDefinition(eventName);
        
        public static Builders.Transition.WithEvent On(string eventType)
            => Builders.Transition.WithEvent.OfEventType(eventType);
        public static Builders.Transition.WithEvent Immediately
            => Builders.Transition.WithEvent.Immediately();
        public static Builders.Transition.WithEvent After(TimeSpan delay)
            => Builders.Transition.WithEvent.Delayed(delay);

        public static ChildTarget Child(string stateNodeName)
            => new ChildTarget(stateNodeName);
        public static SiblingTarget Sibling(string stateNodeName)
            => new SiblingTarget(stateNodeName);
        public static AbsoluteTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new AbsoluteTarget(StatenodeId.Absolute(stateNodeName.Append(stateNodeNames))); // TODO: fix this

        public static SendAction Send(string eventName)
            => new SendAction(eventName);
        public static RaiseAction Raise(string eventName)
            => new RaiseAction(eventName);
        public static LogAction Log(string label)
            => new LogAction(label);
        public static LogAction<TContext> Log<TContext>(Func<TContext, string> message)
            => new LogAction<TContext>(message);
        public static LogAction<TContext, TData> Assign<TContext, TData>(Func<TContext, TData, string> message)
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
        public static Builders.StateNode.WithInvocations WithInvocations(
            this string name,
            ServiceDefinition service,
            params ServiceDefinition[] services)
            => new Builders.StateNode.WithName(name).WithInvocations(service, services);
        public static Builders.StateNode.Final AsFinal(this string name)
            => new Builders.StateNode.WithName(name).AsFinal();
        public static Builders.StateNode.Compound AsCompound(this string name)
            => new Builders.StateNode.WithName(name).AsCompound();
        public static Builders.StateNode.Orthogonal AsOrthogonal(this string name)
            => new Builders.StateNode.WithName(name).AsOrthogonal();
    }
}
