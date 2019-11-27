using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statecharts.NET.Interpreter
{
    public static class BaseTransitionFunctions
    {
        public static TResult Map<TResult, TContext>(
            this BaseTransition<TContext> transition,
            Func<InitialTransition<TContext>, TResult> fInitial,
            Func<UnguardedImmediateTransition<TContext>, TResult> fUnguardedImmediate,
            Func<GuardedImmediateTransition<TContext>, TResult> fGuardedImmediate,
            Func<UnguardedEventTransition<TContext>, TResult> fUnguardedEvent,
            Func<GuardedEventTransition<TContext>, TResult> fGuardedEvent)
            where TContext : IEquatable<TContext>
        {
            switch (transition)
            {
                case InitialTransition<TContext> initial:
                    return fInitial(initial);
                case UnguardedImmediateTransition<TContext> unguardedImmediate:
                    return fUnguardedImmediate(unguardedImmediate);
                case GuardedImmediateTransition<TContext> guardedImmediate:
                    return fGuardedImmediate(guardedImmediate);
                case UnguardedEventTransition<TContext> unguardedEvent:
                    return fUnguardedEvent(unguardedEvent);
                case GuardedEventTransition<TContext> guardedEvent:
                    return fGuardedEvent(guardedEvent);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        public static IEnumerable<BaseStateNode<TContext>> GetTargets<TContext>(
            this BaseTransition<TContext> transition)
            where TContext : IEquatable<TContext>
            => transition.Map(
                initial => new[] { initial.Target },
                unguarded => unguarded.Targets,
                guarded => guarded.Targets,
                unguarded => unguarded.Targets,
                guarded => guarded.Targets);
    }

    public abstract class BaseTransition<TContext>
        where TContext : IEquatable<TContext>
    {
        public BaseStateNode<TContext> Source { get; }

        internal BaseTransition(BaseStateNode<TContext> source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }
    }

    public interface GuardedTransition
    {
        BaseGuard Guard { get; }
    }

    public class InitialTransition<TContext> : BaseTransition<TContext>
        where TContext : IEquatable<TContext>
    {
        public BaseStateNode<TContext> Target { get; }
        public IEnumerable<Action> Actions { get; }

        public InitialTransition(
            BaseStateNode<TContext> source,
            BaseStateNode<TContext> target,
            IEnumerable<Action> actions) : base(source)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Actions = actions ?? Enumerable.Empty<Action>();
        }
    }
    public class UnguardedImmediateTransition<TContext> : BaseTransition<TContext>
        where TContext : IEquatable<TContext>
    {
        public IEnumerable<BaseStateNode<TContext>> Targets { get; }
        public IEnumerable<Action> Actions { get; }

        public UnguardedImmediateTransition(
            BaseStateNode<TContext> source,
            IEnumerable<BaseStateNode<TContext>> targets,
            IEnumerable<Action> actions) : base(source)
        {
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            Actions = actions ?? Enumerable.Empty<Action>();
        }
    }
    public class GuardedImmediateTransition<TContext> : BaseTransition<TContext>, GuardedTransition
        where TContext : IEquatable<TContext>
    {
        public BaseGuard Guard { get; }
        public IEnumerable<BaseStateNode<TContext>> Targets { get; }
        public IEnumerable<Action> Actions { get; }

        public GuardedImmediateTransition(
            BaseGuard guard,
            BaseStateNode<TContext> source,
            IEnumerable<BaseStateNode<TContext>> targets,
            IEnumerable<Action> actions) : base(source)
        {
            Guard = guard ?? throw new ArgumentNullException(nameof(guard));
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            Actions = actions ?? Enumerable.Empty<Action>();
        }
    }
    public class UnguardedEventTransition<TContext> : BaseTransition<TContext>
        where TContext : IEquatable<TContext>
    {
        public BaseEvent Event { get; }
        public IEnumerable<BaseStateNode<TContext>> Targets { get; }
        public IEnumerable<EventAction> Actions { get; }

        public UnguardedEventTransition(BaseStateNode<TContext> source, BaseEvent @event,
            IEnumerable<BaseStateNode<TContext>> targets, IEnumerable<EventAction> actions) : base(source)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            Actions = actions ?? Enumerable.Empty<EventAction>();
        }
    }
    public class GuardedEventTransition<TContext> : BaseTransition<TContext>, GuardedTransition
        where TContext : IEquatable<TContext>
    {
        public BaseGuard Guard { get; }
        public BaseEvent Event { get; }
        public IEnumerable<BaseStateNode<TContext>> Targets { get; }
        public IEnumerable<EventAction> Actions { get; }
        
        public GuardedEventTransition(BaseStateNode<TContext> source, BaseGuard guard, BaseEvent @event,
            IEnumerable<BaseStateNode<TContext>> targets, IEnumerable<EventAction> actions) : base(source)
        {
            Guard = guard ?? throw new ArgumentNullException(nameof(guard));
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            Actions = actions ?? Enumerable.Empty<EventAction>();
        }
    }
}
