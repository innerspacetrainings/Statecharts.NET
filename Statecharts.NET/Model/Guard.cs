using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public abstract class Guard : OneOfBase<InStateGuard, ConditionContextGuard, ConditionContextDataGuard> { }

    public class InStateGuard : Guard
    {
        public StateConfiguration State { get; }
        public InStateGuard(StateConfiguration state) => State = state;
    }
    public class ConditionContextGuard : Guard
    {
        public Func<object, bool> Condition { get; }
        public ConditionContextGuard(Func<object, bool> condition) => Condition = condition;
    }
    public class ConditionContextDataGuard : Guard
    {
        public Func<object, object, bool> Condition { get; }
        public ConditionContextDataGuard(Func<object, object, bool> condition) => Condition = condition;
    }
}