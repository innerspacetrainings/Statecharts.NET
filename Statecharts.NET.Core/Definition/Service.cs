using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public abstract class Service : OneOfBase<ActivityService, TaskService, TaskDataService>
    {
        public abstract Option<string> Id { get; }
        public abstract Option<OneOf<UnguardedTransition, UnguardedContextTransition>> OnErrorTransition { get; }
    }

    public abstract class ActivityService : Service
    {
        public abstract Model.Activity Activity { get; }
    }

    public abstract class TaskService : Service
    {
        public abstract Model.Task Task { get; }
        public abstract Option<OneOf<UnguardedTransition, UnguardedContextTransition>> OnSuccessDefinition { get; }
    }
    public abstract class TaskDataService : Service
    {
        public abstract Model.Task<object> Task { get; }
        public abstract Option<OneOf<UnguardedTransition, UnguardedContextTransition, UnguardedContextDataTransition>> OnSuccessDefinition { get; }
    }
}