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
        public string EventName { get; }
        public SendActionDefinition(string eventName) => EventName = eventName;
    }

    public class RaiseActionDefinition : ActionDefinition
    {
        public string EventName { get; }
        public RaiseActionDefinition(string eventName) => EventName = eventName;
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
        public Action Mutation { get; }
        public AssignActionDefinition(Action mutation) => Mutation = mutation;
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
        public Action Function { get; }
        public SideEffectActionDefinition(Action function) => Function = function;
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

    // TODO: refactor this
    public static class ActionExtensionFunctions
    {
        internal static IEnumerable<Action> ToModelActions(this IEnumerable<Definition.Action> definitionActions)
            => definitionActions.Select(Action.From);
        internal static IEnumerable<Action> ToModelActions(this IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> definitionActions)
            => definitionActions.Select(action => action.Match(Action.From, Action.From));
        internal static IEnumerable<Action> ToModelActions(this IEnumerable<OneOf<Definition.Action, Definition.ContextAction, Definition.ContextDataAction>> definitionActions)
            => definitionActions.Select(action => action.Match(Action.From, Action.From, Action.From));
    }

    public abstract class Action : OneOfBase<SendAction, RaiseAction, LogAction, AssignAction, SideEffectAction>
    {
        public static Action From(Definition.Action action) =>
            action.Match(
                send => new SendAction(send.EventName) as Action,
                raise => new RaiseAction(raise.EventName),
                log => new LogAction((context, data) => log.Label),
                assign => new AssignAction((context, data) => assign.Mutation()),
                sideEffect => new SideEffectAction((context, data) => sideEffect.Function()));
        public static Action From(Definition.ContextAction action) =>
            action.Match(
                log => new LogAction((context, data) => log.Message(context)) as Action, 
                assign => new AssignAction((context, data) => assign.Mutation(context)), 
                sideEffect => new SideEffectAction((context, data) => sideEffect.Function(context)));
        public static Action From(Definition.ContextDataAction action) =>
            action.Match(
                log => new LogAction(log.Message) as Action,
                assign => new AssignAction(assign.Mutation),
                sideEffect => new SideEffectAction(sideEffect.Function));
    }

    public class SendAction : Action
    {
        public string EventName { get; }
        public SendAction(string eventName) => EventName = eventName;
    }
    public class RaiseAction : Action
    {
        public string EventName { get; }
        public RaiseAction(string eventName) => EventName = eventName;
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
}