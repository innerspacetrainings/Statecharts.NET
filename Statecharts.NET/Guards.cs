using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET
{
    public abstract class Guard { }
    public abstract class Guard<TContext> : OneOfBase<ConditionGuard<TContext>, InStateGuard<TContext>>
        where TContext : IEquatable<TContext> { }

    public abstract class Guard<TContext, TData> : OneOfBase<ConditionDataGuard<TContext, TData>, InStateGuard<TContext>>
        where TContext : IEquatable<TContext> { }

    public class ConditionGuard<TContext>
        where TContext : IEquatable<TContext>
    {
        public Func<TContext, bool> Condition { get; }
        public ConditionGuard(Func<TContext, bool> condition) => Condition = condition;
    }

    public class ConditionDataGuard<TContext, TData>
        where TContext : IEquatable<TContext>
    {
        public Func<TContext, TData, bool> Condition { get; }
        public ConditionDataGuard(Func<TContext, TData, bool> condition) => Condition = condition;
    }

    public class InStateGuard<TContext>
        where TContext : IEquatable<TContext>
    {
        public StateConfiguration State { get; }
        public InStateGuard(StateConfiguration state) => State = state;
    }
}
