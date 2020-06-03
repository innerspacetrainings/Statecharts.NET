using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class Microstep
    {
        public IEvent Event { get; }
        public Transition Transition { get; }
        public IEnumerable<ParsedStatenode> EnteredStatenodes { get; }
        public IEnumerable<ParsedStatenode> ExitedStatenodes { get; }

        public Microstep(
            IEvent @event,
            Transition transition,
            IEnumerable<ParsedStatenode> enteredStatenodes,
            IEnumerable<ParsedStatenode> exitedStatenodes)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Transition = transition;
            EnteredStatenodes = enteredStatenodes ?? throw new ArgumentNullException(nameof(enteredStatenodes));
            ExitedStatenodes = exitedStatenodes ?? throw new ArgumentNullException(nameof(exitedStatenodes));
        }

        public IEnumerable<Actionblock> EnteredActionBlocks =>
            EnteredStatenodes.Select(stateNode => stateNode.EntryActions);
        public IEnumerable<Actionblock> ExitedActionBlocks =>
            ExitedStatenodes.Select(stateNode => stateNode.ExitActions);
        public Actionblock TransitionActionBlock =>
            Transition?.Actions ?? Actionblock.Empty();

        public static Microstep InitializeStatechart(ParsedStatenode statechartRootnode) =>
            new Microstep(new InitializeStatechartEvent(), null, statechartRootnode.Yield(), Enumerable.Empty<ParsedStatenode>());

        public override string ToString() => $"{Transition}, (entered: {EnteredStatenodes.Count()}[{EnteredActionBlocks.Count()}], exited: {ExitedStatenodes.Count()}[{ExitedActionBlocks.Count()}])";
    }
}
