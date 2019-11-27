using System;
using System.Collections.Generic;
using System.Text;

namespace Statecharts.NET.SCXML
{
    interface ITransition
    {
        string Event { get; }
        string Type { get; }
        IState Source { get; }
        IEnumerable<IState> Target { get; }
        IEnumerable<Action> Actions { get; }
        Func<bool> Condition { get; }
    }

    interface IState
    {
        string Id { get; }
        IState Parent { get; }
        ITransition InitialTransition { get; }
        IEnumerable<ITransition> Transitions { get; }
        IEnumerable<Action> ExitActions { get; }
        IEnumerable<Action> EntryActions { get; }
    }

    interface IStatechart
    {
        ITransition InitialTransition { get; }
    }
}
