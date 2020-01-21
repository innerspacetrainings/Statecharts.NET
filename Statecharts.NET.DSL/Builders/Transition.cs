using System;
using System.Collections.Generic;
using System.Text;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Language.Transition
{
    public class WithEventDefinition
    {
        public BaseEventDefinition EventDefinition { get; }

        private WithEventDefinition(BaseEventDefinition eventDefinition) => EventDefinition = eventDefinition;

        internal static WithEventDefinition Immediately() =>
            new WithEventDefinition(new ImmediateEventDefinition());
        internal static WithEventDefinition OfEventType(string eventType) =>
            new WithEventDefinition(new EventDefinition(new Event(eventType)));
        internal static WithEventDefinition<TEventData> OfEventType<TEventData>(string eventType) =>
            new WithEventDefinition<TEventData>(new EventDefinition(new Event(eventType)));
        internal static WithEventDefinition Delayed(TimeSpan delay) =>
            new WithEventDefinition(new DelayedEventDefinition(delay));
        internal static WithEventDefinition Done() =>
            new WithEventDefinition(new DoneEventDefinition());
        internal static WithEventDefinition Forbidden() =>
            new WithEventDefinition(new ForbiddenEventDefinition());

        public UnguardedTransitionTo TransitionTo =>
            new UnguardedTransitionTo();

        public Guarded If<TContext>(Guard<TContext> guard)
            where TContext : IEquatable<TContext> =>
            new Guarded();
    }
    public class WithEventDefinition<TEventData>
    {
        public EventDefinition EventDefinition { get; }

        internal WithEventDefinition(EventDefinition eventDefinition) => EventDefinition = eventDefinition;

        public UnguardedTransitionTo TransitionTo =>
            new UnguardedTransitionTo();

        public Guarded If<TContext>(DataGuard<TContext, TEventData> guard)
            where TContext : IEquatable<TContext> =>
            new Guarded();
    }

    public class UnguardedTransitionTo
    {
        public WithTarget Child(string stateName) =>
            new WithTarget();
        public WithTarget Sibling(string stateName) =>
            new WithTarget();
        public WithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget();
        public WithTarget Multiple(BaseTargetDefinition target, params BaseTargetDefinition[] targets) =>
            new WithTarget();
    }
    public class UnguardedTransitionTo<TEventData>
    {
        public WithTarget<TEventData> Child(string stateName) =>
            new WithTarget<TEventData>();
        public WithTarget<TEventData> Sibling(string stateName) =>
            new WithTarget<TEventData>();
        public WithTarget<TEventData> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget<TEventData>();
        public WithTarget<TEventData> Multiple(BaseTargetDefinition target, params BaseTargetDefinition[] targets) =>
            new WithTarget<TEventData>();
    }

    public class Guarded
    {
        public GuardedTransitionTo TransitionTo =>
            new GuardedTransitionTo();
    }
    public class Guarded<TEventData>
    {
        public GuardedTransitionTo TransitionTo =>
            new GuardedTransitionTo();
    }

    public class GuardedTransitionTo
    {
        public WithTarget Child(string stateName) =>
            new WithTarget();
        public WithTarget Sibling(string stateName) =>
            new WithTarget();
        public WithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget();
        public WithTarget Multiple(BaseTargetDefinition target, params BaseTargetDefinition[] targets) =>
            new WithTarget();
    }
    public class GuardedTransitionTo<TEventData>
    {
        public WithTarget<TEventData> Child(string stateName) =>
            new WithTarget<TEventData>();
        public WithTarget<TEventData> Sibling(string stateName) =>
            new WithTarget<TEventData>();
        public WithTarget<TEventData> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget<TEventData>();
        public WithTarget<TEventData> Multiple(BaseTargetDefinition target, params BaseTargetDefinition[] targets) =>
            new WithTarget<TEventData>();
    }

    public class WithTarget : IEventDefinition
    {
        public WithActions WithActions(Action action, params Action[] actions) =>
            new WithActions();
        public WithActions WithActions<TContext>(Action<TContext> action, params Action<TContext>[] actions) =>
            new WithActions();
    }
    public class WithTarget<TEventData> : IEventDefinition
    {
        public WithActions WithActions<TContext>(Action<TContext, TEventData> action, params Action<TContext, TEventData>[] actions) =>
            new WithActions();
    }

    public class WithActions : IEventDefinition
    {
    }
}
