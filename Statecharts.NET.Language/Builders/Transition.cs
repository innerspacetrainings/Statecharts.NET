using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language.Builders.Transition
{
    public class WithEvent
    {
        public IEventDefinition Event { get; }

        private WithEvent(IEventDefinition @event) => Event = @event;

        internal static WithEvent OfNamedEvent(NamedEvent namedEvent) =>
            new WithEvent(namedEvent);
        internal static WithDataEvent<TEventData> OfNamedDataEvent<TEventData>(NamedDataEvent<TEventData> namedDataEvent) =>
            new WithDataEvent<TEventData>(namedDataEvent);
        internal static WithEvent Immediately() =>
            new WithEvent(new ImmediateEventDefinition());
        internal static WithEvent Delayed(TimeSpan delay) =>
            new WithEvent(new DelayedEventDefinition(delay));
        internal static WithEvent OnDone() =>
            new WithEvent(new DoneEventDefinition());
        internal static WithEvent OnServiceSuccess() =>
            new WithEvent(new ServiceSuccessEventDefinition());
        internal static WithEvent OnServiceError() =>
            new WithEvent(new ServiceErrorEventDefinition());
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
        public IEventDefinition Event { get; }

        internal WithDataEvent(IEventDefinition @event) => Event = @event;

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
        public IEventDefinition Event { get; }
        public UnguardedTransitionTo(IEventDefinition @event) => Event = @event;

        public WithTarget Child(string stateName) =>
            new WithTarget(Event, Keywords.Child(stateName));
        public WithTarget Sibling(string stateName) =>
            new WithTarget(Event, Keywords.Sibling(stateName));
        public WithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithTarget(Event, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public WithTarget Self =>
            new WithTarget(Event, new SelfTarget());
        public WithTarget Multiple(Target target, params Target[] targets) =>
            new WithTarget(Event, target, targets);
    }

    public class Guarded
    {
        public IEventDefinition Event { get; }
        public InStateGuard Guard { get; }

        public Guarded(IEventDefinition @event, InStateGuard guard)
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
        public IEventDefinition Event { get; }
        public ConditionContextGuard Guard { get; }

        public Guarded(IEventDefinition @event, ConditionContextGuard guard)
        {
            Event = @event;
            Guard = guard;
        }

        public GuardedTransitionTo<TContext> TransitionTo =>
            new GuardedTransitionTo<TContext>(Event, Guard);
    }

    public class GuardedTransitionTo
    {
        public IEventDefinition Event { get; }
        public InStateGuard Guard { get; }

        public GuardedTransitionTo(IEventDefinition @event, InStateGuard guard)
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
        public IEventDefinition Event { get; }
        public ConditionContextGuard Guard { get; }

        public GuardedTransitionTo(IEventDefinition @event, ConditionContextGuard guard)
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

    public class WithTarget : UnguardedTransitionDefinition
    {
        internal WithTarget(IEventDefinition @event, IEnumerable<Target> targets)
        {
            Event = @event;
            Targets = targets;
        }
        internal WithTarget(IEventDefinition @event, Target target, params Target[] targets) : this(@event,
            target.Append(targets))
        { }

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<ActionDefinition> Actions => Enumerable.Empty<ActionDefinition>();

        public WithActions WithActions(Language.Action action, params Language.Action[] actions) =>
            new WithActions(this, action, actions);
        public WithActions<TContext> WithActions<TContext>(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions)
            where TContext : IEquatable<TContext> =>
            new WithActions<TContext>(this, action, actions);
    }
    public class GuardedWithTarget : GuardedTransitionDefinition
    {
        internal GuardedWithTarget(IEventDefinition @event, InStateGuard guard, IEnumerable<Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(IEventDefinition @event, InStateGuard guard, Target target, params Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override IEventDefinition Event { get; }
        public override InStateGuard Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<ActionDefinition> Actions => Enumerable.Empty<ActionDefinition>();

        public GuardedWithActions WithActions(Language.Action action, params Language.Action[] actions) =>
            new GuardedWithActions(this, action, actions);
        public GuardedWithActions<TContext> WithActions<TContext>(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions) where TContext : IEquatable<TContext> =>
            new GuardedWithActions<TContext>(this, action, actions);
    }
    public class GuardedWithTarget<TContext> : GuardedContextTransitionDefinition
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithTarget(IEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, IEnumerable<Target> targets)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
        }
        internal GuardedWithTarget(IEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, Target target, params Target[] targets) : this(@event, guard,
            target.Append(targets))
        { }

        public override IEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions => Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition>>();

        public GuardedWithActions<TContext> WithActions(
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions) =>
            new GuardedWithActions<TContext>(this, action, actions);
    }

    public class WithActions : WithTarget
    {
        internal WithActions(
            UnguardedTransitionDefinition transition,
            Language.Action action,
            params Language.Action[] actions) : base(transition.Event, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());

        public override IEnumerable<ActionDefinition> Actions { get; }
    }
    public class WithActions<TContext> : UnguardedContextTransitionDefinition
        where TContext : IEquatable<TContext>
    {
        private UnguardedTransitionDefinition Transition { get; }

        internal WithActions(
            UnguardedTransitionDefinition transition,
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions)
        {
            Transition = transition;
            Actions = action.Append(actions).Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        }

        public override IEventDefinition Event => Transition.Event;
        public override IEnumerable<Target> Targets => Transition.Targets;
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public class GuardedWithActions : GuardedWithTarget
    {
        internal GuardedWithActions(
            GuardedTransitionDefinition transition,
            Language.Action action,
            params Language.Action[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());

        public override IEnumerable<ActionDefinition> Actions { get; }
    }
    public class GuardedWithActions<TContext> : GuardedWithTarget<TContext>
        where TContext : IEquatable<TContext>
    {
        internal GuardedWithActions(
            GuardedTransitionDefinition transition,
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        internal GuardedWithActions(
            GuardedContextTransitionDefinition transition,
            OneOf<Language.Action, Language.Action<TContext>> action,
            params OneOf<Language.Action, Language.Action<TContext>>[] actions) : base(transition.Event, transition.Guard, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction())); // TODO: refactor this code to a utility function

        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
}
