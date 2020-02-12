using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Model;

namespace Statecharts.NET.Interpreter
{
    internal class EventQueue
    {
        private Queue<Model.Event> internalEvents = new Queue<Model.Event>(); // TODO: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
        private Queue<Model.Event> externalEvents = new Queue<Model.Event>();

        public Model.Event Dequeue()
            => internalEvents.Count > 0
                ? internalEvents.Dequeue()
                : externalEvents.Count > 0
                    ? externalEvents.Dequeue()
                    : null;

        public bool IsEmpty => !internalEvents.Any() && !externalEvents.Any();

        public void EnqueueExternal(NamedEvent @event) => externalEvents.Enqueue(@event);
        public void EnqueueInternal(NamedEvent @event) => internalEvents.Enqueue(@event);
    }
}
