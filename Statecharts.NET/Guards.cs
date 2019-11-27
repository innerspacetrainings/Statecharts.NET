using System;

namespace Statecharts.NET
{
    public static class BaseGuardFunctions
    {
        public static TResult Map<TResult, TContext>(
            this BaseGuard guard,
            Func<InlineGuard<TContext>, TResult> fInline,
            Func<InStateGuard<TContext>, TResult> fInState)
            where TContext : IEquatable<TContext>
        {
            switch (guard)
            {
                case InlineGuard<TContext> inline:
                    return fInline(inline);
                case InStateGuard<TContext> inState:
                    return fInState(inState);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }


    public class BaseGuard { }

    public class InlineGuard<TContext> : BaseGuard
        where TContext : IEquatable<TContext>
    {
            public Func<TContext, BaseEvent, bool> Condition { get; set; }
    }

    public class InStateGuard<TContext> : BaseGuard
        where TContext : IEquatable<TContext>
    {
            public StateConfiguration State { get; set; }
    }
}
