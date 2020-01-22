using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language.Transition
{
    public class WithEvent
    {
        public Definition.Event Event { get; }

        private WithEvent(Definition.Event @event) => Event = @event;

        internal static WithEvent OfEventType(string eventType) =>
            new WithEvent(new Definition.CustomEvent(eventType));
        internal static WithEvent Immediately() =>
            new WithEvent(new Definition.ImmediateEvent());
        internal static WithEvent Delayed(TimeSpan delay) =>
            new WithEvent(new Definition.DelayedEvent(delay));

        public WithDataEvent<TEventData> WithData<TEventData>() =>
            new WithDataEvent<TEventData>(Event);

        public Guarded IfIn(StateConfiguration stateConfiguration) =>
            new Guarded(Event, new Definition.InStateGuard(stateConfiguration));
        public Guarded<TContext> If<TContext>(Func<TContext, bool> condition)
            where TContext : IEquatable<TContext> =>
            new Guarded<TContext>(Event,
                new Definition.ConditionContextGuard(context => condition((TContext)context))); // TODO: make sure this Typecast works

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
        public Definition.InStateGuard Guard { get; }

        public Guarded(Definition.Event @event, Definition.InStateGuard guard)
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
        public Definition.ConditionContextGuard Guard { get; }

        public Guarded(Definition.Event @event, Definition.ConditionContextGuard guard)
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
        public Definition.InStateGuard Guard { get; }

        public GuardedTransitionTo(Definition.Event @event, Definition.InStateGuard guard)
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
        public Definition.ConditionContextGuard Guard { get; }

        public GuardedTransitionTo(Definition.Event @event, Definition.ConditionContextGuard guard)
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

    public class WithTarget : Definition.UnguardedTransition
    {
        internal WithTarget(Definition.Event @event, IEnumerable<Definition.Target> targets)
        {
            Event = @event;
            Targets = targets;
        }
        internal WithTarget(Definition.Event @event, Definition.Target target, params Definition.Target[] targets) : this(@event,
            target.Append(targets))
        { }

        public override Definition.Event Event { get; }
        public override IEnumerable<Definition.Target> Targets { get; }
        public override IEnumerable<Definition.Action> Actions => Enumerable.Empty<Definition.Action>();

        public WithActions WithActions(Definition.Action action, params Definition.Action[] actions) =>
            new WithActions(this, action, actions);
        public WithActions WithActions<TContext>(
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] actions) =>
            throw new NotImplementedException(); // new WithActions<TContext>(this, action, actions); 
    }
    public class GuardedWithTarget : Definition.GuardedTransition
    {
        internal GuardedWithTarget(Definition.Event @event, Definition.InStateGuard guard, IEnumerable<Definition.Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(Definition.Event @event, Definition.InStateGuard guard, Definition.Target target, params Definition.Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override Definition.Event Event { get; }
        public override Definition.InStateGuard Guard { get; }
        public override IEnumerable<Definition.Target> Targets { get; }
        public override IEnumerable<Definition.Action> Actions => Enumerable.Empty<Definition.Action>();

        public GuardedWithActions WithActions(Definition.Action action, params Definition.Action[] actions) =>
            new GuardedWithActions(this, action, actions);
        public GuardedWithActions<TContext> WithActions<TContext>(
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] actions) where TContext : IEquatable<TContext> =>
            new GuardedWithActions<TContext>(this, action, actions);
    }
    public class GuardedWithTarget<TContext> : Definition.GuardedContextTransition
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithTarget(Definition.Event @event, OneOf<Definition.InStateGuard, Definition.ConditionContextGuard> guard, IEnumerable<Definition.Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(Definition.Event @event, OneOf<Definition.InStateGuard, Definition.ConditionContextGuard> guard, Definition.Target target, params Definition.Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override Definition.Event Event { get; }
        public override OneOf<Definition.InStateGuard, Definition.ConditionContextGuard> Guard { get; }
        public override IEnumerable<Definition.Target> Targets { get; }
        public override IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> Actions => Enumerable.Empty<OneOf<Definition.Action, Definition.ContextAction>>();

        public GuardedWithActions<TContext> WithActions(
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] actions) =>
            new GuardedWithActions<TContext>(this, action, actions);
    }

    public class WithActions : WithTarget
    {
        internal WithActions(
            Definition.UnguardedTransition transition,
            Definition.Action action,
            params Definition.Action[] actions) : base(transition.Event, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<Definition.Action> Actions { get; }
    }
    public class GuardedWithActions : GuardedWithTarget
    {
        internal GuardedWithActions(
            Definition.GuardedTransition transition,
            Definition.Action action,
            params Definition.Action[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<Definition.Action> Actions { get; }
    }
    public class GuardedWithActions<TContext> : GuardedWithTarget<TContext>
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithActions(
            Definition.GuardedTransition transition,
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);
        internal GuardedWithActions(
            Definition.GuardedContextTransition transition,
            OneOf<Definition.Action, Definition.ContextAction> action,
            params OneOf<Definition.Action, Definition.ContextAction>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> Actions { get; }
    }
}
