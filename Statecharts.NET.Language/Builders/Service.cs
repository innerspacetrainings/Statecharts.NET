﻿using System;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language.Builders.Service;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language
{
    public class TaskService : TaskServiceDefinition
    {
        internal DefinitionData DefinitionData { get; }

        public TaskService(Model.TaskDelegate task)
        {
            DefinitionData = new DefinitionData();
            Task = task;
        }

        public override Model.TaskDelegate Task { get; }
        public override Option<string> Id => DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => DefinitionData.OnErrorTransition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => DefinitionData.OnSuccessDefinition;

        public object WithId => throw new NotImplementedException();
        public Builders.TaskService.WithOnSuccess OnSuccess => new Builders.TaskService.WithOnSuccess(this);
        public object OnError => throw new NotImplementedException();
    }
    public class ActivityService : ActivityServiceDefinition
    {
        internal DefinitionData DefinitionData { get; }

        public ActivityService(Activity activity)
        {
            DefinitionData = new DefinitionData();
            Activity = activity;
        }

        public override Activity Activity { get; }
        public override Option<string> Id => DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => DefinitionData.OnErrorTransition;

        public Builders.ActivityService.WithId WithId(string id) => new Builders.ActivityService.WithId(this, id);
        public Builders.ActivityService.WithOnError OnError => new Builders.ActivityService.WithOnError(this);
    }
}
namespace Statecharts.NET.Language.Builders.Service
{
    internal class DefinitionData
    {
        public Option<string> Id { get; set; }
        public Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition { get; set; }
        public Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition { get; set; }

        public DefinitionData()
        {
            Id = Option.None<string>();
            OnErrorTransition = Option.None<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>();
            OnSuccessDefinition = Option.None<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>();
        }
    }
}
namespace Statecharts.NET.Language.Builders.TaskService
{
    public class WithOnSuccess
    {
        internal Language.TaskService Service { get; }

        internal WithOnSuccess(Language.TaskService service) => Service = service;

        public WithOnSuccessTransitionTo TransitionTo => new WithOnSuccessTransitionTo(this);
    }
    public class WithOnSuccessTransitionTo
    {
        internal Language.TaskService Service { get; }

        internal WithOnSuccessTransitionTo(WithOnSuccess service) => Service = service.Service;

        public WithOnSuccessTransition Child(string stateName) =>
            new WithOnSuccessTransition(this, Keywords.Child(stateName));
        public WithOnSuccessTransition Sibling(string stateName) =>
            new WithOnSuccessTransition(this, Keywords.Sibling(stateName));
        public WithOnSuccessTransition Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithOnSuccessTransition(this, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public WithOnSuccessTransition Multiple(Target target, params Target[] targets) =>
            new WithOnSuccessTransition(this, target, targets);
    }
    public class WithOnSuccessTransition : TaskServiceDefinition
    {
        internal Language.TaskService Service { get; }
        internal UnguardedWithTarget OnSuccessTransition { get; }

        internal WithOnSuccessTransition(WithOnSuccessTransitionTo service, Target target, params Target[] targets)
        {
            Service = service.Service;
            OnSuccessTransition = WithEvent.OnServiceSuccess().TransitionTo.Multiple(target, targets);
            Service.DefinitionData.OnSuccessDefinition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(OnSuccessTransition);
        }

        public WithOnSuccessTransitionWithActions WithActions(Action action, params Action[] actions) =>
            new WithOnSuccessTransitionWithActions(this, action, actions);
        public WithOnSuccessTransitionWithActions<TContext> WithActions<TContext>(
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions)
            where TContext : IContext<TContext> =>
            new WithOnSuccessTransitionWithActions<TContext>(this, action, actions);

        public override Model.TaskDelegate Task => Service.Task;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Service.DefinitionData.OnSuccessDefinition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;
    }
    public class WithOnSuccessTransitionWithActions : TaskServiceDefinition
    {
        private Language.TaskService Service { get; }
        internal UnguardedWithActions OnSuccessTransition { get; }

        internal WithOnSuccessTransitionWithActions(WithOnSuccessTransition service, Action action, params Action[] actions)
        {
            Service = service.Service;
            OnSuccessTransition = service.OnSuccessTransition.WithActions(action, actions);
            Service.DefinitionData.OnSuccessDefinition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(OnSuccessTransition);
        }

        public override Model.TaskDelegate Task => Service.Task;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Service.DefinitionData.OnSuccessDefinition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;
    }
    public class WithOnSuccessTransitionWithActions<TContext> : TaskServiceDefinition where TContext : IContext<TContext>
    {
        private Language.TaskService Service { get; }
        internal ContextUnguardedWithActions<TContext> OnSuccessTransition { get; }

        internal WithOnSuccessTransitionWithActions(
            WithOnSuccessTransition service,
            OneOf<Action, Action<TContext>> action,
            params OneOf<Action, Action<TContext>>[] actions)
        {
            Service = service.Service;
            OnSuccessTransition = service.OnSuccessTransition.WithActions(action, actions);
            Service.DefinitionData.OnSuccessDefinition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(OnSuccessTransition);
        }

        public override Model.TaskDelegate Task => Service.Task;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition => Service.DefinitionData.OnSuccessDefinition;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;
    }
}
namespace Statecharts.NET.Language.Builders.ActivityService
{
    public class WithId : ActivityServiceDefinition
    {
        internal Language.ActivityService Service { get; }

        internal WithId(Language.ActivityService service, string id)
        {
            Service = service;
            Service.DefinitionData.Id = id.ToOption();
        }

        public override Activity Activity => Service.Activity;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;

        public WithOnError OnError => new WithOnError(this);
    }
    public class WithOnError
    {
        internal Language.ActivityService Service { get; }

        internal WithOnError(Language.ActivityService service) => Service = service;
        internal WithOnError(WithId service) => Service = service.Service;

        public WithOnErrorTransitionTo TransitionTo => new WithOnErrorTransitionTo(this);
    }
    public class WithOnErrorTransitionTo
    {
        internal Language.ActivityService Service { get; }

        internal WithOnErrorTransitionTo(WithOnError service) => Service = service.Service;

        public WithOnErrorTransition Child(string stateName) =>
            new WithOnErrorTransition(this, Keywords.Child(stateName));
        public WithOnErrorTransition Sibling(string stateName) =>
            new WithOnErrorTransition(this, Keywords.Sibling(stateName));
        public WithOnErrorTransition Absolute(string stateChartName, string stateNodeName, params string[] stateNodeNames) =>
            new WithOnErrorTransition(this, Keywords.Absolute(stateChartName, stateNodeName, stateNodeNames));
        public WithOnErrorTransition Multiple(Target target, params Target[] targets) =>
            new WithOnErrorTransition(this, target, targets);
    }
    public class WithOnErrorTransition : ActivityServiceDefinition
    {
        private Language.ActivityService Service { get; }

        internal WithOnErrorTransition(WithOnErrorTransitionTo service, Target target, params Target[] targets)
        {
            Service = service.Service;
            Service.DefinitionData.OnErrorTransition = Option.From<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>>(WithEvent.OnServiceError().TransitionTo.Multiple(target, targets));
        }

        public override Activity Activity => Service.Activity;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;
    }
}
