using System;
using System.Collections.Generic;

namespace Statecharts.NET.Definition
{
    public class InitialTransitionDefinition
    {
        public ChildTargetDefinition Target { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }

    public class ForbiddenTransitionDefinition
    {
        public CustomEvent Event { get; set; }
    }

    public class UnguardedTransitionDefinition
    {
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
    public class UnguardedTransitionDefinition<TContext>
    {
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action<TContext>> Actions { get; set; }
    }
    public class UnguardedTransitionDefinition<TContext, TData>
    {
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action<TContext, TData>> Actions { get; set; }
    }
    public class GuardedTransitionDefinition<TContext>
        where TContext : IEquatable<TContext>
    {
        public Guard<TContext> Guard { get; set; }
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action<TContext>> Actions { get; set; }
    }
    public class GuardedTransitionDefinition<TContext, TData>
        where TContext : IEquatable<TContext>
    {
        public Guard<TContext, TData> Guard { get; set; }
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action<TContext, TData>> Actions { get; set; }
    }
}
