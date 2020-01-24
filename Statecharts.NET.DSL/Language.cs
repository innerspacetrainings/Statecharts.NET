using System;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET.Language
{
    public static class Keywords
    {
        public static Statechart.Builder Statechart => new Statechart.Builder();
        public static Event.Builder Event => new Event.Builder();
        public static Transition.WithEvent OnDone => Transition.WithEvent.OnCompoundDone();

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
            OneOf<Action, ContextAction> action,
            params OneOf<Action, ContextAction>[] entryActions)
            => new StateNode.WithName(name).WithEntryActions(action, entryActions);
        public static StateNode.WithExitActions WithExitActions(
            this string name,
            OneOf<Action, ContextAction> action,
            params OneOf<Action, ContextAction>[] exitActions)
            => new StateNode.WithName(name).WithExitActions(action, exitActions);
        public static StateNode.WithTransitions WithTransitions(
            this string name,
            Definition.Transition transition,
            params Definition.Transition[] transitions)
            => new StateNode.WithName(name).WithTransitions(transition, transitions);
        public static StateNode.WithActivities WithActivities(
            this string name,
            Activity activity,
            params Activity[] activities)
            => new StateNode.WithName(name).WithActivities(activity, activities);
        public static StateNode.WithServices WithInvocations(
            this string name,
            OneOf<Service.ServiceLogic, Model.Service> service,
            params OneOf<Service.ServiceLogic, Model.Service>[] services)
            => new StateNode.WithName(name).WithInvocations(service, services);
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
            Model.Target target)
            => new Service.WithLogic(logic).OnSuccess(target);
        public static Service.WithOnErrorHandler OnError(
            this Service.ServiceLogic logic,
            Model.Target target)
            => new Service.WithLogic(logic).OnError(target);

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
