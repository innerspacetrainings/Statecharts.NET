using System;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public static class Statechart
    {
        public static Builders.Statechart<TContext> WithInitialContext<TContext>(TContext initialContext)
            where TContext : IEquatable<TContext>
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
        public static CustomDataEvent WithData<TEventData>(this NamedEvent @event) => // TODO: Event.Define("...")[.WithData<TEventData>()];
            new CustomDataEvent(@event.EventName, default);
    }

    public static class Keywords
    {
        public static TaskService Chain(
            OneOf<Model.Task, Definition.TaskService> first,
            OneOf<Model.Task, Definition.TaskService> second,
            params OneOf<Model.Task, Definition.TaskService>[] remaining)
            => Service.DefineTask(async token =>
            {
                foreach (var wrappedTask in first.Append(second).Append(remaining))
                {
                    var task = wrappedTask.Match(Functions.Identity, service => service.Task);
                    await task(token);
                    token.ThrowIfCancellationRequested();
                }
            });

        public static Definition.ForbiddenTransition Ignore(string eventName) =>
            new Definition.ForbiddenTransition(eventName);
        
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
            new AbsoluteTarget(
                new StateNodeId((
                    new RootStateNodeKey(stateChartName) as StateNodeKey)
                    .Append(new NamedStateNodeKey(stateNodeName))
                    .Concat(stateNodeNames.Select(name => new NamedStateNodeKey(name)))));

        public static SendAction Send()
            => new SendAction();
        public static RaiseAction Raise()
            => new RaiseAction();
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
        public static SideEffectAction Run(System.Action action)
            => new SideEffectAction(action);
        public static SideEffectAction<TContext> Run<TContext>(System.Action<TContext> action)
            => new SideEffectAction<TContext>(action);
        public static SideEffectAction<TContext, TData> Run<TContext, TData>(System.Action<TContext, TData> action)
            => new SideEffectAction<TContext, TData>(action);
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
            Definition.Transition transition,
            params Definition.Transition[] transitions)
            => new Builders.StateNode.WithName(name).WithTransitions(transition, transitions);
        public static Builders.StateNode.WithServices WithInvocations(
            this string name,
            Definition.Service service,
            params Definition.Service[] services)
            => new Builders.StateNode.WithName(name).WithInvocations(service, services);
        public static Builders.StateNode.Final AsFinal(this string name)
            => new Builders.StateNode.WithName(name).AsFinal();
        public static Builders.StateNode.Compound AsCompound(this string name)
            => new Builders.StateNode.WithName(name).AsCompound();
        public static Builders.StateNode.Orthogonal AsOrthogonal(this string name)
            => new Builders.StateNode.WithName(name).AsOrthogonal();
    }
}
