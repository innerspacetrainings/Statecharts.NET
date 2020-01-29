using System;
using Statecharts.NET.Definition;
using Statecharts.NET.Language.Service;
using Statecharts.NET.Language.Transition;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Language.Service
{
    public class Builder
    {
        public ActivityService.ServiceActivity DefineActivity(System.Action start, System.Action stop) =>
            new ActivityService.ServiceActivity(new Activity(start, stop));
        public ActivityService.ServiceActivity DefineActivity(Model.Activity activity) =>
            new ActivityService.ServiceActivity(activity);
        public TaskService.ServiceTask DefineTask(Model.Task task) =>
            new TaskService.ServiceTask(task);
    }
    internal class DefinitionData
    {
        public Option<string> Id { get; set; }
        public Option<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>> OnErrorTransition { get; set; }
        public Option<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>> OnSuccessDefinition { get; set; }

        public DefinitionData()
        {
            Id = Option.None<string>();
            OnErrorTransition = Option.None<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>>();
            OnSuccessDefinition = Option.None<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>>();
        }
    }
}
namespace Statecharts.NET.Language.TaskService
{
    public class ServiceTask : Definition.TaskService
    {
        internal Service.DefinitionData DefinitionData { get; }

        public ServiceTask(Model.Task task)
        {
            DefinitionData = new DefinitionData();
            Task = task;
        }

        public override Model.Task Task { get; }
        public override Option<string> Id => DefinitionData.Id;
        public override Option<OneOf<UnguardedTransition, UnguardedContextTransition>> OnErrorTransition => DefinitionData.OnErrorTransition;
        public override Option<OneOf<UnguardedTransition, UnguardedContextTransition>> OnSuccessDefinition => DefinitionData.OnSuccessDefinition;

        public object WithId => throw new NotImplementedException();
        public WithOnSuccess OnSuccess => new WithOnSuccess(this);
        public object OnError => throw new NotImplementedException();
    }
    public class WithOnSuccess
    {
        internal ServiceTask Service { get; }

        internal WithOnSuccess(ServiceTask service) => Service = service;

        public WithOnSuccessTransitionTo TransitionTo => new WithOnSuccessTransitionTo(this);
    }
    public class WithOnSuccessTransitionTo
    {
        internal ServiceTask Service { get; }

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
    public class WithOnSuccessTransition : Definition.TaskService
    {
        private ServiceTask Service { get; }

        internal WithOnSuccessTransition(WithOnSuccessTransitionTo service, Target target, params Target[] targets)
        {
            Service = service.Service;
            Service.DefinitionData.OnSuccessDefinition = Option.From<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>>(WithEvent.OnServiceSuccess().TransitionTo.Multiple(target, targets));
        }

        public override Model.Task Task => Service.Task;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOf<UnguardedTransition, UnguardedContextTransition>> OnSuccessDefinition => Service.DefinitionData.OnSuccessDefinition;
        public override Option<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;
    }
}
namespace Statecharts.NET.Language.ActivityService
{
    public class ServiceActivity : Definition.ActivityService
    {
        internal Service.DefinitionData DefinitionData { get; }

        public ServiceActivity(Activity activity)
        {
            DefinitionData = new Service.DefinitionData();
            Activity = activity;
        }

        public override Activity Activity { get; }
        public override Option<string> Id => DefinitionData.Id;
        public override Option<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>> OnErrorTransition => DefinitionData.OnErrorTransition;

        public WithId WithId(string id) => new WithId(this, id);
        public WithOnError OnError => new WithOnError(this);
    }
    public class WithId : Definition.ActivityService
    {
        internal ServiceActivity Service { get; }

        internal WithId(ServiceActivity service, string id)
        {
            Service = service;
            Service.DefinitionData.Id = id.ToOption();
        }

        public override Activity Activity => Service.Activity;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;

        public WithOnError OnError => new WithOnError(this);
    }
    public class WithOnError
    {
        internal ServiceActivity Service { get; }

        internal WithOnError(ServiceActivity service) => Service = service;
        internal WithOnError(WithId service) => Service = service.Service;

        public WithOnErrorTransitionTo TransitionTo => new WithOnErrorTransitionTo(this);
    }
    public class WithOnErrorTransitionTo
    {
        internal ServiceActivity Service { get; }

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
    public class WithOnErrorTransition : Definition.ActivityService
    {
        private ServiceActivity Service { get; }

        internal WithOnErrorTransition(WithOnErrorTransitionTo service, Target target, params Target[] targets)
        {
            Service = service.Service;
            Service.DefinitionData.OnErrorTransition = Option.From<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>>(WithEvent.OnServiceError().TransitionTo.Multiple(target, targets));
        }

        public override Activity Activity => Service.Activity;
        public override Option<string> Id => Service.DefinitionData.Id;
        public override Option<OneOf<Definition.UnguardedTransition, Definition.UnguardedContextTransition>> OnErrorTransition => Service.DefinitionData.OnErrorTransition;
    }
}
