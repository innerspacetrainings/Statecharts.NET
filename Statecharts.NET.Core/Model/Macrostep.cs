using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Model
{
    public class Macrostep<TContext> where TContext : IContext<TContext>
    {
        public State<TContext> State { get; }
        public IEvent Event { get; }
        public IEnumerable<IEvent> QueuedEvents { get; }
        public IList<(IEvent @event, IEnumerable<Microstep> causedMicrosteps)> OccuredEvents { get; }

        public Macrostep(
            State<TContext> state,
            IEvent @event,
            IEnumerable<IEvent> queuedEvents,
            IList<(IEvent @event, IEnumerable<Microstep> causedMicrosteps)> occuredEvents)
        {
            State = state;
            Event = @event;
            QueuedEvents = queuedEvents;
            OccuredEvents = occuredEvents;
        }

        public IEnumerable<Microstep> Microsteps => OccuredEvents.SelectMany(entry => entry.causedMicrosteps);
    }
}
