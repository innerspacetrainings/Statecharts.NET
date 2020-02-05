using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Interpreter
{
    public class InitialTransition
    {
        public InitialTransition(StateNode source, StateNode target, IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> actions)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Actions = actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>();
        }
        public StateNode Source { get; }
        public StateNode Target { get; }
        public IEnumerable<Model.Action> Actions { get; }
    }

    public abstract class Transition : OneOfBase<ForbiddenTransition, UnguardedTransition, GuardedTransition>
    {
        public StateNode Source { get; }

        internal Transition(StateNode source) => Source = source ?? throw new ArgumentNullException(nameof(source));

        public IEnumerable<StateNode> GetTargets() =>
            this.Match(
                forbidden => Enumerable.Empty<StateNode>(),
                unguarded => unguarded.Targets,
                guarded => guarded.Targets);
    }

    public class ForbiddenTransition : Transition
    {
        public NamedEvent Event { get; }
        public ForbiddenTransition(StateNode source, NamedEvent @event) : base(source) =>
            Event = @event ?? throw new ArgumentNullException(nameof(@event));

        public override string ToString() => $"{Source}: {Event} ⛔";
    }
    public class UnguardedTransition : Transition
    {
        public OneOf<Model.Event, Model.CustomDataEvent> Event { get; }
        public IEnumerable<StateNode> Targets { get; }
        public IEnumerable<Model.Action> Actions { get; }

        public UnguardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event, // TODO: check this, don't like it
            IEnumerable<StateNode> targets,
            IEnumerable<Definition.Action> actions) :
            this(source,
                @event,
                targets,
                actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>())
        { }
        public UnguardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            IEnumerable<StateNode> targets,
            IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> actions) :
            this(source,
                @event,
                targets,
                actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>())
        { }
        public UnguardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            IEnumerable<StateNode> targets,
            IEnumerable<OneOf<Definition.Action, Definition.ContextAction, Definition.ContextDataAction>> actions) :
            this(source,
                @event,
                targets,
                actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>())
        { }
        private UnguardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            IEnumerable<StateNode> targets,
            IEnumerable<Model.Action> actions) : base(source)
        {
            if (@event.Equals(null)) throw new ArgumentNullException(nameof(@event));
            Event = @event;
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            Actions = actions;
        }

        public override string ToString() => $"{Source}: {Event.Match(e => e.ToString(), cde => cde.ToString())} ✔ to [{string.Join(", ", Targets)}] ({Actions.Count()} Actions)";
    }
    public class GuardedTransition : Transition
    {
        public OneOf<Model.Event, Model.CustomDataEvent> Event { get; }
        public OneOf<Model.InStateGuard, Model.ConditionContextGuard, Model.ConditionContextDataGuard> Guard { get; }
        public IEnumerable<StateNode> Targets { get; }
        public IEnumerable<Model.Action> Actions { get; }

        private GuardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            OneOf<Model.InStateGuard, Model.ConditionContextGuard, Model.ConditionContextDataGuard> guard,
            IEnumerable<StateNode> targets,
            IEnumerable<Model.Action> actions) : base(source)
        {
            if (@event.Equals(null)) throw new ArgumentNullException(nameof(@event));
            if (guard.Equals(null)) throw new ArgumentNullException(nameof(@event));
            Event = @event;
            Guard = guard;
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            Actions = actions;
        }
        public GuardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            Model.InStateGuard guard,
            IEnumerable<StateNode> targets,
            IEnumerable<Definition.Action> actions) :
            this(source,
                @event,
                guard,
                targets,
                actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>())
        { }
        public GuardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            OneOf<Model.InStateGuard, Model.ConditionContextGuard> guard,
            IEnumerable<StateNode> targets,
            IEnumerable<OneOf<Definition.Action, Definition.ContextAction>> actions) :
            this(source,
                @event,
                guard.Match<OneOf<Model.InStateGuard, Model.ConditionContextGuard, Model.ConditionContextDataGuard>>(g => g, g => g),
                targets,
                actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>())
        { }
        public GuardedTransition(
            StateNode source,
            OneOf<Model.Event, Model.CustomDataEvent> @event,
            OneOf<Model.InStateGuard, Model.ConditionContextGuard, Model.ConditionContextDataGuard> guard,
            IEnumerable<StateNode> targets,
            IEnumerable<OneOf<Definition.Action, Definition.ContextAction, Definition.ContextDataAction>> actions) :
            this(source,
                @event,
                guard,
                targets,
                actions != null ? actions.ToModelActions() : Enumerable.Empty<Model.Action>())
        { }

        public override string ToString() => $"{Source}: {Event.Match(e => e.ToString(), cde => cde.ToString())} ❓ to [{string.Join(", ", Targets)}] ({Actions.Count()} Actions)";
    }
}
