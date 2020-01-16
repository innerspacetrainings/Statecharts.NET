using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Builders;
using Statecharts.NET.Definition;
using Statecharts.NET.Internal.Extensions;
using Statecharts.NET.Language.Service;
using Statecharts.NET.Language.StateNode;
using Statecharts.NET.Language.Transition;
using Statecharts.NET.Utils;

namespace Statecharts.NET.Language
{
    public static class Keywords
    {
        public static StatechartDefinitionBuilder Statechart => new StatechartDefinitionBuilder();
        public static ServiceLogic Chain(params ServiceLogic[] tasks)
            => async token =>
            {
                foreach (var task in tasks)
                {
                    await task(token);
                    token.ThrowIfCancellationRequested();
                }
            };

        public static ForbiddenEventDefinition Ignore(string eventName) =>
            new ForbiddenEventDefinition {Event = new Event(eventName)}; // TODO: constructor + naming (Event Type vs. Event Name)
        
        public static WithEventDefinition On(string eventType)
            => WithEventDefinition.OfEventType(eventType);
        public static WithEventDefinition<TEventData> On<TEventData>(string eventType)
            => WithEventDefinition.OfEventType<TEventData>(eventType);
        public static WithEventDefinition OnDone
            => WithEventDefinition.Done();
        public static WithEventDefinition Immediately
            => WithEventDefinition.Immediately();
        public static WithEventDefinition After(TimeSpan delay)
            => WithEventDefinition.Delayed(delay);

        public static ChildTargetDefinition Child(string stateNodeName)
            => new ChildTargetDefinition(stateNodeName);
        public static SiblingTargetDefinition Sibling(string stateNodeName)
            => new SiblingTargetDefinition(stateNodeName);
        public static AbsoluteTargetDefinition Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new AbsoluteTargetDefinition(
                new StateNodeId((
                    new RootStateNodeKey(stateChartName) as StateNodeKey)
                    .Append(new NamedStateNodeKey(stateNodeName))
                    .Concat(stateNodeNames.Select(name => new NamedStateNodeKey(name)))));
    }
    public static class ExtensionMethods
    {
        #region StateNode
        public static WithEntryActions WithEntryActions(this string name, Action action, params Action[] entryActions)
            => new WithName(name).WithEntryActions(action, entryActions);
        public static WithExitActions WithExitActions(this string name, Action action, params Action[] exitActions)
            => new WithName(name).WithExitActions(action, exitActions);
        public static WithEvents WithEvents(this string name, BaseEventDefinition @event, params BaseEventDefinition[] events)
            => new WithName(name).WithEvents(@event, events);
        public static WithActivities WithActivities(this string name, IActivity activity, params IActivity[] activities)
            => new WithName(name).WithActivities(activity, activities);
        public static WithServices WithServices(
            this string name,
            OneOf<ServiceLogic, IBaseServiceDefinition> service,
            params OneOf<ServiceLogic, IBaseServiceDefinition>[] services)
            => new WithName(name).WithServices(service, services);
        public static Final AsFinal(this string name)
            => new WithName(name).AsFinal();
        public static Compound AsCompound(this string name)
            => new WithName(name).AsCompound();
        public static Orthogonal AsOrthogonal(this string name)
            => new WithName(name).AsOrthogonal();
        #endregion
        #region Service
        public static WithId WithId(this ServiceLogic task, string id)
            => new WithLogic(task).WithId(id);
        public static WithOnSuccessHandler OnSuccess(this ServiceLogic task, UnguardedEventTransitionDefinition transitionDefinition)
            => new WithLogic(task).OnSuccess(transitionDefinition);
        public static WithOnErrorHandler OnError(this ServiceLogic task, UnguardedEventTransitionDefinition transitionDefinition)
            => new WithLogic(task).OnError(transitionDefinition);

        // TODO: Generics
        //public static Service.WithId<T> WithId<T>(this Func<CancellationToken, Task<T>> task, string id)
        //    => throw new NotImplementedException();
        #endregion

        //public static BaseEventDefinition GotoSibling(this string at, string to)
        //    => new EventDefinition<Event>
        //    {
        //        Event = new Event(at),
        //        Transitions = new List<EventTransitionDefinition>
        //        {
        //            new UnguardedEventTransitionDefinition
        //            {
        //                Targets = new List<BaseTargetDefinition>
        //                {
        //                    new ChildTargetDefinition
        //                    {
        //                        Key = new NamedStateNodeKey(to)
        //                    }
        //                }
        //            }
        //        }
        //    };
    }

}
