using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language.Builders.Transition
{
    #region WithEvent, WithNamedEvent & WithNamedDataEvent
    public class WithEvent
    {
        private readonly IEventDefinition _event;

        private WithEvent(IEventDefinition @event) => _event = @event;

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

        public Guarded IfIn(StateConfiguration stateConfiguration) =>
            new Guarded(_event, new InStateGuard(stateConfiguration));
        public ContextGuarded<TContext> If<TContext>(Func<TContext, bool> condition)
            where TContext : IContext<TContext> =>
            new ContextGuarded<TContext>(_event, new ConditionContextGuard(context => condition((TContext)context)));

        public UnguardedTransitionTo TransitionTo =>
            new UnguardedTransitionTo(_event);
    }
    public class WithNamedEvent
    {
        private readonly NamedEvent _event;

        internal WithNamedEvent(string eventName) : this(new NamedEvent(eventName)) { }
        internal WithNamedEvent(NamedEvent @event) => _event = @event;

        public WithNamedDataEvent<TEventData> WithData<TEventData>() =>
            new WithNamedDataEvent<TEventData>(new NamedDataEvent<TEventData>(_event.Name, default));

        public Guarded IfIn(StateConfiguration stateConfiguration) =>
            new Guarded(_event, new InStateGuard(stateConfiguration));
        public ContextGuarded<TContext> If<TContext>(Func<TContext, bool> condition)
            where TContext : IContext<TContext> =>
            new ContextGuarded<TContext>(_event, new ConditionContextGuard(context => condition((TContext)context)));

        public UnguardedTransitionTo TransitionTo =>
            new UnguardedTransitionTo(_event);
    }
    public class WithNamedDataEvent<TEventData>
    {
        private readonly IDataEventDefinition _event;

        internal WithNamedDataEvent(IDataEventDefinition @event) => _event = @event;

        public UnguardedDataTransitionTo<TEventData> TransitionTo =>
            new UnguardedDataTransitionTo<TEventData>(_event);

        public DataGuarded<TEventData> IfIn(StateConfiguration stateConfiguration) =>
            new DataGuarded<TEventData>(_event, new InStateGuard(stateConfiguration));
        public ContextDataGuarded<TContext, TEventData> If<TContext>(Func<TContext, bool> condition)
            where TContext : IContext<TContext> =>
            new ContextDataGuarded<TContext, TEventData>(_event, new ConditionContextGuard(context => condition((TContext)context)));
        public ContextDataGuarded<TContext, TEventData> If<TContext>(Func<TContext, TEventData, bool> condition)
            where TContext : IContext<TContext> =>
            new ContextDataGuarded<TContext, TEventData>(_event, new ConditionContextDataGuard((context, data) => condition((TContext)context, (TEventData)data)));
    }
    #endregion
    #region Unguarded
    public class UnguardedTransitionTo
    {
        private readonly IEventDefinition _event;

        public UnguardedTransitionTo(IEventDefinition @event) => _event = @event;

        public UnguardedWithTarget Child(string stateName) =>
            new UnguardedWithTarget(_event, Keywords.Child(stateName));
        public UnguardedWithTarget Sibling(string stateName) =>
            new UnguardedWithTarget(_event, Keywords.Sibling(stateName));
        public UnguardedWithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new UnguardedWithTarget(_event, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public UnguardedWithTarget Self =>
            new UnguardedWithTarget(_event, new SelfTarget());
        public UnguardedWithTarget Multiple(Target target, params Target[] targets) =>
            new UnguardedWithTarget(_event, target, targets);
    }
    public class UnguardedDataTransitionTo<TEventData>
    {
        private readonly IDataEventDefinition _event;

        public UnguardedDataTransitionTo(IDataEventDefinition @event) => _event = @event;

        public DataUnguardedWithTarget<TEventData> Child(string stateName) =>
            new DataUnguardedWithTarget<TEventData>(_event, Keywords.Child(stateName));
        public DataUnguardedWithTarget<TEventData> Sibling(string stateName) =>
            new DataUnguardedWithTarget<TEventData>(_event, Keywords.Sibling(stateName));
        public DataUnguardedWithTarget<TEventData> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new DataUnguardedWithTarget<TEventData>(_event, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public DataUnguardedWithTarget<TEventData> Self =>
            new DataUnguardedWithTarget<TEventData>(_event, new SelfTarget());
        public DataUnguardedWithTarget<TEventData> Multiple(Target target, params Target[] targets) =>
            new DataUnguardedWithTarget<TEventData>(_event, target, targets);
    }
    #endregion
    #region Guarded
    public class Guarded
    {
        private readonly IEventDefinition _event;
        private readonly InStateGuard _guard;

        public Guarded(IEventDefinition @event, InStateGuard guard)
        {
            _event = @event;
            _guard = guard;
        }

        public GuardedTransitionTo TransitionTo =>
            new GuardedTransitionTo(_event, _guard);
    }
    public class ContextGuarded<TContext>
        where TContext : IContext<TContext>
    {
        private readonly IEventDefinition _event;
        private readonly ConditionContextGuard _guard;

        public ContextGuarded(IEventDefinition @event, ConditionContextGuard guard)
        {
            _event = @event;
            _guard = guard;
        }

        public ContextGuardedTransitionTo<TContext> TransitionTo =>
            new ContextGuardedTransitionTo<TContext>(_event, _guard);
    }
    public class DataGuarded<TEventData>
    {
        private readonly IDataEventDefinition _event;
        private readonly InStateGuard _guard;

        public DataGuarded(IDataEventDefinition @event, InStateGuard guard)
        {
            _event = @event;
            _guard = guard;
        }

        public DataGuardedTransitionTo<TEventData> TransitionTo =>
            new DataGuardedTransitionTo<TEventData>(_event, _guard);
    }
    public class ContextDataGuarded<TContext, TEventData>
        where TContext : IContext<TContext>
    {
        private readonly IDataEventDefinition _event;
        private readonly OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> _guard;

        private ContextDataGuarded(IDataEventDefinition @event) => _event = @event;
        public ContextDataGuarded(IDataEventDefinition @event, ConditionContextGuard guard) : this(@event) => _guard = guard;
        public ContextDataGuarded(IDataEventDefinition @event, ConditionContextDataGuard guard) : this(@event) => _guard = guard;

        public ContextDataGuardedTransitionTo<TContext, TEventData> TransitionTo =>
            new ContextDataGuardedTransitionTo<TContext, TEventData>(_event, _guard);
    }

    public class GuardedTransitionTo
    {
        private readonly IEventDefinition _event;
        private readonly InStateGuard _guard;

        public GuardedTransitionTo(IEventDefinition @event, InStateGuard guard)
        {
            _event = @event;
            _guard = guard;
        }

        public GuardedWithTarget Child(string stateName) =>
            new GuardedWithTarget(_event, _guard, Keywords.Child(stateName));
        public GuardedWithTarget Sibling(string stateName) =>
            new GuardedWithTarget(_event, _guard, Keywords.Sibling(stateName));
        public GuardedWithTarget Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new GuardedWithTarget(_event, _guard, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public GuardedWithTarget Self =>
            new GuardedWithTarget(_event, _guard, new SelfTarget());
        public GuardedWithTarget Multiple(Target target, params Target[] targets) =>
            new GuardedWithTarget(_event, _guard, target, targets);
    }
    public class ContextGuardedTransitionTo<TContext>
        where TContext : IContext<TContext>
    {
        private readonly IEventDefinition _event;
        private readonly ConditionContextGuard _guard;

        public ContextGuardedTransitionTo(IEventDefinition @event, ConditionContextGuard guard)
        {
            _event = @event;
            _guard = guard;
        }

        public ContextGuardedWithTarget<TContext> Child(string stateName) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, Keywords.Child(stateName));
        public ContextGuardedWithTarget<TContext> Sibling(string stateName) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, Keywords.Sibling(stateName));
        public ContextGuardedWithTarget<TContext> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public ContextGuardedWithTarget<TContext> Self =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, new SelfTarget());
        public ContextGuardedWithTarget<TContext> Multiple(Target target, params Target[] targets) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, target, targets);
    }
    public class DataGuardedTransitionTo<TEventData>
    {
        private readonly IDataEventDefinition _event;
        private readonly InStateGuard _guard;

        public DataGuardedTransitionTo(IDataEventDefinition @event, InStateGuard guard)
        {
            _event = @event;
            _guard = guard;
        }

        public DataGuardedWithTarget<TEventData> Child(string stateName) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, Keywords.Child(stateName));
        public DataGuardedWithTarget<TEventData> Sibling(string stateName) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, Keywords.Sibling(stateName));
        public DataGuardedWithTarget<TEventData> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public DataGuardedWithTarget<TEventData> Self =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, new SelfTarget());
        public DataGuardedWithTarget<TEventData> Multiple(Target target, params Target[] targets) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, target, targets);
    }
    public class ContextDataGuardedTransitionTo<TContext, TEventData>
        where TContext : IContext<TContext>
    {
        private readonly IDataEventDefinition _event;
        private readonly OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> _guard;

        public ContextDataGuardedTransitionTo(IDataEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> guard)
        {
            _event = @event;
            _guard = guard;
        }

        public ContextDataGuardedWithTarget<TContext, TEventData> Child(string stateName) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, Keywords.Child(stateName));
        public ContextDataGuardedWithTarget<TContext, TEventData> Sibling(string stateName) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, Keywords.Sibling(stateName));
        public ContextDataGuardedWithTarget<TContext, TEventData> Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public ContextDataGuardedWithTarget<TContext, TEventData> Self =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, new SelfTarget());
        public ContextDataGuardedWithTarget<TContext, TEventData> Multiple(Target target, params Target[] targets) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, target, targets);
    }
    #endregion
    #region WithTarget
    public class UnguardedWithTarget : UnguardedTransitionDefinition
    {
        internal UnguardedWithTarget(IEventDefinition @event, Target target, params Target[] targets)
        {
            Event = @event;
            Targets = target.Append(targets);
        }

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<ActionDefinition> Actions => Enumerable.Empty<ActionDefinition>();

        public UnguardedWithActions WithActions(Action action, params Action[] actions) =>
            new UnguardedWithActions(this, action, actions);
        public ContextUnguardedWithActions<TContext> WithActions<TContext>(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextUnguardedWithActions<TContext>(this, action, actions);
    }
    public class DataUnguardedWithTarget<TEventData> : UnguardedContextDataTransitionDefinition
    {
        internal DataUnguardedWithTarget(IDataEventDefinition @event, Target target, params Target[] targets)
        {
            Event = @event;
            Targets = target.Append(targets);
        }

        public override IDataEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions => Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>();

        public UnguardedWithActions WithActions(Action action, params Action[] actions) =>
            new UnguardedWithActions(this, action, actions);
        public ContextUnguardedWithActions<TContext> WithActions<TContext>(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextUnguardedWithActions<TContext>(this, action, actions);
        public ContextDataUnguardedWithActions<TContext, TEventData> WithActions<TContext>(
            OneOf<Action, Action<TContext>, Action<TContext, TEventData>> action,
            params OneOf<Action, Action<TContext>, Action<TContext, TEventData>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextDataUnguardedWithActions<TContext, TEventData>(this, action, actions);
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

        public GuardedWithActions WithActions(Action action, params Action[] actions) =>
            new GuardedWithActions(this, action, actions);
        public ContextGuardedWithActions<TContext> WithActions<TContext>(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextGuardedWithActions<TContext>(this, action, actions);
    }
    public class ContextGuardedWithTarget<TContext> : GuardedContextTransitionDefinition
        where TContext : IContext<TContext>
    {
        internal ContextGuardedWithTarget(IEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, Target target, params Target[] targets)
        {
            Event = @event;
            Guard = guard;
            Targets = target.Append(targets);
        }

        public override IEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions => Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition>>();

        public ContextGuardedWithActions<TContext> WithActions(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) =>
            new ContextGuardedWithActions<TContext>(this, action, actions);
    }
    public class DataGuardedWithTarget<TEventData> : GuardedContextDataTransitionDefinition
    {
        internal DataGuardedWithTarget(IDataEventDefinition @event, InStateGuard guard, Target target, params Target[] targets)
        {
            Event = @event;
            Guard = guard;
            Targets = target.Append(targets);
        }

        public override IDataEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions => Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>();

        public ContextDataGuardedWithActions<TContext, TEventData> WithActions<TContext>(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextDataGuardedWithActions<TContext, TEventData>(this, action, actions);
        public ContextDataGuardedWithActions<TContext, TEventData> WithActions<TContext>(
            OneOf<Action, Action<TContext>, Action<TContext, TEventData>> action,
            params OneOf<Action, Action<TContext>, Action<TContext, TEventData>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextDataGuardedWithActions<TContext, TEventData>(this, action, actions);
    }
    public class ContextDataGuardedWithTarget<TContext, TEventData> : GuardedContextDataTransitionDefinition
        where TContext : IContext<TContext>
    {
        internal ContextDataGuardedWithTarget(IDataEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> guard, Target target, params Target[] targets)
        {
            Event = @event;
            Guard = guard;
            Targets = target.Append(targets);
        }

        public override IDataEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions => Enumerable.Empty<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>();

        public ContextDataGuardedWithActions<TContext, TEventData> WithActions(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) =>
            new ContextDataGuardedWithActions<TContext, TEventData>(this, action, actions);
        public ContextDataGuardedWithActions<TContext, TEventData> WithActions(
            OneOf<Action, Action<TContext>, Action<TContext, TEventData>> action,
            params OneOf<Action, Action<TContext>, Action<TContext, TEventData>>[] actions) =>
            new ContextDataGuardedWithActions<TContext, TEventData>(this, action, actions);
    }
    #endregion
    #region WithActions
    public class UnguardedWithActions : UnguardedTransitionDefinition
    {
        private UnguardedWithActions(IEventDefinition @event, IEnumerable<Target> targets)
        {
            Event = @event;
            Targets = targets;
        }
        internal UnguardedWithActions(
            UnguardedTransitionDefinition transition,
            Action action,
            params Action[] actions) : this(transition.Event, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());
        internal UnguardedWithActions(
            UnguardedContextDataTransitionDefinition transition,
            Action action,
            params Action[] actions) : this(transition.Event, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<ActionDefinition> Actions { get; }
    }
    public class ContextUnguardedWithActions<TContext> : UnguardedContextTransitionDefinition
        where TContext : IContext<TContext>
    {
        private ContextUnguardedWithActions(IEventDefinition @event, IEnumerable<Target> targets, IEnumerable<OneOf<Action, Action<TContext>>> actions)
        {
            Event = @event;
            Targets = targets;
            Actions = actions.Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        }
        internal ContextUnguardedWithActions(
            UnguardedTransitionDefinition transition,
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) : this(transition.Event, transition.Targets, action.Append(actions)) { }
        internal ContextUnguardedWithActions(
            UnguardedContextDataTransitionDefinition transition,
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) : this(transition.Event, transition.Targets, action.Append(actions)) { }

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public class ContextDataUnguardedWithActions<TContext, TEventData> : UnguardedContextDataTransitionDefinition
        where TContext : IContext<TContext>
    {
        internal ContextDataUnguardedWithActions(
            UnguardedContextDataTransitionDefinition transition,
            OneOf<Action, Action<TContext>, Action<TContext, TEventData>> action,
            params OneOf<Action, Action<TContext>, Action<TContext, TEventData>>[] actions)
        {
            Event = transition.Event;
            Targets = transition.Targets;
            Actions = action.Append(actions).Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction(),
                contextDataAction => contextDataAction.ToDefinitionAction()));
        }

        public override IDataEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    public class GuardedWithActions : GuardedTransitionDefinition
    {
        internal GuardedWithActions(
            GuardedTransitionDefinition transition,
            Action action,
            params Language.Action[] actions)
        {
            Event = transition.Event;
            Guard = transition.Guard;
            Targets = transition.Targets;
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());
        }

        public override IEventDefinition Event { get; }
        public override InStateGuard Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<ActionDefinition> Actions { get; }
    }
    public class ContextGuardedWithActions<TContext> : GuardedContextTransitionDefinition
        where TContext : IContext<TContext>
    {
        private ContextGuardedWithActions(IEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, IEnumerable<Target> targets, IEnumerable<OneOf<Action, Action<TContext>>> actions)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
            Actions = actions.Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        }
        internal ContextGuardedWithActions(
            GuardedTransitionDefinition transition,
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) : this(transition.Event, transition.Guard, transition.Targets, action.Append(actions)) { }
        internal ContextGuardedWithActions(
            GuardedContextTransitionDefinition transition,
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) : this(transition.Event, transition.Guard, transition.Targets, action.Append(actions)) { }
        
        public override IEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public class ContextDataGuardedWithActions<TContext, TEventData> : GuardedContextDataTransitionDefinition
        where TContext : IContext<TContext>
    {
        private ContextDataGuardedWithActions(GuardedContextDataTransitionDefinition transition)
        {
            Event = transition.Event;
            Guard = transition.Guard;
            Targets = transition.Targets;
        }
        internal ContextDataGuardedWithActions(
            GuardedContextDataTransitionDefinition transition,
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions) : this(transition) =>
            Actions = action.Append(actions).Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        internal ContextDataGuardedWithActions(
            GuardedContextDataTransitionDefinition transition,
            OneOf<Action, Action<TContext>, Action<TContext, TEventData>> action,
            params OneOf<Action, Action<TContext>, Action<TContext, TEventData>>[] actions) : this(transition) =>
            Actions = action.Append(actions).Select(a => a.Match<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction(),
                contextDataAction => contextDataAction.ToDefinitionAction()));

        public override IDataEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    #endregion
}
