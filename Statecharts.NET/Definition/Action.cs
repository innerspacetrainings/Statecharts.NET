﻿using System;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public interface IPureAction { }
    public interface IMutatingAction { }

    public abstract class Action : OneOfBase<SendAction, RaiseAction, LogAction> { }
    public abstract class ContextAction : OneOfBase<LogContextAction, AssignContextAction, SideEffectContextAction> { }
    public abstract class ContextDataAction : OneOfBase<LogContextDataAction, AssignContextDataAction, SideEffectContextAction> { }

    public class SendAction : IPureAction { }

    public class RaiseAction : IPureAction { }

    public class LogAction : IPureAction
    {
        public string Label { get; }
        public LogAction(string label) => Label = label;
    }
    public class LogContextAction : IPureAction
    {
        public Func<object, string> Message { get; }
        public LogContextAction(Func<object, string> message) => Message = message;
    }
    public class LogContextDataAction : IPureAction
    {
        public Func<object, object, string> Message { get; }
        public LogContextDataAction(Func<object, object, string> message) => Message = message;
    }

    public class AssignContextAction : IPureAction
    {
        public Action<object> Mutation { get; }
        public AssignContextAction(Action<object> mutation) => Mutation = mutation;
    }
    public class AssignContextDataAction : IPureAction
    {
        public Action<object, object> Mutation { get; }
        public AssignContextDataAction(Action<object, object> mutation) => Mutation = mutation;
    }

    public class SideEffectContextAction : IMutatingAction
    {
        public Action<object> Function { get; }
        public SideEffectContextAction(Action<object> function) => Function = function;
    }
    public class SideEffectContextDataAction : IMutatingAction
    {
        public Action<object, object> Function { get; }
        public SideEffectContextDataAction(Action<object, object> function) => Function = function;
    }
}