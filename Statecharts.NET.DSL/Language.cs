using System;
using System.Linq;
using Statecharts.NET.Language.TaskService;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public static class Keywords
    {
        public static Statechart.Builder Statechart => new Statechart.Builder();
        public static Service.Builder Service => new Service.Builder();
        public static SideEffect.Builder SideEffect => new SideEffect.Builder();
        public static Event.Builder Event => new Event.Builder();

        public static ServiceTask Chain(
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
        
        public static Transition.WithEvent On(string eventType)
            => Transition.WithEvent.OfEventType(eventType);
        public static Transition.WithEvent Immediately
            => Transition.WithEvent.Immediately();
        public static Transition.WithEvent After(TimeSpan delay)
            => Transition.WithEvent.Delayed(delay);

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

        // TODO: create Keywords for all Action Types
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
        public static StateNode.WithEntryActions WithEntryActions(
            this string name,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] entryActions)
            => new StateNode.WithName(name).WithEntryActions(action, entryActions);
        public static StateNode.WithExitActions WithExitActions(
            this string name,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] exitActions)
            => new StateNode.WithName(name).WithExitActions(action, exitActions);
        public static StateNode.WithTransitions WithTransitions(
            this string name,
            Definition.Transition transition,
            params Definition.Transition[] transitions)
            => new StateNode.WithName(name).WithTransitions(transition, transitions);
        public static StateNode.WithServices WithInvocations(
            this string name,
            Definition.Service service,
            params Definition.Service[] services)
            => new StateNode.WithName(name).WithInvocations(service, services);
        public static StateNode.Final AsFinal(this string name)
            => new StateNode.WithName(name).AsFinal();
        public static StateNode.Compound AsCompound(this string name)
            => new StateNode.WithName(name).AsCompound();
        public static StateNode.Orthogonal AsOrthogonal(this string name)
            => new StateNode.WithName(name).AsOrthogonal();
    }
}
