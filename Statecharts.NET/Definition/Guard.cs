using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET
{
    public abstract class Guard : OneOfBase<InStateGuard> { }
    public abstract class Guard<> : OneOfBase<ConditionGuard<TContext>, InStateGuard>
        where TContext : IEquatable<TContext> { }

    public abstract class Guard<> : OneOfBase<ConditionDataGuard<TContext, TData>, InStateGuard>
        where TContext : IEquatable<TContext> { }

    public class ConditionGuard<TContext> : Guard<TContext>
        where TContext : IEquatable<TContext>
    {
        public Func<TContext, bool> Condition { get; }
        public ConditionGuard(Func<TContext, bool> condition) => Condition = condition;
    }

    public class ConditionDataGuard<TContext, TData> : Guard<TContext, TData>
        where TContext : IEquatable<TContext>
    {
        public Func<TContext, TData, bool> Condition { get; }
        public ConditionDataGuard(Func<TContext, TData, bool> condition) => Condition = condition;
    }

    public class InStateGuard : Guard
    {
        public StateConfiguration State { get; }
        public InStateGuard(StateConfiguration state) => State = state;
    }
}
