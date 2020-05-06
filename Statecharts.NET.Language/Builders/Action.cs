using System;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public abstract class ActionDefinition : OneOfBase<SendActionDefinition, RaiseActionDefinition, LogActionDefinition, AssignActionDefinition, SideEffectActionDefinition>
    {
        internal Model.ActionDefinition ToDefinitionAction() =>
            Match(
                send => new Model.SendActionDefinition(send.Event) as Model.ActionDefinition, 
                raise => new Model.RaiseActionDefinition(raise.Event),
                log => new Model.LogActionDefinition(log.Label),
                assign => new Model.AssignActionDefinition(assign.Mutation),
                sideEffect => new Model.SideEffectActionDefinition(sideEffect.Function));
    }
    public abstract class ActionDefinition<TContext> : OneOfBase<LogActionDefinition<TContext>, AssignActionDefinition<TContext>, SideEffectActionDefinition<TContext>>
    {
        internal ContextActionDefinition ToDefinitionAction() =>
            Match(
                log => new LogContextActionDefinition(context => log.Message((TContext)context)) as ContextActionDefinition,
                assign => new AssignContextActionDefinition(context => assign.Mutation((TContext) context)), 
                sideEffect => new SideEffectContextActionDefinition(context => sideEffect.Function((TContext) context)));
    }
    public abstract class ActionDefinition<TContext, TEventData> : OneOfBase<LogActionDefinition<TContext, TEventData>, AssignActionDefinition<TContext, TEventData>,
        SideEffectActionDefinition<TContext, TEventData>>
    {
        internal ContextDataActionDefinition ToDefinitionAction() =>
            Match(
                log => new LogContextDataActionDefinition((context, eventData) => log.Message((TContext)context, (TEventData)eventData)) as ContextDataActionDefinition,
                assign => new AssignContextDataActionDefinition((context, eventData) => assign.Mutation((TContext)context, (TEventData)eventData)),
                sideEffect => new SideEffectContextDataActionDefinition((context, eventData) => sideEffect.Function((TContext)context, (TEventData) eventData)));
    }

    public class SendActionDefinition : ActionDefinition {
        public ISendableEvent Event { get; }
        public SendActionDefinition(ISendableEvent @event) => Event = @event;
    }

    public class RaiseActionDefinition : ActionDefinition {
        public ISendableEvent Event { get; }
        public RaiseActionDefinition(ISendableEvent @event) => Event = @event;
    }

    public class LogActionDefinition : ActionDefinition
    {
        public string Label { get; }
        public LogActionDefinition(string label) => Label = label;
    }
    public class LogActionDefinition<TContext> : ActionDefinition<TContext>
    {
        public Func<TContext, string> Message { get; }
        public LogActionDefinition(Func<TContext, string> message) => Message = message;
    }
    public class LogActionDefinition<TContext, TData> : ActionDefinition<TContext, TData>
    {
        public Func<TContext, TData, string> Message { get; }
        public LogActionDefinition(Func<TContext, TData, string> message) => Message = message;
    }

    public class AssignActionDefinition : ActionDefinition
    {
        public Action Mutation { get; }
        public AssignActionDefinition(Action mutation) => Mutation = mutation;
    }
    public class AssignActionDefinition<TContext> : ActionDefinition<TContext>
    {
        public Action<TContext> Mutation { get; }
        public AssignActionDefinition(Action<TContext> mutation) => Mutation = mutation;
    }
    public class AssignActionDefinition<TContext, TData> : ActionDefinition<TContext, TData>
    {
        public Action<TContext, TData> Mutation { get; }
        public AssignActionDefinition(Action<TContext, TData> mutation) => Mutation = mutation;
    }

    public class SideEffectActionDefinition : ActionDefinition
    {
        public Action Function { get; }
        public SideEffectActionDefinition(Action function) => Function = function;
    }
    public class SideEffectActionDefinition<TContext> : ActionDefinition<TContext>
    {
        public Action<TContext> Function { get; }
        public SideEffectActionDefinition(Action<TContext> function) => Function = function;
    }
    public class SideEffectActionDefinition<TContext, TData> : ActionDefinition<TContext, TData>
    {
        public Action<TContext, TData> Function { get; }
        public SideEffectActionDefinition(Action<TContext, TData> function) => Function = function;
    }
}
