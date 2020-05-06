﻿using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class Microstep
    {
        public IEvent Event { get; }
        public Transition Transition { get; }
        public IEnumerable<Statenode> EnteredStatenodes { get; }
        public IEnumerable<Statenode> ExitedStatenodes { get; }

        public Microstep(
            IEvent @event,
            Transition transition,
            IEnumerable<Statenode> enteredStatenodes,
            IEnumerable<Statenode> exitedStatenodes)
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

        public static Microstep InitializeStatechart(Statenode statechartRootnode) =>
            new Microstep(new InitializeStatechartEvent(), null, statechartRootnode.Yield(), Enumerable.Empty<Statenode>());

        public override string ToString() => $"{Transition}, (entered: {EnteredStatenodes.Count()}[{EnteredActionBlocks.Count()}], exited: {ExitedStatenodes.Count()}[{ExitedActionBlocks.Count()}])";
    }
}
