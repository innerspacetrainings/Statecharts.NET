using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
using Jint.Runtime;
using Statecharts.NET.Definition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class Transition
    {
        internal string _event;
        internal string _target;
        internal Option<string> _condition;
        internal IList<OneOf<Action, ContextAction>> _actions = new List<OneOf<Action, ContextAction>>();

        private NamedEvent Event => new NamedEvent(_event);
        private IEnumerable<Target> Targets => new [] { new SiblingTarget(_target) };
        private IEnumerable<Action> Actions => _actions.Select(a => a.Match(action => action, contextAction => null)).Where(action => action != null);
        private IEnumerable<OneOf<Action, ContextAction>> ContextActions => _actions;

        internal void AddAction(LogAction logAction) =>
            _actions.Add(logAction.AsContextAction());
        internal void AddAction(AssignAction assignAction) =>
            _actions.Add(assignAction.AsContextAction());

        internal Statecharts.NET.Definition.Transition AsTransitionDefinition()
            => _condition.Match<Statecharts.NET.Definition.Transition>(
                conditionExpression => new GuardedTransition(Event, Targets, new ConditionContextGuard(context => TypeConverter.ToBoolean(((ECMAScriptContext)context).Engine.Execute(conditionExpression).GetCompletionValue())), ContextActions),
                () => new UnguardedTransition(Event, Targets, Actions));
    }

    internal class UnguardedTransition : Statecharts.NET.Definition.UnguardedTransition
    {
        public UnguardedTransition(Event @event, IEnumerable<Target> targets, IEnumerable<Action> actions)
        {
            Event = @event;
            Targets = targets;
            Actions = actions.ToOption();
        }

        public override Event Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override Option<IEnumerable<Action>> Actions { get; }
    }
    internal class GuardedTransition : Statecharts.NET.Definition.GuardedContextTransition
    {
        public GuardedTransition(Event @event, IEnumerable<Target> targets, OneOf<InStateGuard, ConditionContextGuard> guard, IEnumerable<OneOf<Action, ContextAction>> actions)
        {
            Event = @event;
            Guard = guard;
            Targets = targets;
            Actions = actions.ToOption();
        }

        public override Event Event { get; }
        public override OneOf<InStateGuard, ConditionContextGuard> Guard { get; }
        public override IEnumerable<Target> Targets { get; }
        public override Option<IEnumerable<OneOf<Action, ContextAction>>> Actions { get; }
    }
}
