using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Model;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class Transition
    {
        internal string _event;
        internal string _target;

        internal string Target => _target;

        internal Statecharts.NET.Definition.Transition AsTransitionDefinition()
            => new UnguardedTransition(new NamedEvent(_event), new [] { new SiblingTarget(_target) });
    }

    internal class UnguardedTransition : Statecharts.NET.Definition.UnguardedTransition
    {
        public UnguardedTransition(Event @event, IEnumerable<Target> targets, IEnumerable<Action> actions = null)
        {
            Event = @event;
            Targets = targets;
            Actions = actions ?? Enumerable.Empty<Action>();
        }

        public override Event Event { get; }
        public override IEnumerable<Target> Targets { get; }
        public override IEnumerable<Action> Actions { get; }
    }
}
