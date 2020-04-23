using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public abstract class ActionDefinition : OneOfBase<SendActionDefinition, RaiseActionDefinition, LogActionDefinition, AssignActionDefinition, SideEffectActionDefinition> { }
    public abstract class ContextActionDefinition : OneOfBase<LogContextActionDefinition, AssignContextActionDefinition, SideEffectContextActionDefinition> { }
    public abstract class ContextDataActionDefinition : OneOfBase<LogContextDataActionDefinition, AssignContextDataActionDefinition, SideEffectContextDataActionDefinition> { }

    public class SendActionDefinition : ActionDefinition
    {
        public ISendableEvent Event { get; }
        public SendActionDefinition(ISendableEvent @event) => Event = @event;
    }

    public class RaiseActionDefinition : ActionDefinition
    {
        public ISendableEvent Event { get; }
        public RaiseActionDefinition(ISendableEvent @event) => Event = @event;
    }

    public class LogActionDefinition : ActionDefinition
    {
        public string Label { get; }
        public LogActionDefinition(string label) => Label = label;
    }
    public class LogContextActionDefinition : ContextActionDefinition
    {
        public Func<object, string> Message { get; }
        public LogContextActionDefinition(Func<object, string> message) => Message = message;
    }
    public class LogContextDataActionDefinition : ContextDataActionDefinition
    {
        public Func<object, object, string> Message { get; }
        public LogContextDataActionDefinition(Func<object, object, string> message) => Message = message;
    }

    public class AssignActionDefinition : ActionDefinition
    {
        public System.Action Mutation { get; }
        public AssignActionDefinition(System.Action mutation) => Mutation = mutation;
    }
    public class AssignContextActionDefinition : ContextActionDefinition
    {
        public Action<object> Mutation { get; }
        public AssignContextActionDefinition(Action<object> mutation) => Mutation = mutation;
    }
    public class AssignContextDataActionDefinition : ContextDataActionDefinition
    {
        public Action<object, object> Mutation { get; }
        public AssignContextDataActionDefinition(Action<object, object> mutation) => Mutation = mutation;
    }

    public class SideEffectActionDefinition : ActionDefinition
    {
        public System.Action Function { get; }
        public SideEffectActionDefinition(System.Action function) => Function = function;
    }
    public class SideEffectContextActionDefinition : ContextActionDefinition
    {
        public Action<object> Function { get; }
        public SideEffectContextActionDefinition(Action<object> function) => Function = function;
    }
    public class SideEffectContextDataActionDefinition : ContextDataActionDefinition
    {
        public Action<object, object> Function { get; }
        public SideEffectContextDataActionDefinition(Action<object, object> function) => Function = function;
    }
    #endregion
    #region Executable
    public class SendAction : Action
    {
        public ISendableEvent Event { get; }
        public SendAction(ISendableEvent @event) => Event = @event;
    }
    public class RaiseAction : Action
    {
        public ISendableEvent Event { get; }
        public RaiseAction(ISendableEvent @event) => Event = @event;
    }
    public class LogAction : Action
    {
        public Func<object, object, string> Message { get; }
        public LogAction(Func<object, object, string> message) => Message = message;
    }
    public class AssignAction : Action
    {
        public Action<object, object> Mutation { get; }
        public AssignAction(Action<object, object> mutation) => Mutation = mutation;
    }
    public class SideEffectAction : Action
    {
        public Action<object, object> Function { get; }
        public SideEffectAction(Action<object, object> function) => Function = function;
    }
    public class StartDelayedTransitionAction : Action
    {
        public StatenodeId StatenodeId { get; }
        public TimeSpan Delay { get; }
        internal StartDelayedTransitionAction(StatenodeId statenodeId, TimeSpan delay)
        {
            StatenodeId = statenodeId;
            Delay = delay;
        }
    }
    #endregion

    // TODO: refactor this
    public static class ActionExtensionFunctions
    {
        internal static IEnumerable<Action> ToModelActions(this IEnumerable<ActionDefinition> definitionActions)
            => definitionActions.Select(Action.From);
        internal static IEnumerable<Action> ToModelActions(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> definitionActions)
            => definitionActions.Select(action => action.Match(Action.From, Action.From));
        internal static IEnumerable<Action> ToModelActions(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition>> definitionActions)
            => definitionActions.Select(action => action.Match(Action.From, Action.From, Action.From));
    }

    public abstract class Action : OneOfBase<SendAction, RaiseAction, LogAction, AssignAction, SideEffectAction, StartDelayedTransitionAction>
    {
        public static Action From(ActionDefinition actionDefinition) =>
            actionDefinition.Match(
                send => new SendAction(send.Event) as Action,
                raise => new RaiseAction(raise.Event),
                log => new LogAction((context, data) => log.Label),
                assign => new AssignAction((context, data) => assign.Mutation()),
                sideEffect => new SideEffectAction((context, data) => sideEffect.Function()));
        public static Action From(ContextActionDefinition actionDefinition) =>
            actionDefinition.Match(
                log => new LogAction((context, data) => log.Message(context)) as Action, 
                assign => new AssignAction((context, data) => assign.Mutation(context)), 
                sideEffect => new SideEffectAction((context, data) => sideEffect.Function(context)));
        public static Action From(ContextDataActionDefinition actionDefinition) =>
            actionDefinition.Match(
                log => new LogAction(log.Message) as Action,
                assign => new AssignAction(assign.Mutation),
                sideEffect => new SideEffectAction(sideEffect.Function));

        public static Action From(OneOf<ActionDefinition, ContextActionDefinition> actionDefinition) =>
            actionDefinition.Match(From, From);
        public static Action From(OneOf<ActionDefinition, ContextActionDefinition, ContextDataActionDefinition> actionDefinition) =>
            actionDefinition.Match(From, From, From);
    }
}