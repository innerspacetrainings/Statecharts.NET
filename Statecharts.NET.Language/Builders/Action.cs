using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public abstract class Action : OneOfBase<SendAction, RaiseAction, LogAction, AssignAction, SideEffectAction>
    {
        internal Definition.Action ToDefinitionAction() =>
            Match(
                send => new Definition.SendAction(send.EventName) as Definition.Action, 
                raise => new Definition.RaiseAction(raise.EventName),
                log => new Definition.LogAction(log.Label),
                assign => new Definition.AssignAction(assign.Mutation),
                sideEffect => new Definition.SideEffectAction(sideEffect.Function));
    }
    public abstract class Action<TContext> : OneOfBase<LogAction<TContext>, AssignAction<TContext>, SideEffectAction<TContext>>
    {
        internal Definition.ContextAction ToDefinitionAction() =>
            this.Match(
                log => new Definition.LogContextAction(context => log.Message((TContext)context)) as Definition.ContextAction,
                assign => new Definition.AssignContextAction(context => assign.Mutation((TContext) context)), 
                sideEffect => new Definition.SideEffectContextAction(context => sideEffect.Function((TContext) context)));
    }
    public abstract class Action<TContext, TData> : OneOfBase<LogAction<TContext, TData>, AssignAction<TContext, TData>, SideEffectAction<TContext, TData>> { }

    public class SendAction : Action {
        public string EventName { get; }
        public SendAction(string eventName) => EventName = eventName;
    }

    public class RaiseAction : Action {
        public string EventName { get; }
        public RaiseAction(string eventName) => EventName = eventName;
    }

    public class LogAction : Action
    {
        public string Label { get; }
        public LogAction(string label) => Label = label;
    }
    public class LogAction<TContext> : Action<TContext>
    {
        public Func<TContext, string> Message { get; }
        public LogAction(Func<TContext, string> message) => Message = message;
    }
    public class LogAction<TContext, TData> : Action<TContext, TData>
    {
        public Func<TContext, TData, string> Message { get; }
        public LogAction(Func<TContext, TData, string> message) => Message = message;
    }

    public class AssignAction : Action
    {
        public System.Action Mutation { get; }
        public AssignAction(System.Action mutation) => Mutation = mutation;
    }
    public class AssignAction<TContext> : Action<TContext>
    {
        public System.Action<TContext> Mutation { get; }
        public AssignAction(System.Action<TContext> mutation) => Mutation = mutation;
    }
    public class AssignAction<TContext, TData> : Action<TContext, TData>
    {
        public System.Action<TContext, TData> Mutation { get; }
        public AssignAction(System.Action<TContext, TData> mutation) => Mutation = mutation;
    }

    public class SideEffectAction : Action
    {
        public System.Action Function { get; }
        public SideEffectAction(System.Action function) => Function = function;
    }
    public class SideEffectAction<TContext> : Action<TContext>
    {
        public System.Action<TContext> Function { get; }
        public SideEffectAction(System.Action<TContext> function) => Function = function;
    }
    public class SideEffectAction<TContext, TData> : Action<TContext, TData>
    {
        public System.Action<TContext, TData> Function { get; }
        public SideEffectAction(System.Action<TContext, TData> function) => Function = function;
    }
}
