using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET.Language.Builders.Transition
{
    public class WithEvent
    {
        public Model.Event Event { get; }

        private WithEvent(Model.Event @event) => Event = @event;

        internal static WithEvent OfEventType(string eventType) =>
            new WithEvent(new CustomEvent(eventType));
        internal static WithEvent Immediately() =>
            new WithEvent(new ImmediateEvent());
        internal static WithEvent Delayed(TimeSpan delay) =>
            new WithEvent(new DelayedEvent(delay));
        internal static WithEvent OnCompoundDone() =>
            new WithEvent(new CustomEvent("#compound.done")); // TODO: CompoundDoneEvent()
        internal static WithEvent OnServiceSuccess() =>
            new WithEvent(new CustomEvent("#compound.done")); // TODO: ServiceSuccessEvent()
        internal static WithEvent OnServiceError() =>
            new WithEvent(new CustomEvent("#service.error")); // TODO: ServiceErrorEvent()

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
        public Model.Event Event { get; }

        internal WithDataEvent(Model.Event @event) => Event = @event;

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
        public Model.Event Event { get; }
        public UnguardedTransitionTo(Model.Event @event) => Event = @event;

        public WithTarget Child(string stateName) =>
            new WithTarget(Event, Keywords.Child(stateName));
        public WithTarget Sibling(string stateName) =>
            new WithTarget(Event, Keywords.Sibling(stateName));
        public WithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget(Event, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public WithTarget Multiple(Target target, params Target[] targets) =>
            new WithTarget(Event, target, targets);
    }

    public class Guarded
    {
        public Model.Event Event { get; }
        public InStateGuard Guard { get; }

        public Guarded(Model.Event @event, InStateGuard guard)
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
        public Model.Event Event { get; }
        public ConditionContextGuard Guard { get; }

        public Guarded(Model.Event @event, ConditionContextGuard guard)
        {
            Event = @event;
            Guard = guard;
        }

        public GuardedTransitionTo<TContext> TransitionTo =>
            new GuardedTransitionTo<TContext>(Event, Guard);
    }

    public class GuardedTransitionTo
    {
        public Model.Event Event { get; }
        public InStateGuard Guard { get; }

        public GuardedTransitionTo(Model.Event @event, InStateGuard guard)
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
        public GuardedWithTarget Multiple(Target target, params Target[] targets) =>
            new GuardedWithTarget(Event, Guard, target, targets);
    }
    public class GuardedTransitionTo<TContext>
        where TContext : IEquatable<TContext>
    {
        public Model.Event Event { get; }
        public ConditionContextGuard Guard { get; }

        public GuardedTransitionTo(Model.Event @event, ConditionContextGuard guard)
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
        public GuardedWithTarget<TContext> Multiple(Target target, params Target[] targets) =>
            new GuardedWithTarget<TContext>(Event, Guard, target, targets);
    }

    public class WithTarget : Definition.UnguardedTransition
    {
        internal WithTarget(Model.Event @event, IEnumerable<Target> targets)
        {
            Event = @event;
            Targets = targets;
        }
        internal WithTarget(Model.Event @event, Target target, params Target[] targets) : this(@event,
            target.Append(targets))
        { }

        public override Model.Event Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<Model.Action> Actions => Enumerable.Empty<Model.Action>();

        public WithActions WithActions(Model.Action action, params Model.Action[] actions) =>
            new WithActions(this, action, actions);
        public WithActions<TContext> WithActions<TContext>(
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] actions)
            where TContext : IEquatable<TContext> =>
            new WithActions<TContext>(this, action, actions);
    }
    public class GuardedWithTarget : Definition.GuardedTransition
    {
        internal GuardedWithTarget(Model.Event @event, InStateGuard guard, IEnumerable<Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(Model.Event @event, InStateGuard guard, Target target, params Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override Model.Event Event { get; }
        public override InStateGuard Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<Model.Action> Actions => Enumerable.Empty<Model.Action>();

        public GuardedWithActions WithActions(Model.Action action, params Model.Action[] actions) =>
            new GuardedWithActions(this, action, actions);
        public GuardedWithActions<TContext> WithActions<TContext>(
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] actions) where TContext : IEquatable<TContext> =>
            new GuardedWithActions<TContext>(this, action, actions);
    }
    public class GuardedWithTarget<TContext> : Definition.GuardedContextTransition
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithTarget(Model.Event @event, OneOf<InStateGuard, ConditionContextGuard> guard, IEnumerable<Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(Model.Event @event, OneOf<InStateGuard, ConditionContextGuard> guard, Target target, params Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override Model.Event Event { get; }
        public override OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<Model.Action, ContextAction>> Actions => Enumerable.Empty<OneOf<Model.Action, ContextAction>>();

        public GuardedWithActions<TContext> WithActions(
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] actions) =>
            new GuardedWithActions<TContext>(this, action, actions);
    }

    public class WithActions : WithTarget
    {
        internal WithActions(
            Definition.UnguardedTransition transition,
            Model.Action action,
            params Model.Action[] actions) : base(transition.Event, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<Model.Action> Actions { get; }
    }
    public class WithActions<TContext> : UnguardedContextTransition
        where TContext : IEquatable<TContext>
    {
        private Definition.UnguardedTransition Transition { get; }

        internal WithActions(
            Definition.UnguardedTransition transition,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] actions)
        {
            Transition = transition;
            Actions = action.Append(actions);
        }

        public override Model.Event Event => Transition.Event;
        public override IEnumerable<Target> Targets => Transition.Targets;
        public override IEnumerable<OneOf<Action, ContextAction>> Actions { get; }
    }
    public class GuardedWithActions : GuardedWithTarget
    {
        internal GuardedWithActions(
            Definition.GuardedTransition transition,
            Model.Action action,
            params Model.Action[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<Model.Action> Actions { get; }
    }
    public class GuardedWithActions<TContext> : GuardedWithTarget<TContext>
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithActions(
            Definition.GuardedTransition transition,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);
        internal GuardedWithActions(
            Definition.GuardedContextTransition transition,
            OneOf<Model.Action, ContextAction> action,
            params OneOf<Model.Action, ContextAction>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions);

        public override IEnumerable<OneOf<Model.Action, ContextAction>> Actions { get; }
    }
}
