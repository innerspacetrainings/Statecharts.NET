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

        public UnguardedWithTarget Child(string stateName, params string[] childStatenodesNames) =>
            new UnguardedWithTarget(_event, Keywords.Child(stateName, childStatenodesNames));
        public UnguardedWithTarget Sibling(string stateName, params string[] childStatenodesNames) =>
            new UnguardedWithTarget(_event, Keywords.Sibling(stateName, childStatenodesNames));
        public UnguardedWithTarget Absolute(string statechartName, params string[] childStatenodesNames) =>
            new UnguardedWithTarget(_event, Keywords.Absolute(statechartName, childStatenodesNames));
        public UnguardedWithTarget Self =>
            new UnguardedWithTarget(_event, new SelfTarget());
        public UnguardedWithTarget Target(Target target) =>
            new UnguardedWithTarget(_event, target);
        public UnguardedWithTarget Multiple(Target target, params Target[] targets) =>
            new UnguardedWithTarget(_event, target, targets);
    }
    public class UnguardedDataTransitionTo<TEventData>
    {
        private readonly IDataEventDefinition _event;

        public UnguardedDataTransitionTo(IDataEventDefinition @event) => _event = @event;

        public DataUnguardedWithTarget<TEventData> Child(string stateName, params string[] childStatenodesNames) =>
            new DataUnguardedWithTarget<TEventData>(_event, Keywords.Child(stateName, childStatenodesNames));
        public DataUnguardedWithTarget<TEventData> Sibling(string stateName, params string[] childStatenodesNames) =>
            new DataUnguardedWithTarget<TEventData>(_event, Keywords.Sibling(stateName, childStatenodesNames));
        public DataUnguardedWithTarget<TEventData> Absolute(string statechartName, params string[] childStatenodesNames) =>
            new DataUnguardedWithTarget<TEventData>(_event, Keywords.Absolute(statechartName, childStatenodesNames));
        public DataUnguardedWithTarget<TEventData> Self =>
            new DataUnguardedWithTarget<TEventData>(_event, new SelfTarget());
        public DataUnguardedWithTarget<TEventData> Target(Target target) =>
            new DataUnguardedWithTarget<TEventData>(_event, target);
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

        public GuardedWithTarget Child(string stateName, params string[] childStatenodesNames) =>
            new GuardedWithTarget(_event, _guard, Keywords.Child(stateName, childStatenodesNames));
        public GuardedWithTarget Sibling(string stateName, params string[] childStatenodesNames) =>
            new GuardedWithTarget(_event, _guard, Keywords.Sibling(stateName, childStatenodesNames));
        public GuardedWithTarget Absolute(string statechartName, params string[] childStatenodesNames) =>
            new GuardedWithTarget(_event, _guard, Keywords.Absolute(statechartName, childStatenodesNames));
        public GuardedWithTarget Self =>
            new GuardedWithTarget(_event, _guard, new SelfTarget());
        public GuardedWithTarget Target(Target target) =>
            new GuardedWithTarget(_event, _guard, target);
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

        public ContextGuardedWithTarget<TContext> Child(string stateName, params string[] childStatenodesNames) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, Keywords.Child(stateName, childStatenodesNames));
        public ContextGuardedWithTarget<TContext> Sibling(string stateName, params string[] childStatenodesNames) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, Keywords.Sibling(stateName, childStatenodesNames));
        public ContextGuardedWithTarget<TContext> Absolute(string statechartName, params string[] childStatenodesNames) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, Keywords.Absolute(statechartName, childStatenodesNames));
        public ContextGuardedWithTarget<TContext> Self =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, new SelfTarget());
        public ContextGuardedWithTarget<TContext> Target(Target target) =>
            new ContextGuardedWithTarget<TContext>(_event, _guard, target);
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

        public DataGuardedWithTarget<TEventData> Child(string stateName, params string[] childStatenodesNames) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, Keywords.Child(stateName, childStatenodesNames));
        public DataGuardedWithTarget<TEventData> Sibling(string stateName, params string[] childStatenodesNames) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, Keywords.Sibling(stateName, childStatenodesNames));
        public DataGuardedWithTarget<TEventData> Absolute(string statechartName, params string[] childStatenodesNames) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, Keywords.Absolute(statechartName, childStatenodesNames));
        public DataGuardedWithTarget<TEventData> Self =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, new SelfTarget());
        public DataGuardedWithTarget<TEventData> Target(Target target) =>
            new DataGuardedWithTarget<TEventData>(_event, _guard, target);
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

        public ContextDataGuardedWithTarget<TContext, TEventData> Child(string stateName, params string[] childStatenodesNames) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, Keywords.Child(stateName, childStatenodesNames));
        public ContextDataGuardedWithTarget<TContext, TEventData> Sibling(string stateName, params string[] childStatenodesNames) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, Keywords.Sibling(stateName, childStatenodesNames));
        public ContextDataGuardedWithTarget<TContext, TEventData> Absolute(string statechartName, params string[] childStatenodesNames) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, Keywords.Absolute(statechartName, childStatenodesNames));
        public ContextDataGuardedWithTarget<TContext, TEventData> Self =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, new SelfTarget());
        public ContextDataGuardedWithTarget<TContext, TEventData> Target(Target target) =>
            new ContextDataGuardedWithTarget<TContext, TEventData>(_event, _guard, target);
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
        public override IEnumerable<Model.ActionDefinition> Actions => Enumerable.Empty<Model.ActionDefinition>();

        public UnguardedWithActions WithActions(ActionDefinition action, params ActionDefinition[] actions) =>
            new UnguardedWithActions(this, action, actions);
        public ContextUnguardedWithActions<TContext> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
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
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions => Enumerable.Empty<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>();

        public UnguardedWithActions WithActions(ActionDefinition action, params ActionDefinition[] actions) =>
            new UnguardedWithActions(this, action, actions);
        public ContextUnguardedWithActions<TContext> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextUnguardedWithActions<TContext>(this, action, actions);
        public ContextDataUnguardedWithActions<TContext, TEventData> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>>[] actions)
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
        public override IEnumerable<Model.ActionDefinition> Actions => Enumerable.Empty<Model.ActionDefinition>();

        public GuardedWithActions WithActions(ActionDefinition action, params ActionDefinition[] actions) =>
            new GuardedWithActions(this, action, actions);
        public ContextGuardedWithActions<TContext> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
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
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> Actions => Enumerable.Empty<OneOf<Model.ActionDefinition, ContextActionDefinition>>();

        public ContextGuardedWithActions<TContext> WithActions(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions) =>
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
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions => Enumerable.Empty<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>();

        public ContextDataGuardedWithActions<TContext, TEventData> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new ContextDataGuardedWithActions<TContext, TEventData>(this, action, actions);
        public ContextDataGuardedWithActions<TContext, TEventData> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>>[] actions)
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
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions => Enumerable.Empty<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>();

        public ContextDataGuardedWithActions<TContext, TEventData> WithActions(
            OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>>[] actions) =>
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
            ActionDefinition action,
            params ActionDefinition[] actions) : this(transition.Event, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());
        internal UnguardedWithActions(
            UnguardedContextDataTransitionDefinition transition,
            ActionDefinition action,
            params ActionDefinition[] actions) : this(transition.Event, transition.Targets) =>
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<Model.ActionDefinition> Actions { get; }
    }
    public class ContextUnguardedWithActions<TContext> : UnguardedContextTransitionDefinition
        where TContext : IContext<TContext>
    {
        private ContextUnguardedWithActions(IEventDefinition @event, IEnumerable<Target> targets, IEnumerable<OneOf<ActionDefinition, ActionDefinition<TContext>>> actions)
        {
            Event = @event;
            Targets = targets;
            Actions = actions.Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        }
        internal ContextUnguardedWithActions(
            UnguardedTransitionDefinition transition,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions) : this(transition.Event, transition.Targets, action.Append(actions)) { }
        internal ContextUnguardedWithActions(
            UnguardedContextDataTransitionDefinition transition,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions) : this(transition.Event, transition.Targets, action.Append(actions)) { }

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    public class ContextDataUnguardedWithActions<TContext, TEventData> : UnguardedContextDataTransitionDefinition
        where TContext : IContext<TContext>
    {
        internal ContextDataUnguardedWithActions(
            UnguardedContextDataTransitionDefinition transition,
            OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>>[] actions)
        {
            Event = transition.Event;
            Targets = transition.Targets;
            Actions = action.Append(actions).Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction(),
                contextDataAction => contextDataAction.ToDefinitionAction()));
        }

        public override IDataEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    public class GuardedWithActions : GuardedTransitionDefinition
    {
        internal GuardedWithActions(
            GuardedTransitionDefinition transition,
            ActionDefinition action,
            params Language.ActionDefinition[] actions)
        {
            Event = transition.Event;
            Guard = transition.Guard;
            Targets = transition.Targets;
            Actions = action.Append(actions).Select(a => a.ToDefinitionAction());
        }

        public override IEventDefinition Event { get; }
        public override InStateGuard Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<Model.ActionDefinition> Actions { get; }
    }
    public class ContextGuardedWithActions<TContext> : GuardedContextTransitionDefinition
        where TContext : IContext<TContext>
    {
        private ContextGuardedWithActions(IEventDefinition @event, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, IEnumerable<Target> targets, IEnumerable<OneOf<ActionDefinition, ActionDefinition<TContext>>> actions)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
            Actions = actions.Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        }
        internal ContextGuardedWithActions(
            GuardedTransitionDefinition transition,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions) : this(transition.Event, transition.Guard, transition.Targets, action.Append(actions)) { }
        internal ContextGuardedWithActions(
            GuardedContextTransitionDefinition transition,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions) : this(transition.Event, transition.Guard, transition.Targets, action.Append(actions)) { }
        
        public override IEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition>> Actions { get; }
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
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions) : this(transition) =>
            Actions = action.Append(actions).Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction()));
        internal ContextDataGuardedWithActions(
            GuardedContextDataTransitionDefinition transition,
            OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>, ActionDefinition<TContext, TEventData>>[] actions) : this(transition) =>
            Actions = action.Append(actions).Select(a => a.Match<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>>(
                contextlessAction => contextlessAction.ToDefinitionAction(),
                contextAction => contextAction.ToDefinitionAction(),
                contextDataAction => contextDataAction.ToDefinitionAction()));

        public override IDataEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard, ConditionContextDataGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<Model.ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> Actions { get; }
    }
    #endregion
}
