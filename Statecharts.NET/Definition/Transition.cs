using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class InitialTransition
    {
        public ChildTarget Target { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }

    public class Transition :
        OneOfBase<
            ForbiddenTransition,
            UnguardedTransition,
            GuardedTransition>
    { }
    public class Transition<> :
        OneOfBase<
            ForbiddenTransition,
            UnguardedTransition,
            UnguardedTransition<TContext>,
            GuardedTransition,
            GuardedTransition<TContext>> where TContext : IEquatable<TContext>
    { }
    public class Transition<> :
        OneOfBase<
            ForbiddenTransition,
            UnguardedTransition,
            UnguardedTransition<TContext>,
            UnguardedTransition<TContext, TData>,
            GuardedTransition,
            GuardedTransition<TContext>,
            GuardedTransition<TContext, TData>> where TContext : IEquatable<TContext>
    { }

    public class ForbiddenTransition : Transition
    {
        public CustomEvent Event { get; }
        public ForbiddenTransition(string eventName) => Event = new CustomEvent(eventName);
    }
    public class UnguardedTransition : Transition
    {
        public Event Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
    public class UnguardedTransition<TContext> : Transition<TContext> where TContext : IEquatable<TContext>
    {
        public Event Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action<TContext>> Actions { get; set; }
    }
    public class UnguardedTransition<TContext, TData> : Transition<TContext, TData> where TContext : IEquatable<TContext>
    {
        public Event<TData> Event { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action<TContext, TData>> Actions { get; set; }
    }
    public class GuardedTransition : Transition
    {
        public Event Event { get; set; }
        public Guard Guard { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
    public class GuardedTransition<TContext> : Transition<TContext> where TContext : IEquatable<TContext>
    {
        public Event Event { get; set; }
        public Guard<TContext> Guard { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action<TContext>> Actions { get; set; }
    }
    public class GuardedTransition<TContext, TData> : Transition<TContext, TData> where TContext : IEquatable<TContext>
    {
        public Event<TData> Event { get; set; }
        public Guard<TContext, TData> Guard { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<Action<TContext, TData>> Actions { get; set; }
    }
}
