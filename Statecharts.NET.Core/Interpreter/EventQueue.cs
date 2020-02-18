using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    internal class EventQueue
    {
        private readonly Queue<IEvent> _externalEvents = new Queue<IEvent>();
        private readonly Queue<IEvent> _internalEvents = new Queue<IEvent>();

        public Option<IEvent> Dequeue()
            => _internalEvents.Count > 0
                ? _internalEvents.Dequeue().ToOption()
                : _externalEvents.Count > 0
                    ? _externalEvents.Dequeue().ToOption()
                    : Option.None<IEvent>();

        public bool IsEmpty => !_internalEvents.Any() && !_externalEvents.Any();

        public void EnqueueExternal(IEvent @event) => _externalEvents.Enqueue(@event);
        public void EnqueueInternal(NamedEvent @event) => _internalEvents.Enqueue(@event);
    }
}
