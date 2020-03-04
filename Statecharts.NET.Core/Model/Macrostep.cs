using System;
using System.Collections;
using System.Collections.Generic;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Model
{
    public class Macrostep<TContext> where TContext : IContext<TContext>
    {
        public State<TContext> State { get; }
        public IEnumerable<IEvent> RemainingEvents { get; }
        public IList<Microstep> Microsteps { get; }

        public Macrostep(State<TContext> state, IEnumerable<IEvent> remainingEvents, IList<Microstep> microsteps)
        {
            State = state;
            RemainingEvents = remainingEvents;
            Microsteps = microsteps;
        }
    }
}
