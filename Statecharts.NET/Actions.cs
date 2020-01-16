using System;
using System.Linq.Expressions;

namespace Statecharts.NET
{
    public interface PureAction { }
    public interface MutatingAction { }

    public abstract class BaseAction { }
    public abstract class Action : BaseAction { }
    public abstract class ActionWithData : BaseAction { }

    class SendAction<TContext> : Action, PureAction { }
    class RaiseAction<TContext> : Action, PureAction { }
    class LogAction<TContext> : Action, PureAction
    {
        public string Label { get; set; }
    }
    class LogEventAction<TContext> : ActionWithData, PureAction
    {
        public string Label { get; set; }
    }
    public class AssignAction<TContext, TResult> : Action, PureAction
    {
        public Action<TContext> Mutation { get; set; }
    }
    public class AssignActionWithData<TContext, TData> : ActionWithData, PureAction
    {
        public Action<TContext, TData> Mutation { get; set; }
    }
    public class SideEffectAction<TContext> : Action, MutatingAction
    {
        public Action<TContext> Function { get; set; }
    }
    public class SideEffectActionWithData<TContext, TData> : ActionWithData, MutatingAction
    {
        public Action<TContext, TData> Function { get; set; }
    }
}