using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class InitialTransitionDefinition
    {
        public ChildTargetDefinition Target { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }

    public class TransitionDefinition :
        OneOfBase<
            ForbiddenTransitionDefinition,
            UnguardedTransitionDefinition,
            GuardedTransitionDefinition>
    {
        public TResult Match<TResult>(
            Func<ForbiddenTransitionDefinition, TResult> fForbidden,
            Func<IAllowedTransitionDefinition, TResult> fAllowed)
            => this.Match(fForbidden, fAllowed, fAllowed);
    }

    public class ForbiddenTransitionDefinition
    {
        public CustomEvent Event { get; }
        public ForbiddenTransitionDefinition(string eventName) => Event = new CustomEvent(eventName);
    }
    public interface IAllowedTransitionDefinition
    {
        Event Event { get; }
        IEnumerable<BaseTargetDefinition> Targets { get; }
    }
    public class UnguardedTransitionDefinition : IAllowedTransitionDefinition
    {
        public Event Event { get; set; }
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
    public class GuardedTransitionDefinition : IAllowedTransitionDefinition
    {
        public Event Event { get; set; }
        public Guard Guard { get; set; }
        public IEnumerable<BaseTargetDefinition> Targets { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
}
