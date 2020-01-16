using System;
using System.Collections.Generic;

namespace Statecharts.NET.Definition
{
    public static class BaseTransitionDefinitionFunctions
    {
        public static TResult Map<TResult>(
            this BaseTransitionDefinition transitionDefinition,
            Func<UnguardedImmediateTransitionDefinition, TResult> fUnguardedImmediate,
            Func<GuardedImmediateTransitionDefinition, TResult> fGuardedImmediate,
            Func<UnguardedEventTransitionDefinition, TResult> fUnguardedEvent,
            Func<GuardedEventTransitionDefinition, TResult> fGuardedEvent)
        {
            switch (transitionDefinition)
            {
                case UnguardedImmediateTransitionDefinition unguardedImmediate:
                    return fUnguardedImmediate(unguardedImmediate);
                case GuardedImmediateTransitionDefinition guardedImmediate:
                    return fGuardedImmediate(guardedImmediate);
                case UnguardedEventTransitionDefinition unguardedEvent:
                    return fUnguardedEvent(unguardedEvent);
                case GuardedEventTransitionDefinition guardedEvent:
                    return fGuardedEvent(guardedEvent);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        public static TResult Map<TResult>(
            this ImmediateTransitionDefinition immediateTransitionDefinition,
            Func<UnguardedImmediateTransitionDefinition, TResult> fUnguardedImmediate,
            Func<GuardedImmediateTransitionDefinition, TResult> fGuardedImmediate)
        {
            switch (immediateTransitionDefinition)
            {
                case UnguardedImmediateTransitionDefinition unguardedImmediate:
                    return fUnguardedImmediate(unguardedImmediate);
                case GuardedImmediateTransitionDefinition guardedImmediate:
                    return fGuardedImmediate(guardedImmediate);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        public static TResult Map<TResult>(
            this EventTransitionDefinition eventTransitionDefinition,
            Func<UnguardedEventTransitionDefinition, TResult> fUnguardedEvent,
            Func<GuardedEventTransitionDefinition, TResult> fGuardedEvent)
        {
            switch (eventTransitionDefinition)
            {
                case UnguardedEventTransitionDefinition unguardedEvent:
                    return fUnguardedEvent(unguardedEvent);
                case GuardedEventTransitionDefinition guardedEvent:
                    return fGuardedEvent(guardedEvent);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public class InitialTransitionDefinition
    {
        public ChildTargetDefinition Target { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }

    public abstract class BaseTransitionDefinition
    {
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
    }

    public interface UnguardedTransitionDefinition { }

    public interface GuardedTransitionDefinition
    {
        BaseGuard Guard { get; set; }
    }

    public abstract class TransitionDefinition : BaseTransitionDefinition
    {
        public IEnumerable<Action> Actions { get; set; }
    }
    public class UnguardedImmediateTransitionDefinition : TransitionDefinition, UnguardedTransitionDefinition
    {
    }
    public class GuardedImmediateTransitionDefinition : TransitionDefinition, GuardedTransitionDefinition
    {
        public BaseGuard Guard { get; set; } // TODO: Guard without Event
    }
    public abstract class EventTransitionDefinition : BaseTransitionDefinition
    {
        public IEnumerable<ActionWithData> Actions { get; set; }
    }
    public class UnguardedEventTransitionDefinition : EventTransitionDefinition, UnguardedTransitionDefinition
    {
    }
    public class GuardedEventTransitionDefinition : EventTransitionDefinition, GuardedTransitionDefinition
    {
        public BaseGuard Guard { get; set; }
    }
}
