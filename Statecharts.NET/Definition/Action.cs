using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public interface IPureAction { }
    public interface IMutatingAction { }

    public abstract class Action : OneOfBase<
        SendAction,
        RaiseAction,
        LogAction>
    { }
    public abstract class Action<> : OneOfBase<
        Action,
        AssignAction<TContext>,
        SideEffectAction<TContext>>
    { }
    public abstract class Action<> : OneOfBase<
        Action<TContext>,
        AssignAction<TContext, TData>,
        SideEffectAction<TContext, TData>>
    { }

    public class SendAction : IPureAction { }

    public class RaiseAction : IPureAction { }

    public class LogAction : IPureAction
    {
        public string Label { get; }
        public LogAction(string label) => Label = label;
    }
    public class LogAction<TContext> : IPureAction
    {
        public Func<TContext, string> Message { get; }
        public LogAction(Func<TContext, string> message) => Message = message;
    }
    public class LogAction<TContext, TData> : IPureAction
    {
        public Func<TContext, TData, string> Message { get; }
        public LogAction(Func<TContext, TData, string> message) => Message = message;
    }

    public class AssignAction<TContext> : IPureAction
    {
        public Action<TContext> Mutation { get; }
        public AssignAction(Action<TContext> mutation) => Mutation = mutation;
    }
    public class AssignAction<TContext, TData> : IPureAction
    {
        public Action<TContext, TData> Mutation { get; }
        public AssignAction(Action<TContext, TData> mutation) => Mutation = mutation;
    }

    public class SideEffectAction<TContext> : IMutatingAction
    {
        public Action<TContext> Function { get; }
        public SideEffectAction(Action<TContext> function) => Function = function;
    }
    public class SideEffectAction<TContext, TData> : IMutatingAction
    {
        public Action<TContext, TData> Function { get; }
        public SideEffectAction(Action<TContext, TData> function) => Function = function;
    }
}