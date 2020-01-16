using System;

namespace Statecharts.NET
{
    public static class BaseGuardFunctions
    {
        public static TResult Map<TResult, TContext, TData>(
            this BaseGuard baseGuard,
            Func<Guard<TContext>, TResult> fGuard,
            Func<GuardWithData<TContext, TData>, TResult> fGuardWithData,
            Func<InStateGuard<TContext>, TResult> fInState)
            where TContext : IEquatable<TContext>
        {
            switch (baseGuard)
            {
                case Guard<TContext> guard:
                    return fGuard(guard);
                case GuardWithData<TContext, TData> guardWithData:
                    return fGuardWithData(guardWithData);
                case InStateGuard<TContext> inState:
                    return fInState(inState);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public interface IGuard { }
    public interface IDataGuard { }

    public class BaseGuard { }

    public class Guard<TContext> : BaseGuard, IGuard
        where TContext : IEquatable<TContext>
    {
        public Func<TContext, bool> Condition { get; set; }
    }

    public class DataGuard<TContext, TData> : BaseGuard, 
        where TContext : IEquatable<TContext>
    {
            public Func<TContext, TData, bool> Condition { get; set; }
    }

    public class InStateGuard<TContext> : BaseGuard
        where TContext : IEquatable<TContext>
    {
            public StateConfiguration State { get; set; }
    }
}
