using System;
using System.Collections.Generic;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language.Transition
{
    public class WithEvent
    {
        public Definition.Event Event { get; }

        private WithEvent(Definition.Event @event) => Event = @event;

        internal static WithEvent OfEventType(string eventType) =>
            new WithEvent(new CustomEvent(eventType));
        internal static WithEvent Immediately() =>
            new WithEvent(new ImmediateEvent());
        internal static WithEvent Delayed(TimeSpan delay) =>
            new WithEvent(new DelayedEvent(delay));

        public WithDataEvent<TEventData> WithData<TEventData>() =>
            new WithDataEvent<TEventData>(Event);

        public Guarded IfIn(StateConfiguration stateConfiguration) =>
            new Guarded(Event, new InStateGuard(stateConfiguration));
        public Guarded<TContext> If<TContext>(Func<TContext, bool> condition)
            where TContext : IEquatable<TContext> =>
            new Guarded<TContext>(Event,
                new ConditionContextGuard(context => condition((TContext)context))); // TODO: make sure this Typecast works

        public UnguardedTransitionTo TransitionTo =>
            new UnguardedTransitionTo(Event);
    }
    public class WithDataEvent<TEventData>
    {
        public Definition.Event Event { get; }

        internal WithDataEvent(Definition.Event @event) => Event = @event;

        public UnguardedTransitionTo TransitionTo =>
            throw new NotImplementedException();

        public Guarded IfIn(StateConfiguration stateConfiguration) =>
            throw new NotImplementedException(); // new DataGuarded<TEventData>(Event, new InStateGuard(stateConfiguration));
        public Guarded If<TContext>(Func<TContext, bool> condition)
            where TContext : IEquatable<TContext> =>
            throw new NotImplementedException(); // new DataGuarded<TEventData, TContext>(Event, new ConditionContextGuard(context => condition((TContext)context))); // TODO: make sure this Typecast works
        public Guarded If<TContext>(Func<TContext, TEventData, bool> condition)
            where TContext : IEquatable<TContext> =>
            throw new NotImplementedException(); // new DataGuarded<TEventData, TContext>(Event, new ConditionContextDataGuard((context, data) => condition((TContext)context, (TEventData)data))); // TODO: make sure this Typecasts work
    }

    public class UnguardedTransitionTo
    {
        public Definition.Event Event { get; }
        public UnguardedTransitionTo(Definition.Event @event) => Event = @event;

        public WithTarget Child(string stateName) =>
            new WithTarget(Event, Keywords.Child(stateName));
        public WithTarget Sibling(string stateName) =>
            new WithTarget(Event, Keywords.Sibling(stateName));
        public WithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget(Event, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public WithTarget Multiple(Definition.Target target, params Definition.Target[] targets) =>
            new WithTarget(Event, target, targets);
    }

    public class Guarded
    {
        public Definition.Event Event { get; }
        public InStateGuard Guard { get; }

        public Guarded(Definition.Event @event, InStateGuard guard)
        {
            Event = @event;
            Guard = guard;
        }

        public GuardedTransitionTo TransitionTo =>
            new GuardedTransitionTo(Event, Guard);
    }
    public class Guarded<TContext>
        where TContext : IEquatable<TContext>
    {
        public Definition.Event Event { get; }
        public ConditionContextGuard Guard { get; }

        public Guarded(Definition.Event @event, ConditionContextGuard guard)
        {
            Event = @event;
            Guard = guard;
        }

        public GuardedTransitionTo<TContext> TransitionTo =>
            new GuardedTransitionTo<TContext>(Event, Guard);
    }

    public class GuardedTransitionTo
    {
        public Definition.Event Event { get; }
        public InStateGuard Guard { get; }

        public GuardedTransitionTo(Definition.Event @event, InStateGuard guard)
        {
            Event = @event;
            Guard = guard;
        }

        public GuardedWithTarget Child(string stateName) =>
            new GuardedWithTarget(Event, Guard, Keywords.Child(stateName));
        public GuardedWithTarget Sibling(string stateName) =>
            new GuardedWithTarget(Event, Guard, Keywords.Sibling(stateName));
        public GuardedWithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new GuardedWithTarget(Event, Guard, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public GuardedWithTarget Multiple(Definition.Target target, params Definition.Target[] targets) =>
            new GuardedWithTarget(Event, Guard, target, targets);
    }
    public class GuardedTransitionTo<TContext>
        where TContext : IEquatable<TContext>
    {
        public Definition.Event Event { get; }
        public ConditionContextGuard Guard { get; }

        public GuardedTransitionTo(Definition.Event @event, ConditionContextGuard guard)
        {
            Event = @event;
            Guard = guard;
        }

        public GuardedWithTarget<TContext> Child(string stateName) =>
            new GuardedWithTarget<TContext>(Event, Guard, Keywords.Child(stateName));
        public GuardedWithTarget<TContext> Sibling(string stateName) =>
            new GuardedWithTarget<TContext>(Event, Guard, Keywords.Sibling(stateName));
        public GuardedWithTarget<TContext> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new GuardedWithTarget<TContext>(Event, Guard, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public GuardedWithTarget<TContext> Multiple(Definition.Target target, params Definition.Target[] targets) =>
            new GuardedWithTarget<TContext>(Event, Guard, target, targets);
    }

    public class WithTarget : UnguardedTransition
    {
        internal WithTarget(Definition.Event @event, IEnumerable<Definition.Target> targets)
        {
            Event = @event;
            Targets = targets;
        }
        internal WithTarget(Definition.Event @event, Target target, params Target[] targets) : this(@event,
            target.Append(targets))
        { }

        public override Definition.Event Event { get; }
        public override IEnumerable<Definition.Target> Targets { get; }

        public WithActions WithActions(Definition.Action action, params Definition.Action[] actions) =>
            new WithActions(this, action, actions);
        public WithActions WithActions<TContext>(
            OneOf<Definition.Action, ContextAction> action,
            params OneOf<Definition.Action, ContextAction>[] actions) =>
            throw new NotImplementedException(); // new WithActions<TContext>(this, action, actions); 
    }
    public class GuardedWithTarget : GuardedTransition
    {
        internal GuardedWithTarget(Definition.Event @event, Definition.InStateGuard guard, IEnumerable<Definition.Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(Definition.Event @event, Definition.InStateGuard guard, Target target, params Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override Definition.Event Event { get; }
        public override InStateGuard Guard { get; }
        public override IEnumerable<Definition.Target> Targets { get; }

        public GuardedWithActions WithActions(Definition.Action action, params Definition.Action[] actions) =>
            new GuardedWithActions(this, action, actions);
        public GuardedWithActions<TContext> WithActions<TContext>(
            OneOf<Definition.Action, ContextAction> action,
            params OneOf<Definition.Action, ContextAction>[] actions) where TContext : IEquatable<TContext> =>
            new GuardedWithActions<TContext>(this, action, actions);
    }
    public class GuardedWithTarget<TContext> : GuardedContextTransition
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithTarget(Definition.Event @event, OneOf<InStateGuard, ConditionContextGuard> guard, IEnumerable<Definition.Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(Definition.Event @event, OneOf<InStateGuard, ConditionContextGuard> guard, Target target, params Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override Definition.Event Event { get; }
        public override OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Definition.Target> Targets { get; }

        public GuardedWithActions<TContext> WithActions(
            OneOf<Definition.Action, ContextAction> action,
            params OneOf<Definition.Action, ContextAction>[] actions) =>
            new GuardedWithActions<TContext>(this, action, actions);
    }

    public class WithActions : WithTarget
    {
        internal WithActions(
            UnguardedTransition transition,
            Definition.Action action,
            params Definition.Action[] actions) : base(transition.Event, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<Definition.Action> Actions { get; }
    }
    public class GuardedWithActions : GuardedWithTarget
    {
        internal GuardedWithActions(
            GuardedTransition transition,
            Definition.Action action,
            params Definition.Action[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<Definition.Action> Actions { get; }
    }
    public class GuardedWithActions<TContext> : GuardedWithTarget<TContext>
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithActions(
            GuardedTransition transition,
            OneOf<Definition.Action, ContextAction> action,
            params OneOf<Definition.Action, ContextAction>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);
        internal GuardedWithActions(
            GuardedContextTransition transition,
            OneOf<Definition.Action, ContextAction> action,
            params OneOf<Definition.Action, ContextAction>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<OneOf<Definition.Action, ContextAction>> Actions { get; }
    }
}
