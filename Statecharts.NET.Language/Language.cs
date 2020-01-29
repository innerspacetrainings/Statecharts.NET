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
        public static SideEffectContextAction Define(System.Action effect) =>
            new SideEffectContextAction(_ => effect());
        public static SideEffectContextAction Define<T>(System.Action<T> effect) =>
            new SideEffectContextAction(context => effect((T)context));
    }
    public static class Event
    {
        // TODO: Event.Define("...")[.WithData<TEventData>()];
    }

    public static class Keywords
    {
        public static TaskService Chain(
            OneOf<Model.Task, Definition.TaskService> first,
            OneOf<Model.Task, Definition.TaskService> second,
            params OneOf<Model.Task, Definition.TaskService>[] remaining) // TODO: add Model.Task + required first param
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
            => throw new NotImplementedException();
        public static RaiseAction Raise()
            => throw new NotImplementedException();
        public static LogAction Log()
            => throw new NotImplementedException();
        public static AssignContextAction Assign()
            => throw new NotImplementedException();
        public static SideEffectContextAction Run()
            => throw new NotImplementedException();
    }
    public static class Helpers
    {
        public static Builders.StateNode.WithEntryActions WithEntryActions(
            this string name,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] entryActions)
            => new Builders.StateNode.WithName(name).WithEntryActions(action, entryActions);
        public static Builders.StateNode.WithExitActions WithExitActions(
            this string name,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] exitActions)
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
