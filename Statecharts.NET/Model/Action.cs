using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
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
                send => new SendAction() as Action,
                raise => new RaiseAction(),
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

    public class SendAction : Action { }
    public class RaiseAction : Action { }
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