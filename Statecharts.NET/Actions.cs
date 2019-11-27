using System;
using System.Linq.Expressions;

namespace Statecharts.NET
{
    public interface PureAction { }
    public interface MutatingAction { }

    public abstract class BaseAction { }
    public abstract class Action : BaseAction { }
    public abstract class EventAction : BaseAction { }

    class SendAction<TContext> : Action, PureAction { }
    class RaiseAction<TContext> : Action, PureAction { }
    class LogAction<TContext> : Action, PureAction
    {
        public string Label { get; set; }
    }
    class LogEventAction<TContext> : EventAction, PureAction
    {
        public string Label { get; set; }
    }
    public class AssignAction<TContext, TResult> : Action, PureAction
    {
        public Action<TContext> Mutation { get; set; }
    }
    public class AssignEventAction<TContext, TResult> : EventAction, PureAction
    {
        public Action<TContext, BaseEvent> Mutation { get; set; }
    }
    public class SideEffectAction<TContext> : Action, MutatingAction
    {
        public Action<TContext> Function { get; set; }
    }
    public class SideEffectEventAction<TContext> : EventAction, MutatingAction
    {
        public Action<TContext, BaseEvent> Function { get; set; }
    }
}