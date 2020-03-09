using System.Collections.Generic;
using System.Linq;
using Jint.Runtime;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class Transition
    {
        internal string _event;
        internal string _target;
        internal Option<string> _condition;
        internal IList<OneOf<ActionDefinition, ContextActionDefinition>> _actions = new List<OneOf<ActionDefinition, ContextActionDefinition>>();

        private IEventDefinition Event => new NamedEventDefinition(_event);
        private IEnumerable<Target> Targets => new [] { new SiblingTarget(_target) };
        private IEnumerable<ActionDefinition> Actions => _actions.Select(a => a.Match(action => action, contextAction => null)).Where(action => action != null);
        private IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> ContextActions => _actions;

        internal void AddAction(LogAction logAction) =>
            _actions.Add(logAction.AsContextAction());
        internal void AddAction(AssignAction assignAction) =>
            _actions.Add(assignAction.AsContextAction());

        internal TransitionDefinition AsTransitionDefinition()
            => _condition.Match<TransitionDefinition>(
                conditionExpression => new GuardedTransition(
                    Event,
                    Targets,
                    new ConditionContextGuard(context => TypeConverter.ToBoolean(((ECMAScriptContext)context).Engine.Execute(conditionExpression).GetCompletionValue())),
                    ContextActions),
                () => new UnguardedTransition(Event, Targets, Actions));
    }

    internal class UnguardedTransition : UnguardedTransitionDefinition
    {
        public UnguardedTransition(IEventDefinition @event, IEnumerable<Target> targets, IEnumerable<ActionDefinition> actions)
        {
            Event = @event;
            Targets = targets;
            Actions = actions;
        }

        public override IEventDefinition Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<ActionDefinition> Actions { get; }
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
