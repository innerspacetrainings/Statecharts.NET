using System;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public abstract class Action : OneOfBase<SendAction, RaiseAction, LogAction, AssignAction, SideEffectAction>
    {
        internal ActionDefinition ToDefinitionAction() =>
            Match(
                send => new SendActionDefinition(send.EventName) as ActionDefinition, 
                raise => new RaiseActionDefinition(raise.EventName),
                log => new LogActionDefinition(log.Label),
                assign => new AssignActionDefinition(assign.Mutation),
                sideEffect => new SideEffectActionDefinition(sideEffect.Function));
    }
    public abstract class Action<TContext> : OneOfBase<LogAction<TContext>, AssignAction<TContext>, SideEffectAction<TContext>>
    {
        internal ContextActionDefinition ToDefinitionAction() =>
            Match(
                log => new LogContextActionDefinition(context => log.Message((TContext)context)) as ContextActionDefinition,
                assign => new AssignContextActionDefinition(context => assign.Mutation((TContext) context)), 
                sideEffect => new SideEffectContextActionDefinition(context => sideEffect.Function((TContext) context)));
    }
    public abstract class Action<TContext, TEventData> : OneOfBase<LogAction<TContext, TEventData>, AssignAction<TContext, TEventData>,
        SideEffectAction<TContext, TEventData>>
    {
        internal ContextDataActionDefinition ToDefinitionAction() =>
            Match(
                log => new LogContextDataActionDefinition((context, eventData) => log.Message((TContext)context, (TEventData)eventData)) as ContextDataActionDefinition,
                assign => new AssignContextDataActionDefinition((context, eventData) => assign.Mutation((TContext)context, (TEventData)eventData)),
                sideEffect => new SideEffectContextDataActionDefinition((context, eventData) => sideEffect.Function((TContext)context, (TEventData) eventData)));
    }

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
