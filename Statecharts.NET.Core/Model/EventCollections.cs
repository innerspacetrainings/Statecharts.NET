﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    internal interface IQueuedEvent
    {
        IEvent Event { get; }
        int Priority { get; } // lower values have a higher priority
    }
    internal class Stabilization : IQueuedEvent
    {
        public IEvent Event { get; }
        public int Priority => 1;
        public Stabilization(IEvent @event) => Event = @event;
    }
    internal class CurrentStep : IQueuedEvent
    {
        public IEvent Event { get; }
        public int Priority => 2;
        public CurrentStep(IEvent @event) => Event = @event;
    }
    internal class NextStep : IQueuedEvent
    {
        public IEvent Event { get; }
        public int Priority => 3;
        public NextStep(IEvent @event) => Event = @event;
    }

    internal class EventQueue
    {
        private readonly PriorityQueue<OneOfUnion<IQueuedEvent, Stabilization, CurrentStep, NextStep>, int> _queue
            = new PriorityQueue<OneOfUnion<IQueuedEvent, Stabilization, CurrentStep, NextStep>, int>();

        private EventQueue() { }

        public static EventQueue WithEvent(IEvent initialEvent)
        {
            var eventQueue = new EventQueue();
            eventQueue.Enqueue(new CurrentStep(initialEvent));
            return eventQueue;
        }
        public IEvent Dequeue() => _queue.Dequeue().AsBase().Event;
        public void EnqueueStabilizationEvent(StatenodeId statenodeId)
        {
            var @event = new Stabilization(new InitializeEvent(statenodeId));
            _queue.Enqueue(@event, @event.Priority);
        }
        public void EnqueueImmediateEvent()
        {
            var @event = new Stabilization(new ImmediateEvent());
            if(!_queue.Any(queuedEvent => queuedEvent.AsBase().Event is ImmediateEvent)) _queue.Enqueue(@event, @event.Priority);
        }
        public void Enqueue(CurrentStep @event) => _queue.Enqueue(@event, @event.Priority);
        public void Enqueue(NextStep @event) => _queue.Enqueue(@event, @event.Priority);
        public void EnqueueDoneEvent(StatenodeId statenodeId)
        {
            var @event = new DoneEvent(statenodeId);
            if(!_queue.Any(queuedEvent => queuedEvent.Match(_ => false, current => current.Event.Equals(@event), _ => false)))
                Enqueue(new CurrentStep(@event));
        }

        public bool IsNotEmpty => _queue.Any();

        public bool NextIsInternal =>
            _queue.Any() && _queue.First().Match(stabilization => true, currentStep => true, nextStep => false);

        public IEnumerable<IEvent> NextStepEvents => // TODO: don't fully like that
            _queue
                .Select(queuedEvent => queuedEvent.Match(_ => null, _ => null, nextStep => nextStep.Event))
                .WhereNotNull();

        public override string ToString() => string.Join(", ",
            _queue.Select(e => e.Match(
                stabilization => $"stabilization: {stabilization.Event}",
                current => $"current: {current.Event}",
                next => $"next: {next.Event}")));
    }
    internal class EventList : IEnumerable<OneOf<CurrentStep, NextStep>>
    {
        private readonly List<OneOf<CurrentStep, NextStep>> _events;

        private EventList(IEnumerable<OneOf<CurrentStep, NextStep>> events = null) =>
            _events = (events ?? Enumerable.Empty<OneOf<CurrentStep, NextStep>>()).ToList();

        public static EventList Empty() => new EventList();
        public static EventList From(IEnumerable<OneOf<CurrentStep, NextStep>> events) => new EventList(events);

        internal void Enqueue(OneOf<CurrentStep, NextStep> queuedEvent) => _events.Add(queuedEvent);
        public void EnqueueOnCurrentStep(IEvent @event) => Enqueue(new CurrentStep(@event));
        public void EnqueueOnNextStep(IEvent @event) => Enqueue(new NextStep(@event));
        public void AddRange(IEnumerable<OneOf<CurrentStep, NextStep>> events) => _events.AddRange(events);

        public IEnumerator<OneOf<CurrentStep, NextStep>> GetEnumerator() => _events.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
