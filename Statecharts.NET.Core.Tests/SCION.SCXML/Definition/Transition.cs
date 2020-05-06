﻿using System.Collections.Generic;
using System.Linq;
using Jint.Runtime;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class InitialTransition
    {
        internal IList<Transition> _transitions = new List<Transition>();

        public void AddTransition(Transition transition) => _transitions.Add(transition);

        public InitialCompoundTransitionDefinition ToDefinition() =>
            new InitialCompoundTransitionDefinition(new ChildTarget(_transitions.FirstOrDefault()?._target.ValueOr("this should never happen, otherwise there is an error in the SCXML tests")));
    }

    internal class Transition
    {
        internal string _eventName;
        internal Option<string> _target = Option.None<string>();
        internal Option<string> _condition = Option.None<string>();
        internal IList<OneOf<ActionDefinition, ContextActionDefinition>> _actions = new List<OneOf<ActionDefinition, ContextActionDefinition>>();

        private IEventDefinition Event => string.IsNullOrEmpty(_eventName) ? new ImmediateEventDefinition() as IEventDefinition : new NamedEvent(_eventName);
        private IEnumerable<Target> Targets => new [] { _target.Map(statenodeName => new UniquelyIdentifiedTarget(statenodeName) as Target).ValueOr(new SelfTarget()) };
        private IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ContextActions => _actions;

        internal void AddAction(LogAction logAction) =>
            _actions.Add(logAction.AsContextAction());
        internal void AddAction(AssignAction assignAction) =>
            _actions.Add(assignAction.AsContextAction());
        internal void AddAction(RaiseAction raiseAction) =>
            _actions.Add(raiseAction.AsActionDefinition());

        internal TransitionDefinition AsTransitionDefinition()
            => _condition.Match<TransitionDefinition>(
                conditionExpression => new GuardedTransition(
                    Event,
                    Targets,
                    new ConditionContextGuard(context => TypeConverter.ToBoolean(((ECMAScriptContext)context).Engine.Execute(conditionExpression).GetCompletionValue())),
                    ContextActions),
                () => new UnguardedTransition(Event, Targets, ContextActions));
    }

    internal class UnguardedTransition : UnguardedContextTransitionDefinition
    {
        public UnguardedTransition(IEventDefinition @event, IEnumerable<Target> targets, IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
        {
            Event = @event;
            Targets = targets;
            Actions = actions;
        }

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
    internal class GuardedTransition : GuardedContextTransitionDefinition
    {
        public GuardedTransition(IEventDefinition @event, IEnumerable<Target> targets, OneOfUnion<Guard, InStateGuard, ConditionContextGuard> guard, IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
            Actions = actions;
        }

        public override IEventDefinition Event { get; }
        public override OneOfUnion<Guard, InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> Actions { get; }
    }
}
