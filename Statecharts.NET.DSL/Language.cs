using System;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public static class Keywords
    {
        public static Statechart.Builder Statechart => new Statechart.Builder();
        public static Event.Builder Event => new Event.Builder();

        public static Service.ServiceLogic Chain(params Service.ServiceLogic[] tasks)
            => async token =>
            {
                foreach (var task in tasks)
                {
                    await task(token);
                    token.ThrowIfCancellationRequested();
                }
            };

        public static Definition.ForbiddenTransition Ignore(string eventName) =>
            new Definition.ForbiddenTransition(eventName);
        
        public static Transition.WithEvent On(string eventType)
            => Transition.WithEvent.OfEventType(eventType);
        public static Transition.WithEvent Immediately
            => Transition.WithEvent.Immediately();
        public static Transition.WithEvent After(TimeSpan delay)
            => Transition.WithEvent.Delayed(delay);

        public static Definition.ChildTarget Child(string stateNodeName)
            => new Definition.ChildTarget(stateNodeName);
        public static Definition.SiblingTarget Sibling(string stateNodeName)
            => new Definition.SiblingTarget(stateNodeName);
        public static Definition.AbsoluteTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new Definition.AbsoluteTarget(
                new StateNodeId((
                    new RootStateNodeKey(stateChartName) as StateNodeKey)
                    .Append(new NamedStateNodeKey(stateNodeName))
                    .Concat(stateNodeNames.Select(name => new NamedStateNodeKey(name)))));

        // TODO: create Keywords for all Action Types
        public static Definition.SendAction Send()
            => throw new NotImplementedException();
        public static Definition.RaiseAction Raise()
            => throw new NotImplementedException();
        public static Definition.LogAction Log()
            => throw new NotImplementedException();
        public static Definition.AssignContextAction Assign()
            => throw new NotImplementedException();
        public static Definition.SideEffectContextAction Run()
            => throw new NotImplementedException();
    }
    public static class Helpers
    {
        public static StateNode.WithEntryActions WithEntryActions(
            this string name,
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] entryActions)
            => new StateNode.WithName(name).WithEntryActions(action, entryActions);
        public static StateNode.WithExitActions WithExitActions(
            this string name,
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] exitActions)
            => new StateNode.WithName(name).WithExitActions(action, exitActions);
        public static StateNode.WithTransitions WithTransitions(
            this string name,
            Definition.Transition transition,
            params Definition.Transition[] transitions)
            => new StateNode.WithName(name).WithTransitions(transition, transitions);
        public static StateNode.WithActivities WithActivities(
            this string name,
            Definition.Activity activity,
            params Definition.Activity[] activities)
            => new StateNode.WithName(name).WithActivities(activity, activities);
        public static StateNode.WithServices WithServices(
            this string name,
            OneOf<Service.ServiceLogic, Definition.Service> service,
            params OneOf<Service.ServiceLogic, Definition.Service>[] services)
            => new StateNode.WithName(name).WithServices(service, services);
        public static StateNode.Final AsFinal(this string name)
            => new StateNode.WithName(name).AsFinal();
        public static StateNode.Compound AsCompound(this string name)
            => new StateNode.WithName(name).AsCompound();
        public static StateNode.Orthogonal AsOrthogonal(this string name)
            => new StateNode.WithName(name).AsOrthogonal();
        
        public static Service.WithId WithId(
            this Service.ServiceLogic task,
            string id)
            => new Service.WithLogic(task).WithId(id);
        public static Service.WithOnSuccessHandler OnSuccess(
            this Service.ServiceLogic logic,
            OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition> transition)
            => new Service.WithLogic(logic).OnSuccess(transition);
        public static Service.WithOnErrorHandler OnError(
            this Service.ServiceLogic logic,
            OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition> transition)
            => new Service.WithLogic(logic).OnError(transition);

        // TODO: implement this
        ////public static Service.WithId<T> WithId<T>(
        ////    this Service.ServiceLogic<T> logic,
        ////    string id)
        ////    => throw new NotImplementedException();
        ////public static Service.WithOnSuccessHandler<T> OnSuccess<T>(
        ////    this Service.ServiceLogic logic,
        ////    UnguardedContextDataTransition transition)
        ////    => throw new NotImplementedException();
    }
}
