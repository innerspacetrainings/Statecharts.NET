using System;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language.Builders
{
    internal class ServiceDefinitionData
    {
        public Option<string> Id { get; set; }
        public Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition { get; set; }
        public Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition { get; set; }

        public ServiceDefinitionData()
        {
            Id = Option.None<string>();
            OnErrorTransition = Option.None<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>();
            OnSuccessDefinition = Option.None<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>();
        }
    }

    public class TaskService : TaskServiceDefinition
    {
        internal ServiceDefinitionData Definition { get; }

        public TaskService(TaskDelegate task)
        {
            Definition = new ServiceDefinitionData();
            Task = task;
        }

        public override TaskDelegate Task { get; }
        public override Option<string> Id => Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Definition.OnErrorTransition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Definition.OnSuccessDefinition;

        public object WithId => throw new NotImplementedException();
        public TaskServiceWithOnSuccess OnSuccess => new TaskServiceWithOnSuccess(this);
        public object OnError => throw new NotImplementedException();
    }
    public class TaskServiceWithOnSuccess
    {
        internal TaskService Service { get; }

        internal TaskServiceWithOnSuccess(TaskService service) => Service = service;

        public TaskServiceWithOnSuccessTransitionTo TransitionTo => new TaskServiceWithOnSuccessTransitionTo(this);
    }
    public class TaskServiceWithOnSuccessTransitionTo
    {
        internal TaskService Service { get; }

        internal TaskServiceWithOnSuccessTransitionTo(TaskServiceWithOnSuccess service) => Service = service.Service;

        public TaskServiceWithOnSuccessTransition Child(string statenodeName, params string[] childStatenodeNames) =>
            new TaskServiceWithOnSuccessTransition(this, Keywords.Child(statenodeName, childStatenodeNames));
        public TaskServiceWithOnSuccessTransition Sibling(string statenodeName, params string[] childStatenodeNames) =>
            new TaskServiceWithOnSuccessTransition(this, Keywords.Sibling(statenodeName, childStatenodeNames));
        public TaskServiceWithOnSuccessTransition Absolute(string statechartName, params string[] childStatenodeNames) =>
            new TaskServiceWithOnSuccessTransition(this, Keywords.Absolute(statechartName, childStatenodeNames));
        public TaskServiceWithOnSuccessTransition Target(Target target) =>
            new TaskServiceWithOnSuccessTransition(this, target);
        public TaskServiceWithOnSuccessTransition Multiple(Target target, params Target[] targets) =>
            new TaskServiceWithOnSuccessTransition(this, target, targets);
    }
    public class TaskServiceWithOnSuccessTransition : TaskServiceDefinition
    {
        internal TaskService Service { get; }
        internal UnguardedWithTarget OnSuccessTransition { get; }

        internal TaskServiceWithOnSuccessTransition(TaskServiceWithOnSuccessTransitionTo service, Target target, params Target[] targets)
        {
            Service = service.Service;
            OnSuccessTransition = WithEvent.OnServiceSuccess().TransitionTo.Multiple(target, targets);
            Service.Definition.OnSuccessDefinition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(OnSuccessTransition);
        }

        public TaskServiceWithOnSuccessTransitionWithActions WithActions(ActionDefinition action, params ActionDefinition[] actions) =>
            new TaskServiceWithOnSuccessTransitionWithActions(this, action, actions);
        public TaskServiceWithOnSuccessTransitionWithActions<TContext> WithActions<TContext>(
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new TaskServiceWithOnSuccessTransitionWithActions<TContext>(this, action, actions);

        public override TaskDelegate Task => Service.Task;
        public override Option<string> Id => Service.Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Service.Definition.OnSuccessDefinition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.Definition.OnErrorTransition;
    }
    public class TaskServiceWithOnSuccessTransitionWithActions : TaskServiceDefinition
    {
        private TaskService Service { get; }
        internal UnguardedWithActions OnSuccessTransition { get; }

        internal TaskServiceWithOnSuccessTransitionWithActions(TaskServiceWithOnSuccessTransition service, ActionDefinition action, params ActionDefinition[] actions)
        {
            Service = service.Service;
            OnSuccessTransition = service.OnSuccessTransition.WithActions(action, actions);
            Service.Definition.OnSuccessDefinition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(OnSuccessTransition);
        }

        public override TaskDelegate Task => Service.Task;
        public override Option<string> Id => Service.Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Service.Definition.OnSuccessDefinition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.Definition.OnErrorTransition;
    }
    public class TaskServiceWithOnSuccessTransitionWithActions<TContext> : TaskServiceDefinition where TContext : IContext<TContext>
    {
        private TaskService Service { get; }
        internal ContextUnguardedWithActions<TContext> OnSuccessTransition { get; }

        internal TaskServiceWithOnSuccessTransitionWithActions(
            TaskServiceWithOnSuccessTransition service,
            OneOf<ActionDefinition, ActionDefinition<TContext>> action,
            params OneOf<ActionDefinition, ActionDefinition<TContext>>[] actions)
        {
            Service = service.Service;
            OnSuccessTransition = service.OnSuccessTransition.WithActions(action, actions);
            Service.Definition.OnSuccessDefinition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(OnSuccessTransition);
        }

        public override TaskDelegate Task => Service.Task;
        public override Option<string> Id => Service.Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Service.Definition.OnSuccessDefinition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.Definition.OnErrorTransition;
    }

    public class ActivityService : ActivityServiceDefinition
    {
        internal ServiceDefinitionData Definition { get; }

        public ActivityService(Activity activity)
        {
            Definition = new ServiceDefinitionData();
            Activity = activity;
        }

        public override Activity Activity { get; }
        public override Option<string> Id => Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Definition.OnErrorTransition;

        public ActivityServiceWithId WithId(string id) => new ActivityServiceWithId(this, id);
        public ActivityServiceWithOnError OnError => new ActivityServiceWithOnError(this);
    }
    public class ActivityServiceWithId : ActivityServiceDefinition
    {
        internal ActivityService Service { get; }

        internal ActivityServiceWithId(ActivityService service, string id)
        {
            Service = service;
            Service.Definition.Id = id.ToOption();
        }

        public override Activity Activity => Service.Activity;
        public override Option<string> Id => Service.Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.Definition.OnErrorTransition;

        public ActivityServiceWithOnError OnError => new ActivityServiceWithOnError(this);
    }
    public class ActivityServiceWithOnError
    {
        internal ActivityService Service { get; }

        internal ActivityServiceWithOnError(ActivityService service) => Service = service;
        internal ActivityServiceWithOnError(ActivityServiceWithId service) => Service = service.Service;

        public ActivityServiceWithOnErrorTransitionTo TransitionTo => new ActivityServiceWithOnErrorTransitionTo(this);
    }
    public class ActivityServiceWithOnErrorTransitionTo
    {
        internal ActivityService Service { get; }

        internal ActivityServiceWithOnErrorTransitionTo(ActivityServiceWithOnError service) => Service = service.Service;

        public ActivityServiceWithOnErrorTransition Child(string stateName, params string[] childStatenodesNames) =>
            new ActivityServiceWithOnErrorTransition(this, Keywords.Child(stateName, childStatenodesNames));
        public ActivityServiceWithOnErrorTransition Sibling(string statenodeName, params string[] childStatenodesNames) =>
            new ActivityServiceWithOnErrorTransition(this, Keywords.Sibling(statenodeName, childStatenodesNames));
        public ActivityServiceWithOnErrorTransition Absolute(string statechartName, params string[] childStatenodesNames) =>
            new ActivityServiceWithOnErrorTransition(this, Keywords.Absolute(statechartName, childStatenodesNames));
        public ActivityServiceWithOnErrorTransition Target(Target target) =>
            new ActivityServiceWithOnErrorTransition(this, target);
        public ActivityServiceWithOnErrorTransition Multiple(Target target, params Target[] targets) =>
            new ActivityServiceWithOnErrorTransition(this, target, targets);
    }
    public class ActivityServiceWithOnErrorTransition : ActivityServiceDefinition
    {
        private ActivityService Service { get; }

        internal ActivityServiceWithOnErrorTransition(ActivityServiceWithOnErrorTransitionTo service, Target target, params Target[] targets)
        {
            Service = service.Service;
            Service.Definition.OnErrorTransition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(WithEvent.OnServiceError().TransitionTo.Multiple(target, targets));
        }

        public override Activity Activity => Service.Activity;
        public override Option<string> Id => Service.Definition.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.Definition.OnErrorTransition;
    }
}
