using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Definition.Action;

namespace Statecharts.NET.Tests.Definition
{
    internal class AtomicStateNode : Statecharts.NET.Definition.AtomicStateNode
    {
        private string _name;

        public AtomicStateNode(
            string name = null,
            IEnumerable<OneOf<Action, ContextAction>> entryActions = null,
            IEnumerable<OneOf<Action, ContextAction>> exitActions = null,
            IEnumerable<Transition> transitions = null,
            IEnumerable<Service> services = null)
        {
            _name = name;
            EntryActions = entryActions ?? Enumerable.Empty<OneOf<Action, ContextAction>>();
            ExitActions = exitActions ?? Enumerable.Empty<OneOf<Action, ContextAction>>();
            Transitions = transitions ?? Enumerable.Empty<Transition>();
            Services = services ?? Enumerable.Empty<Service>();
        }

        public override string Name => _name;
        public override IEnumerable<OneOf<Action, ContextAction>> EntryActions { get; }
        public override IEnumerable<OneOf<Action, ContextAction>> ExitActions { get; }
        public override IEnumerable<Transition> Transitions { get; }
        public override IEnumerable<Service> Services { get; }

        internal void SetName(string name) => _name = name;
    }
}
